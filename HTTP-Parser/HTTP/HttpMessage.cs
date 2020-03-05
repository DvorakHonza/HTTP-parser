using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HttpMessage
    {
        public HttpHeader Header { get; }
        public byte[] MessageBody { get; }

        public HttpMessage(HttpHeader Header, byte[] MessageBody)
        {
            this.Header = Header;
            this.MessageBody = MessageBody;
        }
        public HttpMessage(HttpHeader Header)
        {
            this.Header = Header;
            Array.Clear(MessageBody, 0, 0);
        }

        public override string ToString() => $"{Header}";
    }
}
