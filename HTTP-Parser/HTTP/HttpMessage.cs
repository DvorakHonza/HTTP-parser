using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.HTTP
{
    public class HttpMessage
    {
        public HttpHeader Header { get; }
        public HttpBody MessageBody { get; }

        public HttpMessage(HttpHeader Header, HttpBody MessageBody)
        {
            this.Header = Header;
            this.MessageBody = MessageBody;
        }
    }
}
