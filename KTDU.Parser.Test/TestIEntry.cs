using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KTDU.Parser.Test
{
    [TestClass]
    public class TestIEntry
    {
        [TestMethod]
        public void TestCallParse()
        {
            CallEntry c = CallEntry.Parse("[a b]");
            Assert.AreEqual("a", c.Name);
            Assert.AreEqual("b", c.ParametersList[0].key);

            CallEntry c2 = CallEntry.Parse(" @ a b=1  ");
            Assert.AreEqual("a", c2.Name);
            Assert.AreEqual("b", c2.ParametersList[0].key);
            Assert.AreEqual("1", c2.Parameters["b"]);

            CallEntry c3 = CallEntry.Parse("@a b=1=c   c d = \"aaaa\" e='b'");
            Assert.AreEqual("a", c3.Name);
            Assert.AreEqual("b", c3.ParametersList[0].key);
            Assert.AreEqual("1=c", c3.Parameters["b"]);
            Assert.IsTrue(c3.Parameters.ContainsKey("c"));
            Assert.IsNull(c3.Parameters["c"]);
            Assert.AreEqual("aaaa", c3.Parameters["d"]);
            Assert.AreEqual("b", c3.Parameters["e"]);


            Assert.ThrowsException<FormatException>(() => CallEntry.Parse(""));
            Assert.ThrowsException<FormatException>(() => CallEntry.Parse("a"));
            Assert.ThrowsException<FormatException>(() => CallEntry.Parse("@a b="));
            Assert.ThrowsException<FormatException>(() => CallEntry.Parse("@a b=c =d"));
        }

        [TestMethod]
        public void TestAnchorParse()
        {
            AnchorEntry a1 = AnchorEntry.Parse("*李文亮没有造谣");
            Assert.AreEqual("李文亮没有造谣", a1.Name);
            AnchorEntry a2 = AnchorEntry.Parse("*workers_of_the_world_unite|全世界无产者联合起来");
            Assert.AreEqual("workers_of_the_world_unite", a2.Name);
            AnchorEntry a3 = AnchorEntry.Parse("*なぜみんな日本語を話してくれないのか");
            Assert.AreEqual("なぜみんな日本語を話してくれないのか", a3.Name);

            Assert.ThrowsException<FormatException>(() => AnchorEntry.Parse("*COVID-19"));
            Assert.ThrowsException<FormatException>(() => AnchorEntry.Parse("*a |"));
            Assert.ThrowsException<FormatException>(() => AnchorEntry.Parse(""));
        }

        [TestMethod]
        public void TestTextParse()
        {
            TextEntry t1 = TextEntry.Parse("啊 这");
            Assert.AreEqual("啊 这", t1.Content);
            Assert.AreEqual(string.Empty, t1.Speaker);
            Assert.AreEqual(string.Empty, t1.DisplayName);

            TextEntry t2 = TextEntry.Parse("< 江泽民/the man who changed china > 你们啊，不要老想着搞个大新闻");
            Assert.AreEqual("你们啊，不要老想着搞个大新闻", t2.Content);
            Assert.AreEqual("江泽民", t2.Speaker);
            Assert.AreEqual("the man who changed china", t2.DisplayName);
            TextEntry t3 = TextEntry.Parse("<a>b");
            Assert.AreEqual("b", t3.Content);
            Assert.AreEqual("a", t3.Speaker);
            Assert.AreEqual("a", t3.DisplayName);
            TextEntry t4 = TextEntry.Parse("【a/b】c");
            Assert.AreEqual("c", t4.Content);
            Assert.AreEqual("a", t4.Speaker);
            Assert.AreEqual("b", t4.DisplayName);
            Assert.ThrowsException<FormatException>(() => TextEntry.Parse(""));
            Assert.ThrowsException<FormatException>(() => TextEntry.Parse("<>"));
        }

        [TestMethod]
        public void TestCommentParse()
        {
            CommentEntry c1 = CommentEntry.Parse("; comment comment");
            Assert.AreEqual("comment comment", c1.Comment);
            Assert.ThrowsException<FormatException>(() => CommentEntry.Parse("a"));
        }
    }
}