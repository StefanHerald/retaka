using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2022Template
{
    internal class Scene
    {
        // the object of the screen to be traced
        internal List<Primitive> objects = new List<Primitive>();
        //the lights of the scene
        internal List<Light> lights = new List<Light>();
        public Scene(){}
    }
}
