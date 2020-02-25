using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HeaderField
    {
        public string FieldName { get; }
        public string FieldValue { get; }

        public HeaderField(string FieldName, string FieldValue)
        {
            this.FieldName = FieldName;
            this.FieldValue = FieldValue;
        }

        public override string ToString()
        {
            return $"{FieldName}: {FieldValue}";
        }
    }
}
