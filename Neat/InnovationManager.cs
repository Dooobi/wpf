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
            NextInnovationNumber = 1;
        }

        public int GetInnovationNumber(string neuronGeneFromId, string neuronGeneToId)
        {
            Innovation foundInnovation = Innovations.Find(innovation => innovation.NeuronGeneFromId == neuronGeneFromId && innovation.NeuronGeneToId == neuronGeneToId);
            if (foundInnovation == null)
            {
                foundInnovation = new Innovation(NextInnovationNumber++, neuronGeneFromId, neuronGeneToId);
                Innovations.Add(foundInnovation);
            }

            return foundInnovation.InnovationNumber;
        }
    }
}
