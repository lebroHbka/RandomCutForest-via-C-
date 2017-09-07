﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomCutForest.src.Additional;

namespace RandomCutForestTests.AdditionalTests
{
    [TestClass]
    public class DecimalToolsTests
    {
        decimal[] a1, a2, a3, a4;

        [TestInitialize]
        public void TestInitialize()
        {
            a1 = new decimal[] { 1, 2 };
            a2 = new decimal[] { 1, 2, 3};
            a3 = new decimal[] { 1, 3, 2 };
            a4 = new decimal[] { 1, 2, 3 };

        }

        [TestMethod]
        public void Test_CompareArrays_difLength()
        {
            var expectedCompare_a1_a2 = false;
            var expectedCompare_a2_a1 = false;

            var actualCompare_a1_a2 = DecimalTools.Compare(a1, a2);
            var actualCompare_a2_a1 = DecimalTools.Compare(a2, a1);

            Assert.AreEqual(expectedCompare_a1_a2, actualCompare_a1_a2);
            Assert.AreEqual(expectedCompare_a2_a1, actualCompare_a2_a1);
        }

        [TestMethod]
        public void Test_CompareArrays()
        {
            var expectedCompare_a2_a3 = false;
            var expectedCompare_a2_a4 = true;

            var actualCompare_a2_a3 = DecimalTools.Compare(a2, a3);
            var actualCompare_a2_a4 = DecimalTools.Compare(a2, a4);

            Assert.AreEqual(expectedCompare_a2_a3, actualCompare_a2_a3);
            Assert.AreEqual(expectedCompare_a2_a4, actualCompare_a2_a4);
        }

        [TestMethod]
        public void Test_Contains()
        {
            var l = new List<decimal[]>
            {
                new decimal[] { 1, 2 },
                new decimal[] { 1, 5 },
                new decimal[] { 1, 2, 4 }
            };

            var expected_l_cont_a1 = true;
            var expected_l_cont_a2 = false;

            var actual_l_cont_a1 = DecimalTools.Contains(l, a1);
            var actual_l_cont_a2 = DecimalTools.Contains(l, a2);

            Assert.AreEqual(expected_l_cont_a1, actual_l_cont_a1);
            Assert.AreEqual(expected_l_cont_a2, actual_l_cont_a2);
        }

    }
}
