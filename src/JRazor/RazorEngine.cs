using System;
using System.Linq;

namespace JRazor
{
    public class RazorEngine
    {
        public static string Parse(string template, dynamic model)
        {
            if(string.IsNullOrWhiteSpace(template))
                throw new Exception("template is null");

            if(model == null)
                throw new Exception("template is null");

            var generator = new CodeGenerator("c" + Guid.NewGuid().ToString("N"));
            var compiler = new Compiler();

            var generatorResult = generator.Generate(template);

            if (!generatorResult.Success)
                throw new Exception(string.Join("\r\n", generatorResult.ParserErrors.Select(x => x.Message).ToArray()));

            var compileResult = compiler.Compile(generatorResult.GeneratedCode);

            if (!compileResult.Success)
                throw new Exception(string.Join("\r\n", compileResult.Errors));

            var obj = (TemplateBase)Activator.CreateInstance(compileResult.TemplateType);

            obj.Model = model;

            obj.ExecuteAsync().Wait();

            return obj.Result;
        }
    }
}
