// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DocAsCode.HtmlToPdf;
using Newtonsoft.Json;

namespace Microsoft.DocAsCode;

[Serializable]
internal class PdfJsonConfig : BuildJsonConfig
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("host")]
    public string Host { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("generatesAppendices")]
    public bool GeneratesAppendices { get; set; }

    [JsonProperty("generatesExternalLink")]
    public bool GeneratesExternalLink { get; set; }

    [JsonProperty("keepRawFiles")]
    public bool KeepRawFiles { get; set; }

    [JsonProperty("excludeDefaultToc")]
    public bool ExcludeDefaultToc { get; set; }

    [JsonProperty("rawOutputFolder")]
    public string RawOutputFolder { get; set; }

    [JsonProperty("excludedTocs")]
    public List<string> ExcludedTocs { get; set; }

    [JsonProperty("css")]
    public string CssFilePath { get; set; }

    [JsonProperty("base")]
    public string BasePath { get; set; }

    /// <summary>
    /// Specify how to handle pages that fail to load: abort, ignore or skip(default abort)
    /// </summary>
    [JsonProperty("errorHandling")]
    public string LoadErrorHandling { get; set; }

    /// <summary>
    /// Specify options specific to the wkhtmltopdf tooling used by the pdf command.
    /// </summary>
    [JsonProperty("wkhtmltopdf")]
    public WkhtmltopdfJsonConfig Wkhtmltopdf { get; set; }

    /// <summary>
    /// Gets or sets the "Table of Contents" bookmark title.
    /// </summary>
    [JsonProperty("tocTitle")]
    public string TocTitle { get; set; } = "Table of Contents";

    /// <summary>
    /// Gets or sets the outline option.
    /// </summary>
    [JsonProperty("outline")]
    public OutlineOption OutlineOption { get; set; } = OutlineOption.DefaultOutline;

    /// <summary>
    /// Gets or sets the cover page title.
    /// </summary>
    [JsonProperty("coverTitle")]
    public string CoverPageTitle { get; set; } = "Cover Page";

    /// <summary>
    /// Are input arguments set using command line
    /// </summary>
    [JsonProperty("noStdin")]
    public bool NoInputStreamArgs { get; set; }
}
