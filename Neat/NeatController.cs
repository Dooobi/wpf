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
            CurrentPopulationBeforeEvaluation = new Stack<Genome>();
            CurrentPopulationAfterEvaluation = new List<Genome>();
            GeneticAlgorithm = new GeneticAlgorithm();
            History.InitHistory();
            CreateInitialPopulation();
        }

        public Genome GetNextGenomeToEvaluate()
        {
            return CurrentPopulationBeforeEvaluation.Pop();
        }

        public bool SubmitGenomeAfterEvaluation(Genome genome)
        {
            CurrentPopulationAfterEvaluation.Add(genome);
            
            if (CurrentPopulationAfterEvaluation.Count == Config.populationSize)
            {
                List<Genome> populationForNextGeneration = GeneticAlgorithm.Epoch(CurrentPopulationAfterEvaluation);
                CurrentPopulationBeforeEvaluation.Clear();
                CurrentPopulationAfterEvaluation.Clear();

                foreach (Genome g in populationForNextGeneration)
                {
                    g.GenerateNetwork(ActivationFunction.SigmoidModified);
                    CurrentPopulationBeforeEvaluation.Push(g);
                }

                // A new Generation has been created
                return true;
            }
            
            return false;
        }

        private void CreateInitialPopulation()
        {
            CurrentPopulationBeforeEvaluation.Clear();
            int generationOfPopulation = History.Generations.Count + 1;

            for (int i = 0; i < Config.populationSize; i++)
            {
                string id = "g" + generationOfPopulation + ": " + i;
                Genome genome = new Genome(id, Config.numberOfInputs, Config.numberOfOutputs);
                genome.SetupInitialNeuronGenes();
                genome.ConnectAllInputNeuronGenesToAllOutputNeuronGenes();
                genome.GenerateNetwork(ActivationFunction.SigmoidModified);
                CurrentPopulationBeforeEvaluation.Push(genome);
            }
        }
    }
}
