using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment18;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        static Block OriginBlock = new Block("Hello World!");

        [TestMethod]
        public void TestOriginBlockConstructor()
        {
            Block B = OriginBlock;
            Assert.AreEqual(1u, B.ID);
            Assert.AreEqual(0u, B.Nonce);
            Assert.AreEqual("Hello World!", B.Data);
            Assert.AreEqual(HashString.Origin.Value, B.PreviousHash);
            Assert.AreEqual("04f660de81972eae51939adc0bc802abb654e55d", B.MyHash);
            Assert.IsFalse(B.IsSigned());
        }

        [TestMethod]
        public void TestMineOnOrigin()
        {
            Block B = OriginBlock; 
            B.Mine();
            Assert.IsTrue(B.IsSigned());
            Assert.AreEqual(41929u, B.Nonce);
            Assert.AreEqual("0000eb2252a2a022fd75007c9073fb8835b2dcb5", B.MyHash);
        }

        [TestMethod]
        public void PropertyUpdates()
        {
            Block B = OriginBlock;
            HashString HS = new HashString(B.MyHash);
            B.Data = "Some other data";
            Assert.AreNotEqual(HS.Value, B.MyHash); //is different
            Assert.IsFalse(B.IsSigned());
        }
    }
}
