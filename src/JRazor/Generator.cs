using Microsoft.AspNetCore.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JRazor
{
    public class Generator
    {
        public string Generate(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = nameof(templet),
                DefaultClassName = "c" + Guid.NewGuid().ToString("N"),
                DefaultNamespace = nameof(JRazor),
                GeneratedClassContext = new Microsoft.AspNetCore.Razor.CodeGenerators.GeneratedClassContext("ExecuteAsync", "Write", "WriteLiteral", new Microsoft.AspNetCore.Razor.CodeGenerators.GeneratedTagHelperContext())
            };
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Dynamic");
            host.NamespaceImports.Add("System.Collections.Generic");

            var engine = new RazorTemplateEngine(host);

            var result = engine.GenerateCode(new StringReader(template));

            if(!result.Success)
            {
                throw new Exception(string.Join("\r\n", result.ParserErrors.Select(x => x.Message)));
            }

            return result.GeneratedCode;
        }
    }
}
