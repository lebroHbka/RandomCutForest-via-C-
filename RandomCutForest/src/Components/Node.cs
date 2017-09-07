using System;
using System.Collections.Generic;
using System.Text;

namespace RandomCutForest.src.Components

{
    public class Node
    {
        public Data Value { get; }
        public Node Parent { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public IEnumerable<Node> Children { get { yield return Left; yield return Right; } }
        public int Level { get; set; }


        public Node(Data data)
        {
            Value = data;
        }

        public bool IsLeaf()
        {
            /*
             *      Check has this node children or no.
             */
            if ((Left == null) && (Right == null))
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            /*
             *      Compare exactly Data object that store both nodes.
             */
            if (!(obj is Node))
                return false;
            var data = (obj as Node).Value;
            return Value.Equals(data);
        }

        public override string ToString()
        {
            return new string('-', Level * 2) + Value.ToString() + "\n";
        }
    }
}
