using System;
using JRazor;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var str = @"Hello @Model.Name Welcome to  repository
                        @foreach(string s in Model.Lst)
                        {
                            @s
                        }";

            var model = new
            {
                Name = "John Doe",
                Title = "RazorLight",
                Lst = new string[] { "aaa", "bbb", "ccc" }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(json);

            var aaaa = JRazorEngine.Parse(str, jsonObj);

            Console.Write(aaaa);

            Console.ReadKey();
        }
    }
}
