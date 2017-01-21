using System;
using System.Linq;

namespace JRazor
{
    public class RazorEngine
    {
        public static string Parse(string template, dynamic model)
        {
            var generator = new CodeGenerator("c" + Guid.NewGuid().ToString("N"));
            var compiler = new Compiler();

            var generatorResult = generator.Generate(template);

            if (!generatorResult.Success)
                throw new Exception(string.Join(",", generatorResult.ParserErrors.Select(x => x.Message).ToArray()));

            var compileResult = compiler.Compile(generatorResult.GeneratedCode);

            if (!compileResult.Success)
                throw new Exception(string.Join(",", compileResult.Errors));

            var obj = (Template)Activator.CreateInstance(compileResult.TemplateType);

            obj.Model = model;

            obj.ExecuteAsync().Wait();

            return obj.Result;
        }
    }
}
