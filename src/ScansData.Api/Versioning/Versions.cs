namespace Template.Api.Versioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Versions
    {
        private static readonly IReadOnlyCollection<int> detectedVersions;
        private static readonly Regex versionRegex;

        static Versions()
        {
            versionRegex = new Regex( @"\.Version([0-9])(?:\.|$)", RegexOptions.Compiled );
            detectedVersions = Assembly.Load( new AssemblyName( "Template.Api" ) )
                                       .GetTypes()
                                       .Where( IsInVersionedNamespace )
                                       .Select( t => GetVersionFromNamespace( t ).Value )
                                       .Distinct()
                                       .OrderBy( v => v )
                                       .ToList();
        }

        public static int Latest => detectedVersions.LastOrDefault();

        public static bool IsInVersionedNamespace( Type type )
        {
            return !string.IsNullOrWhiteSpace( type?.Namespace ) && versionRegex.IsMatch( type.Namespace );
        }

        public static int? GetVersionFromNamespace( Type type )
        {
            if( !IsInVersionedNamespace( type ) )
            {
                return null;
            }

            return int.Parse( versionRegex.Match( type.Namespace ).Groups[1].Value );
        }
    }
}