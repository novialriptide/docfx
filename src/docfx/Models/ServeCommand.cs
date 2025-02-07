// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

namespace Microsoft.DocAsCode;

internal class ServeCommand : Command<ServeCommand.Settings>
{
    [Description("Host a local static website")]
    internal class Settings : CommandSettings
    {
        [Description("Path to the directory to serve")]
        [CommandArgument(0, "[directory]")]
        public string Folder { get; set; }

        [Description("Specify the hostname of the hosted website")]
        [CommandOption("-n|--hostname")]
        public string Host { get; set; }

        [Description("Specify the port of the hosted website")]
        [CommandOption("-p|--port")]
        public int? Port { get; set; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings options)
    {
        return CommandHelper.Run(() => RunServe.Exec(options.Folder, options.Host, options.Port));
    }
}
