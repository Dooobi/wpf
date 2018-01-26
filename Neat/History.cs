using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public static class History
    {
        public static List<Genome> PopulationThroughoutHistory { get; set; }
        public static List<Generation> Generations { get; set; }
        public static Dictionary<Generation, List<Species>> Speciess { get; set; }
        public static Dictionary<Generation, Dictionary<Species, SpeciesTimestamp>> SpeciesTimestampMap { get; set; }

        public static void InitHistory()
        {
            PopulationThroughoutHistory = new List<Genome>();
            Generations = new List<Generation>();
            Speciess = new Dictionary<Generation, List<Species>>();
            SpeciesTimestampMap = new Dictionary<Generation, Dictionary<Species, SpeciesTimestamp>>();
        }

        public static Generation PreviousGeneration
        {
            get
            {
                if (Generations.Count >= 2)
                {
                    return Generations[Generations.Count - 2];
                }
                return null;                
            }
        }

        public static Generation CurrentGeneration
        {
            get
            {
                if (Generations.Count > 0)
                {
                    return Generations[Generations.Count - 1];
                }
                return null;
            }
        }

        public static List<Species> PreviousSpeciess
        {
            get
            {
                Generation previousGeneration = PreviousGeneration;
                if (previousGeneration != null && Speciess.ContainsKey(previousGeneration))
                {
                    return Speciess[previousGeneration];
                }
                return null;
            }
        }

        public static List<Species> CurrentSpeciess
        {
            get
            {
                Generation currentGeneration = CurrentGeneration;
                if (currentGeneration != null && Speciess.ContainsKey(currentGeneration))
                {
                    return Speciess[currentGeneration];
                }
                return null;
            }
        }

        public static Generation CreateAndAddNewGeneration()
        {
            Generation generation;
            int number = 1;
            if (CurrentGeneration != null) {
                number = CurrentGeneration.Number + 1;
            }
            generation = new Generation(number);

            Generations.Add(generation);
            Speciess[generation] = new List<Species>();

            return generation;
        }

        public static Species CreateAndAddNewSpecies(Generation originGeneration)
        {
            Species species;
            string speciesId;
            if (!Speciess.ContainsKey(originGeneration))
            {
                Speciess[originGeneration] = new List<Species>();
            }
            speciesId = "g" + originGeneration.Number + "s" + (Speciess[originGeneration].Count + 1);
            species = new Species(speciesId);

            Speciess[originGeneration].Add(species);

            return species;
        }

        public static void AddGenomeToHistory(Genome genome, Generation generation, Species species)
        {
            if (!Generations.Contains(generation))
            {
                Generations.Add(generation);
            }
            
            if (!Speciess.ContainsKey(generation))
            {
                Speciess[generation] = new List<Species>();
            }
            if (!Speciess[generation].Contains(species))
            {
                Speciess[generation].Add(species);
            }
            
            if (!SpeciesTimestampMap.ContainsKey(generation))
            {
                SpeciesTimestampMap[generation] = new Dictionary<Species, SpeciesTimestamp>();
            }
            if (!SpeciesTimestampMap[generation].ContainsKey(species))
            {
                SpeciesTimestampMap[generation][species] = new SpeciesTimestamp(generation, species);
            }
            SpeciesTimestamp speciesTimestamp = SpeciesTimestampMap[generation][species];

            // The Generation, Species and SpeciesTimestamp now definitely exist
            // and are added to the Generation

            // Set the Genomes properties
            genome.Generation = generation;
            genome.Species = species;
            genome.SpeciesTimestamp = speciesTimestamp;

            // Add the Genome to the Generation, Species and SpeciesTimestamp
            generation.AddGenomeAndUpdateGeneration(genome);
            species.AddGenomeAndUpdateSpecies(genome);
            speciesTimestamp.AddGenomeAndUpdateSpeciesTimestamp(genome);

            PopulationThroughoutHistory.Add(genome);
        }

    }
}
