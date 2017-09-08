using System.Collections.Generic;

namespace RandomCutForest.Components

{
    public class Node
    {

        #region Vars

        public Data Value { get; }
        public Node Parent { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public IEnumerable<Node> Children { get { yield return Left; yield return Right; } }
        public byte Level { get; set; }

        #endregion

        #region Constructor

        public Node(Data data)
        {
            Value = data;
        }

        #endregion

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
