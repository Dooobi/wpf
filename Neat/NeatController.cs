using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class NeatController
    {
        public List<Genome> Genomes { get; set; }

        private List<Generation> Generations;
        public int CurrentGeneration {
            get
            {
                return Generations.Count;
            }
        }

        public InnovationManager InnovationManager { get; set; }

        public NeatController()
        {
            Generations = new List<Generation>();
            InnovationManager = new InnovationManager();
            
        }

        public Species GetSpecies(int generationId, int speciesId)
        {
            return Generations[generationId][speciesId];
        }
    }
}
