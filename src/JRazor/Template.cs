using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace JRazor
{
    public abstract class Template
    {
        private StringBuilder buffer;

        protected Template()
        {
            Model = new ExpandoObject();
            buffer = new StringBuilder();
        }

        public dynamic Model { get; set; }

        public abstract Task Execute();

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            buffer.Append(value);
        }

        public string GetContent()
        {            
            Execute();

            return buffer.ToString();
        }
    }
}
