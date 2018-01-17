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
        public string Id{ get; set; }

        public List<Genome> Population { get; set; }
        public Dictionary<Generation, SpeciesTimestamp> SpeciesTimestamps { get; set; }

        public int GenerationsNoImprovement { get; set; }

        public double BestFitness { get; set; }

        private Species()
        {
            Population = new List<Genome>();
            Id = "undefined";
            GenerationsNoImprovement = 0;
        }

        public Species(string id) : this()
        {
            Id = id;
        }

        public static Species FromJObject(JObject json)
        {
            Species species = new Species();

            string leaderGenomeId = (string)json.GetValue("LeaderGenome");

            JArray jarrayGenomes = new JArray();

            // Build up list with genomes from JArray and assign LeaderGenome
            foreach (JObject jsonGenome in jarrayGenomes)
            {

            }

            species.Id = (string)json.GetValue("Id");
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

            json.Add("Id", Id);
            json.Add("GenerationsNoImprovement", GenerationsNoImprovement);
            json.Add("BestFitness", BestFitness);
            
            return json;
        }
    }
}
