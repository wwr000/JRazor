using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using System.Runtime.Loader;
using System.Reflection.PortableExecutable;
using System.Dynamic;

namespace JRazor
{
    public class Compiler
    {
        public static Assembly viewEngine_Razor_Assembly = typeof(Compiler).GetTypeInfo().Assembly;
        private static List<MetadataReference> ApplicationReferences = GetApplicationReferences();

        public Type Compile(string compilationContent)
        {
            var assemblyName = Path.GetRandomFileName();

            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, path: assemblyName, options: new CSharpParseOptions());


            var compilation = CSharpCompilation.Create(assemblyName,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                    syntaxTrees: new[] { syntaxTree },
                    references: ApplicationReferences
                );

            using (var assemblyStream = new MemoryStream())
            {
                var result = compilation.Emit(
                    assemblyStream,
                    options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));

                if (!result.Success)
                {
                    if (!compilation.References.Any() && !ApplicationReferences.Any())
                    {
                        Console.WriteLine("project.json preserveCompilationContext");
                    }

                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }


                var templateType = LoadTypeForAssemblyStream(assemblyStream);

                return templateType;

            }

        }

        protected virtual DependencyContext GetDependencyContext(Assembly assembly)
        {

            return DependencyContext.Load(assembly);
        }

        private Type LoadTypeForAssemblyStream(MemoryStream assemblyStream)
        {
            assemblyStream.Seek(0, SeekOrigin.Begin);
            Assembly assembly = null;

            assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

            var type = assembly.GetExportedTypes().FirstOrDefault(a => !a.IsNested);

            return type;
        }

        private static List<MetadataReference> GetApplicationReferences()
        {
            var metadataReferences = new List<MetadataReference>();
            var assembly = Assembly.GetEntryAssembly();

            string runtimePath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

            var mscorlibFile = Path.Combine(runtimePath, "mscorlib.dll");
            if (File.Exists(mscorlibFile))
                metadataReferences.Add(CreateMetadataFileReference(mscorlibFile));
            else
                metadataReferences.Add(CreateMetadataFileReference(Path.Combine(runtimePath, "mscorlib.ni.dll")));

            metadataReferences.Add(CreateMetadataFileReference(typeof(object).GetTypeInfo().Assembly.Location));
            metadataReferences.Add(CreateMetadataFileReference(typeof(DynamicObject).GetTypeInfo().Assembly.Location));
            metadataReferences.Add(CreateMetadataFileReference(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location));
            metadataReferences.Add(CreateMetadataFileReference(assembly.Location));


            var referencedAssemblies = assembly.GetReferencedAssemblies();



            //reference razor view engine by this assembly 
            if (!referencedAssemblies.Contains(viewEngine_Razor_Assembly.GetName()))
                metadataReferences.Add(CreateMetadataFileReference(viewEngine_Razor_Assembly.Location));


            foreach (var refAssemblyName in referencedAssemblies)
            {
                var refAssembly = Assembly.Load(refAssemblyName);
                metadataReferences.Add(CreateMetadataFileReference(refAssembly.Location));

            }



            return metadataReferences;
        }

        private static MetadataReference CreateMetadataFileReference(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                return assemblyMetadata.GetReference(filePath: path);
            }
        }
    }
}
