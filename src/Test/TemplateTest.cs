using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    using System;
    using System.Dynamic;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class cf02fe39cd1e4493da68455870923895a : JRazor.templet
    {
        public cf02fe39cd1e4493da68455870923895a()
        {
        }

        public override async Task ExecuteAsync()
        {
            WriteLiteral("Hello ");

            Write(Model.Name1);

            WriteLiteral(" Welcome to  repository\r\n");

            foreach (string s in Model.Lst)
            {

                WriteLiteral("                            <a");
                BeginWriteAttribute("href", " href=\"", 157, "\"", 166, 1);
                WriteAttributeValue("", 164, s, 164, 2, false);
                EndWriteAttribute();
                WriteLiteral(">");
                Write(s);
                WriteLiteral("</a>\r\n");
            }

        }
    }
}
