using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JRazor
{
    public class CompilerResult
    {
        public bool Success { set; get; } = false;

        public Type TemplateType { get; set; } = null;

        public List<string> Errors { set; get; } = new List<string>();
    }
}
