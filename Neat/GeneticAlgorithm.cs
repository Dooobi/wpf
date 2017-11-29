using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class GeneticAlgorithm
    {
        public List<Genome> Genomes { get; set; }

        private List<Generation> Generations;
        public int CurrentGeneration
        {
            get
            {
                return Generations.Count;
            }
        }

        //current population
        vector<CGenome> m_vecGenomes;

        //keep a record of the last generations best genomes. (used to render
        //the best performers to the display if the user presses the 'B' key)
        vector<CGenome> m_vecBestGenomes;

        //all the species
        vector<CSpecies> m_vecSpecies;

        //to keep track of innovations
        CInnovation* m_pInnovation;

        //current generation
        int m_iGeneration;

        int m_iNextGenomeID;

        int m_iNextSpeciesID;

        int m_iPopSize;

        //adjusted fitness scores
        double m_dTotFitAdj,
                          m_dAvFitAdj;

        //index into the genomes for the fittest genome
        int m_iFittestGenome;

        double m_dBestEverFitness;

        //separates each individual into its respective species by calculating
        //a compatibility score with every other member of the population and 
        //niching accordingly.
        void Speciate();

        //adjusts the fitness scores depending on the number sharing the 
        //species and the age of the species.
        void AdjustSpeciesFitnesses();

        CGenome Crossover(CGenome& mum, CGenome& dad);

        CGenome TournamentSelection(const int NumComparisons);

    }
}
