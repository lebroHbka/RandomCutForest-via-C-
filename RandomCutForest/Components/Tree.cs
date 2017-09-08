using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomCutForest.Components
{
    public class Tree
    {
        public Node RootNode { get; set; }

        #region Constructors

        public Tree(Node root)
        {
            RootNode = root;
        }

        public Tree(Data data)
        {
            RootNode = new Node(data);
        }

        #endregion

        #region Create tree

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

        public void Add(Node newNode)
        {
            /*
             *      Add new point in tree by algorithm.
             *           
             */

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
                    newParent.Left = (parentLeft.IsInside(node.Value)) ? node : newNode;
                    newParent.Right = (parentRight.IsInside(node.Value)) ? node : newNode;

                    // create new root
                    if (node.Parent == null)
                    {
                        RootNode = newParent;
                    }
                    else
                    {
                        Replace(node, newParent);
                    }
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
            UpwardForEach(UpdateNodes, sibling);
        }

        #endregion

        #region Anomaly/Complexity

        public int CoDisp(Node node)
        {
            /*
             *      Calculate CoDisp.
             *      
             *      
             */
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

        public int Complexity()
        {
            /*
             *      Calculate complexity of tree - sum of all leafs levels.
             */
            int compexity = 0;

            Action<Node> CalcLevels = (Node a) => { if (a.IsLeaf()) compexity += a.Level; };
            ForEach(CalcLevels);
            return compexity;
        }

        #endregion

        public void ChangeChildren(Node parent, List<Node> children)
        {
            /*
             *      Set new children from list.
             */
            if (children.Count != 2)
                throw new ArgumentException("Parent can get only 2 new child");
            parent.Left = children[0];
            parent.Right = children[1];
        }

        public Node Find(Data data)
        {
            /*
             *   Find node that contain DataContainer - <data>
             */

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

        public void ForEach(Action<Node> action)
        {
            /*
             *    Iterate over all tree
             */
            ForEach(action, RootNode);
        }

        public void ForEach(Action<Node> action, Node start)
        {
            /*
             *    Iterate over all nodes(branches) that has started from <start> node
             */
            if (start == null)
                throw new ArgumentNullException("ForEach can't iterate from null node");
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

        public void Replace(Node old, Node @new)
        {
            /*
             *   Replace <old> node with <new> node
             *   
             *   All <old> node branch is gone.
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

        public Node Siblings(Node node)
        {
            /*
             *    Get all siblings(nodes that has same parent with node) of <node>
             */
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

        public void UpwardForEach(Action<Node> action, Node start)
        {
            /*
             *    Iterate upside down, start from <start> node, and came up to root
             */
            if (start == null)
                throw new ArgumentNullException("UpwardForEach can't iterate from null node");
            var node = start;

            while (node != null)
            {
                action(node);
                node = node.Parent;
            }
        }

    }
}
