using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class Generation
    {
        public List<Genome> Population {
            get
            {
                List<Genome> population = new List<Genome>();
                foreach (Species species in Species.Values)
                {
                    population.AddRange(species.Genomes);
                }
                return population;
            }
        }

        public Dictionary<int, Species> Species { get; set; }

        public double AverageFitness { get; set; }
        public double BestFitness { get; set; }

        public Generation()
        {
            Species = new Dictionary<int, Species>();
        }

        public Species this[int speciesId]
        {
            get { return Species[speciesId]; }
            set { Species[speciesId] = value; }
        }

        public static Generation FromJObject(JObject json)
        {
            Generation generation = new Generation();

            List<NeuronGene> neuronGenes = new List<NeuronGene>();
            List<ConnectionGene> connectionGenes = new List<ConnectionGene>();

            JArray jarraySpecies = (JArray)json.GetValue("Species");

            // Build up dictionary with species from JArray
            foreach (JObject jsonSpecies in jarraySpecies)
            {
                Species species = Neat.Species.FromJObject(jsonSpecies);

                generation.Species[species.Id] = species;
            }

            generation.AverageFitness = (double)json.GetValue("AverageFitness");
            generation.BestFitness = (double)json.GetValue("BestFitness");

            return generation;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            JArray jarraySpecies = new JArray();
            foreach (Species species in Species.Values)
            {
                jarraySpecies.Add(species.ToJson());
            }

            json.Add("Species");
            json.Add("AverageFitness", AverageFitness);
            json.Add("BestFitness", BestFitness);

            return json;
        }
    }
}
