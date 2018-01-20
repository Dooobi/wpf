using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class GeneticAlgorithm
    {
        public List<Genome> Epoch(List<Genome> populationOfCurrentGeneration)
        {
            List<Genome> populationForNextGeneration = new List<Genome>();

            Generation newGeneration = new Generation(History.CurrentGeneration.Number + 1);
            History.Generations.Add(newGeneration);

            Speciate(populationOfCurrentGeneration);
            AdjustFitness();
            DetermineAmountToGenerateForEachSpecies();

            foreach (Genome genome in populationOfCurrentGeneration)
            {
                History.CurrentGeneration.AddGenome(genome);
            }

            List<Genome> elitists = PerformElitism();

            // Crossover (AmountToGeneratByCrossover)
            // The chance for getting selected to reproduce linearly increases
            // the better the fitness of a Genome is, relative to the fitness of the other Genomes in the same Species
            List<Genome> offspring = PerformCrossover();

            // Mutation (Applied to every genome but only with a certain percentage)

            populationForNextGeneration.AddRange(elitists);
            // Determine species
            // Save to file
            // Render
            // Create population for next generation by
            //   - elitism
            //   - crossover
            //   - mutation

            return populationForNextGeneration;
        }

        //------------------ SpeciateAndCalculateSpawnLevels ---------------------
        //
        //  separates each individual into its respective species by calculating
        //  a compatibility score with every other member of the population and 
        //  niching accordingly.
        //------------------------------------------------------------------------
        private void Speciate(List<Genome> evaluatedPopulation)
        {
            foreach (Genome genome in evaluatedPopulation)
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

        public List<Genome> PerformElitism()
        {
            List<Genome> elitists = new List<Genome>();

            foreach (Species species in History.Speciess[History.CurrentGeneration])
            {
                SpeciesTimestamp speciesTimestamp = species.SpeciesTimestamps[History.CurrentGeneration];

                IEnumerable<Genome> genomesOrderedByFitness = speciesTimestamp.Members.OrderByDescending(genome => genome.Fitness);

                int i = 0;
                foreach (Genome genome in genomesOrderedByFitness)
                {
                    if (i < speciesTimestamp.AmountToGenerateByElitism)
                    {
                        elitists.Add(new Genome(genome));
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }
            }

            return elitists;
        }

        public List<Genome> PerformCrossover()
        {
            List<Genome> offspring = new List<Genome>();

            foreach (Species species in History.Speciess[History.CurrentGeneration])
            {
                SpeciesTimestamp speciesTimestamp = species.SpeciesTimestamps[History.CurrentGeneration];

                bool isInterspeciesCrossover = CheckInterspeciesCrossover();

                // To allow crossover either
                //  the Species needs at least 2 members 
                //  or interspecies crossover must be active
                if (speciesTimestamp.Members.Count > 1 || isInterspeciesCrossover)
                {
                    // Select parent1 for crossover (list needs to be sorted by fitness)
                    speciesTimestamp.Members.Sort((genome1, genome2) => genome1.Fitness.CompareTo(genome2.Fitness));
                    Genome parent1 = SelectGenomeForCrossover(speciesTimestamp.Members, null);

                    // Select parent2 for crossover
                    Genome parent2;
                    if (isInterspeciesCrossover)
                    {
                        // In case of interspecies crossover the second parent will
                        // be selected from a list of all the Genomes from this Generation (no matter the Species)
                        History.CurrentGeneration.Population.Sort((genome1, genome2) => genome1.Fitness.CompareTo(genome2.Fitness));
                        // Parent1 must be excluded from the list of available Genomes (second parameter)
                        parent2 = SelectGenomeForCrossover(History.CurrentGeneration.Population, parent1);
                    }
                    else
                    {
                        // In case of regular crossover the second parent will just be selected
                        // from a list of Genomes within the same species as parent1 (but parent1 must be excluded)
                        parent2 = SelectGenomeForCrossover(speciesTimestamp.Members, parent1);
                    }

                    // Actually perform the crossover and put the child 
                    // into the list of Genomes for the next Generation
                    Genome child = ActuallyCrossover(parent1, parent2);
                    offspring.Add(child);
                }
            }

            return offspring;
        }
        
        /*
         * Uses the configured minChanceForSelectionToCrossover and maxChanceForSelectionToCrossover
         * to select a Genome from a list of Genomes which is sorted by fitness (lower index = lower fitness)
         */
        private Genome SelectGenomeForCrossover(List<Genome> availableGenomes, Genome excludedFromAvailableGenomes)
        {
            // Copy the list and remove the excluded Genome in the copied list
            List<Genome> actuallyAvailableGenomes = new List<Genome>(availableGenomes);
            if (excludedFromAvailableGenomes != null)
            {
                availableGenomes.Remove(excludedFromAvailableGenomes);
            }

            int highestIndex = actuallyAvailableGenomes.Count - 1;
            double gradientOfChanceForSelection = (Config.maxChanceForSelectionToCrossover - Config.minChanceForSelectionToCrossover) / highestIndex;
            double rand = Utils.RandDouble(Config.minChanceForSelectionToCrossover, Config.maxChanceForSelectionToCrossover);

            // Go through the list of genomes from best to worst (highest chance to lowest chance)
            // and check if it has been selected by the random number
            double currentChance;
            for (int i = highestIndex, c = 0; i >= 0; i--, c++)
            {
                currentChance = Config.maxChanceForSelectionToCrossover - c * gradientOfChanceForSelection;

                if (rand >= currentChance)
                {
                    return actuallyAvailableGenomes[i];
                }
            }

            return null;
        }

        private bool CheckInterspeciesCrossover()
        {
            return (History.Speciess[History.CurrentGeneration].Count >= 2
                    && Utils.random.NextDouble() <= Config.chanceForInterspeciesCrossover);
        }

        private Genome ActuallyCrossover(Genome parent1, Genome parent2)
        {
            Genome child;



            return child;
        }

        /**
         * Determines how many genomes are generated by:
         *  - elitism
         *  - crossover
         */
        private void DetermineAmountToGenerateForEachSpecies()
        {
            Generation currentGeneration = History.CurrentGeneration;
            List<Species> speciesOfThisGeneration = History.Speciess[currentGeneration];

            double amountToBreed;
            double totalAverageAdjustedFitness = 0.0;

            // Calculate average adjusted fitness and total adjusted fitness for species in this generation
            foreach (Species species in speciesOfThisGeneration)
            {
                double averageAdjustedFitness = species.SpeciesTimestamps[currentGeneration].CalculateAverageAdjustedFitness();
                totalAverageAdjustedFitness += averageAdjustedFitness;
            }

            // Needed for rounding
            double limitForRoundingUp = 1.0 / Config.populationSize;
            double modifierForCorrectRounding = (0.5 - limitForRoundingUp);

            int totalAmountToSustain = 0;

            // Using the calculated averages we can calculate the amount of genomes a species should breed
            foreach (Species species in speciesOfThisGeneration)
            {
                SpeciesTimestamp speciesTimestamp = species.SpeciesTimestamps[currentGeneration];
                double averageAdjustedFitness = species.SpeciesTimestamps[currentGeneration].CalculateAverageAdjustedFitness();
                amountToBreed = (averageAdjustedFitness / totalAverageAdjustedFitness) * Config.populationSize;

                // Round to int
                speciesTimestamp.AmountToGenerateForNextGeneration = (int)Math.Round(amountToBreed + modifierForCorrectRounding, MidpointRounding.AwayFromZero);

                // Total up the amounts to check if it matches the populationSize
                totalAmountToSustain += speciesTimestamp.AmountToGenerateForNextGeneration;
            }

            // The totalAmountToSustain might be to high/low because of rounding errors
            int targetAmountDifference = Config.populationSize - totalAmountToSustain;
            int signTargetAmountDifference = Math.Sign(targetAmountDifference);

            // Increment or decrement amountToBreed of this generations speciess sequentially
            // until the totalAmountToSustain equals the configured PopulationSize
            while (totalAmountToSustain != Config.populationSize)
            {
                foreach (Species species in speciesOfThisGeneration)
                {
                    if (totalAmountToSustain == Config.populationSize)
                    {
                        break;
                    }
                    species.SpeciesTimestamps[currentGeneration].AmountToGenerateForNextGeneration += signTargetAmountDifference;
                    totalAmountToSustain += signTargetAmountDifference;
                }
            }

            // Determine how many genomes for each species of next generation
            // should come from elitism and how many from breeding
            int numGenomesFromElitism, numGenomesFromCrossover;

            foreach (Species species in speciesOfThisGeneration)
            {
                SpeciesTimestamp speciesTimestamp = species.SpeciesTimestamps[currentGeneration];

                numGenomesFromElitism = (int)(speciesTimestamp.Members.Count / Config.genomesFromElitismRatio);
                numGenomesFromElitism = Math.Min(numGenomesFromElitism, speciesTimestamp.AmountToGenerateForNextGeneration);

                numGenomesFromCrossover = speciesTimestamp.AmountToGenerateForNextGeneration - numGenomesFromElitism;

                speciesTimestamp.AmountToGenerateByElitism = numGenomesFromElitism;
                speciesTimestamp.AmountToGenerateByCrossover = numGenomesFromCrossover;

                // OPTIONAL:
                //  Determine how many genomes are created by mutation and how many by crossover
            }
        }

        private void AdjustFitness()
        {
            foreach (Species species in History.Speciess[History.CurrentGeneration])
            {
                foreach (Genome genome in species.SpeciesTimestamps[History.CurrentGeneration].Members)
                {
                    double adjustedFitness = genome.Fitness / species.SpeciesTimestamps[History.CurrentGeneration].Members.Count;
                    genome.AdjustedFitness = adjustedFitness;
                }
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
