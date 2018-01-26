using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class SpeciesTimestamp
    {
        public Generation Generation { get; set; }
        public Species Species { get; set; }
        public Genome Leader { get; set; }
        public List<Genome> Members { get; set; }

        public int AmountToGenerateForNextGeneration { get; set; }
        public int AmountToGenerateByElitism { get; set; }
        public int AmountToGenerateByCrossover { get; set; }
        public int AmountToGenerateByMutation { get; set; }

        public Genome FittestGenome { get; set; }
        public double SumFitness { get; set; }
        public double SumAdjustedFitness { get; set; }
        public double AverageFitness
        {
            get
            {
                return SumFitness / Members.Count;
            }
        }
        public double AverageAdjustedFitness
        {
            get
            {
                return SumAdjustedFitness / Members.Count;
            }
        }

        public SpeciesTimestamp(Generation generation, Species species)
        {
            Generation = generation;
            Species = species;
            Members = new List<Genome>();
        }

        public void SelectLeader()
        {
            int randomIndex = Utils.random.Next(Members.Count);

            Leader = Members[randomIndex];
        }

        public void AddGenomeAndUpdateSpeciesTimestamp(Genome genome)
        {
            // Add to Timestamp
            Members.Add(genome);

            if (Members.Count == 1)
            {
                Leader = genome;
            }

            // Update Timestamp
            SumFitness += genome.Fitness;
            SumAdjustedFitness += genome.AdjustedFitness;
            if (FittestGenome == null || genome.Fitness > FittestGenome.Fitness)
            {
                FittestGenome = genome;
            }
        }
    }
}
