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

        public Generation Generation { get; set; }
        public Species Species { get; set; }

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
         * This constructor creates a copy of a genome
         */
        public Genome(Genome genome) : this()
        {
            Id = genome.Id;
            foreach (NeuronGene neuronGene in genome.NeuronGenes)
            {
                NeuronGenes.Add(new NeuronGene(neuronGene));
            }
            foreach (ConnectionGene connectionGene in genome.ConnectionGenes)
            {
                ConnectionGenes.Add(new ConnectionGene(connectionGene, NeuronGenes));
            }
            Network = new Network(genome.Network);
            Fitness = genome.Fitness;
            AdjustedFitness = genome.AdjustedFitness;
            NumberOfInputs = genome.NumberOfInputs;
            NumberOfOutputs = genome.NumberOfOutputs;
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
                for (int outputIndex = 0; outputIndex < numberOfOutputs; outputIndex++)
                {
                    // And connect the input/bias NeuronGene with the output NeuronGene
                    NeuronGene neuronGeneFrom = NeuronGenes[inputIndex];
                    NeuronGene neuronGeneTo = NeuronGenes[outputIndex + numberOfInputs + 1];

                    AddBasicConnectionGene(neuronGeneFrom, neuronGeneTo);
                }
            }
        }

        public void GenerateNetwork(DelegateActivationFunction activationFunction)
        {
            List<Neuron> neurons = new List<Neuron>();
            List<Connection> connections = new List<Connection>();

            foreach (NeuronGene neuronGene in NeuronGenes)
            {
                neurons.Add(new Neuron(neuronGene.Id, neuronGene.Type));
            }
            foreach (ConnectionGene connectionGene in ConnectionGenes)
            {
                if (connectionGene.IsEnabled)
                {
                    Neuron neuronFrom = neurons.Find(neuron => connectionGene.NeuronGeneFrom.Id == neuron.Id);
                    Neuron neuronTo = neurons.Find(neuron => connectionGene.NeuronGeneTo.Id == neuron.Id);
                    Connection connection = new Connection(connectionGene.Id, connectionGene.Weight, neuronFrom, neuronTo);
                    neuronFrom.OutgoingConnections.Add(connection);
                    neuronTo.IncomingConnections.Add(connection);
                    connections.Add(connection);
                }
            }

            Network = new Network(neurons, activationFunction);
        }

        public string GetNextNeuronGeneId(NeuronType neuronType)
        {
            List<NeuronGene> neuronGenesOfType = new List<NeuronGene>();
            foreach (NeuronGene neuronGene in NeuronGenes)
            {
                if (neuronGene.Type == neuronType)
                {
                    neuronGenesOfType.Add(neuronGene);
                }
            }
            string id = "";
            switch (neuronType)
            {
                case NeuronType.Hidden:
                    id = "h";
                    break;
                case NeuronType.Bias:
                    id = "bias";
                    break;
                case NeuronType.Input:
                    id = "in";
                    break;
                case NeuronType.Output:
                    id = "out";
                    break;
                case NeuronType.None:
                    id = "none";
                    break;
            }
            return id + (neuronGenesOfType.Count+1);
        }

        public Dictionary<int, ConnectionGene> GetConnectionGenesByInnovationNumber()
        {
            Dictionary<int, ConnectionGene> connectionGenesByInnovationNumber = new Dictionary<int, ConnectionGene>();

            foreach (ConnectionGene connectionGene in ConnectionGenes)
            {
                int innovationNumber = connectionGene.InnovationNumber;
                connectionGenesByInnovationNumber[innovationNumber] = connectionGene;
            }

            return connectionGenesByInnovationNumber;
        }

        /* Adds the ConnectionGene to the child and overwrites the 
         * NeuronGeneFrom and NeuronGeneTo properties with the actual
         * NeuronGenes of the child Genome (creates the NeuronGenes if they don't exist)
         */
        public void AddConnectionGeneOverwriteNeuronGenes(ConnectionGene connectionGene)
        {
            string neuronGeneFromId = connectionGene.NeuronGeneFrom.Id;
            string neuronGeneToId = connectionGene.NeuronGeneTo.Id;

            NeuronGene neuronGeneFrom = NeuronGenes.Find(neuronGene => neuronGene.Id == neuronGeneFromId);
            NeuronGene neuronGeneTo = NeuronGenes.Find(neuronGene => neuronGene.Id == neuronGeneToId);

            if (neuronGeneFrom == null)
            {
                neuronGeneFrom = new NeuronGene(neuronGeneFromId, connectionGene.NeuronGeneFrom.Type);
                NeuronGenes.Add(neuronGeneFrom);
            }
            if (neuronGeneFrom == null)
            {
                neuronGeneTo = new NeuronGene(neuronGeneToId, connectionGene.NeuronGeneTo.Type);
                NeuronGenes.Add(neuronGeneTo);
            }

            connectionGene.NeuronGeneFrom = neuronGeneFrom;
            connectionGene.NeuronGeneTo = neuronGeneTo;

            ConnectionGenes.Add(connectionGene);
        }

        public ConnectionGene AddBasicConnectionGene(NeuronGene neuronGeneFrom, NeuronGene neuronGeneTo)
        {
            string connectionId = "c_" + neuronGeneFrom.Id + "_" + neuronGeneTo.Id;
            int innovationNumber = InnovationManager.Instance.GetInnovationNumber(neuronGeneFrom.Id, neuronGeneTo.Id);
            ConnectionGene connectionGene = new ConnectionGene(connectionId, innovationNumber, true, Utils.RandDouble(Config.minWeightLimit, Config.maxWeightLimit), neuronGeneFrom, neuronGeneTo);
            ConnectionGenes.Add(connectionGene);

            return connectionGene;
        }

        public static Genome FromJObject(JObject json)
        {
            Genome genome = new Genome();
            
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
            json.Add("Generation", Generation.Number);
            json.Add("Species", Species.Id);
            json.Add("NeuronGenes", neuronGenes);
            json.Add("ConnectionGenes", connectionGenes);
            json.Add("NumberOfInputs", NumberOfInputs);
            json.Add("NumberOfOutputs", NumberOfOutputs);
            json.Add("Fitness", Fitness);

            return json;
        }
    }
}
