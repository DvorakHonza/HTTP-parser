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
            using (var fs = new FileStream("Resources/Streams/http_stream.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(string.Concat(res.Value));
        }

        [Fact]
        public void HeadRequestResponseTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/head_request_response.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(string.Concat(res.Value));
        }

        [Fact]
        public void ProxyConnectionTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/proxy_connection.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(string.Concat(res.Value));
        }
    }
}
