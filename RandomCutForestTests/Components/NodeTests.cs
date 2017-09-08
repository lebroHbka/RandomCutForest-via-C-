using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RandomCutForest.Components;

namespace RandomCutForestTests.Components
{
    [TestClass]
    public class NodeTests
    {
        Node n1, n2, n3;

        [TestInitialize]
        public void TestInitialize()
        {
            var p1 = new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{4, 10}
            };
            var p2 = new List<decimal[]>
            {
                new decimal[]{1, 2}
            };
            var p3 = new List<decimal[]>
            {
                new decimal[]{4, 10},
                new decimal[]{1, 2}
            };

            n1 = new Node(new Data(p1));
            n2 = new Node(new Data(p2));
            n3 = new Node(new Data(p3));

            n1.Left = n2;
            n2.Right = n3;
        }


        [TestMethod]
        public void Test_IsLeaf()
        {
            var expected_n1_isleaf = false;
            var expected_n2_isleaf = false;
            var expected_n3_isleaf = true;

            var actual_n1_isleaf = n1.IsLeaf();
            var actual_n2_isleaf = n2.IsLeaf();
            var actual_n3_isleaf = n3.IsLeaf();

            Assert.AreEqual(expected_n1_isleaf, actual_n1_isleaf);
            Assert.AreEqual(expected_n2_isleaf, actual_n2_isleaf);
            Assert.AreEqual(expected_n3_isleaf, actual_n3_isleaf);
        }

        [TestMethod]
        public void Test_Equals()
        {
            var expected_n1_equals_n2 = false;
            var expected_n1_equals_n3 = true;

            var actual_n1_equals_n2 = n1.Equals(n2);
            var actual_n1_equals_n3 = n1.Equals(n3);

            Assert.AreEqual(expected_n1_equals_n2, actual_n1_equals_n2);
            Assert.AreEqual(expected_n1_equals_n3, actual_n1_equals_n3);
        }

    }
}
