using NeuralNetwork;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

        private Genome()
        {
            NeuronGenes = new List<NeuronGene>();
            ConnectionGenes = new List<ConnectionGene>();
        }

        /**
         * This constructors create a basic genome with:
         *   Input NeuronGenes depending on numberOfInputs,
         *   Output NeuronGenes depending on numberOfOutputs and
         *   one Bias NeuronGene.
         *   The inputs and the bias are connected to all outputs
         */
        public Genome(string id, int numberOfInputs, int numberOfOutputs) : this()
        {
            Id = id;
            NumberOfInputs = numberOfInputs;
            NumberOfOutputs = numberOfOutputs;

            // Create input NeuronGenes
            for (int i = 0; i < NumberOfInputs; i++)
            {
                NeuronGenes.Add(new NeuronGene("in" + i + 1, NeuronType.Input));
            }

            // Create bias NeuronGene
            NeuronGenes.Add(new NeuronGene("bias", NeuronType.Bias));

            // Create output NeuronGenes
            for (int i = 0; i < numberOfOutputs; i++)
            {
                NeuronGenes.Add(new NeuronGene("out" + i + 1, NeuronType.Output));
            }

            // Create the ConnectionGenes
            // Go through every input NeuronGene and the bias NeuronGene
            for (int inputIndex = 0; inputIndex < numberOfInputs + 1; inputIndex++)
            {
                // For every input NeuronGene and the bias NeuronGene:
                //  Go through every output NeuronGene
                for (int outputIndex = 0; outputIndex < numberOfOutputs + 1; outputIndex++)
                {
                    // And connect the input/bias NeuronGene with the output NeuronGene
                    NeuronGene neuronGeneFrom = NeuronGenes[inputIndex];
                    NeuronGene neuronGeneTo = NeuronGenes[outputIndex + numberOfInputs + 1];
                    string connectionId = "c_" + neuronGeneFrom.Id + "_" + neuronGeneTo.Id;
                    ConnectionGenes.Add(new ConnectionGene(connectionId, INNOVATION_NUMBER, true, Utils.RandDouble(-1.0, 1.0), neuronGeneFrom, neuronGeneTo);
                }
            }

        }

        public static Genome FromFile(string filepath)
        {
            string text = File.ReadAllText(filepath);
            Genome genome = new Genome();

            JObject json = JObject.Parse(text);

            List<NeuronGene> neuronGenes = new List<NeuronGene>();
            List<ConnectionGene> connectionGenes = new List<ConnectionGene>();

            JArray jsonNeuronGenes = (JArray)json.GetValue("NeuronGenes");
            JArray jsonConnectionGenes = (JArray)json.GetValue("ConnectionGenes");            

            // Create list with neuronGenes from JArray
            foreach (JObject jsonNeuronGene in jsonNeuronGenes)
            {
                NeuronType neuronType = NeuronType.None;
                foreach (NeuronType type in Enum.GetValues(typeof(NeuronType)))
                {
                    if (type.ToString() == (string)jsonNeuronGene.GetValue("Type"))
                    {
                        neuronType = type;
                    }
                }
                NeuronGene neuronGene = new NeuronGene((string)jsonNeuronGene.GetValue("Id"), neuronType);
                neuronGenes.Add(neuronGene);
            }

            // Create connectionGenes from JArray and connect them with neurons
            foreach (JObject jsonConnectionGene in jsonConnectionGenes)
            {
                NeuronGene neuronGeneFrom = null;
                NeuronGene neuronGeneTo = null;
                string neuronGeneFromId = (string)jsonConnectionGene.GetValue("NeuronGeneFrom");
                string neuronGeneToId = (string)jsonConnectionGene.GetValue("NeuronGeneTo");

                foreach (NeuronGene neuronGene in neuronGenes)
                {
                    if (neuronGene.Id == neuronGeneFromId)
                    {
                        neuronGeneFrom = neuronGene;
                    }
                    if (neuronGene.Id == neuronGeneToId)
                    {
                        neuronGeneTo = neuronGene;
                    }
                }
                
                connectionGenes.Add(new ConnectionGene((string)jsonConnectionGene.GetValue("Id"), (int)jsonConnectionGene.GetValue("InnovationNumber"), (bool)jsonConnectionGene.GetValue("IsEnabled"), (double)jsonConnectionGene.GetValue("Weight"), neuronGeneFrom, neuronGeneTo));
            }

            return genome;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            JArray neuronGenes = new JArray();
            foreach (NeuronGene neuronGene in NeuronGenes)
            {
                neuronGenes.Add(neuronGene.ToJson());
            }

            JArray connectionGenes = new JArray();
            foreach (ConnectionGene connectionGene in ConnectionGenes)
            {
                connectionGenes.Add(connectionGene.ToJson());
            }

            json.Add("Id", Id);
            json.Add("NeuronGenes", neuronGenes);
            json.Add("ConnectionGenes", connectionGenes);
            json.Add("NumberOfInputs", NumberOfInputs);
            json.Add("NumberOfOutputs", NumberOfOutputs);
            json.Add("Fitness", Fitness);

            return json;
        }
    }
}
