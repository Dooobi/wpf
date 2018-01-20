using NeuralNetwork;
using Newtonsoft.Json.Linq;
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

        /*
         * This constructor creates a copy of a NeuronGene
         */
        public NeuronGene(NeuronGene neuronGene)
        {
            Id = neuronGene.Id;
            Type = neuronGene.Type;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json.Add("Id", Id);
            json.Add("Type", Type.ToString());

            return json;
        }
    }
}
