using JRazor;
using System;
using System.Dynamic;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = @"<h1>Hello @Model.Name Welcome to  repository</h1>
                            <ul>
                            @foreach(var s in Model.Color)
                            {
                                <li>
                                    <a href=""@s"">@s</a>
                                </li>
                            }
                            </ul>";

            var model = new
            {
                Name    =   "John Doe",
                Age     =   23,
                Color   =   new string[] { "red", "yellow", "blue" }
            };

            //var str = Engine.Parse(template, model);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json);
            
            var str = Engine.Parse(template, jsonObj);

            Console.Write(str);

            Console.ReadKey(true);
        }
    }
}
