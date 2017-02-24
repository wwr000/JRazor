using JRazor;
using System;
using System.Dynamic;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var template = @"<h1>Hello @json.name Welcome to  repository</h1>
                            <ul>
                            @foreach(var s in json.color)
                            {
                                <li>
                                    <a href=""@s"">@s</a>
                                </li>
                            }
                            </ul>";


            var json = @"{
                            ""name"":""John Doe"",
                            ""age"":""23"",
                            ""color"":[""red"",""yellow"",""blue""]
                         }";

            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json);
            
            var str = Engine.Parse(template, model);

            Console.Write(str);

            Console.ReadKey(true);
        }
    }
}
