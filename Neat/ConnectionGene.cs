using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class ConnectionGene
    {
        public string Id { get; set; }

        public NeuronGene NeuronGeneFrom { get; set; }
        public NeuronGene NeuronGeneTo { get; set; }

        public double Weight { get; set; }
        public int InnovationNumber { get; set; }
        public bool IsEnabled { get; set; }

        public ConnectionGene(string id, int innovationNumber, bool isEnabled, double weight, NeuronGene neuronGeneFrom, NeuronGene neuronGeneTo)
        {
            Id = Id;
            InnovationNumber = innovationNumber;
            IsEnabled = isEnabled;
            Weight = weight;
            NeuronGeneFrom = neuronGeneFrom;
            NeuronGeneTo = neuronGeneTo;
        }
    }
}
