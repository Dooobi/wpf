using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class Species
    {
        public int Number { get; set; }

        //public Dictionary<Generation, List<Genome>> PopulationByGeneration { get; set; }
        public List<Genome> Population { get; set; }
        public Genome LeaderGenome { get; set; }
        
        public int GenerationsNoImprovement { get; set; }

        public double BestFitness { get; set; }

        private Species()
        {
            //PopulationByGeneration = new Dictionary<Generation, List<Genome>>();
            Population = new List<Genome>();
            Number = -1;
            GenerationsNoImprovement = 0;
        }

        public Species(int number) : this()
        {
            Number = number;
        }

        //public void AddGenome(Genome genome)
        //{
        //    if (PopulationByGeneration[genome.Generation] == null)
        //    {
        //        PopulationByGeneration[genome.Generation] = new List<Genome>();
        //    }
        //    PopulationByGeneration[genome.Generation].Add(genome);
        //}

        public static Species FromJObject(JObject json)
        {
            Species species = new Species();

            string leaderGenomeId = (string)json.GetValue("LeaderGenome");

            JArray jarrayGenomes = new JArray();

            // Build up list with genomes from JArray and assign LeaderGenome
            foreach (JObject jsonGenome in jarrayGenomes)
            {

            }

            species.Number = (int)json.GetValue("Number");
            species.GenerationsNoImprovement = (int)json.GetValue("GenerationsNoImprovement");

            return species;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            //JArray generations = new JArray();
            //foreach (Generation generation in PopulationByGeneration.Keys)
            //{
            //    generations.Add(generation.Number);
            //}

            json.Add("Number", Number);
            json.Add("LeaderGenome", LeaderGenome.Id);
            json.Add("Generations", generations);
            json.Add("GenerationsNoImprovement", GenerationsNoImprovement);
            json.Add("BestFitness", BestFitness);
            
            return json;
        }
    }
}
