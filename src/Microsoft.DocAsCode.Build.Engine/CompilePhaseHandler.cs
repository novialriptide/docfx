﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;

using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace Microsoft.DocAsCode.Build.Engine;

internal class CompilePhaseHandler : IPhaseHandler
{
    private readonly List<TreeItemRestructure> _restructions = new();

    public string Name => nameof(CompilePhaseHandler);

    public BuildPhase Phase => BuildPhase.Compile;

    public DocumentBuildContext Context { get; }

    public List<TreeItemRestructure> Restructions => _restructions;

    public CompilePhaseHandler(DocumentBuildContext context)
    {
        Context = context;
    }

    public void Handle(List<HostService> hostServices, int maxParallelism)
    {
        Prepare(hostServices, maxParallelism);
        hostServices.RunAll(hostService =>
        {
            using (new LoggerPhaseScope(hostService.Processor.Name, LogLevel.Verbose))
            {
                var steps = string.Join("=>", hostService.Processor.BuildSteps.OrderBy(step => step.BuildOrder).Select(s => s.Name));
                Logger.LogInfo($"Building {hostService.Models.Count} file(s) in {hostService.Processor.Name}({steps})...");
                Logger.LogVerbose($"Processor {hostService.Processor.Name}: Prebuilding...");
                using (new LoggerPhaseScope("Prebuild", LogLevel.Verbose))
                {
                    Prebuild(hostService);
                }

                // Register all the delegates to handler
                if (hostService.TableOfContentRestructions != null)
                {
                    lock (_restructions)
                    {
                        _restructions.AddRange(hostService.TableOfContentRestructions);
                    }
                }
            }
        }, maxParallelism);

        DistributeTocRestructions(hostServices);

        foreach (var hostService in hostServices)
        {
            using (new LoggerPhaseScope(hostService.Processor.Name, LogLevel.Verbose))
            {
                Logger.LogVerbose($"Processor {hostService.Processor.Name}: Building...");
                using (new LoggerPhaseScope("Build", LogLevel.Verbose))
                {
                    BuildArticle(hostService, maxParallelism);
                }
            }
        }
    }

    #region Private Methods

    private void Prepare(List<HostService> hostServices, int maxParallelism)
    {
        if (Context == null)
        {
            return;
        }
        foreach (var hostService in hostServices)
        {
            hostService.SourceFiles = Context.AllSourceFiles;
            hostService.Models.RunAll(
                m =>
                {
                    m.LocalPathFromRoot ??= StringExtension.ToDisplayPath(Path.Combine(m.BaseDir, m.File));
                },
                maxParallelism);
        }
    }

    private void DistributeTocRestructions(List<HostService> hostServices)
    {
        if (_restructions.Count > 0)
        {
            var restructions = _restructions.ToImmutableList();
            // Distribute delegates to all the hostServices
            foreach (var hostService in hostServices)
            {
                hostService.TableOfContentRestructions = restructions;
            }
        }
    }

    private static void Prebuild(HostService hostService)
    {
        BuildPhaseUtility.RunBuildSteps(
            hostService.Processor.BuildSteps,
            buildStep =>
            {
                Logger.LogVerbose($"Processor {hostService.Processor.Name}, step {buildStep.Name}: Prebuilding...");
                using (new LoggerPhaseScope(buildStep.Name, LogLevel.Verbose))
                {
                    var models = buildStep.Prebuild(hostService.Models, hostService);
                    if (!object.ReferenceEquals(models, hostService.Models))
                    {
                        Logger.LogVerbose($"Processor {hostService.Processor.Name}, step {buildStep.Name}: Reloading models...");
                        hostService.Reload(models);
                    }
                }
            });
    }

    private static void BuildArticle(HostService hostService, int maxParallelism)
    {
        using var aggregatedPerformanceScope = new AggregatedPerformanceScope();
        hostService.Models.RunAll(
            m =>
            {
                using (new LoggerFileScope(m.LocalPathFromRoot))
                {
                    Logger.LogDiagnostic($"Processor {hostService.Processor.Name}: Building...");
                    BuildPhaseUtility.RunBuildSteps(
                        hostService.Processor.BuildSteps,
                        buildStep =>
                        {
                            Logger.LogDiagnostic($"Processor {hostService.Processor.Name}, step {buildStep.Name}: Building...");
                            using (new LoggerPhaseScope(buildStep.Name, LogLevel.Diagnostic, aggregatedPerformanceScope))
                            {
                                try
                                {
                                    buildStep.Build(m, hostService);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError($"Trouble processing file - {m.FileAndType.FullPath}, with error - {ex.Message}");
                                    throw;
                                }
                            }
                        });
                }
            },
            maxParallelism);
    }

    #endregion
}
