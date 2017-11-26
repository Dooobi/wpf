using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            InputNeurons = new List<Neuron>();
            HiddenNeurons = new List<Neuron>();
            OutputNeurons = new List<Neuron>();
            BiasNeurons = new List<Neuron>();
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

            // Update the network once completely for every HiddenNeuron it contains
            for (int i = 0; i < HiddenNeurons.Count; i++)
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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }
    }
}
