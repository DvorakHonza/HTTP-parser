using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP.Utilities
{
    public class StringFormatting
    {
        public static string PrependChar(IEnumerable<string> segments, char delimiter)
        {
            var stringBuilder = new StringBuilder();
            foreach (var segment in segments)
            {
                stringBuilder.Append(delimiter);
                stringBuilder.Append(segment);
            }
            return stringBuilder.ToString();
        }
    }
}
