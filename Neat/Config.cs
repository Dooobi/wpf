using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public static class Config
    {
        public static int populationSize = 200;
        public static int numberOfInputs = 2;
        public static int numberOfOutputs = 1;
        public static double minWeightLimit = -1.0;
        public static double maxWeightLimit = 1.0;
        public static double compatibilityThreshold = 1.0;
        public static double compatibilityCoefficientNumExcessGenes = 1.0;
        public static double compatibilityCoefficientNumDisjointGenes = 1.0;
        public static double compatibilityCoefficientAvgWeightDifference = 0.4;
        public static double genomesFromElitismRatio = 0.05;
        public static double offspringFromMutationRatio = 0.25;
        public static double minChanceForSelectionToCrossover = 0.05;
        public static double maxChanceForSelectionToCrossover = 0.4;
        public static double minChanceForSelectionToMutate = 0.05;
        public static double maxChanceForSelectionToMutate = 0.4;
        public static double chanceForInterspeciesCrossover = 0.001;
        public static double chanceToDisableConnectionGeneIfDisabledInEitherParent = 0.75;
        public static double chanceToMutateWeightsOfGenome = 0.8;
        public static double chanceToMutateOneWeight = 0.9;
        public static double chanceForNewConnectionMutation = 0.07;
        public static double chanceForNewNeuronMutation = 0.04;
        public static double maxWeightPerturbation = 0.5;
        public static double chanceForRandomizingWeight = 0.1;
    }
}
