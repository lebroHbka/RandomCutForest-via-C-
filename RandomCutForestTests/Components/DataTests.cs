using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RandomCutForest.src.Components;
using RandomCutForest.src.Additional;

namespace RandomCutForestTests.Components
{
    [TestClass]
    public class DataTests
    {
        Data d1, d2, d3, d4, d5;

        [TestInitialize]
        public void TestInitialize()
        {
            Data.DimensionsCount = 2;

            d1 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3}
            });

            d2 = new Data(new List<decimal[]>
            {
                new decimal[]{3, 3},
                new decimal[]{2, 8}
            });

            d3 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 4}
            });

            d4 = new Data(new List<decimal[]>
            {
                new decimal[]{3, 3}
            });

            d5 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 8},
            });
        }

        #region Add points
        [TestMethod]
        public void Test_AddPoints_AllDifferentElements()
        {

            var expected_d1_add_d2 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3},
                new decimal[]{3, 3},
                new decimal[]{2, 8}
            });

            var expected_d3_add_d4 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 4},
                new decimal[]{3, 3}
            });

            d1.AddPoints(d2);
            d3.AddPoints(d4);

            var actual_d1_add_d2 = d1;
            var actual_d3_add_d4 = d3;

            Assert.AreEqual(expected_d1_add_d2, actual_d1_add_d2);
            Assert.AreEqual(expected_d3_add_d4, actual_d3_add_d4);

            Assert.AreNotEqual(actual_d1_add_d2, d2);
        }

        [TestMethod]
        public void Test_AddPoints_withSameElements()
        {

            var expected_d2_add_d4 = new Data(new List<decimal[]>
            {
                new decimal[]{3, 3},
                new decimal[]{2, 8}
            });

            d2.AddPoints(d4);
            var actual_d2_add_d4 = d2;

            Assert.AreEqual(expected_d2_add_d4, actual_d2_add_d4, "d2 add d2");

        }

        #endregion

        #region Remove points

        [TestMethod]
        public void Test_RemovePoints_NotExistPoint()
        {
            var expectedD1 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3}
            });

            d1.RemovePoints(d4);
            var actualD1 = d1;

            Assert.AreEqual(expectedD1, actualD1);
        }

        [TestMethod]
        public void Test_RemovePoints_ExistPoint()
        {
            var d6 = Data.Merge(d1, d2);
            var expected_d2_remove_d4 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 8}
            });

            var expected_d6_remove_d5 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 3},
                new decimal[]{3, 3}
            });


            d2.RemovePoints(d4);
            d6.RemovePoints(d5);
            var actual_d2_remove_d4 = d2;
            var actual_d6_remove_d5 = d6;


            Assert.AreEqual(expected_d2_remove_d4, actual_d2_remove_d4);
            Assert.AreEqual(expected_d6_remove_d5, actual_d6_remove_d5);
        }

        #endregion

        #region IsInside
        [TestMethod]
        public void Test_IsInside()
        {

            var d5 = Data.Merge(d1, d2);
            var d6 = Data.Merge(d1, d3);
            var d7 = Data.Merge(d2, d3);

            var expected_d1_In_d5 = true;
            var expected_d2_In_d5 = true;
            var expected_d4_In_d5 = true;
            var expected_d4_In_d2 = true;
            var expected_d4_In_d6 = false;
            var expected_d5_In_d1 = false;
            var expected_d3_In_d2 = true;
            var expected_d4_In_d1 = false;

            var actual_d1_In_d5 = d5.IsInside(d1);
            var actual_d2_In_d5 = d5.IsInside(d2);
            var actual_d4_In_d5 = d5.IsInside(d4);
            var actual_d4_In_d2 = d2.IsInside(d4);
            var actual_d4_In_d6 = d6.IsInside(d4);
            var actual_d5_In_d1 = d1.IsInside(d5);
            var actual_d3_In_d2 = d2.IsInside(d3);
            var actual_d4_In_d1 = d1.IsInside(d4);

            Assert.AreEqual(expected_d1_In_d5, actual_d1_In_d5, "d1 in d5");
            Assert.AreEqual(expected_d2_In_d5, actual_d2_In_d5,"d2 in d5");
            Assert.AreEqual(expected_d4_In_d5, actual_d4_In_d5, "d4 in d5");
            Assert.AreEqual(expected_d4_In_d2, actual_d4_In_d2, "d4 in d2");
            Assert.AreEqual(expected_d4_In_d6, actual_d4_In_d6, "d4 in d6");
            Assert.AreEqual(expected_d5_In_d1, actual_d5_In_d1, "d5 in d1");
            Assert.AreEqual(expected_d3_In_d2, actual_d3_In_d2, "d3 in d1");
            Assert.AreEqual(expected_d4_In_d1, actual_d4_In_d1, "d4in d1");
        }

        #endregion

        #region Merge

        [TestMethod]
        public void Test_Merget()
        {
            var expected_D4 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3},
                new decimal[]{2, 8},
                new decimal[]{3, 3},
            });

            var expected_D5 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 3},
                new decimal[]{1, 2},
                new decimal[]{2, 4},
            });

            var expected_D6 = new Data(new List<decimal[]>
            {
                new decimal[]{3, 3},
                new decimal[]{2, 8}
            });


            var actual_D4 = Data.Merge(d1, d2);
            var actual_D5 = Data.Merge(d1, d3);
            var actual_D6 = Data.Merge(d2, d4);


            Assert.AreEqual(expected_D4, actual_D4);
            Assert.AreEqual(expected_D5, actual_D5);
            Assert.AreEqual(expected_D6, actual_D6);

        }

        [TestMethod]
        public void Test_Merge_stability_after_deleting()
        {
            // before delete
            var expected_D6 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3},
                new decimal[]{2, 8},
                new decimal[]{3, 3},
            });

            var actual_D6 = Data.Merge(d1, d2);

            Assert.AreEqual(expected_D6, actual_D6);


            // after delete
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[]{2, 3},
            });
            expected_D6 = new Data(new List<decimal[]>
            {
                new decimal[]{1, 2},
                new decimal[]{2, 3},
                new decimal[]{2, 8},
                new decimal[]{3, 3},
            });

            d1.RemovePoints(new Data(new List<decimal[]>
            {
                new decimal[]{1, 2}
            }));
            var actual_D1 = d1;

            Assert.AreEqual(expected_D1, actual_D1);
            Assert.AreEqual(expected_D6, actual_D6);

        }

        #endregion

        #region Position
        [TestMethod]
        public void Test_Position()
        {
            // { 1, 2 },{ 2, 3 }
            d1.SplitDimension = 1;
            d1.SplitValue = 2.4M;

            // {3, 3},{ 2, 8 }
            d2.SplitDimension = 0;
            d2.SplitValue = 2.3M;

            var d3 = new Data(new List<decimal[]>
            {
                new decimal[]{9, 8},
                new decimal[]{20, 3},
            });
            d3.SplitDimension = 0;
            d3.SplitValue = 10;

            var p1 = new Data(new List<decimal[]>
            {
                new decimal[]{3, 2.3M},
            });

            var p2 = new Data(new List<decimal[]>
            {
                new decimal[]{11, 5},
            });

            var expected_p1_in_d1 = "left";
            var expected_p2_in_d1 = "right";
            var expected_p1_in_d2 = "right";
            var expected_p2_in_d2 = "right";
            var expected_p1_in_d3 = "left";
            var expected_p2_in_d3 = "right";

            var actual_p1_in_d1 = d1.Position(p1);
            var actual_p2_in_d1 = d1.Position(p2);
            var actual_p1_in_d2 = d2.Position(p1);
            var actual_p2_in_d2 = d2.Position(p2);
            var actual_p1_in_d3 = d3.Position(p1);
            var actual_p2_in_d3 = d3.Position(p2);

            StringAssert.Contains(expected_p1_in_d1, actual_p1_in_d1);
            StringAssert.Contains(expected_p2_in_d1, actual_p2_in_d1);
            StringAssert.Contains(expected_p1_in_d2, actual_p1_in_d2);
            StringAssert.Contains(expected_p2_in_d2, actual_p2_in_d2);
            StringAssert.Contains(expected_p1_in_d3, actual_p1_in_d3);
            StringAssert.Contains(expected_p2_in_d3, actual_p2_in_d3);

        }

        #endregion

        #region Equal
        [TestMethod]
        public void Test_Equal()
        {
            var l1 = new List<decimal[]>
            {
                new decimal[]{2, 8},
                new decimal[]{3, 3}
            };

            var l2 = new List<decimal[]>
            {
                new decimal[]{3, 3},
                new decimal[]{2, 8}
            };

            var l3 = new List<decimal[]>
            {
                new decimal[]{3, 3}
            };

            var l4 = new List<decimal[]>
            {
                new decimal[]{2, 8},
                new decimal[]{3, 3}
            };

            var l5 = new List<decimal[]>
            {
                new decimal[]{8, 2},
                new decimal[]{3, 3}
            };

            var l6 = new List<decimal[]>
            {
                new decimal[]{1, 4},
                new decimal[]{7, 6}
            };

            var d1 = new Data(l1);
            var d2 = new Data(l2);
            var d3 = new Data(l3);
            var d4 = new Data(l4);
            var d5 = new Data(l5);
            var d6 = new Data(l6);

            Assert.AreEqual(d1, d2);
            Assert.AreNotEqual(d1, d3);
            Assert.AreEqual(d1, d4);
            Assert.AreNotEqual(d1, d5);
            Assert.AreNotEqual(d1, d6);
        }

        #endregion

    }
}
