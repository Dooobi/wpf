using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class SpeciesTimestamp
    {
        public Species Species { get; set; }
        public Genome Leader { get; set; }
        public List<Genome> Members { get; set; }
        public int AmountToGenerateForNextGeneration { get; set; }
        public int AmountToGenerateByElitism { get; set; }
        public int AmountToGenerateByCrossover { get; set; }

        public SpeciesTimestamp(Species species)
        {
            Species = species;
            Members = new List<Genome>();
        }

        public void SelectLeader()
        {
            int randomIndex = Utils.random.Next(Members.Count);

            Leader = Members[randomIndex];
        }

        public double CalculateAverageAdjustedFitness()
        {
            double sumAdjustedFitness = 0.0;

            foreach (Genome genome in Members)
            {
                sumAdjustedFitness += genome.AdjustedFitness;
            }

            return sumAdjustedFitness / Members.Count;
        }
    }
}
