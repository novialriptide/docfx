﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using YamlDotNet.Serialization;

namespace Microsoft.DocAsCode.YamlSerialization.TypeInspectors;

public sealed class ExtensibleNamingConventionTypeInspector : ExtensibleTypeInspectorSkeleton
{
    private readonly IExtensibleTypeInspector innerTypeDescriptor;
    private readonly INamingConvention namingConvention;

    public ExtensibleNamingConventionTypeInspector(IExtensibleTypeInspector innerTypeDescriptor, INamingConvention namingConvention)
    {
        if (innerTypeDescriptor == null)
        {
            throw new ArgumentNullException(nameof(innerTypeDescriptor));
        }

        this.innerTypeDescriptor = innerTypeDescriptor;

        if (namingConvention == null)
        {
            throw new ArgumentNullException(nameof(namingConvention));
        }

        this.namingConvention = namingConvention;
    }

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) =>
        from p in innerTypeDescriptor.GetProperties(type, container)
        select (IPropertyDescriptor)new PropertyDescriptor(p) { Name = namingConvention.Apply(p.Name) };

    public override IPropertyDescriptor GetProperty(Type type, object container, string name) =>
        innerTypeDescriptor.GetProperty(type, container, name);
}
