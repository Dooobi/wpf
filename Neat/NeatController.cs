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
                List<Species> availableSpeciesForThisGeneration = new List<Species>(History.Speciess[History.PreviousGeneration]);
                availableSpeciesForThisGeneration.AddRange(History.Speciess[History.CurrentGeneration]);

                foreach (Species species in availableSpeciesForThisGeneration)
                {
                    Genome leaderGenome = species.SpeciesTimestamps[History.PreviousGeneration].Leader;
                    if (leaderGenome == null)
                    {
                        leaderGenome = species.SpeciesTimestamps[History.CurrentGeneration].Leader;
                    }

                    double compatibility = CalculateCompatibility(genome, leaderGenome);

                    if (compatibility <= Config.compatibilityThreshold)
                    {
                        genome.Species = species;
                        species.Population.Add(genome);
                    }
                }

                // Genome is not compatible with any existing species -> create a new species
                string speciesId = "g" + History.CurrentGeneration.Number + "s" + (History.Speciess[History.CurrentGeneration].Count + 1);
                Species newSpecies = new Species(speciesId);
                SpeciesTimestamp speciesTimestamp = new SpeciesTimestamp(newSpecies);
                newSpecies.SpeciesTimestamps[History.CurrentGeneration] = speciesTimestamp;
                speciesTimestamp.Leader = genome;
                newSpecies.Population.Add(genome);
                newSpecies.BestFitness = genome.Fitness;
                genome.Species = newSpecies;
                History.Speciess[History.CurrentGeneration].Add(newSpecies);
            }

            // Determine leaders for the speciess in the current generation
            foreach (Species species in History.Speciess[History.CurrentGeneration])
            {
                species.SpeciesTimestamps[History.CurrentGeneration].SelectLeader();
            }

        }

        private void AdjustFitness()
        {
            foreach (Species species in History.Speciess[History.CurrentGeneration])
            {
                foreach (Genome genome in species.PopulationByGeneration[History.CurrentGeneration])
                {
                    double adjustedFitness = genome.Fitness / species.PopulationByGeneration[History.CurrentGeneration].Count;
                    genome.AdjustedFitness = adjustedFitness;
                }
            }
        }

        private void DetermineAmountToBreed()
        {
            Generation currentGeneration = History.CurrentGeneration;

            double amountToBreed;
            double totalAverageAdjustedFitness = 0.0;

            // Calculate average adjusted fitness and total adjusted fitness for species in this generation
            foreach (Species species in History.Speciess[currentGeneration])
            {
                double averageAdjustedFitness = species.SpeciesTimestamps[currentGeneration].CalculateAverageAdjustedFitness();
                totalAverageAdjustedFitness += averageAdjustedFitness;
            }

            // Needed for rounding
            double limitForRoundingUp = 1.0 / Config.populationSize;
            double modifierForCorrectRounding = (0.5 - limitForRoundingUp);

            int totalAmountToBreed = 0;

            // Using the calculated averages we can calculate the amount of genomes a species should breed
            foreach (Species species in History.Speciess[currentGeneration])
            {
                SpeciesTimestamp speciesTimestamp = species.SpeciesTimestamps[currentGeneration];
                double averageAdjustedFitness = species.SpeciesTimestamps[currentGeneration].CalculateAverageAdjustedFitness();
                amountToBreed = (averageAdjustedFitness / totalAverageAdjustedFitness) * Config.populationSize;

                // Round to int
                speciesTimestamp.AmountToBreed = (int)Math.Round(amountToBreed + modifierForCorrectRounding, MidpointRounding.AwayFromZero);

                // Total up the amounts to check if it matches the populationSize
                totalAmountToBreed += speciesTimestamp.AmountToBreed;
            }

            int targetAmountDifference = totalAmountToBreed - Config.populationSize;


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
                Epoch();
            }
        }

        private void Epoch()
        {
            Generation newGeneration = new Generation(History.CurrentGeneration.Number + 1);
            History.Generations.Add(newGeneration);

            Speciate();
            AdjustFitness();
            DetermineAmountToBreed();

            foreach (Genome genome in CurrentPopulationAfterEvaluation)
            {
                History.CurrentGeneration.AddGenome(genome);
            }

            // Determine species
            // Save to file
            // Render
            // Create population for next generation by
            //   - elitism
            //   - crossover
            //   - mutation
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
