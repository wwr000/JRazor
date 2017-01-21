using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;

namespace JRazor
{
    public abstract class TemplateBase
    {
        private StringBuilder buffer;

        protected TemplateBase()
        {
            Model = new System.Dynamic.ExpandoObject();
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

        public virtual Task ExecuteAsync()
        {
            return Task.FromResult(0);
        }

        public virtual void BeginWriteAttribute(object value)
        {
            buffer.Append(value);
        }

        public virtual void WriteAttributeValue(object value)
        {
            buffer.Append(value);
        }

        public virtual void EndWriteAttribute(object value)
        {
            buffer.Append(value);
        }

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
