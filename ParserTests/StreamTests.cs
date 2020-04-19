using HTTP_Parser.Parsers;
using System;
using System.IO;
using System.Text;
using HTTP_Parser.HTTP;
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
            using (var fs = new FileStream("Resources/Streams/http_head.bin", FileMode.Open))
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
            using (var fs = new FileStream("Resources/Streams/http_proxy_connection.bin", FileMode.Open))
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
        public void GzippedStreamTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/http_gzip.bin", FileMode.Open))
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
        public void HttpTcpStream0Test()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/http_tcp_stream_0.bin", FileMode.Open))
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
        public void HttpTcpStream1Test()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/http_tcp_stream_1.bin", FileMode.Open))
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
        public void HttpJpegStreamTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/http_jpeg.bin", FileMode.Open))
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
        public void HttpChunkedGzipStreamTest()
        {
            string input;
            using (var fs = new FileStream("Resources/Streams/http_chunked_gzip.bin", FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                input = Encoding.ASCII.GetString(bin, 0, bin.Length);
            }
            var res = HttpParser.Parse(input);
            Assert.True(res.Success);
            output.WriteLine(string.Concat(res.Value));
            foreach (var mes in res.Value)
            {
                if (mes.Header.StartLine.Type == MessageType.Response)
                {
                    output.WriteLine(mes.MessageBody.Length.ToString());
                }   
            }
        }
    }
}
