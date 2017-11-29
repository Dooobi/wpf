using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class InnovationManager
    {
        public int NextInnovationNumber { get; set; }

        public List<Innovation> Innovations { get; set; }

        private InnovationManager()
        {
            Innovations = new List<Innovation>();
            NextInnovationNumber = 0;
        }

        public InnovationManager(List<NeuronGene> startNeuronGenes, List<ConnectionGene> startConnectionGenes) : this()
        {
            // Add the NeuronGenes
            foreach (NeuronGene neuronGene in startNeuronGenes)
            {
                Innovations.Add(new Innovation());
                //SInnovation(start_neurons[nd], m_NextInnovationNum++, m_NextNeuronID++);
            }

            // Add the ConnectionGenes
            foreach (ConnectionGene connectionGene in startConnectionGenes)
            {
                //SInnovation NewInnov(start_genes[cGen].FromNeuron,
                //                 start_genes[cGen].ToNeuron,
                //                 new_link,
                //                 m_NextInnovationNum++);

                Innovations.Add(new Innovation());
            }
        }
    }
}
