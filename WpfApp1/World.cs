using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class World
    {
        private List<Dictionary<int, Species>> world;

        public World()
        {
            world = new List<Dictionary<int, Species>>();
        }
        
        public void GetPopulation(int g, int s)
        {
            Species species = world[g][s];
        }
    }
}
