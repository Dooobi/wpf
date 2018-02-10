using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class GeneticAlgorithm
    {
        public History History {
            get
            {
                return History.Singleton;
            }
        }

        public List<Genome> Epoch(List<Genome> populationOfCurrentGeneration)
        {
            // Setup the list of Genomes which will contain the next population
            List<Genome> populationForNextGeneration = new List<Genome>();

            // Create a new Generation for the evaluated population
            Generation newGeneration = History.CreateAndAddNewGeneration();

            // Speciates the evaluated population and adds them to the History
            Speciate(populationOfCurrentGeneration);

            // Adjusts the fitness of the evaluated Genomes based on their 
            // original fitness and the number of members in their Species
            AdjustFitness(populationOfCurrentGeneration);
            DetermineAmountToGenerateForEachSpecies();
            
            // Add the elitists to the population for the next Generation
            PerformElitism(ref populationForNextGeneration);

            // Crossover
            // The chance for getting selected for crossover linearly increases the
            // better the fitness of a Genome is
            List<Genome> offspring = PerformCrossover(ref populationForNextGeneration);

            // Mutation
            // - Mutates the children which resulted from crossover
            // - Creates mutants for the Speciess AmountToGenerateByMutation
            // - Only elitists are not mutated
            PerformMutation(ref populationForNextGeneration, offspring);
            
            // Determine species
            // Save to file
            // Render
            // Create population for next generation by
            //   - elitism
            //   - crossover
            //   - mutation

            return populationForNextGeneration;
        }

 
        /****************************************************************/
        /*********************** EVOLUTION METHODS **********************/
        /****************************************************************/
        // - Speciate
        // - PerformElitism
        // - PerformCrossover
        // - DetermineAmountToGenerateForEachSpecies
        // - AdjustFitness
        /****************************************************************/

        //------------------ SpeciateAndCalculateSpawnLevels ---------------------
        //
        //  separates each individual into its respective species by calculating
        //  a compatibility score with every other member of the population and 
        //  niching accordingly.
        //------------------------------------------------------------------------
        // History.CurrentGeneration is the Generation of the evaluated Population
        private void Speciate(List<Genome> evaluatedPopulation)
        {
            Generation previousGeneration = History.PreviousGeneration;
            Generation currentGeneration = History.CurrentGeneration;
            List<Species> availableSpecies;
            List<Species> availableSpeciesFromPreviousGeneration;
            List<Species> availableSpeciesFromCurrentGeneration;

            if (History.PreviousSpeciess != null)
            {
                availableSpeciesFromPreviousGeneration = new List<Species>(History.PreviousSpeciess);
            }
            else
            {
                availableSpeciesFromPreviousGeneration = new List<Species>();
            }

            foreach (Genome genome in evaluatedPopulation)
            {
                // Get the availableSpeciesFromCurrentGeneration after the speciating of the individual Genomes
                // (this can change while the genomes are being speciated because new Species could be created during this Generation)
                availableSpeciesFromCurrentGeneration = History.CurrentSpeciess;
                if (availableSpeciesFromCurrentGeneration == null)
                {
                    availableSpeciesFromCurrentGeneration = new List<Species>();
                }

                // Merge the availalbe Species from the previous and the current Generation into one single List
                availableSpecies = new List<Species>(availableSpeciesFromPreviousGeneration);
                availableSpecies.AddRange(availableSpeciesFromCurrentGeneration);

                // Iterate through this List of available Species and 
                // check if the Genome is compatible with it. 
                // If not -> create a new Species
                foreach (Species species in availableSpecies)
                {
                    // The compatibility will be checked against the leader of the SpeciesTimestamp
                    Genome leaderGenome;

                    if (previousGeneration != null
                        && History.SpeciesTimestampMap.ContainsKey(previousGeneration)
                        && History.SpeciesTimestampMap[previousGeneration].ContainsKey(species))
                    {
                        // Take the leader of this Species from the previous Generation
                        leaderGenome = History.SpeciesTimestampMap[previousGeneration][species].Leader;
                    }
                    else
                    {
                        // If the Species was created during this Epoch there won't be a
                        // leader of this Species for the previous Generation.
                        // So take the leader of this Generation instead
                        leaderGenome = History.SpeciesTimestampMap[currentGeneration][species].Leader;
                    }

                    double compatibility = CalculateCompatibility(genome, leaderGenome);

                    /* Update Stats */
                    Stats.UpdateHighestCompatibilityValue(compatibility);

                    if (compatibility <= Config.compatibilityThreshold)
                    {
                        // So add the Genome to the History with 
                        // the current Generation and the compatible Species
                        History.AddGenomeToHistory(genome, currentGeneration, species);

                        // And stop looking for a compatible Species for this Genome
                        break;
                    }
                }

                // Check if a compatible Species has been found
                if (genome.Species == null)
                {
                    // Genome is not compatible with any existing species -> create a new species
                    Species newSpecies = History.CreateAndAddNewSpecies(currentGeneration);

                    // Will add the Genome to the History and
                    // update the Generation, Species and SpeciesTimestamp.
                    // A new SpeciesTimestamp will be created if it doesn't exist yet
                    // and the leader of it will be initialized with this genome.
                    History.AddGenomeToHistory(genome, currentGeneration, newSpecies);
                }                
            }
        }

        // History.CurrentGeneration is the Generation of the evaluated Population
        public List<Genome> PerformElitism(ref List<Genome> populationForNextGeneration)
        {
            Generation currentGeneration = History.CurrentGeneration;
            List<Genome> elitists = new List<Genome>();

            foreach (SpeciesTimestamp speciesTimestamp in currentGeneration.SpeciesTimestamps)
            {
                IEnumerable<Genome> genomesOrderedByFitness = speciesTimestamp.Members.OrderByDescending(genome => genome.Fitness);

                int i = 0;
                foreach (Genome genome in genomesOrderedByFitness)
                {
                    if (i < speciesTimestamp.AmountToGenerateByElitism)
                    {
                        Genome elitistCopy = new Genome(genome);
                        elitistCopy.Id = "g" + (currentGeneration.Number + 1) + ": " + (populationForNextGeneration.Count + 1);
                        elitistCopy.AdjustedFitness = 0;
                        elitistCopy.Fitness = 0;
                        elitistCopy.Generation = null;
                        elitistCopy.Species = null;

                        elitists.Add(elitistCopy);
                        populationForNextGeneration.Add(elitistCopy);
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

        public List<Genome> PerformCrossover(ref List<Genome> populationForNextGeneration)
        {
            Generation currentGeneration = History.CurrentGeneration;
            List<Genome> offspring = new List<Genome>();

            // Crossover for every Species
            foreach (SpeciesTimestamp speciesTimestamp in currentGeneration.SpeciesTimestamps)
            {
                // Until the amount to generate by crossover is reached by that SpeciesTimestamp
                for (int i = 0; i < speciesTimestamp.AmountToGenerateByCrossover; i++) { 

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
                            currentGeneration.Population.Sort((genome1, genome2) => genome1.Fitness.CompareTo(genome2.Fitness));
                            // Parent1 must be excluded from the list of available Genomes (second parameter)
                            parent2 = SelectGenomeForCrossover(currentGeneration.Population, parent1);
                        }
                        else
                        {
                            // In case of regular crossover the second parent will just be selected
                            // from a list of Genomes within the same species as parent1 (but parent1 must be excluded)
                            parent2 = SelectGenomeForCrossover(speciesTimestamp.Members, parent1);
                        }

                        // Actually perform the crossover and put the child 
                        // into the list of Genomes for the next Generation
                        string idForChild = "g" + (currentGeneration.Number + 1) + ": " + (populationForNextGeneration.Count + 1);
                        Genome child = Crossover(parent1, parent2, idForChild);

                        offspring.Add(child);
                        populationForNextGeneration.Add(child);
                    }
                    else
                    {
                        // Crossover is not possible because the Species doesn't have at least 2 members
                        // and interspecies crossover is not active -> Mutate instead
                        speciesTimestamp.AmountToGenerateByMutation++;
                        speciesTimestamp.AmountToGenerateByCrossover--;
                        i--;
                    }
                }
            }

            return offspring;
        }
                
        public List<Genome> PerformMutation(ref List<Genome> populationForNextGeneration, List<Genome> offspring)
        {
            Generation currentGeneration = History.CurrentGeneration;
            List<Genome> mutants = new List<Genome>();

            // Mutate the children which resulted from crossover
            foreach (Genome child in offspring)
            {
                Mutate(child);
            }

            // Create mutants for each Species until their AmountToGenerateByMutation is hit
            foreach (SpeciesTimestamp speciesTimestamp in currentGeneration.SpeciesTimestamps)
            {
                for (int i = 0; i < speciesTimestamp.AmountToGenerateByMutation; i++)
                {
                    Genome selectedGenome = SelectGenomeForMutation(speciesTimestamp.Members);
                    Genome mutant = new Genome(selectedGenome);
                    mutant.Id = "g" + (currentGeneration.Number + 1) + ": " + (populationForNextGeneration.Count + 1);
                    mutant.AdjustedFitness = 0;
                    mutant.Fitness = 0;
                    mutant.Generation = null;
                    mutant.Species = null;

                    Mutate(mutant);

                    mutants.Add(mutant);
                    populationForNextGeneration.Add(mutant);
                }
            }

            // Only the elitists are not mutated

            return mutants;
        }

        private void AdjustFitness(List<Genome> evaluatedAndSpeciatedPopulation)
        {
            foreach (Genome genome in evaluatedAndSpeciatedPopulation)
            {
                double adjustedFitness = genome.Fitness / genome.SpeciesTimestamp.Members.Count;
                genome.AdjustedFitness = adjustedFitness;
                    
                // Update the sum of adjusted Fitness in Generation, Species and SpeciesTimestamp
                // (and with it the average of adjusted Fitness)
                genome.Species.SumAdjustedFitness += adjustedFitness;
                genome.Generation.SumAdjustedFitness += adjustedFitness;
                genome.SpeciesTimestamp.SumAdjustedFitness += adjustedFitness;
            }
        }

        /**
         * Determines how many genomes are generated by:
         *  - elitism
         *  - crossover
         */
        private void DetermineAmountToGenerateForEachSpecies()
        {
            Generation currentGeneration = History.CurrentGeneration;
            List<SpeciesTimestamp> currentSpeciesTimestamps = currentGeneration.SpeciesTimestamps;

            double amountToGenerate;
            double totalAverageAdjustedFitness = 0.0;

            // Calculate average adjusted fitness and total adjusted fitness for species in this generation
            foreach (SpeciesTimestamp speciesTimestamp in currentSpeciesTimestamps)
            {
                totalAverageAdjustedFitness += speciesTimestamp.AverageAdjustedFitness;
            }

            // Needed for rounding
            double limitForRoundingUp = 1.0 / Config.populationSize;
            double modifierForCorrectRounding = (0.5 - limitForRoundingUp);

            int totalAmountToGenerate = 0;

            // Using the calculated averages we can calculate the amount of genomes a species should generate
            foreach (SpeciesTimestamp speciesTimestamp in currentSpeciesTimestamps)
            {
                amountToGenerate = (speciesTimestamp.AverageAdjustedFitness / totalAverageAdjustedFitness) * Config.populationSize;

                // Round to int
                speciesTimestamp.AmountToGenerateForNextGeneration = (int)Math.Round(amountToGenerate + modifierForCorrectRounding, MidpointRounding.AwayFromZero);

                // Total up the amounts to check if it matches the populationSize
                totalAmountToGenerate += speciesTimestamp.AmountToGenerateForNextGeneration;
            }

            // The totalAmountToGenerate might be to high/low because of rounding errors
            int targetAmountDifference = Config.populationSize - totalAmountToGenerate;
            int signTargetAmountDifference = Math.Sign(targetAmountDifference);

            // Increment or decrement amountToGenerate of this generations speciess sequentially
            // until the totalAmountToSustain equals the configured PopulationSize
            while (totalAmountToGenerate != Config.populationSize)
            {
                foreach (SpeciesTimestamp speciesTimestamp in currentSpeciesTimestamps)
                {
                    if (totalAmountToGenerate == Config.populationSize)
                    {
                        break;
                    }
                    speciesTimestamp.AmountToGenerateForNextGeneration += signTargetAmountDifference;
                    totalAmountToGenerate += signTargetAmountDifference;
                }
            }

            // Determine how many genomes for each species of next generation
            // should come from elitism and how many from breeding
            int numGenomesFromElitism, numGenomesFromCrossover, numGenomesFromMutation;

            foreach (SpeciesTimestamp speciesTimestamp in currentSpeciesTimestamps)
            {
                numGenomesFromElitism = (int)(speciesTimestamp.Members.Count * Config.genomesFromElitismRatio);
                numGenomesFromElitism = Math.Min(numGenomesFromElitism, speciesTimestamp.AmountToGenerateForNextGeneration);

                numGenomesFromMutation = (int)((speciesTimestamp.AmountToGenerateForNextGeneration - numGenomesFromElitism) * Config.offspringFromMutationRatio);
                numGenomesFromCrossover = speciesTimestamp.AmountToGenerateForNextGeneration - (numGenomesFromElitism + numGenomesFromMutation);

                speciesTimestamp.AmountToGenerateByElitism = numGenomesFromElitism;
                speciesTimestamp.AmountToGenerateByCrossover = numGenomesFromCrossover;
                speciesTimestamp.AmountToGenerateByMutation = numGenomesFromMutation;
            }
        }

        /****************************************************************/
        /************************ HELPER METHODS ************************/
        /****************************************************************/
        // - SelectGenomeForCrossover
        // - ActuallyCrossover
        // - GetConnectionGeneByCrossover
        // - CheckInterspeciesCrossover
        // - CalculateCompatibility
        /****************************************************************/

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
                actuallyAvailableGenomes.Remove(excludedFromAvailableGenomes);
            }

            int highestIndex = actuallyAvailableGenomes.Count - 1;
            if (highestIndex == 0)
            {
                return availableGenomes[0];
            }
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

        /*
        * Uses the configured minChanceForSelectionToMutate and maxChanceForSelectionToMutate
        * to select a Genome from a list of Genomes which is sorted by fitness (lower index = lower fitness)
        */
        private Genome SelectGenomeForMutation(List<Genome> availableGenomes)
        {
            int highestIndex = availableGenomes.Count - 1;
            if (highestIndex == 0)
            {
                return availableGenomes[0];
            }
            double gradientOfChanceForSelection = (Config.maxChanceForSelectionToMutate - Config.minChanceForSelectionToMutate) / highestIndex;
            double rand = Utils.RandDouble(Config.minChanceForSelectionToMutate, Config.maxChanceForSelectionToMutate);

            // Go through the list of genomes from best to worst (highest chance to lowest chance)
            // and check if it has been selected by the random number
            double currentChance;
            for (int i = highestIndex, c = 0; i >= 0; i--, c++)
            {
                currentChance = Config.maxChanceForSelectionToCrossover - c * gradientOfChanceForSelection;

                if (rand >= currentChance)
                {
                    return availableGenomes[i];
                }
            }

            return null;
        }

        private void Mutate(Genome genome)
        {
            // Check if the Connection weights should be mutated
            if (Utils.random.NextDouble() < Config.chanceToMutateWeightsOfGenome)
            {
                MutateWeights(genome);
            }

            // Mutate Add Neuron
            if (Utils.random.NextDouble() < Config.chanceForNewNeuronMutation)
            {
                MutateAddNeuron(genome);
            }

            // Mutate Add Connection
            if (Utils.random.NextDouble() < Config.chanceForNewConnectionMutation)
            {
                MutateAddConnection(genome);
            }
        }

        private void MutateAddConnection(Genome genome)
        {
            List<NeuronGene> candidatesToConnectFrom = new List<NeuronGene>(genome.NeuronGenes);
            List<NeuronGene> candidatesToConnectTo;

            while (candidatesToConnectFrom.Count > 0)
            {
                // Get a random candidate to connect from
                NeuronGene candidateToConnectFrom = Utils.RandomListItem(genome.NeuronGenes, null);

                // Find all the candidates to which the candidateToConnectFrom has no Connection yet
                candidatesToConnectTo = new List<NeuronGene>();
                foreach (NeuronGene candidateToConnectTo in genome.NeuronGenes)
                {
                    if (!CheckForConnectionGeneFromTo(genome, candidateToConnectFrom, candidateToConnectTo))
                    {
                        candidatesToConnectTo.Add(candidateToConnectTo);
                    }
                }
                if (candidatesToConnectTo.Count == 0)
                {
                    // The selected candidateToConnectFrom has no candidates to connect to
                    // so remove it from the candidatesToConnectFrom and try with
                    // another candidateToConnectFrom
                    candidatesToConnectFrom.Remove(candidateToConnectFrom);
                }
                else
                {
                    // The selected candidateToConnectFrom has candidates to connect to
                    // so choose on of them by random and create the ConnectionGene
                    NeuronGene neuronGeneToConnectTo = Utils.RandomListItem(candidatesToConnectTo, null);

                    genome.AddBasicConnectionGene(candidateToConnectFrom, neuronGeneToConnectTo);
                    break;
                }
            }
        }

        private void MutateAddNeuron(Genome genome)
        {
            List<ConnectionGene> copiedConnectionGenes = new List<ConnectionGene>(genome.ConnectionGenes);

            while (copiedConnectionGenes.Count > 0)
            {
                ConnectionGene selectedConnectionGene = Utils.RandomListItem(genome.ConnectionGenes, null);
                if (selectedConnectionGene != null && selectedConnectionGene.IsEnabled)
                {
                    // Eligible ConnectionGene found
                    // Now disable it and create two 
                    // new ConnectionGenes and a NeuronGene 
                    // which will be connected by them
                    NeuronGene originalNeuronGeneFrom = selectedConnectionGene.NeuronGeneFrom;
                    NeuronGene originalNeuronGeneTo = selectedConnectionGene.NeuronGeneTo;

                    NeuronGene newNeuronGene = new NeuronGene(genome.GetNextNeuronGeneId(NeuronType.Hidden), NeuronType.Hidden);
                    genome.NeuronGenes.Add(newNeuronGene);

                    ConnectionGene newConnectionGene1 = genome.AddBasicConnectionGene(originalNeuronGeneFrom, newNeuronGene);
                    ConnectionGene newConnectionGene2 = genome.AddBasicConnectionGene(newNeuronGene, originalNeuronGeneTo);

                    newConnectionGene1.Weight = 1.0;
                    newConnectionGene2.Weight = selectedConnectionGene.Weight;

                    selectedConnectionGene.IsEnabled = false;
                    break;
                }
            }            
        }

        private void MutateWeights(Genome genome)
        {
            // Mutate Connection weights
            foreach (ConnectionGene connectionGene in genome.ConnectionGenes)
            {
                // Every Connection weight has a chance to be mutated
                if (Utils.random.NextDouble() < Config.chanceToMutateOneWeight)
                {
                    // Mutate this connection weight
                    if (Utils.random.NextDouble() < Config.chanceForRandomizingWeight)
                    {
                        // Completely randomize this Connection weight
                        connectionGene.Weight = Utils.RandDouble(Config.minWeightLimit, Config.maxWeightLimit);
                    }
                    else
                    {
                        // Perturb this Connection weight
                        connectionGene.Weight += Utils.RandDouble(-1.0, 1.0) * Config.maxWeightPerturbation;
                        // Don't overshoot the limits
                        connectionGene.Weight = Math.Max(connectionGene.Weight, Config.minWeightLimit);
                        connectionGene.Weight = Math.Min(connectionGene.Weight, Config.maxWeightLimit);
                    }
                }
            }
        }

        private Genome Crossover(Genome parent1, Genome parent2, string idForChild)
        {
            Genome child = new Genome(idForChild, Config.numberOfInputs, Config.numberOfOutputs);
            child.SetupInitialNeuronGenes();

            bool equalFitness = false;
            Genome parentHigherFitness, parentLowerFitness;
            if (parent1.Fitness == parent2.Fitness)
            {
                equalFitness = true;
            }
            if (parent1.Fitness >= parent2.Fitness)
            {
                parentHigherFitness = parent1;
                parentLowerFitness = parent2;
            }
            else
            {
                parentHigherFitness = parent2;
                parentLowerFitness = parent1;
            }

            Dictionary<int, ConnectionGene> connectionGenesParentHigherFitness = parentHigherFitness.GetConnectionGenesByInnovationNumber();
            Dictionary<int, ConnectionGene> connectionGenesParentLowerFitness = parentLowerFitness.GetConnectionGenesByInnovationNumber();

            int lowestInnovationNumber = Math.Min(connectionGenesParentHigherFitness.Keys.Min(), connectionGenesParentLowerFitness.Keys.Min());
            int highestInnovationNumber = Math.Max(connectionGenesParentHigherFitness.Keys.Max(), connectionGenesParentLowerFitness.Keys.Max());

            for (int i = lowestInnovationNumber; i <= highestInnovationNumber; i++)
            {
                ConnectionGene connectionGeneParentHigherFitness = (connectionGenesParentHigherFitness.ContainsKey(i) ? connectionGenesParentHigherFitness[i] : null);
                ConnectionGene connectionGeneParentLowerFitness = (connectionGenesParentHigherFitness.ContainsKey(i) ? connectionGenesParentHigherFitness[i] : null);
                ConnectionGene chosenConnectionGene = null, rejectedConnectionGene = null;
                ConnectionGene connectionGeneChild;

                if (connectionGeneParentHigherFitness == null && connectionGeneParentLowerFitness == null)
                {
                    // No parent has a ConnectionGene with this InnovationNumber
                    // Just move on to the next InnovationNumber
                    continue;
                }
                if (connectionGeneParentHigherFitness == null || connectionGeneParentLowerFitness == null)
                {
                    // Only one parent has a ConnectionGene with this InnovationNumber (it's Excess or Disjoint)
                    // Choose the ConnectionGene from the more fit parent unless they have equal fitness
                    if (equalFitness)
                    {
                        // If they have equal fitness choose the ConnectionGene by random
                        int rand = Utils.random.Next(2); // rand is 0 or 1
                        if (rand == 0)
                        {
                            chosenConnectionGene = connectionGeneParentHigherFitness;
                            rejectedConnectionGene = connectionGeneParentLowerFitness;
                        }
                        else
                        {
                            chosenConnectionGene = connectionGeneParentLowerFitness;
                            rejectedConnectionGene = connectionGeneParentHigherFitness;
                        }
                    }
                    else
                    {
                        // If they don't have equal fitness choose the ConnectionGene of the fitter parent
                        chosenConnectionGene = connectionGeneParentHigherFitness;
                        rejectedConnectionGene = connectionGeneParentLowerFitness;
                    }
                }
                if (connectionGeneParentHigherFitness != null && connectionGeneParentLowerFitness != null)
                {
                    // Both parents have a ConnectionGene with this InnovationNumber
                    // Choose the ConnectionGene by random
                    int rand = Utils.random.Next(2); // rand is 0 or 1
                    if (rand == 0)
                    {
                        chosenConnectionGene = connectionGeneParentHigherFitness;
                        rejectedConnectionGene = connectionGeneParentLowerFitness;
                    }
                    else
                    {
                        chosenConnectionGene = connectionGeneParentLowerFitness;
                        rejectedConnectionGene = connectionGeneParentHigherFitness;
                    }
                }

                connectionGeneChild = GetConnectionGeneByCrossover(chosenConnectionGene, rejectedConnectionGene);

                if (connectionGeneChild != null)
                {
                    connectionGeneChild.InnovationNumber = i;
                    // Adds the ConnectionGene to the child and overwrites the 
                    // NeuronGeneFrom and NeuronGeneTo properties with the actual
                    // NeuronGenes of the child Genome (creates the NeuronGenes if they don't exist)
                    child.AddConnectionGeneOverwriteNeuronGenes(connectionGeneChild);
                }
            }

            return child;
        }

        private ConnectionGene GetConnectionGeneByCrossover(ConnectionGene chosenConnectionGene, ConnectionGene rejectedConnectionGene)
        {
            ConnectionGene connectionGeneChild = new ConnectionGene();

            if (chosenConnectionGene == null)
            {
                // A non-existing ConnectionGene was selected
                // Can happen if there is Disjoint or Excess
                // or if there is an InnovationNumber without any match in the parents
                // => No ConnectionGene for this InnovationNumber will be added to the child
                return null;
            }

            // Set the properties of connectionGeneChild
            connectionGeneChild.Id = "c_" + chosenConnectionGene.NeuronGeneFrom.Id + "_" + chosenConnectionGene.NeuronGeneTo.Id;
            connectionGeneChild.Weight = chosenConnectionGene.Weight;
            connectionGeneChild.IsEnabled = chosenConnectionGene.IsEnabled;
            if (!chosenConnectionGene.IsEnabled || (rejectedConnectionGene != null && !rejectedConnectionGene.IsEnabled))
            {
                // If the ConnectionGene is disabled in either parent it
                // has a preset chance to stay disabled otherwise it gets enabled
                if (Utils.random.NextDouble() < Config.chanceToDisableConnectionGeneIfDisabledInEitherParent)
                {
                    connectionGeneChild.IsEnabled = false;
                }
                //else
                //{
                //    connectionGeneChild.IsEnabled = true;
                //}
            }
            // These will be overwritten with the corresponding NeuronGene of
            // the child Genome later on
            connectionGeneChild.NeuronGeneFrom = chosenConnectionGene.NeuronGeneFrom;
            connectionGeneChild.NeuronGeneTo = chosenConnectionGene.NeuronGeneTo;

            // Only InnovationNumber is missing
            return connectionGeneChild;
        }

        private bool CheckInterspeciesCrossover()
        {
            return (History.CurrentSpeciess.Count >= 2
                    && Utils.random.NextDouble() <= Config.chanceForInterspeciesCrossover);
        }

        private double CalculateCompatibility(Genome genome1, Genome genome2)
        {
            int numGenesLargerGenome, numExcessGenes = 0, numDisjointGenes = 0, numMatchingGenes = 0;
            double sumWeightDifference = 0.0, avgWeightDifference;

            Dictionary<int, ConnectionGene> connectionGenesByInnovationNumber1 = genome1.GetConnectionGenesByInnovationNumber();
            Dictionary<int, ConnectionGene> connectionGenesByInnovationNumber2 = genome2.GetConnectionGenesByInnovationNumber();
            int smallerMaxInnovationNumber = Math.Min(connectionGenesByInnovationNumber1.Keys.Max(), connectionGenesByInnovationNumber2.Keys.Max());
            int higherMaxInnovationNumber = Math.Max(connectionGenesByInnovationNumber1.Keys.Max(), connectionGenesByInnovationNumber2.Keys.Max());

            for (int i = 1; i <= higherMaxInnovationNumber; i++)
            {
                ConnectionGene connectionGene1 = null, connectionGene2 = null;
                if (connectionGenesByInnovationNumber1.ContainsKey(i))
                {
                    connectionGene1 = connectionGenesByInnovationNumber1[i];
                }
                if (connectionGenesByInnovationNumber2.ContainsKey(i))
                {
                    connectionGene2 = connectionGenesByInnovationNumber2[i];
                }

                if (connectionGene1 != null && connectionGene2 == null
                    || connectionGene1 == null && connectionGene2 != null)
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
                else if (connectionGene1 != null && connectionGene2 != null)
                {
                    // The connectionGenes are matching
                    numMatchingGenes++;
                    sumWeightDifference += Math.Abs(connectionGene1.Weight - connectionGene2.Weight);
                }
            }

            numGenesLargerGenome = Math.Max(genome1.ConnectionGenes.Count, genome2.ConnectionGenes.Count);
            avgWeightDifference = sumWeightDifference / numMatchingGenes;

            return (Config.compatibilityCoefficientNumExcessGenes * numExcessGenes) / numGenesLargerGenome + (Config.compatibilityCoefficientNumDisjointGenes * numDisjointGenes) / numGenesLargerGenome + Config.compatibilityCoefficientAvgWeightDifference * avgWeightDifference;
        }

        private bool CheckForConnectionGeneFromTo(Genome genome, NeuronGene neuronGeneFrom, NeuronGene neuronGeneTo)
        {
            foreach (ConnectionGene connectionGene in genome.ConnectionGenes)
            {
                if (connectionGene.NeuronGeneFrom == neuronGeneFrom
                    && connectionGene.NeuronGeneTo == neuronGeneTo)
                {
                    // A ConnectionGene from the neuronGeneFrom to the neuronGeneTo exists
                    return true;
                }
            }
            // No ConnectionGene from the neuronGeneFrom to the neuronGeneTo exists
            return false;
        }
    }
}
