using System.IO;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.AspNetCore.Razor.Parser.Internal;

namespace JRazor
{
    public class JRazorCodeGenerator
    {
        string templateName = string.Empty;

        public JRazorCodeGenerator(string templateName)
        {
            this.templateName = templateName;
        }

        public GeneratorResults Generate(string template)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage(), () => new HtmlMarkupParser())
            {
                DefaultBaseClass = "JRazorTemplate",
                DefaultClassName = templateName,
                DefaultNamespace = "JRazor"
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Collections.Generic");

            var engine = new RazorTemplateEngine(host);

            return engine.GenerateCode(new StringReader(template));
        }
    }
}
