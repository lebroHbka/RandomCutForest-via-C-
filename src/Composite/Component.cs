using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Random_Cut_Forest.src.Composite
{
    abstract class Component
    {
        public static Component Root { get; set; }
        public int Level { get; set; }

        public Component Left { get; set; }
        public Component Right { get; set; }
        public Component Parent { get; set; }
        
        public Data DataPoints { get; set; }

        public abstract void Split();
        public abstract void Show();
    }
}
