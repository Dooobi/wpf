using Neat;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für Test.xaml
    /// </summary>
    public partial class Test : UserControl
    {
        public List<string> MyStrings { get; set; }
        public List<Generation> MyCollection { get; set; }
        public History History { get; set; }

        private List<List<double>> inputPatterns;
        private List<double> answers;

        public Test()
        {
            InitializeComponent();
            Initialize();

            // Make tooltips stay forever
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            itemsControl.DataContext = this;

            History = History.Singleton;

            MyCollection = new List<Generation>(new Generation[] {
                new Generation(1),
                new Generation(2),
                new Generation(3),
                new Generation(4),
                new Generation(5)
            });

            MyStrings = new List<string>(new string[] { "a", "b", "c", "d" });

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
            Generation currentGeneration = History.CurrentGeneration;
            bool nextGeneration = true;
            while (currentGeneration == null || currentGeneration.Number < 100)
            {
                currentGeneration = History.CurrentGeneration;

                if (nextGeneration)
                {
                    if (currentGeneration != null)
                    {
                        foreach (SpeciesTimestamp speciesTimestamp in currentGeneration.SpeciesTimestamps)
                        {
                            Console.WriteLine(" {0}: {1}", speciesTimestamp.Species.Id, speciesTimestamp.AverageFitness);
                        }

                        Dispatcher.Invoke(() =>
                        {
                            //Dein Code der synchronisiert zur GUI läuft
                            //UpdateGrid();
                        });
                    }
                    if (currentGeneration == null)
                    {
                        Console.WriteLine("-- Start Generation {0} --", 1);
                    }
                    else
                    {
                        Console.WriteLine("-- Start Generation {0} --", currentGeneration.Number + 1);
                    }
                }

                Genome genome = neatController.GetNextGenomeToEvaluate();
                genome.Fitness = EvaluateFitness(genome.Network);

                nextGeneration = neatController.SubmitGenomeAfterEvaluation(genome);
            }
            Dispatcher.Invoke(() =>
            {
                //Dein Code der synchronisiert zur GUI läuft
                //UpdateGrid();
            });

            //Console.WriteLine("  highestCompatibilityValue: {0}", Stats.highestCompatibilityValue);
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
            return Math.Pow(4.0 - totalErrorDiff, 2);
        }
    }
}
