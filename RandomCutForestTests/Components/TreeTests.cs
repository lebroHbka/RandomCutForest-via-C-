using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RandomCutForest.src.Components;

namespace RandomCutForestTests.Components
{
    [TestClass]
    public class TreeTests
    {
        Tree tree;
        Node n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10;
        Data d0, d1, d2, d3, d4, d5, d6, d7, d8, d9, d10;


        [TestInitialize]
        public void TestInitialize()
        {
            /*           <n0>  |               (1,1)(2,8)(4,20)(5,6)(8,5)(10,9) 
             *                                        [dim: 1, val: 12,5]
             *                                           /            \
             *        <n1,n2>  |   (1,1)(2,8)(5,6)(8,5)(10,9)        (4, 20)
             *                        [dim: 1, val: 5,6]
             *                               /              \                          
             *        <n3,n4>  |    (2,8)(5,6)(10,9)        (1,1)(8,5)
             *                     [dim: 0, val: 4,4]    [dim: 1, val: 4,5]
             *                           /       \           /       \
             *  <n5,n6,n7,n8>  | (5,6)(10,9)     (2,8)    (1,1)    (8,5)
             *                [dim: 0, val: 9,8]
             *                      /     \
             *       <n9,n10>  | (5,6)   (10,9)
             *        
             * 
             */
            Data.DimensionsCount = 2;
            var l0 = new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9},
            };
            d0 = new Data(l0);
            d0.SplitDimension = 1;
            d0.SplitValue = 12.5M;
            n0 = new Node(d0);
            n0.Level = 0;


