namespace HTTP_Parser.HTTP
{
    public class StatusLine : StartLine
    {
        public double Version { get; }
        public int StatusCode { get; }
        public string ReasonPhrase { get; }

        public StatusLine(double version, int statusCode, string reasonPhrase)
        {
            Version = version;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Type = MessageType.Response;
        }

        public override string ToString()
        {
            return $"HTTP/{Version} {StatusCode} {ReasonPhrase}";
        }
    }
}
