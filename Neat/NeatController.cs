using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public class NeatController
    {
        public GeneticAlgorithm GeneticAlgorithm { get; set; }

        public Stack<Genome> CurrentPopulationBeforeEvaluation { get; set; }
        public List<Genome> CurrentPopulationAfterEvaluation { get; set; }

        public NeatController()
        {
            GeneticAlgorithm = new GeneticAlgorithm();
            History.InitHistory();
        }

        public Genome GetNextGenomeToEvaluate()
        {
            return CurrentPopulationBeforeEvaluation.Pop();
        }

        public void SubmitGenomeAfterEvaluation(Genome genome)
        {
            CurrentPopulationAfterEvaluation.Add(genome);
            
            if (CurrentPopulationAfterEvaluation.Count == Config.populationSize)
            {
                GeneticAlgorithm.Epoch(CurrentPopulationAfterEvaluation);
            }
        }

        private void CreateInitialPopulation()
        {
            CurrentPopulationBeforeEvaluation = new Stack<Genome>();
            int generationOfPopulation = History.Generations.Count + 1;

            for (int i = 0; i < Config.populationSize; i++)
            {
                string id = "g" + generationOfPopulation + ": " + i;
                CurrentPopulationBeforeEvaluation.Push(new Genome(id, Config.numberOfInputs, Config.numberOfOutputs));
            }
        }
    }
}
