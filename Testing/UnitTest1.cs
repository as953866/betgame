using System;
using BetGame.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFactory()
        {
            Punter punter = Factory.GetAPunter(1);
            Assert.AreEqual(punter is Joe,true);
        }
    }
}
