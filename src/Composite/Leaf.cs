using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Random_Cut_Forest.src.Composite
{
    class Leaf : Component
    {
        public Leaf(Data data)
        {
            DataPoints = data;
        }

        public override void Split()
        {
            //throw new NotImplementedException();
        }
        public override void Show()
        {
            Console.WriteLine(new string('-', Level*2) + DataPoints.ToString());
        }
    }
}
