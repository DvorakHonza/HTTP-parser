using HTTP_Parser.Parsers;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ParserTests
{
    public class StreamTests
    {
        private readonly ITestOutputHelper output;
        public StreamTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HttpStreamTest()
        {
            string input;
            using (FileStream fs = new FileStream("Resources/Streams/http_stream.bin", FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            foreach ( var mess in res.Value)
            {
                output.WriteLine(mess.ToString());
            }
        }
    }
}
