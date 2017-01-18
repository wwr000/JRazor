using System;
using JRazor;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = @"Hello @Model.undefine Welcome to  repository
                        @foreach(string s in Model.Lst)
                        {
                            @s
                        }";

            var model = new
            {
                Name = "John Doe",
                Title = "RazorLight",
                Age = 23,
                Lst = new string[] { "aaa", "bbb", "ccc" }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            //var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(json);

            var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JRazorExpandoObject>(json);

            var str = JRazorEngine.Parse(template, jsonObj);

            Console.Write(str);

            Console.ReadKey();
        }
    }
}
