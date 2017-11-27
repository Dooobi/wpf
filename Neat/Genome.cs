using NeuralNetwork;
using System;
using System.Collections.Generic;
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
         * This constructor create a basic genome with:
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

    }
}
