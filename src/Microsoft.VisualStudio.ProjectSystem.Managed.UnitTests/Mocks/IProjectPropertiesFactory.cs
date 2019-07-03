﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ProjectSystem.Properties;

using Moq;

#nullable disable

namespace Microsoft.VisualStudio.ProjectSystem
{
    internal static class IProjectPropertiesFactory
    {
        public static Mock<IProjectProperties> MockWithProperty(string propertyName)
        {
            return MockWithProperties(ImmutableArray.Create(propertyName));
        }

        public static Mock<IProjectProperties> MockWithProperties(IEnumerable<string> propertyNames)
        {
            var mock = new Mock<IProjectProperties>();

            mock.Setup(t => t.GetPropertyNamesAsync())
                .ReturnsAsync(propertyNames);

            return mock;
        }

        public static Mock<IProjectProperties> MockWithPropertyAndValue(string propertyName, string setValue)
        {
            return MockWithPropertiesAndValues(new Dictionary<string, string>() { { propertyName, setValue } });
        }

        public static Mock<IProjectProperties> MockWithPropertiesAndValues(Dictionary<string, string> propertyNameAndValues)
        {
            var mock = MockWithProperties(propertyNameAndValues.Keys);

            mock.Setup(t => t.GetEvaluatedPropertyValueAsync(
                It.IsIn<string>(propertyNameAndValues.Keys)))
                 .Returns<string>(k => Task.FromResult(propertyNameAndValues[k]));

            mock.Setup(t => t.GetUnevaluatedPropertyValueAsync(
                It.IsIn<string>(propertyNameAndValues.Keys)))
                 .Returns<string>(k => Task.FromResult(propertyNameAndValues[k]));

            mock.Setup(t => t.SetPropertyValueAsync(
                It.IsIn<string>(propertyNameAndValues.Keys),
                It.IsAny<string>(), null))
                 .Returns<string, string, IReadOnlyDictionary<string, string>>((k, v, d) =>
                    {
                        propertyNameAndValues[k] = v;
                        return Task.CompletedTask;
                    });

            mock.Setup(t => t.DeletePropertyAsync(
                It.IsIn<string>(propertyNameAndValues.Keys),
                null))
                 .Returns<string, IReadOnlyDictionary<string, string>>((propName, d) =>
                    {
                        propertyNameAndValues[propName] = null;
                        return Task.CompletedTask;
                    });

            return mock;
        }

        public static IProjectProperties CreateWithProperty(string propertyName)
            => MockWithProperty(propertyName).Object;

        public static IProjectProperties CreateWithPropertyAndValue(string propertyName, string setValue)
            => MockWithPropertyAndValue(propertyName, setValue).Object;

        public static IProjectProperties CreateWithPropertiesAndValues(Dictionary<string, string> propertyNameAndValues)
            => MockWithPropertiesAndValues(propertyNameAndValues).Object;
    }
}
