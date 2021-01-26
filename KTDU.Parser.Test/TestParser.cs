using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;

namespace KTDU.Parser.Test
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestParserParse()
        {
            ImmutableArray<IEntry> p0 = Parser.Parse("");
            Assert.AreEqual(0, p0.Length);
            ImmutableArray<IEntry> p1 = Parser.Parse(@"
[a b]
aaaaa
*a
<a>a
;comment

");
            Assert.AreEqual(5, p1.Length);
            Assert.IsInstanceOfType(p1[0], typeof(CallEntry));
            Assert.IsInstanceOfType(p1[1], typeof(TextEntry));
            Assert.IsInstanceOfType(p1[2], typeof(AnchorEntry));
            Assert.IsInstanceOfType(p1[3], typeof(TextEntry));
            Assert.IsInstanceOfType(p1[4], typeof(CommentEntry));
        }
    }
}