            var l1 = new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9},
            };
            d1 = new Data(l1);
            d1.SplitDimension = 1;
            d1.SplitValue = 5.6M;
            n1 = new Node(d1);
            n1.Level = 1;

            var l2 = new List<decimal[]>
            {
                new decimal[] {4, 20},
            };
            d2 = new Data(l2);
            n2 = new Node(d2);
            n2.Level = 1;

            var l3 = new List<decimal[]>
            {
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {10, 9},
            };
            d3 = new Data(l3);
            d3.SplitDimension = 0;
            d3.SplitValue = 4.4M;
            n3 = new Node(d3);
            n3.Level = 2;

            var l4 = new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {8, 5}
            };
            d4 = new Data(l4);
            d4.SplitDimension = 1;
            d4.SplitValue = 4.5M;
            n4 = new Node(d4);
            n4.Level = 2;

            var l5 = new List<decimal[]>
            {
                new decimal[] {5, 6},
                new decimal[] {10, 9}
            };
            d5 = new Data(l5);
            d5.SplitDimension = 0;
            d5.SplitValue = 9.8M;
            n5 = new Node(d5);
            n5.Level = 3;

            var l6 = new List<decimal[]>
            {
                new decimal[] {2, 8}
            };
            d6 = new Data(l6);
            n6 = new Node(d6);
            n6.Level = 3;

            var l7 = new List<decimal[]>
            {
                new decimal[] {1, 1}
            };
            d7 = new Data(l7);
            n7 = new Node(d7);
            n7.Level = 3;

            var l8 = new List<decimal[]>
            {
                new decimal[] {8, 5}
            };
            d8 = new Data(l8);
            n8 = new Node(d8);
            n8.Level = 3;

            var l9 = new List<decimal[]>
            {
                new decimal[] {5, 6}
            };
            d9 = new Data(l9);
            n9 = new Node(d9);
            n9.Level = 4;

            var l10 = new List<decimal[]>
            {
                new decimal[] {10, 9}
            };
            d10 = new Data(l10);
            n10 = new Node(d10);
            n10.Level = 4;


            // make tree
            tree = new Tree(n0);
            n0.Left = n1;
            n0.Right = n2;
            n1.Parent = n0;
            n2.Parent = n0;

            n1.Left = n3;
            n1.Right = n4;
            n3.Parent = n1;
            n4.Parent = n1;

            n4.Left = n7;
            n4.Right = n8;
            n7.Parent = n4;
            n8.Parent = n4;

            n3.Left = n5;
            n3.Right = n6;
            n5.Parent = n3;
            n6.Parent = n3;

            n5.Left = n9;
            n5.Right = n10;
            n9.Parent = n5;
            n10.Parent = n5;
        }

        #region Delete

        [TestMethod]
        public void Test_Delete_n2()
        {
            // before delete
            var expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10 };
            var expected_root = n0;
            Node expected_rootParent = null;
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });

            var actual_nodes = new List<Node>();
            var actual_root = tree.RootNode;
            var actual_rootParent = tree.RootNode.Parent;
            var actual_D1 = n1.Value;

            Action<Node> CalcNodes = (Node a) => { actual_nodes.Add(a); };
            tree.ForEach(CalcNodes);


            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_root, actual_root);
            Assert.AreEqual(expected_rootParent, actual_rootParent);
            Assert.AreEqual(expected_D1, actual_D1);

            // after delete
            tree.Delete(n2);

            expected_nodes = new List<Node> { n1, n3, n4, n5, n6, n7, n8, n9, n10 };
            expected_root = n1;
            expected_rootParent = null;
            expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });

            actual_nodes = new List<Node>();
            actual_root = tree.RootNode;
            actual_rootParent = tree.RootNode.Parent;
            actual_D1 = n1.Value;

            tree.ForEach(CalcNodes);

            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_root, actual_root);
            Assert.AreEqual(expected_rootParent, actual_rootParent);
            Assert.AreEqual(expected_D1, actual_D1);
        }
                    
        [TestMethod]
        public void Test_Delete_n6()
        {
            // befor delete
            var expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10 };
            var expected_root = n0;
            var expected_N1_left = n3;
            var expected_N5_level = 3;

            var actual_D0 = n0.Value;
            var actual_D1 = n1.Value;
            var actual_nodes = new List<Node>();
            var actual_root = tree.RootNode;
            var actual_N1_left = n1.Left;
            var actual_N5_level = n5.Level;

            Action<Node> CalcNodes = (Node a) => { actual_nodes.Add(a); };
            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_root, actual_root);
            Assert.AreEqual(expected_N1_left, actual_N1_left);
            Assert.AreEqual(expected_N5_level, actual_N5_level);

            // after delete
            tree.Delete(n6);

            expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_nodes = new List<Node> { n0, n1, n2, n5, n4, n9, n10, n7, n8 };
            expected_root = n0;
            expected_N1_left = n5;
            expected_N5_level = 2;

            actual_D0 = n0.Value;
            actual_D1 = n1.Value;
            actual_nodes = new List<Node>();
            actual_root = tree.RootNode;
            actual_N1_left = n1.Left;
            actual_N5_level = n5.Level;

            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_root, actual_root);
            Assert.AreEqual(expected_N1_left, actual_N1_left);
            Assert.AreEqual(expected_N5_level, actual_N5_level);
        }
                    
        [TestMethod]
        public void Test_Delete_n7()
        {
            // befor delete
            var expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D0_bbmin = new List<decimal> { 1, 1 };
            var expected_D0_bbmax = new List<decimal> { 10, 20 };
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D1_bbmin = new List<decimal> { 1, 1 };
            var expected_D1_bbmax = new List<decimal> { 10, 9 };
            var expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10 };
            var expected_N1_right = n4;
            var expected_N8_level = 3;

            var actual_D0 = n0.Value;
            var actual_D0_bbmin = n0.Value.BoxMin;
            var actual_D0_bbmax = n0.Value.BoxMax;
            var actual_D1 = n1.Value;
            var actual_D1_bbmin = n1.Value.BoxMin;
            var actual_D1_bbmax = n1.Value.BoxMax;
            var actual_nodes = new List<Node>();
            var actual_N1_right = n1.Right;
            var actual_N8_level = n8.Level;

            Action<Node> CalcNodes = (Node a) => { actual_nodes.Add(a); };
            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_N1_right, actual_N1_right);
            Assert.AreEqual(expected_N8_level, actual_N8_level);

            // after delete
            tree.Delete(n7);

            expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_D0_bbmin = new List<decimal> { 2, 5 };
            expected_D0_bbmax = new List<decimal> { 10, 20 };
            expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_D1_bbmin = new List<decimal> { 2, 5 };
            expected_D1_bbmax = new List<decimal> { 10, 9 };
            expected_nodes = new List<Node> { n0, n1, n2, n3, n8, n5, n6, n9, n10 };
            expected_N1_right = n8;
            expected_N8_level = 2;

            actual_D0 = n0.Value;
            actual_D0_bbmin = n0.Value.BoxMin;
            actual_D0_bbmax = n0.Value.BoxMax;
            actual_D1 = n1.Value;
            actual_D1_bbmin = n1.Value.BoxMin;
            actual_D1_bbmax = n1.Value.BoxMax;
            actual_nodes = new List<Node>();
            actual_N1_right = n1.Right;
            actual_N8_level = n8.Level;

            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            //CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            //Assert.AreEqual(expected_D1, actual_D1);
            //CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            //CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            //CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            //Assert.AreEqual(expected_N1_right, actual_N1_right);
            //Assert.AreEqual(expected_N8_level, actual_N8_level);
        }
                    
        [TestMethod]
        public void Test_Delete_n8()
        {
            // befor delete
            var expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D0_bbmin = new List<decimal> { 1, 1 };
            var expected_D0_bbmax = new List<decimal> { 10, 20 };
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D1_bbmin = new List<decimal> { 1, 1 };
            var expected_D1_bbmax = new List<decimal> { 10, 9 };
            var expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10 };
            var expected_N1_right = n4;
            var expected_N7_level = 3;

            var actual_D0 = n0.Value;
            var actual_D0_bbmin = n0.Value.BoxMin;
            var actual_D0_bbmax = n0.Value.BoxMax;
            var actual_D1 = n1.Value;
            var actual_D1_bbmin = n1.Value.BoxMin;
            var actual_D1_bbmax = n1.Value.BoxMax;
            var actual_nodes = new List<Node>();
            var actual_N1_right = n1.Right;
            var actual_N7_level = n7.Level;

            Action<Node> CalcNodes = (Node a) => { actual_nodes.Add(a); };
            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_N1_right, actual_N1_right);
            Assert.AreEqual(expected_N7_level, actual_N7_level);

            // after delete
            tree.Delete(n8);

            expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {10, 9}
            });
            expected_D0_bbmin = new List<decimal> { 1, 1 };
            expected_D0_bbmax = new List<decimal> { 10, 20 };
            expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {10, 9}
            });
            expected_D1_bbmin = new List<decimal> { 1, 1 };
            expected_D1_bbmax = new List<decimal> { 10, 9 };
            expected_nodes = new List<Node> { n0, n1, n2, n3, n7, n5, n6, n9, n10 };
            expected_N1_right = n7;
            expected_N7_level = 2;

            actual_D0 = n0.Value;
            actual_D0_bbmin = n0.Value.BoxMin;
            actual_D0_bbmax = n0.Value.BoxMax;
            actual_D1 = n1.Value;
            actual_D1_bbmin = n1.Value.BoxMin;
            actual_D1_bbmax = n1.Value.BoxMax;
            actual_nodes = new List<Node>();
            actual_N1_right = n1.Right;
            actual_N7_level = n7.Level;

            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_N1_right, actual_N1_right);
            Assert.AreEqual(expected_N7_level, actual_N7_level);
        }
                    
        [TestMethod]
        public void Test_Delete_n9()
        {
            // befor delete
            var expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D0_bbmin = new List<decimal> { 1, 1 };
            var expected_D0_bbmax = new List<decimal> { 10, 20 };
            var expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            var expected_D1_bbmin = new List<decimal> { 1, 1 };
            var expected_D1_bbmax = new List<decimal> { 10, 9 };
            var expected_D3 = new Data(new List<decimal[]>
            {
                new decimal[] {2, 8},
                new decimal[] {5, 6},
                new decimal[] {10, 9}
            });
            var expected_D3_bbmin = new List<decimal> { 2, 6 };
            var expected_D3_bbmax = new List<decimal> { 10, 9 };
            var expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10 };
            var expected_N1_left = n3;
            var expected_N3_left = n5;
            var expected_N10_level = 4;
            var expected_N10_parent = n5;

            var actual_D0 = n0.Value;
            var actual_D0_bbmin = n0.Value.BoxMin;
            var actual_D0_bbmax = n0.Value.BoxMax;
            var actual_D1 = n1.Value;
            var actual_D1_bbmin = n1.Value.BoxMin;
            var actual_D1_bbmax = n1.Value.BoxMax;
            var actual_D3 = n3.Value;
            var actual_D3_bbmin = n3.Value.BoxMin;
            var actual_D3_bbmax = n3.Value.BoxMax;
            var actual_nodes = new List<Node>();
            var actual_N1_left = n1.Left;
            var actual_N3_left = n3.Left;
            var actual_N10_level = n10.Level;
            var actual_N10_parent = n10.Parent;

            Action<Node> CalcNodes = (Node a) => { actual_nodes.Add(a); };
            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            Assert.AreEqual(expected_D3, actual_D3);
            CollectionAssert.AreEqual(expected_D3_bbmin, actual_D3_bbmin);
            CollectionAssert.AreEqual(expected_D3_bbmax, actual_D3_bbmax);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_N1_left, actual_N1_left);
            Assert.AreEqual(expected_N3_left, actual_N3_left);
            Assert.AreEqual(expected_N10_level, actual_N10_level);
            Assert.AreEqual(expected_N10_parent, actual_N10_parent);

            // after delete
            tree.Delete(n9);

            expected_D0 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {4, 20},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_D0_bbmin = new List<decimal> { 1, 1 };
            expected_D0_bbmax = new List<decimal> { 10, 20 };
            expected_D1 = new Data(new List<decimal[]>
            {
                new decimal[] {1, 1},
                new decimal[] {2, 8},
                new decimal[] {8, 5},
                new decimal[] {10, 9}
            });
            expected_D1_bbmin = new List<decimal> { 1, 1 };
            expected_D1_bbmax = new List<decimal> { 10, 9 };
            expected_D3 = new Data(new List<decimal[]>
            {
                new decimal[] {2, 8},
                new decimal[] {10, 9}
            });
            expected_D3_bbmin = new List<decimal> { 2, 8 };
            expected_D3_bbmax = new List<decimal> { 10, 9 };
            expected_nodes = new List<Node> { n0, n1, n2, n3, n4, n10, n6, n7, n8 };
            expected_N1_left = n3;
            expected_N3_left = n10;
            expected_N10_level = 3;
            expected_N10_parent = n3;

            actual_D0 = n0.Value;
            actual_D0_bbmin = n0.Value.BoxMin;
            actual_D0_bbmax = n0.Value.BoxMax;
            actual_D1 = n1.Value;
            actual_D1_bbmin = n1.Value.BoxMin;
            actual_D1_bbmax = n1.Value.BoxMax;
            actual_D3 = n3.Value;
            actual_D3_bbmin = n3.Value.BoxMin;
            actual_D3_bbmax = n3.Value.BoxMax;
            actual_nodes = new List<Node>();
            actual_N1_left = n1.Left;
            actual_N3_left = n3.Left;
            actual_N10_level = n10.Level;
            actual_N10_parent = n10.Parent;

            tree.ForEach(CalcNodes);

            Assert.AreEqual(expected_D0, actual_D0);
            CollectionAssert.AreEqual(expected_D0_bbmin, actual_D0_bbmin);
            CollectionAssert.AreEqual(expected_D0_bbmax, actual_D0_bbmax);
            Assert.AreEqual(expected_D1, actual_D1);
            CollectionAssert.AreEqual(expected_D1_bbmin, actual_D1_bbmin);
            CollectionAssert.AreEqual(expected_D1_bbmax, actual_D1_bbmax);
            Assert.AreEqual(expected_D3, actual_D3);
            CollectionAssert.AreEqual(expected_D3_bbmin, actual_D3_bbmin);
            CollectionAssert.AreEqual(expected_D3_bbmax, actual_D3_bbmax);
            CollectionAssert.AreEqual(expected_nodes, actual_nodes);
            Assert.AreEqual(expected_N1_left, actual_N1_left);
            Assert.AreEqual(expected_N3_left, actual_N3_left);
            Assert.AreEqual(expected_N10_level, actual_N10_level);
            Assert.AreEqual(expected_N10_parent, actual_N10_parent);
        }

        #endregion

        #region CoDisp
        [TestMethod]
        public void Test_CoDisp_n2()
        {
            var expected_codisp_n2 = 18;
            var expected_complexity = 18;

            var actual_codisp_n2 = tree.CoDisp(n2);
            var actual_complexity = tree.Complexity();

            Assert.AreEqual(expected_codisp_n2, actual_codisp_n2);
            Assert.AreEqual(expected_complexity, actual_complexity);
        }

        [TestMethod]
        public void Test_CoDisp_n6()
        {
            var expected_codisp_n6 = 11;
            var expected_complexity = 18;

            var actual_codisp_n6 = tree.CoDisp(n6);
            var actual_complexity = tree.Complexity();

            Assert.AreEqual(expected_codisp_n6, actual_codisp_n6);
            Assert.AreEqual(expected_complexity, actual_complexity);
        }

        [TestMethod]
        public void Test_CoDisp_n8()
        {
            var expected_codisp_n8 = 6;
            var expected_complexity = 18;

            var actual_codisp_n8 = tree.CoDisp(n8);
            var actual_complexity = tree.Complexity();

            Assert.AreEqual(expected_codisp_n8, actual_codisp_n8);
            Assert.AreEqual(expected_complexity, actual_complexity);
        }
        #endregion

        #region Complexity

        [TestMethod]
        public void Test_Complexity_fullTree()
        {
            var expected_tree_complexity = 18;

            var actual_tree_complexity = tree.Complexity();

            Assert.AreEqual(expected_tree_complexity, actual_tree_complexity);
        }

        [TestMethod]
        public void Test_Complexity_delete_n9()
        {
            var expected_tree_complexity = 13;

            tree.Delete(n9);
            var actual_tree_complexity = tree.Complexity();

            Assert.AreEqual(expected_tree_complexity, actual_tree_complexity);
        }

        [TestMethod]
        public void Test_Complexity_delete_n2()
        {
            var expected_tree_complexity = 12;

            tree.Delete(n2);
            var actual_tree_complexity = tree.Complexity();

            Assert.AreEqual(expected_tree_complexity, actual_tree_complexity);
        }
        #endregion

        #region Find

        [TestMethod]
        public void Test_FindRoot()
        {
            var expected_find = n0;

            var actual_find = tree.Find(d0);

            Assert.AreEqual(expected_find, actual_find);
        }

        [TestMethod]
        public void Test_FindN6()
        {
            var expected_find = n6;

            var actual_find = tree.Find(d6);

            Assert.AreEqual(expected_find, actual_find);
        }

        [TestMethod]
        public void Test_Find_NotExistObject()
        {
            Node expected_find = null;

            var actual_find = tree.Find(new Data(new List<decimal[]> { new decimal[] { 99, 9 } }));

            Assert.AreEqual(expected_find, actual_find);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_Find_nullObject()
        {
            var actual_find = tree.Find(null);
        }


        #endregion

        #region ForEach

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_ForEach_StartFromNull()
        {
            tree.ForEach((Node a) => { }, null);
        }

        [TestMethod]
        public void Test_ForEach_FromRoot()
        {
            var expected_node_count = 11;


            int actual_node_count = 0;
            tree.ForEach((Node a) => { actual_node_count++; });

            Assert.AreEqual(expected_node_count, actual_node_count);
        }

        [TestMethod]
        public void Test_ForEach_FromN3()
        {
            var expected_node_count = 5;


            int actual_node_count = 0;
            tree.ForEach((Node a) => { actual_node_count++; }, n3);

            Assert.AreEqual(expected_node_count, actual_node_count);
        }

        #endregion

        #region Replace

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_Replace_nullOldObject()
        {
            tree.Replace(null, n2);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_Replace_nullNewObject()
        {
            tree.Replace(n2, null);
        }

        [TestMethod]
        public void Test_Replace_n5_with_n4()
        {
            // before replace
            var expected_n3Left = n5;
            var expected_n5Parent = n3;

            var actual_n3Left = n3.Left;
            var actual_n5Parent = n5.Parent;

            Assert.AreEqual(expected_n3Left, actual_n3Left);
            Assert.AreEqual(expected_n5Parent, actual_n5Parent);


            // after replace
            tree.Replace(n5, n4);

            expected_n3Left = n4;
            expected_n5Parent = null;

            actual_n3Left = n3.Left;
            actual_n5Parent = n5.Parent;

            Assert.AreEqual(expected_n3Left, actual_n3Left);
            Assert.AreEqual(expected_n5Parent, actual_n5Parent);
        }

        #endregion

        #region Sibling

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_SiblingNullObject()
        {
            tree.Siblings(null);
        }

        [TestMethod]
        public void Test_SiblingRoot()
        {
            Node expected_sibling = null;

            var actual_sibling = tree.Siblings(tree.RootNode);
        }

        [TestMethod]
        public void Test_SiblingN4()
        {
            Node expected_sibling = n3;

            var actual_sibling = tree.Siblings(n4);
        }

        #endregion

        #region UpwardForEach

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void Test_UpwardForEach_nullObject()
        {
            tree.ForEach((Node a)=> { }, null);
        }

        [TestMethod]
        public void Test_UpwardForEachN3()
        {
            var expected_nodes_count = 3;

            var actual_nodes_count = 0;
            tree.UpwardForEach((Node a) => { actual_nodes_count++; }, n3);

            Assert.AreEqual(expected_nodes_count, actual_nodes_count);

        }
        #endregion


    }
}
