using Neat;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _showFittestGenomeOfSpeciesTimestamp;
        public ICommand ShowFittestGenomeOfSpeciesTimestamp
        {
            get
            {
                if (_showFittestGenomeOfSpeciesTimestamp == null)
                {
                    _showFittestGenomeOfSpeciesTimestamp = new RelayCommand(
                        parameter => true,
                        parameter =>
                        {
                            SpeciesTimestamp selectedSpeciesTimestamp = parameter as SpeciesTimestamp;
                            if (selectedSpeciesTimestamp != null)
                            {
                                SelectedGenome = selectedSpeciesTimestamp.FittestGenome;
                            }                                
                        });
                }
                return _showFittestGenomeOfSpeciesTimestamp;
            }
        }

        public List<string> MyStrings { get; set; }
        public List<Generation> MyCollection { get; set; }
        public History History { get; set; }

        private List<List<double>> inputPatterns;
        private List<double> answers;

        private Genome selectedGenome;
        public Genome SelectedGenome
        {
            get
            {
                return selectedGenome;
            }
            set
            {
                if (value != this.selectedGenome)
                {
                    this.selectedGenome = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MainWindowViewModel()
        {
            Initialize();

            History = History.Singleton;
            History.Test = "test";

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

        // NotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
