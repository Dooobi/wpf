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
        public string Id { get; set; }

        public List<Genome> Population { get; set; }
        public Dictionary<Generation, SpeciesTimestamp> SpeciesTimestamps { get; set; }

        public int GenerationsNoImprovement { get; set; }

        public double BestFitness { get; set; }

        private Species()
        {
            Population = new List<Genome>();
            SpeciesTimestamps = new Dictionary<Generation, SpeciesTimestamp>();
            Id = "undefined";
            GenerationsNoImprovement = 0;
        }

        public Species(string id) : this()
        {
            Id = id;
        }

        /*
         * Adds a Genome to this Species and also adds it 
         * to the corresponding timestamp for the genomes Generation.
         * If no timestamp exists for its Generation a new one will be created.
         * 
         * Updates the BestFitness of the Species
         */
        public void AddGenomeAndUpdateSpecies(Genome genome)
        {
            SpeciesTimestamp speciesTimestamp = SpeciesTimestamps[genome.Generation];
            if (speciesTimestamp == null)
            {
                speciesTimestamp = new SpeciesTimestamp(this);
                SpeciesTimestamps[genome.Generation] = speciesTimestamp;
                speciesTimestamp.Leader = genome;
            }

            speciesTimestamp.Members.Add(genome);
            Population.Add(genome);

            if (genome.Fitness > BestFitness)
            {
                BestFitness = genome.Fitness;
            }
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
