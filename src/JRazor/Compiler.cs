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
    public class Compiler
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
                    throw new Exception(string.Join("\r\n", result.Diagnostics.Select(x => x.GetMessage())));
                }

                assemblyStream.Seek(0, SeekOrigin.Begin); ;

                var assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

                return assembly.GetTypes()[0];              
            }
        }

        private static List<MetadataReference> GetApplicationReferences()
        {
            var metadataReferences = new List<MetadataReference>();

            var libPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "mscorlib.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Runtime.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Threading.Tasks.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Dynamic.Runtime.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "Microsoft.CSharp.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Collections.dll")));
            metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath, "System.Collections.Immutable.dll")));

            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(templet).GetTypeInfo().Assembly.Location));

            return metadataReferences;
        }
    }
}
