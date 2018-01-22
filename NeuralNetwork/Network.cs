using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Network
    {
        public List<Neuron> Neurons { get; set; }

        private List<Neuron> InputNeurons { get; set; }
        private List<Neuron> HiddenNeurons { get; set; }
        private List<Neuron> OutputNeurons { get; set; }
        private List<Neuron> BiasNeurons { get; set; }
        
        public DelegateActivationFunction ActivationFunction { get; set; }

        private Network()
        {
            Neurons = new List<Neuron>();
            InputNeurons = new List<Neuron>();
            HiddenNeurons = new List<Neuron>();
            OutputNeurons = new List<Neuron>();
            BiasNeurons = new List<Neuron>();
        }

        /*
         * This constructor creates a copy of a Network
         */
        public Network(Network network)
        {
            List<Connection> copiedConnections = new List<Connection>();

            // Copy every Connection
            // IMPORTANT: 
            //  Their NeuronFrom and NeuronTo properties set to the original Neurons
            //  They will be replaced by the copies later
            foreach (Connection connection in network.GetAllConnections())
            {
                if (!copiedConnections.Contains(connection))
                {
                    copiedConnections.Add(new Connection(connection));
                }
            }

            // Now create copies of every Neuron and pass them the copiedConnections
            // so they can use them to connect themselves
            foreach (Neuron inputNeuron in network.InputNeurons)
            {
                Neuron copiedNeuron = new Neuron(inputNeuron, copiedConnections);
                InputNeurons.Add(copiedNeuron);
                Neurons.Add(copiedNeuron);
            }
            foreach (Neuron hiddenNeuron in network.HiddenNeurons)
            {
                Neuron copiedNeuron = new Neuron(hiddenNeuron, copiedConnections);
                HiddenNeurons.Add(copiedNeuron);
                Neurons.Add(copiedNeuron);
            }
            foreach (Neuron outputNeuron in network.OutputNeurons)
            {
                Neuron copiedNeuron = new Neuron(outputNeuron, copiedConnections);
                OutputNeurons.Add(copiedNeuron);
                Neurons.Add(copiedNeuron);
            }
            foreach (Neuron biasNeuron in network.BiasNeurons)
            {
                Neuron copiedNeuron = new Neuron(biasNeuron, copiedConnections);
                BiasNeurons.Add(copiedNeuron);
                Neurons.Add(copiedNeuron);
            }

            // Now that the copies of the Neurons are created the
            // NeuronFrom and NeuronTo properties of the Connections will be 
            // replaced with the corresponding copies of the Neurons
            foreach (Connection connection in copiedConnections)
            {
                connection.NeuronFrom = Neurons.Find(neuron => neuron.Id == connection.NeuronFrom.Id);
                connection.NeuronTo = Neurons.Find(neuron => neuron.Id == connection.NeuronTo.Id);
            }
        }

        public Network(List<Neuron> neurons, DelegateActivationFunction activationFunction) : this()
        {
            Neurons = neurons;
            ActivationFunction = activationFunction;

            // Add neurons to different lists depending on their type
            foreach (Neuron neuron in neurons)
            {
                switch (neuron.Type)
                {
                    case NeuronType.Input:
                        InputNeurons.Add(neuron);
                        break;
                    case NeuronType.Hidden:
                        HiddenNeurons.Add(neuron);
                        break;
                    case NeuronType.Output:
                        OutputNeurons.Add(neuron);
                        break;
                    case NeuronType.Bias:
                        BiasNeurons.Add(neuron);
                        break;
                }
            }
        }

        public List<double> Update(List<double> inputs)
        {
            List<double> outputs = new List<double>();

            // Make a temporary List which contains the hidden neurons and the output neurons
            List<Neuron> hiddenAndOutputNeurons = new List<Neuron>();
            hiddenAndOutputNeurons.AddRange(HiddenNeurons);
            hiddenAndOutputNeurons.AddRange(OutputNeurons);

#if DEBUG
            int updateCounter = 0;
            string debug = "";
#endif


            if (inputs.Count != InputNeurons.Count)
            {
                throw new Exception(String.Format("Network.Update(): Number of inputs ({0}) is not the same as number of InputNeurons ({1})!", inputs.Count, InputNeurons.Count));
            }

            // Set output of InputNeurons to the input of the Network
            for (int i = 0; i < InputNeurons.Count; i++)
            {
                InputNeurons[i].Output = inputs[i];
            }

            // Set the Output of every BiasNeuron to 1.0
            foreach (Neuron neuron in BiasNeurons)
            {
                neuron.Output = 1.0;
            }

            // Set the Output of every HiddenNeuron and OutputNeuron to 0.0 to begin with
            foreach (Neuron neuron in hiddenAndOutputNeurons)
            {
                neuron.Output = 0.0;
            }

            // Update the network once completely for every HiddenNeuron and OutputNeuron it contains
            int numberHiddenAndOutputNeurons = HiddenNeurons.Count + OutputNeurons.Count;
            for (int i = 0; i < numberHiddenAndOutputNeurons; i++)
            {
                // Go through every hidden and output Neuron and calculate their NewOutput
                //  based on the sum of the Output from incoming Neurons (Output of Neurons is 0.0 at the beginning)
                //  and the ActivationFunction of the Network
                foreach (Neuron neuron in hiddenAndOutputNeurons)
                {
                    neuron.UpdateNewOutput(ActivationFunction);
                }

                // All new outputs have been calculated so the neurons Output can now be updated to their NewOutput
                foreach (Neuron neuron in hiddenAndOutputNeurons)
                {
                    neuron.Output = neuron.NewOutput;
                }

#if DEBUG
                updateCounter++;
                Console.WriteLine("Current Network update cycle: {0}", updateCounter);
                debug = "";
                for (int counter = 0; counter < hiddenAndOutputNeurons.Count; counter++)
                {
                    if (counter > 0)
                    {
                        debug += ", ";
                    }
                    debug += String.Format("{0}: {1:0.##}", hiddenAndOutputNeurons[counter].Id, hiddenAndOutputNeurons[counter].Output);
                }
                Console.WriteLine("  Output of neurons are: [" + debug + "]");
#endif
            }

#if DEBUG
            Console.WriteLine("Network has finished updating after {0} cycles.", updateCounter);
#endif

            // All Outputs have stabilized so the outputs of the OutputNeurons can now be returned
            foreach (Neuron outputNeuron in OutputNeurons)
            {
                outputs.Add(outputNeuron.Output);
            }


#if DEBUG
            debug = "";
            for (int counter = 0; counter < hiddenAndOutputNeurons.Count; counter++)
            {
                if (counter > 0)
                {
                    debug += ", ";
                }
                debug += String.Format("{0}: {1:0.##}", hiddenAndOutputNeurons[counter].Id, hiddenAndOutputNeurons[counter].Output);
            }
            Console.WriteLine("  Output of neurons are: [" + debug + "]");
#endif

            return outputs;
        }

        public List<Connection> GetAllConnections()
        {
            List<Connection> connections = new List<Connection>();

            foreach (Neuron neuron in Neurons)
            {
                foreach (Connection connection in neuron.IncomingConnections)
                {
                    if (!connections.Contains(connection))
                    {
                        connections.Add(connection);
                    }
                }
                foreach (Connection connection in neuron.OutgoingConnections)
                {
                    if (!connections.Contains(connection))
                    {
                        connections.Add(connection);
                    }
                }
            }

            return connections;
        }

        public static Network FromJObject(JObject json)
        {
            DelegateActivationFunction activationFunction = null;

            List<Neuron> neurons = new List<Neuron>();
            
            foreach (MethodInfo method in typeof(ActivationFunction).GetMethods())
            {
                if (method.Name == (string)json.GetValue("ActivationFunction"))
                {
                    activationFunction = (DelegateActivationFunction)Delegate.CreateDelegate(typeof(DelegateActivationFunction), method);
                }
            }
            
            JArray jsonConnections = (JArray)json.GetValue("Connections");
            JArray jsonNeurons = (JArray)json.GetValue("Neurons");

            // Create list with neurons from JArray
            foreach (JObject jsonNeuron in jsonNeurons)
            {
                NeuronType neuronType = NeuronType.None;
                foreach (NeuronType type in Enum.GetValues(typeof(NeuronType)))
                {
                    if (type.ToString() == (string)jsonNeuron.GetValue("Type"))
                    {
                        neuronType = type;
                    }
                }
                Neuron neuron = new Neuron((string)jsonNeuron.GetValue("Id"), neuronType);
                neuron.Output = (double)jsonNeuron.GetValue("Output");
                neuron.NewOutput = (double)jsonNeuron.GetValue("NewOutput");
                neurons.Add(neuron);
            }

            // Create connections from JArray and connect them with neurons
            foreach (JObject jsonConnection in jsonConnections)
            {
                Neuron neuronFrom = null;
                Neuron neuronTo = null;
                string neuronFromId = (string)jsonConnection.GetValue("NeuronFrom");
                string neuronToId = (string)jsonConnection.GetValue("NeuronTo");

                foreach (Neuron neuron in neurons)
                {
                    if (neuron.Id == neuronFromId)
                    {
                        neuronFrom = neuron;
                    }
                    if (neuron.Id == neuronToId)
                    {
                        neuronTo = neuron;
                    }
                }

                Connection connection = new Connection((string)jsonConnection.GetValue("Id"), (double)jsonConnection.GetValue("Weight"), neuronFrom, neuronTo);
                if (neuronFrom != null)
                {
                    neuronFrom.OutgoingConnections.Add(connection);
                }
                if (neuronTo != null)
                {
                    neuronTo.IncomingConnections.Add(connection);
                }
            }

            return new Network(neurons, activationFunction);
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public JObject ToJson()
        {
            JObject json = new JObject();

            JArray neurons = new JArray();
            foreach (Neuron neuron in Neurons)
            {
                neurons.Add(neuron.ToJson());
            }

            JArray connections = new JArray();
            foreach (Connection connection in GetAllConnections())
            {
                connections.Add(connection.ToJson());
            }

            json.Add("Neurons", neurons);
            json.Add("Connections", connections);
            json.Add("ActivationFunction", ActivationFunction.Method.Name);

            return json;
        }
    }
}
