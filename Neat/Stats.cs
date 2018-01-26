using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public static class Stats
    {
        public static double highestCompatibilityValue = -1.0;

        public static void UpdateHighestCompatibilityValue(double compatibility)
        {
            if (compatibility > highestCompatibilityValue)
            {
                highestCompatibilityValue = compatibility;
            }
        }
    }
}
