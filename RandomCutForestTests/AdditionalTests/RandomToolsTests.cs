using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomCutForest.Additional;

namespace RandomCutForestTests.AdditionalTests
{
    [TestClass]
    public class RandomToolsTests
    {

        [TestMethod]
        public void Test_RandomDecimal_CheckInterval()
        {
            var a1 = RandomTools.RandomDecimal(0, 0.0000003M);
            var a2 = RandomTools.RandomDecimal(0, 1.0000003M);
            var a3 = RandomTools.RandomDecimal(10.1M, 5250.023M);
            var a4 = RandomTools.RandomDecimal(-10.2M, 9.03M);

            var rez1 = ((a1 >= 0) && (a1 < 0.000003M));
            var rez2 = ((a2 >= 0) && (a2 < 1.0000003M));
            var rez3 = ((a3 >= 10.1M) && (a3 < 5250.023M));
            var rez4 = ((a4 >= -10.2M) && (a4 < 9.03M));

            Assert.IsTrue(rez1);
            Assert.IsTrue(rez2);
            Assert.IsTrue(rez3);
            Assert.IsTrue(rez4);
        }

        [TestMethod]
        public void Test_RandomInt_CheckInterval()
        {
            var a1 = RandomTools.RandomInt(0, 1);
            var a2 = RandomTools.RandomInt(0, 9000);
            var a3 = RandomTools.RandomInt(-10, 10);

            var rez1 = (a1 == 0);
            var rez2 = ((a2 >= 0) && (a2 < 9000));
            var rez3 = ((a3 >= -10) && (a3 < 10));

            Assert.IsTrue(rez1);
            Assert.IsTrue(rez2);
            Assert.IsTrue(rez3);
        }
    }
}
