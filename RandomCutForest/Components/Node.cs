using System.Collections.Generic;

namespace RandomCutForest.Components
{
    /// <summary>
    /// This class represent node that stored in tree.
    /// </summary>
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

        /// <summary>
        /// Check has this node children or no.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            if ((Left == null) && (Right == null))
                return true;
            return false;
        }

        /// <summary>
        /// Compare exactly Data objects that store both nodes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
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
