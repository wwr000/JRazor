using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace JRazor
{
    public abstract class Templet
    {
        private StringBuilder buffer;

        protected Templet()
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

        protected void Write(object value)
        {
            WriteTo(value);
        }

        protected void Write(string value)
        {
            WriteTo(value);
        }

        protected void WriteLiteral(string value)
        {
            WriteLiteralTo(value);
        }

        protected void WriteLiteral(object value)
        {
            WriteLiteralTo(value);
        }

        protected void WriteTo(object value)
        {
            if (value != null)
            {
               WriteTo(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }

        protected void WriteTo(string value)
        {
            WriteLiteralTo(value);
        }

        protected void WriteLiteralTo(object value)
        {
            WriteLiteralTo(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        protected void WriteLiteralTo(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                buffer.Append(value);
            }
        }

        private string AttributeEnding { get; set; }

        protected void BeginWriteAttribute(string name, string begining, int startPosition, string ending, int endPosition, int thingy)
        {
            Debug.Assert(string.IsNullOrEmpty(AttributeEnding));

            buffer.Append(begining);
            AttributeEnding = ending;
        }

        private List<string> AttributeValues { get; set; }

        protected void WriteAttributeValue(string thingy, int startPostion, object value, int endValue, int dealyo, bool yesno)
        {
            if (AttributeValues == null)
            {
                AttributeValues = new List<string>();
            }

            AttributeValues.Add(value.ToString());
        }

        protected void EndWriteAttribute()
        {
            Debug.Assert(!string.IsNullOrEmpty(AttributeEnding));

            var attributes = string.Join(" ", AttributeValues);
            buffer.Append(attributes);
            AttributeValues = null;

           buffer.Append(AttributeEnding);
           AttributeEnding = null;
        }

        protected void WriteAttributeTo(
          string name,
          string leader,
          string trailer,
          params AttributeValue[] values)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (leader == null)
            {
                throw new ArgumentNullException(nameof(leader));
            }

            if (trailer == null)
            {
                throw new ArgumentNullException(nameof(trailer));
            }


            WriteLiteralTo(leader);
            foreach (var value in values)
            {
                WriteLiteralTo(value.Prefix);

                // The special cases here are that the value we're writing might already be a string, or that the
                // value might be a bool. If the value is the bool 'true' we want to write the attribute name
                // instead of the string 'true'. If the value is the bool 'false' we don't want to write anything.
                // Otherwise the value is another object (perhaps an HtmlString) and we'll ask it to format itself.
                string stringValue;
                if (value.Value is bool)
                {
                    if ((bool)value.Value)
                    {
                        stringValue = name;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    stringValue = value.Value as string;
                }

                // Call the WriteTo(string) overload when possible
                if (value.Literal && stringValue != null)
                {
                    WriteLiteralTo(stringValue);
                }
                else if (value.Literal)
                {
                    WriteLiteralTo(value.Value);
                }
                else if (stringValue != null)
                {
                    WriteTo(stringValue);
                }
                else
                {
                    WriteTo(value.Value);
                }
            }
            WriteLiteralTo(trailer);
        }


        public override string ToString()
        {
            return buffer.ToString();
        }

        protected class AttributeValue
        {
            public AttributeValue(string prefix, object value, bool literal)
            {
                Prefix = prefix;
                Value = value;
                Literal = literal;
            }

            public string Prefix { get; }

            public object Value { get; }

            public bool Literal { get; }

            public static AttributeValue FromTuple(Tuple<string, object, bool> value)
            {
                return new AttributeValue(value.Item1, value.Item2, value.Item3);
            }

            public static AttributeValue FromTuple(Tuple<string, string, bool> value)
            {
                return new AttributeValue(value.Item1, value.Item2, value.Item3);
            }

            public static implicit operator AttributeValue(Tuple<string, object, bool> value)
            {
                return FromTuple(value);
            }
        }
    }    
}
