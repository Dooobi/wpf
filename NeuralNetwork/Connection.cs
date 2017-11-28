using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Connection
    {
        public string Id { get; set; }

        public double Weight { get; set; }

        public Neuron NeuronFrom { get; set; }
        public Neuron NeuronTo { get; set; }

        public Connection(string id)
        {
            Id = id;
            Weight = 0.0;
        }

        public Connection(string id, double weight) : this(id)
        {
            Weight = weight;
        }

        public Connection(string id, Neuron neuronFrom, Neuron neuronTo) : this(id)
        {
            NeuronFrom = neuronFrom;
            NeuronTo = neuronTo;
        }

        public Connection(string id, double weight, Neuron neuronFrom, Neuron neuronTo) : this(id, neuronFrom, neuronTo)
        {
            Weight = weight;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json.Add("Id", Id);
            json.Add("Weight", Weight);
            if (NeuronFrom != null)
            {
                json.Add("NeuronFrom", NeuronFrom.Id);
            }
            else
            {
                json.Add("NeuronFrom", "null");
            }
            if (NeuronTo != null)
            {
                json.Add("NeuronTo", NeuronTo.Id);
            }
            else
            {
                json.Add("NeuronTo", "null");
            }
            return json;
        }
    }
}
