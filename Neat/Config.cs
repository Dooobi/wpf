using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public static class Config
    {
        public static int populationSize = 100;
        public static int numberOfInputs = 2;
        public static int numberOfOutputs = 3;
        public static double compatibilityThreshold = 3.0;
        public static double compatibilityCoefficientNumExcessGenes = 1.0;
        public static double compatibilityCoefficientNumDisjointGenes = 1.0;
        public static double compatibilityCoefficientAvgWeightDifference = 0.4;
    }
}
