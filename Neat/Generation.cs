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
        public int Number { get; set; }

        public List<Genome> Population
        {
            get
            {
                List<Genome> population = new List<Genome>();

                foreach (List<Genome> populationBySpecies in PopulationBySpecies.Values)
                {
                    population.AddRange(populationBySpecies);
                }

                return population;
            }
        }
        public Dictionary<Species, List<Genome>> PopulationBySpecies { get; set; }

        public double AverageFitness { get; set; }
        public double BestFitness { get; set; }

        public Generation()
        {
            PopulationBySpecies = new Dictionary<Species, List<Genome>>();
        }

        public Generation(int number) : this()
        {
            Number = number;
        }

        public void AddGenome(Genome genome)
        {
            if (PopulationBySpecies[genome.Species] == null)
            {
                PopulationBySpecies[genome.Species] = new List<Genome>();
            }
            PopulationBySpecies[genome.Species].Add(genome);
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
            foreach (Species species in PopulationBySpecies.Keys)
            {
                jarraySpecies.Add(species.Id);
            }

            json.Add("Species", jarraySpecies);
            json.Add("AverageFitness", AverageFitness);
            json.Add("BestFitness", BestFitness);

            return json;
        }
    }
}
