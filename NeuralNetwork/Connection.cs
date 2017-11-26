using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Connection
    {
        public double Weight { get; set; }

        public Neuron NeuronFrom { get; set; }
        public Neuron NeuronTo { get; set; }

        public Connection()
        {
        }

        public Connection(Neuron neuronFrom, Neuron neuronTo)
        {
            NeuronFrom = neuronFrom;
            NeuronTo = neuronTo;
        }

        public Connection(Neuron neuronFrom, Neuron neuronTo, double weight) : this(neuronFrom, neuronTo)
        {
            Weight = weight;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
