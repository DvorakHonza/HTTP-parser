using System;

namespace HTTP_Parser.HTTP
{
    public class HttpMessage
    {
        public HttpHeader Header { get; }
        public byte[] MessageBody { get; }

        public HttpMessage(HttpHeader header, byte[] messageBody)
        {
            Header = header;
            MessageBody = messageBody;
        }
        public HttpMessage(HttpHeader header)
        {
            Header = header;
            MessageBody = Array.Empty<byte>();
        }

        public override string ToString() => $"{Header}";
    }
}
