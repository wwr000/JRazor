using System.IO;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.Razor.Parser.Internal;

namespace JRazor
{
    public class CodeGenerator
    {
        string templateName = string.Empty;

        public CodeGenerator(string templateName)
        {
            this.templateName = templateName;
        }

        public GeneratorResults Generate(string template)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = nameof(TemplateBase),
                DefaultClassName = templateName,
                DefaultNamespace = nameof(JRazor)
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Collections.Generic");

            var engine = new RazorTemplateEngine(host);

            return engine.GenerateCode(new StringReader(template));
        }
    }
}
