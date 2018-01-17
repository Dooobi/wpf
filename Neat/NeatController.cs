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
            int numGenesLargerGenome, numExcessGenes = 0, numDisjointGenes = 0, numMatchingGenes = 0;
            double sumWeightDifference = 0.0, avgWeightDifference;
            
            Dictionary<int, ConnectionGene> genesByInnovationNumber1 = genome1.GetConnectionGenesByInnovationNumber();
            Dictionary<int, ConnectionGene> genesByInnovationNumber2 = genome1.GetConnectionGenesByInnovationNumber();
            int smallerMaxInnovationNumber = Math.Min(genesByInnovationNumber1.Keys.Max(), genesByInnovationNumber2.Keys.Max());
            int higherMaxInnovationNumber = Math.Max(genesByInnovationNumber1.Keys.Max(), genesByInnovationNumber2.Keys.Max());

            for (int i = 1; i <= higherMaxInnovationNumber; i++)
            {
                ConnectionGene gene1 = genesByInnovationNumber1[i];
                ConnectionGene gene2 = genesByInnovationNumber2[i];

                if (gene1 != null && gene2 == null
                    || gene1 == null && gene2 != null)
                {
                    // The connectionGene is either disjoint or excess
                    if (i > smallerMaxInnovationNumber)
                    {
                        // It's excess
                        numExcessGenes++;
                    }
                    else
                    {
                        // It's disjoint
                        numDisjointGenes++;
                    }
                }
                else if (gene1 != null && gene2 != null)
                {
                    // The connectionGenes are matching
                    numMatchingGenes++;
                    sumWeightDifference += Math.Abs(gene1.Weight - gene2.Weight);
                }
            }

            numGenesLargerGenome = Math.Max(genome1.ConnectionGenes.Count, genome2.ConnectionGenes.Count);
            avgWeightDifference = sumWeightDifference / numMatchingGenes;

            return (Config.compatibilityCoefficientNumExcessGenes * numExcessGenes) / numGenesLargerGenome + (Config.compatibilityCoefficientNumDisjointGenes * numDisjointGenes) / numGenesLargerGenome + Config.compatibilityCoefficientAvgWeightDifference * avgWeightDifference;
        }
    }
}
