﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class Innovation
    {
        public int InnovationNumber { get; set; }
        public string NeuronGeneFromId { get; set; }
        public string NeuronGeneToId { get; set; }

        public Innovation(int innovationNumber, string neuronGeneFromId, string neuronGeneToId)
        {
            InnovationNumber = innovationNumber;
            NeuronGeneFromId = neuronGeneFromId;
            NeuronGeneToId = neuronGeneToId;
        }
    }
}
