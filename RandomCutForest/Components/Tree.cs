using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomCutForest.Components
{
    /// <summary>
    /// This class represent tree that stored in Random Cut Forest.
    /// </summary>
    public class Tree
    {
        public Node RootNode { get; set; }

        #region Constructors

        /// <summary>
        /// Initialize tree with special root node.
        /// </summary>
        /// <param name="node">Root node</param>
        public Tree(Node node)
        {
            RootNode = node;
        }
        /// <summary>
        /// Initialize tree with special root node.
        /// </summary>
        /// <param name="node">Root node</param>
        public Tree(Data data) : this(new Node(data))
        {
        }

        #endregion

        #region Create tree

        /// <summary>
        /// Build tree by spliting root node, untill all point will be leafs.
        /// </summary>
        public void MakeTree()
        {
            /*
             *      Make tree by spliting each node(not leaf) for 2 node and recurce until
             *      all points will be in leafs
             */

            // queue for nodes, instead recursion for split
            var nodes = new Queue<Node>();
            nodes.Enqueue(RootNode);

            while (nodes.Count > 0)
            {
                var n = nodes.Dequeue();
                Data nData = n.Value;

                // if node has enough points inside, make split
                if (nData.Length > 1)
                {
                    // split data 
                    Data l, r;
                    Node left, right;
                    nData.Split(out l, out r);

                    // make nodes from splited data
                    left = new Node(l);
                    right = new Node(r);

                    // set level, parent for splited nodes
                    left.Parent = n;
                    left.Level = (byte)(n.Level + 1);

                    right.Parent = n;
                    right.Level = (byte)(n.Level + 1);

                    // set new children for node that split tham
                    n.Left = left;
                    n.Right = right;

                    // add splited node in queue
                    nodes.Enqueue(left);
                    nodes.Enqueue(right);
                }
            }
        }

        #endregion

        #region Add/Delete

        /// <summary>
        ///   Add new point in tree by algorithm.
        /// </summary>
        /// <param name="newNode">New node that will be inserted in tree</param>
        public void Add(Node newNode)
        {
            Node newParent;
            var node = RootNode;
            while (true)
            {
                // merge node with new point
                newParent = new Node(Data.Merge(node.Value, newNode.Value));

                // make split from merged node
                Data parentLeft, parentRight;
                newParent.Value.Split(out parentLeft, out parentRight);

                // new split separete old node and new point
                if ((parentLeft.IsInside(node.Value)) || (parentRight.IsInside(node.Value)))
                {
                    // create new parent with child - node, newNode(newData)
                    if (parentLeft.IsInside(node.Value))
                    {
                        newParent.Left = node;
                        newParent.Right = newNode;
                    }
                    else
                    {
                        newParent.Left = newNode;
                        newParent.Right = node;
                    }

                    // create new root
                    if (node.Parent == null)
                        RootNode = newParent;
                    else
                        Replace(node, newParent);

                    // change dependency
                    node.Parent = newParent;
                    newNode.Parent = newParent;
                    break;
                }
                // not separate node from new point
                else
                {
                    var newNodePos = node.Value.Position(newNode.Value);
                    if (newNodePos == "left")
                        node = node.Left;
                    else
                        node = node.Right;
                }
            }
            // after inserting new point need update levels and add new points in nodes
            Action<Node> UpdateLevel = (Node a) =>
            {
                if (a.Parent == null)
                    a.Level = 0;
                else
                    a.Level = (byte)(a.Parent.Level + 1);
            };
            ForEach(UpdateLevel, newParent);

            Action<Node> UpdateNodes = (Node a) => { a.Value.AddPoints(newNode.Value); };
            UpwardForEach(UpdateNodes, newParent);
        }

        /// <summary>
        /// Delete node(might be exists) from tree.
        /// </summary>
        /// <param name="node">Node that will be delete</param>
        public void Delete(Node node)
        {
            /*
             *      Delete node - parent of deleted node has only 1 child(after deleting)
             *      so he replace with this child.
             */
            if (node == null)
                return;

            var sibling = Siblings(node);

            Replace(node.Parent, sibling);

            // delete old node
            Action<Node> UpdateNodes = (Node a) => { a.Value.RemovePoints(node.Value); };
            UpwardForEach(UpdateNodes, sibling.Parent);
        }

        #endregion

        #region Anomaly/Complexity

        /// <summary>
        /// Method calculate and return CoDisp of node(might be exists).
        /// </summary>
        /// <param name="node">Node that calculate CoDisp</param>
        /// <returns>CoDisp value</returns>
        public int CoDisp(Node node)
        {
            var parent = node.Parent;
            if (parent == null)
                return 0;

            // calculate current complexity
            int coDisp = Complexity();

            // temporary delete children of node parent(imitation of deleting) and calculate new complexity
            var oldChildren = parent.Children.ToList();
            var newChildren = new List<Node> { null, null };
            ChangeChildren(parent, newChildren);

            // calculate difference
            coDisp -= Complexity() - parent.Level;

            // restore children
            ChangeChildren(parent, oldChildren);
            return coDisp;
        }

        /// <summary>
        /// Calculate complexity of tree - sum of all leafs levels.
        /// </summary>
        /// <returns></returns>
        public int Complexity()
        {
            int compexity = 0;

            Action<Node> CalcLevels = (Node a) => { if (a.IsLeaf()) compexity += a.Level; };
            ForEach(CalcLevels);
            return compexity;
        }

        #endregion

        /// <summary>
        /// Set new children from list to parent node.
        /// </summary>
        /// <param name="parent">Node that need change children</param>
        /// <param name="children">New children for parent node</param>
        public void ChangeChildren(Node parent, List<Node> children)
        {
            if (children.Count != 2)
                throw new ArgumentException("Parent can get only 2 new child");
            parent.Left = children[0];
            parent.Right = children[1];
        }

        /// <summary>
        /// Find node that contain data
        /// </summary>
        /// <param name="data">Data that need find</param>
        /// <returns></returns>
        public Node Find(Data data)
        {
            // <nodes> - collection with all nodes that contain <data>
            // start value roots children
            if (data == null)
                throw new ArgumentNullException("Finding node can't contain null data");

            var includeNode = RootNode;

            while (true)
            {
                if (includeNode.Value.Equals(data))
                    break;
                else if(includeNode.IsLeaf())   // not equal and leaf -> no such node with data
                {
                    includeNode = null;
                    break;
                }
                else                            // figure out in that direction move(left, right child)
                {
                    includeNode = (includeNode.Left.Value.IsInside(data)) ? includeNode.Left : includeNode.Right;
                }
            }
            return includeNode;
        }

        /// <summary>
        /// Iterate over all tree(breadth-first search)
        /// </summary>
        /// <param name="action">Action</param>
        public void ForEach(Action<Node> action)
        {
            ForEach(action, RootNode);
        }

        /// <summary>
        /// Iterate over all tree(breadth-first search), starting from start node
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="start">Start node</param>
        public void ForEach(Action<Node> action, Node start)
        {
            /*
             *    Iterate over all nodes(branches) that has started from <start> node
             */
            if (start == null)
                return;
            var nodes = new Queue<Node>();
            action(start);
            foreach (var c in start.Children)
                if (c != null)
                    nodes.Enqueue(c);

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                action(node);

                foreach (var child in node.Children)
                {
                    if (child != null)
                        nodes.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Replace old branch(start in node "old") with new(start in node "new")
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
        public void Replace(Node old, Node @new)
        {
            /*
             *   All "old" node branch is gone.
             *   (!) If @new has parent he still has child @new
             *   
             *   Level will be update
             */

            if ((old == null) || (@new == null))
                throw new ArgumentNullException("Can't replace null node");

            if (old.Parent == null)
            {
                RootNode = @new;
                @new.Parent = null;
            }
            else
            {
                // change dependencies
                @new.Parent = old.Parent;
                if (old.Parent.Left == old)
                    old.Parent.Left = @new;
                else
                    old.Parent.Right = @new;
                old.Parent = null;
            }

            // update levels in nested nodes in newNode
            Action<Node> Update = (Node a) =>
            {
                a.Level = (a.Parent != null) ? (byte)(a.Parent.Level + 1) : (byte)0;
            };

            ForEach(Update, @new);
        }

        /// <summary>
        /// Get sibling(nodes that has same parent with "node") of "node"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node Siblings(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("Null node has no siblings");

            if (node.Parent != null)
            {
                foreach (var parentChild in node.Parent.Children)
                {
                    if (parentChild != node)
                        return parentChild;
                }
            }
            return null;
        }


        public override string ToString()
        {
            var str = new StringBuilder();
            var nodes = new Stack<Node>();
            nodes.Push(RootNode);

            while (nodes.Count > 0)
            {
                var n = nodes.Pop();
                str.Append(n.ToString());

                if (n.Right != null)
                    nodes.Push(n.Right);
                if (n.Left != null)
                    nodes.Push(n.Left);
            }
            return str.ToString();
        }

        /// <summary>
        /// Iterate upside down, start from "start" node, and came up to root
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="start">Start position</param>
        public void UpwardForEach(Action<Node> action, Node start)
        {
            if (start == null)
                return;
            var node = start;

            while (node != null)
            {
                action(node);
                node = node.Parent;
            }
        }

    }
}
