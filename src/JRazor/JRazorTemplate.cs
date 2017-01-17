using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;

namespace JRazor
{
    public abstract class JRazorTemplate
    {
        private StringBuilder buffer;

        protected JRazorTemplate()
        {
            Model = new ExpandoObject();
            buffer = new StringBuilder();
        }

        public dynamic Model { get; set; }

        public string Result
        {
            get
            {
                return buffer.ToString();
            }
        }

        public abstract Task ExecuteAsync();

        public virtual void Write(object value)
        {
            buffer.Append(value);
        }

        public virtual void WriteLiteral(object value)
        {
            buffer.Append(value);
        }

        public override string ToString()
        {
            return buffer.ToString();
        }
    }
}
