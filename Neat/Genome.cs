using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class Genome
    {
        public string Id { get; set; }

        public List<NeuronGene> NeuronGenes { get; set; }
        public List<ConnectionGene> ConnectionGenes { get; set; }

        public Network Network { get; set; }

        public double Fitness { get; set; }
        public double AdjustedFitness { get; set; }

        public int NumberOfInputs { get; set; }
        public int NumberOfOutputs { get; set; }

        public Genome(string id, int numberOfInputs, int numberOfOutputs)
        {
            Id = id;
            NumberOfInputs = numberOfInputs;
            NumberOfOutputs = numberOfOutputs;
        }

    }
}
