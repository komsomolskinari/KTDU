using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KTDU.Interpreter.Test
{
    [TestClass]
    public class TestScriptLocation
    {
        [TestMethod]
        public void TestScriptLocationParse()
        {
            ScriptLocation l1 = ScriptLocation.Parse("a");
            Assert.IsNull(l1.File);
            Assert.AreEqual("a", l1.Anchor);
            Assert.IsTrue(l1.Line < 0);

            ScriptLocation l2 = ScriptLocation.Parse("#1");
            Assert.IsNull(l2.File);
            Assert.IsNull(l2.Anchor);
            Assert.AreEqual(0, l2.Line);

            ScriptLocation l3 = ScriptLocation.Parse("@a/b.ks");
            Assert.AreEqual("a/b.ks", l3.File);
            Assert.IsNull(l3.Anchor);
            Assert.IsTrue(l3.Line < 0);

            ScriptLocation l4 = ScriptLocation.Parse("a#1@a@b.ks");
            Assert.AreEqual("a@b.ks", l4.File);
            Assert.AreEqual("a#1", l4.Anchor);
            Assert.IsTrue(l4.Line < 0);

            ScriptLocation l5 = ScriptLocation.Parse("#1@");
            Assert.IsNull(l5.File);
            Assert.IsNull(l5.Anchor);
            Assert.AreEqual(0, l5.Line);

            Assert.ThrowsException<FormatException>(() => ScriptLocation.Parse("#a"));
            Assert.ThrowsException<ArgumentException>(() => ScriptLocation.Parse(null));
        }

        [TestMethod]
        public void TestScriptStep()
        {
            ScriptLocation f = new ScriptLocation("1", 0);
            Assert.AreEqual(0, f.Line);
            f.Step();
            Assert.AreEqual(1, f.Line);
        }
    }
}