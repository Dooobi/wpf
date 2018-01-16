using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class InnovationManager
    {
        public static InnovationManager Instance = new InnovationManager();

        public int NextInnovationNumber { get; set; }

        public List<Innovation> Innovations { get; set; }

        public InnovationManager()
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
            }

            // Add the ConnectionGenes
            foreach (ConnectionGene connectionGene in startConnectionGenes)
            {
                Innovations.Add(new Innovation());
            }
        }

        public int GetInnovationNumber(string neuronGeneFromId, string neuronGeneToId) {

            foreach (Innovation innovation in Innovations)
            {
                if (innovation.NeuronGeneFromId == neuronGeneFromId
                    && innovation.NeuronGeneToId == neuronGeneToId)
                {
                    return innovation.InnovationNumber;
                }
            }

            return NextInnovationNumber++;
        }
    }
}
