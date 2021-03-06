﻿using Aiursoft.Scanner.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aiursoft.Scanner.Services
{
    public class ClassScanner
    {
        private IEnumerable<Assembly> ScanAssemblies(Assembly entry, bool removeSystem, bool removeMicrosoft)
        {
            yield return entry;
            var references = entry
                .GetReferencedAssemblies()
                .Filter(removeSystem, t => !t.Name.StartsWith("System"))
                .Filter(removeMicrosoft, t => !t.Name.StartsWith("Microsoft"));
            foreach (var referenced in references)
            {
                foreach (var scanned in ScanAssemblies(Assembly.Load(referenced), removeSystem, removeMicrosoft))
                {
                    yield return scanned;
                }
            }
        }

        public List<Type> AllAccessibleClass(bool includeSystem, bool includeMicrosoft)
        {
            var entry = Assembly.GetEntryAssembly();
            return ScanAssemblies(entry, !includeSystem, !includeMicrosoft)
                .Distinct()
                .SelectMany(t => t.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsNestedPrivate)
                .Where(t => !t.IsGenericType)
                .Where(t => !t.IsInterface)
                .ToList();
        }

        public List<Type> AllLibraryClass(Assembly calling, bool includeSystem, bool includeMicrosoft)
        {
            return ScanAssemblies(calling, !includeSystem, !includeMicrosoft)
                .Distinct()
                .SelectMany(t => t.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsNestedPrivate)
                .Where(t => !t.IsGenericType)
                .Where(t => !t.IsInterface)
                .ToList();
        }
    }
}
