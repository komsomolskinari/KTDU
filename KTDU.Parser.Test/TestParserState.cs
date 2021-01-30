using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KTDU.Parser.Test
{
    [TestClass]
    public class TestParserState
    {
        [TestMethod]
        public void TestNextAdvance()
        {
            ParserState ps = new ParserState("a");
            Assert.AreEqual('a', ps.Next());
            Assert.AreEqual('a', ps.Advance());
            Assert.IsNull(ps.Next());
            Assert.IsNull(ps.Advance());
            Assert.IsNull(ps.Advance());
        }

        [TestMethod]
        public void TestReadLine()
        {
            ParserState ps = new ParserState("a\r\nb\n\ncccccc\rd");
            Assert.AreEqual("a", ps.ReadLine());
            Assert.AreEqual("b", ps.ReadLine());
            Assert.AreEqual('\n', ps.Next());
            Assert.AreEqual("", ps.ReadLine());
            Assert.AreEqual('c', ps.Next());
            Assert.AreEqual("cccccc", ps.ReadLine());
            Assert.AreEqual("d", ps.ReadLine());
            Assert.IsNull(ps.Next());
        }

        [TestMethod]
        public void TestReadUntil()
        {
            ParserState ps = new ParserState("aaaaa d");
            Assert.AreEqual("aaaaa", ps.ReadUntil(new[] {' '}));
            Assert.AreEqual(' ', ps.Advance());
            Assert.AreEqual("d", ps.ReadUntil(new[] {'a'}));
        }

        [TestMethod]
        public void TestSkip()
        {
            ParserState ps = new ParserState("a    b");
            Assert.AreEqual('a', ps.Advance());
            Assert.AreEqual(' ', ps.Next());
            ps.Skip(new[] {' '});
            Assert.AreEqual('b', ps.Advance());
            ps.Skip(new[] {' '});
            Assert.IsNull(ps.Next());
        }
    }
}