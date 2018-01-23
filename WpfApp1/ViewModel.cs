using Neat;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    public class ViewModel
    {
        private List<List<double>> inputPatterns;
        private List<double> answers;

        public ViewModel()
        {
            Initialize();
            TestGeneticAlgorithm();
        }

        private void Initialize()
        {
            // Initialize input and correct output for the NeuralNetworks
            inputPatterns = new List<List<double>>();
            inputPatterns.Add(new List<double>(new double[] { 0, 0 }));
            inputPatterns.Add(new List<double>(new double[] { 0, 1 }));
            inputPatterns.Add(new List<double>(new double[] { 1, 0 }));
            inputPatterns.Add(new List<double>(new double[] { 1, 1 }));
            answers = new List<double>(new double[] { 0, 1, 1, 0 });
        }

        private void TestGeneticAlgorithm()
        {
            // Connect to game

            // Initialize NeatController
            NeatController neatController = new NeatController();
            bool nextGeneration = true;
            while (History.CurrentGeneration == null || History.CurrentGeneration.Number < 100)
            {
                if (nextGeneration)
                {
                    if (History.CurrentGeneration == null)
                    {
                        Console.WriteLine("-- Start Generation {0} --", 1);
                    }
                    else
                    {
                        Console.WriteLine("-- Start Generation {0} --", History.CurrentGeneration.Number + 1);
                    }
                }

                Genome genome = neatController.GetNextGenomeToEvaluate();

                genome.Fitness = EvaluateFitness(genome.Network);

                nextGeneration = neatController.SubmitGenomeAfterEvaluation(genome);
            }
        }

        private double EvaluateFitness(Network network)
        {
            double totalErrorDiff = 0.0;
            for (int i = 0; i < inputPatterns.Count; i++)
            {
                List<double> inputPattern = inputPatterns[i];
                List<double> output = network.Update(inputPattern);
                double answer = answers[i];

                totalErrorDiff += Math.Abs(output[0] - answer);
            }
            return Math.Sqrt(4.0 - totalErrorDiff);
        }
    }
}
