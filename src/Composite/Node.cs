using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Random_Cut_Forest.src.Composite
{
    class Node : Component
    {
        public Node(Data data)
        {
            DataPoints = data;
        }

        public void BuildTree()
        {
            Parent = null;
            Root = this;
            Level = 0;
            Split();
        }


        public override void Split()
        {
            if (DataPoints.PointsCount >= 2)
            {
                Data leftData;
                Data rightData;

                DataPoints.DividePoint(out leftData, out rightData);

                Left = (leftData.PointsCount > 1) ? (new Node(leftData)) as Component: 
                                                    (new Leaf(leftData) as Component);
                Left.Parent = this;
                Left.Level = Level + 1;
                Left.Split();


                Right = (rightData.PointsCount > 1) ? (new Node(rightData)) as Component :
                                                      (new Leaf(rightData) as Component);
                Right.Parent = this;
                Right.Level = Level + 1;
                Right.Split();
            }
        }

        public override void Show()
        {
            Console.WriteLine(new string('-', Level * 2) + DataPoints.ToString());
            Left.Show();
            Right.Show();

        }


    }
}
