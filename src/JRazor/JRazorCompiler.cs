using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace JRazor
{
    public class JRazorCompiler
    {
        private static List<MetadataReference> references = GetApplicationReferences();

        public Type Compile(string compilationContent)
        {
            var assemblyName = Path.GetRandomFileName();

            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, path: assemblyName, options: new CSharpParseOptions(LanguageVersion.CSharp6));
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(assemblyName, options: options, syntaxTrees: new[] { syntaxTree }, references: references);

            using (var assemblyStream = new MemoryStream())
            {
                var result = compilation.Emit(assemblyStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                assemblyStream.Seek(0, SeekOrigin.Begin); ;

                var assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

                var templateType = assembly.GetTypes()[0];

                return templateType;
            }
        }

        private static List<MetadataReference> GetApplicationReferences()
        {
            var metadataReferences = new List<MetadataReference>();



            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\mscorlib.dll"));
            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Private.CoreLib.ni.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Users\wwr\Source\Repos\JRazor\src\Test\bin\Debug\netcoreapp1.1\Test.dll"));
            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Runtime.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Runtime.InteropServices.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Diagnostics.Debug.dll"));
            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Dynamic.Runtime.dll"));
            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Threading.Tasks.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Users\wwr\.nuget\packages\Microsoft.AspNetCore.Razor\1.1.0\lib\netstandard1.3\Microsoft.AspNetCore.Razor.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\Microsoft.CodeAnalysis.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Reflection.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Collections.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\Microsoft.CodeAnalysis.CSharp.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Collections.Immutable.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.IO.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.IO.FileSystem.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Console.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Users\wwr\.nuget\packages\Newtonsoft.Json\9.0.1\lib\netstandard1.0\Newtonsoft.Json.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Text.Encoding.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Runtime.Extensions.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Runtime.Loader.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Linq.dll"));
            ////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\Microsoft.CSharp.dll"));
            //////metadataReferences.Add(MetadataReference.CreateFromFile(@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\1.1.0\System.Reflection.Metadata.dll"));

            var libPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "mscorlib.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Runtime.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Threading.Tasks.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Dynamic.Runtime.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "Microsoft.CSharp.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Collections.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Collections.Immutable.dll")));

            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(JRazorTemplate).GetTypeInfo().Assembly.Location));

            return metadataReferences;
        }
    }
}
