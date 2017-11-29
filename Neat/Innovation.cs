using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public enum InnovationType
    {
        NewNeuronGene,
        NewConnectionGene
    }

    public class Innovation
    {
        public int Id { get; set; }
        public InnovationType Type { get; set; }

    }
}
