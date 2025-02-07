﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DocAsCode.Common;

public interface ILogItem
{
    LogLevel LogLevel { get; }
    string Message { get; }
    string Phase { get; }
    string File { get; }
    string Line { get; }
    string Code { get; }
}
