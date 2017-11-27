using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class NeuronGene
    {
        public string Id { get; set; }

        public NeuronType Type { get; set; }

        public NeuronGene(string id, NeuronType type)
        {
            Id = id;
            Type = type;
        }
    }
}
