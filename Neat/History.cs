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
        
        public static void InitHistory()
        {
            PopulationThroughoutHistory = new List<Genome>();
            Generations = new List<Generation>();
            Speciess = new Dictionary<Generation, List<Species>>();
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
                if (Generations.Count >= 1)
                {
                    return Generations[Generations.Count - 1];
                }
                return null;
            }
        }

        //public static void AddGenomeToHistory(Genome genome)
        //{
        //    if (genome.Generation == null)
        //    {
        //        Console.WriteLine("Can't add genome ({0}) to History because it doesn't have a Generation", genome.Id);
        //        return;
        //    }
        //    if (genome.Species == null)
        //    {
        //        Console.WriteLine("Can't add genome ({0}) to History because it doesn't have a Species", genome.Id);
        //        return;
        //    }
            
        //    if (!Generations.Contains(genome.Generation))
        //    {
        //        Generations.Add(genome.Generation);
        //    }

        //    Species species = FindSpecies(genome.Species);
        //    if (!Speciess.Contain)
        //    {
        //        Speciess.Add(genome.Species);
        //    }

        //    genome.Generation.AddGenome(genome);
        //    genome.Species.AddGenome(genome);
        //    Population.Add(genome);
        //}
        
    }
}
