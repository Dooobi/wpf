using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class NeatController
    {
        public Stack<Genome> CurrentPopulationBeforeEvaluation { get; set; }
        public List<Genome> CurrentPopulationAfterEvaluation { get; set; }

        public NeatController()
        {
            History.InitHistory();
        }

        //------------------ SpeciateAndCalculateSpawnLevels ---------------------
        //
        //  separates each individual into its respective species by calculating
        //  a compatibility score with every other member of the population and 
        //  niching accordingly. The function then adjusts the fitness scores of
        //  each individual by species age and by sharing and also determines
        //  how many offspring each individual should spawn.
        //------------------------------------------------------------------------
        private void Speciate()
        {
            foreach (Genome genome in CurrentPopulationAfterEvaluation)
            {
                foreach (Species species in History.Speciess[History.LastGeneration])
                {
                    double compatibility = CalculateCompatibility(genome, species.LeaderGenome);

                    if (compatibility <= Config.compatibilityThreshold)
                    {
                        genome.Species = species;
                        species.Population.Add(genome);

                    }
                }
            }
        }

        public Genome GetNextGenomeBeforeEvaluation()
        {
            return CurrentPopulationBeforeEvaluation.Pop();
        }

        public void SubmitGenomeAfterEvaluation(Genome genome)
        {
            CurrentPopulationAfterEvaluation.Add(genome);
            
            if (CurrentPopulationAfterEvaluation.Count == Config.populationSize)
            {
                Speciate();

                // Determine species
                // Save to file
                // Render
                // Create population for next generation by
                //   - elitism
                //   - crossover
                //   - mutation
            }
        }

        private void CreatePopulation()
        {
            CurrentPopulationBeforeEvaluation = new Stack<Genome>();

            for (int i = 0; i < Config.populationSize; i++)
            {
                CurrentPopulationBeforeEvaluation.Push(new Genome("" + i, Config.numberOfInputs, Config.numberOfOutputs));
            }
        }

        private double CalculateCompatibility(Genome genome1, Genome genome2)
        {
            double numGenesLargerGenome, numExcessGenes, numDisjointGenes, avgWeightDifference;
            double coefficient1, coefficient2, coefficient3;

            return (coefficient1 * numExcessGenes) / numGenesLargerGenome + (coefficient2 * numDisjointGenes) / numGenesLargerGenome + coefficient3 * avgWeightDifference;
        }
    }
}
