﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json;

namespace Microsoft.DocAsCode.Plugins;

public class TreeItem
{
    [JsonProperty("items")]
    public List<TreeItem> Items { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}
