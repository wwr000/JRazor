using System;

namespace JRazor
{
    public class JRazorEngine
    {
        public static string Parse(string template, dynamic model)
        {
            var g = new JRazorCodeGenerator("c" + Guid.NewGuid().ToString("N"));
            var c = new JRazorCompiler();

            var result = g.Generate(template);

            var type = c.Compile(result.GeneratedCode);

            var obj = (JRazorTemplate)Activator.CreateInstance(type);

            obj.Model = model;

            obj.ExecuteAsync().Wait();

            

            return obj.Result;
        }
    }
}
