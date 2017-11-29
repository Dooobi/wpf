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
        public int Id { get; set; }

        public Genome LeaderGenome { get; set; }
        public List<Genome> Genomes { get; set; }

        public int OriginatedInGeneration { get; set; }
        public int GenerationsNoImprovement { get; set; }
        public int Age { get; set; }

        public double BestFitness { get; set; }

        private Species()
        {
            Genomes = new List<Genome>();
            Id = -1;
            Age = 0;
            OriginatedInGeneration = 0;
            GenerationsNoImprovement = 0;
        }

        public Species(int id, int originatedInGeneration) : this()
        {
            Id = id;
            OriginatedInGeneration = originatedInGeneration;
        }

        public static Species FromJObject(JObject json)
        {
            Species species = new Species();

            string leaderGenomeId = (string)json.GetValue("LeaderGenome");
            JArray jarrayGenomes = (JArray)json.GetValue("Genomes");
            
            // Build up list with genomes from JArray and assign LeaderGenome
            foreach (JObject jsonGenome in jarrayGenomes)
            {
                Genome genome = Genome.FromJObject(jsonGenome);

                species.Genomes.Add(genome);

                // Check if this genome is the leader
                if (genome.Id == leaderGenomeId)
                {
                    species.LeaderGenome = genome;
                }
            }

            species.Id = (int)json.GetValue("Id");
            species.OriginatedInGeneration = (int)json.GetValue("OriginatedInGeneration");
            species.GenerationsNoImprovement = (int)json.GetValue("GenerationsNoImprovement");
            species.Age = (int)json.GetValue("Age");

            return species;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            JArray genomes = new JArray();
            foreach (Genome genome in Genomes)
            {
                genomes.Add(genome.ToJson());
            }

            json.Add("Id", Id);
            json.Add("LeaderGenome", LeaderGenome.Id);
            json.Add("Genomes", genomes);
            json.Add("OriginatedInGeneration", OriginatedInGeneration);
            json.Add("GenerationsNoImprovement", GenerationsNoImprovement);
            json.Add("Age");
            json.Add("BestFitness", BestFitness);
            
            return json;
        }
    }
}
