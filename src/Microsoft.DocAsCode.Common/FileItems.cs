// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DocAsCode;

[Serializable]
public class FileItems : List<string>
{
    private static IEnumerable<string> Empty = new List<string>();
    public FileItems(string file) : base()
    {
        this.Add(file);
    }

    public FileItems(IEnumerable<string> files) : base(files ?? Empty)
    {
    }

    public static explicit operator FileItems(string input)
    {
        return new FileItems(input);
    }
}
