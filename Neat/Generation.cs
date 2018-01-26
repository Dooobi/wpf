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

        public List<Genome> Population { get; set; }
        public List<SpeciesTimestamp> SpeciesTimestamps { get; set; }

        public Genome FittestGenome { get; set; }
        public double SumFitness { get; set; }
        public double SumAdjustedFitness { get; set; }
        public double AverageFitness
        {
            get
            {
                return SumFitness / Population.Count;
            }
        }
        public double AverageAdjustedFitness
        {
            get
            {
                return SumAdjustedFitness / Population.Count;
            }
        }
        
        public Generation()
        {
            Population = new List<Genome>();
            SpeciesTimestamps = new List<SpeciesTimestamp>();
        }

        public Generation(Generation generation) : this()
        {
            Number = generation.Number;
            SumFitness = generation.SumFitness;
            SumAdjustedFitness = generation.SumAdjustedFitness;
        }

        public Generation(int number) : this()
        {
            Number = number;
        }

        public void AddGenomeAndUpdateGeneration(Genome genome)
        {
            // Add to population
            Population.Add(genome);

            // Add to Timestamp
            if (!SpeciesTimestamps.Contains(genome.SpeciesTimestamp))
            {
                SpeciesTimestamps.Add(genome.SpeciesTimestamp);
            }

            // Update Generation
            SumFitness += genome.Fitness;
            SumAdjustedFitness += genome.AdjustedFitness;

            if (FittestGenome == null || genome.Fitness > FittestGenome.Fitness)
            {
                FittestGenome = genome;
            }
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

            //generation.AverageFitness = (double)json.GetValue("AverageFitness");
            //generation.BestFitness = (double)json.GetValue("BestFitness");

            return generation;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            //JArray jarraySpecies = new JArray();
            //foreach (Species species in PopulationBySpecies.Keys)
            //{
            //    jarraySpecies.Add(species.Id);
            //}

            //json.Add("Species", jarraySpecies);
            //json.Add("AverageFitness", AverageFitness);
            //json.Add("BestFitness", BestFitness);

            return json;
        }
    }
}
