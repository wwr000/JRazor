using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace JRazor
{
    public class Engine
    {
        public static string Parse(string template,dynamic model)
        {
            if(string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException(nameof(template));

            if(model == null)
                throw new ArgumentNullException(nameof(model));

            var templateCode = Generate(template);

            var templateType = Compile(templateCode);

            var obj = (Templet)Activator.CreateInstance(templateType);

            obj.Json = model;

            obj.ExecuteAsync().Wait();

            return obj.Result;
        }

        public static string Generate(string template)
        {
            if(string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = nameof(Templet),
                DefaultClassName = "c" + Guid.NewGuid().ToString("N"),
                DefaultNamespace = nameof(JRazor),
                GeneratedClassContext = new GeneratedClassContext("ExecuteAsync","Write","WriteLiteral",new GeneratedTagHelperContext())
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Collections.Generic");

            var engine = new RazorTemplateEngine(host);

            var result = engine.GenerateCode(new StringReader(template));

            if(!result.Success)
            {
                throw new Exception(string.Join("\r\n",result.ParserErrors.Select(x => x.Message)));
            }

            return result.GeneratedCode;
        }

        public static Type Compile(string generatedCode)
        {
            var assemblyName = Path.GetRandomFileName();

            var sourceText = SourceText.From(generatedCode,Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText,path: assemblyName,options: new CSharpParseOptions(LanguageVersion.CSharp6));
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(assemblyName,options: options,syntaxTrees: new[] { syntaxTree },references: ApplicationReferences);

            using(var assemblyStream = new MemoryStream())
            {
                var result = compilation.Emit(assemblyStream,options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));

                if(!result.Success)
                {
                    throw new Exception(string.Join("\r\n",result.Diagnostics.Select(x => x.GetMessage())));
                }

                assemblyStream.Seek(0,SeekOrigin.Begin);

                var assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

                return assembly.GetTypes()[0];
            }
        }

        private static List<MetadataReference> ApplicationReferences
        {
            get
            {
                var metadataReferences = new List<MetadataReference>();

                var libPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);

                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"mscorlib.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"System.Runtime.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"System.Threading.Tasks.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"System.Dynamic.Runtime.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"Microsoft.CSharp.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"System.Collections.dll")));
                metadataReferences.Add(MetadataReference.CreateFromFile(Path.Combine(libPath,"System.Collections.Immutable.dll")));

                metadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));
                metadataReferences.Add(MetadataReference.CreateFromFile(typeof(Templet).GetTypeInfo().Assembly.Location));

                return metadataReferences;
            }
        }
    }
}
