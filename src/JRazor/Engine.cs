using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace JRazor
{
    public class Engine
    {
        public static string Parse(string template, dynamic model)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException(nameof(template));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var templateCode = new Generator().Generate(template);

            var templateType = new Compiler().Compile(templateCode);

            var obj = (templet)Activator.CreateInstance(templateType);

            obj.Model = model;
           
            obj.ExecuteAsync().Wait();         

            return obj.Result;
        }
    }
}
