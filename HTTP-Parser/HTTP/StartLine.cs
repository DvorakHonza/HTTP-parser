namespace HTTP_Parser.HTTP
{
    public abstract class StartLine
    {
        public MessageType Type { get; set; }

        public abstract override string ToString();


    }
}
