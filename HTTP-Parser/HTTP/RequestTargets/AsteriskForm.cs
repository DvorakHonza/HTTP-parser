namespace HTTP_Parser.HTTP.RequestTargets
{
    public class AsteriskForm : RequestTarget
    {
        public string Asterisk { get; }
        public AsteriskForm()
        {
            Asterisk = "*";
        }
        public override string ToString()
        {
            return Asterisk;
        }
    }
}
