using Newtonsoft.Json.Linq;
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

        public ConnectionGene()
        {
        }

        public ConnectionGene(string id, int innovationNumber, bool isEnabled, double weight, NeuronGene neuronGeneFrom, NeuronGene neuronGeneTo)
        {
            Id = id;
            InnovationNumber = innovationNumber;
            IsEnabled = isEnabled;
            Weight = weight;
            NeuronGeneFrom = neuronGeneFrom;
            NeuronGeneTo = neuronGeneTo;
        }

        /*
         * This constructor creates a copy of a ConnectionGene
         * It needs a list of copied NeuronGenes to set the
         *  NeuronGeneFrom and NeuronGeneTo properties
         * to a copy of the original NeuronGenes
         */
        public ConnectionGene(ConnectionGene connectionGene, List<NeuronGene> copiedNeuronGenes)
        {
            Id = connectionGene.Id;
            NeuronGeneFrom = copiedNeuronGenes.Find(neuronGene => neuronGene.Id == connectionGene.NeuronGeneFrom.Id);
            NeuronGeneTo = copiedNeuronGenes.Find(neuronGene => neuronGene.Id == connectionGene.NeuronGeneTo.Id);
            Weight = connectionGene.Weight;
            InnovationNumber = connectionGene.InnovationNumber;
            IsEnabled = connectionGene.IsEnabled;
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
            json.Add("InnovationNumber", InnovationNumber);
            json.Add("IsEnabled", IsEnabled);
            if (NeuronGeneFrom != null)
            {
                json.Add("NeuronGeneFrom", NeuronGeneFrom.Id);
            }
            else
            {
                json.Add("NeuronGeneFrom", "null");
            }
            if (NeuronGeneTo != null)
            {
                json.Add("NeuronGeneTo", NeuronGeneTo.Id);
            }
            else
            {
                json.Add("NeuronGeneTo", "null");
            }
            return json;
        }
    }
}
