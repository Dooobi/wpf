using Neat;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public History History {
            get
            {
                return History.Singleton;
            }
        }

        private double generationDiameter = 60;
        private double maxSpeciesDiameter = 60;
        private double minSpeciesDiameter = 5;

        private TextBlock headerGeneration, headerSpecies;

        private List<List<double>> inputPatterns;
        private List<double> answers;

        public MainWindow()
        {
            InitializeComponent();
            InitGrid();
            Initialize();
        }

        private void InitGrid()
        {
            // Init headers
            headerGeneration = new TextBlock();
            headerGeneration.FontSize = 18;
            headerGeneration.TextAlignment = TextAlignment.Center;
            headerGeneration.Text = "Gen.";
            headerGeneration.Foreground = Brushes.White;

            headerSpecies = new TextBlock();
            headerSpecies.FontSize = 18;
            headerSpecies.TextAlignment = TextAlignment.Justify;
            headerSpecies.Text = "S p e c i e s";
            headerSpecies.Foreground = Brushes.White;

            // Column for the Generations
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(Math.Max(generationDiameter, maxSpeciesDiameter));
            grid.ColumnDefinitions.Add(column);

            // Empty column between Generation and Speciess
            column = new ColumnDefinition();
            column.Width = new GridLength(20);
            grid.ColumnDefinitions.Add(column);

            // Start with one Species column
            column = new ColumnDefinition();
            column.Width = new GridLength(Math.Max(generationDiameter, maxSpeciesDiameter));
            grid.ColumnDefinitions.Add(column);

            // Row for the headers
            //RowDefinition row = new RowDefinition();
            //row.Height = new GridLength(40);
            //grid.RowDefinitions.Add(row);

            //// Add headers
            //Grid.SetRow(headerGeneration, 0);
            //Grid.SetColumn(headerGeneration, 0);
            //grid.Children.Add(headerGeneration);

            //Grid.SetRow(headerSpecies, 0);
            //Grid.SetColumn(headerSpecies, 2);
            //grid.Children.Add(headerSpecies);
        }

        private void UpdateGrid()
        {
            int numberOfSpeciesColumns = grid.ColumnDefinitions.Count - 2;
            int numberOfGenerationRows = grid.RowDefinitions.Count - 1;

            // Grid is updated when a new Generation was created

            int maxNumberOfSpeciesPerGeneration = 0;
            foreach (List<Species> speciess in History.Speciess.Values)
            {
                if (speciess.Count > maxNumberOfSpeciesPerGeneration)
                {
                    maxNumberOfSpeciesPerGeneration = speciess.Count;
                }
            }

            for (; numberOfSpeciesColumns < maxNumberOfSpeciesPerGeneration; numberOfSpeciesColumns++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(Math.Max(generationDiameter, maxSpeciesDiameter));

                grid.ColumnDefinitions.Add(column);

                Grid.SetColumnSpan(headerSpecies, numberOfSpeciesColumns + 1);
                headerSpecies.Width = numberOfSpeciesColumns * Math.Max(generationDiameter, maxSpeciesDiameter);
            }

            for (; numberOfGenerationRows < History.Generations.Count; numberOfGenerationRows++)
            {
                Generation generation = History.Generations[numberOfGenerationRows];

                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(Math.Max(generationDiameter, maxSpeciesDiameter));
                grid.RowDefinitions.Add(row);

                AddGenerationButton(generation, numberOfGenerationRows + 1);

                for (int col = 0; col < generation.SpeciesTimestamps.Count; col++)
                {
                    SpeciesTimestamp speciesTimestamp = generation.SpeciesTimestamps[col];

                    AddSpeciesButton(speciesTimestamp, numberOfGenerationRows + 1, col + 2);
                }
            }
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

        private void AddGenerationButton(Generation generation, int rowIndex)
        {
            RoundButton generationButton = new RoundButton();
            generationButton.DataContext = generation;
            generationButton.Color = Brushes.CornflowerBlue;
            generationButton.Diameter = generationDiameter;
            generationButton.FontSize = generationButton.Diameter / 3;
            generationButton.Text = generation.Number + "";

            Grid.SetRow(generationButton, rowIndex);
            Grid.SetColumn(generationButton, 0);
            grid.Children.Add(generationButton);
        }

        private void AddSpeciesButton(SpeciesTimestamp speciesTimestamp, int rowIndex, int colIndex)
        {
            double areaFactor = ((double)speciesTimestamp.Members.Count / Config.populationSize);
            double maxArea = Math.Pow(maxSpeciesDiameter/2.0, 2) * Math.PI;
            double adjustedArea = Math.Max(minSpeciesDiameter, maxArea * areaFactor);
            double diameter = Math.Sqrt(adjustedArea / Math.PI) * 2;

            RoundButton speciesButton = new RoundButton();
            double hue = Utils.Map(speciesTimestamp.AverageFitness, 0.0, (double)16.0, 0.0, 110.0);
            Brush brush = new SolidColorBrush(Utils.HSBtoRGB(hue, 255.0, 255.0, 255.0));
            speciesButton.Color = brush;
            speciesButton.Diameter = diameter;
            speciesButton.FontSize = speciesButton.Diameter / 3.0 + 0.1;
            speciesButton.Text = speciesTimestamp.Species.Id;

            Grid.SetRow(speciesButton, rowIndex);
            Grid.SetColumn(speciesButton, colIndex);
            grid.Children.Add(speciesButton);
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
                    if (currentGeneration != null) {
                        foreach (SpeciesTimestamp speciesTimestamp in currentGeneration.SpeciesTimestamps)
                        {
                            Console.WriteLine(" {0}: {1}", speciesTimestamp.Species.Id, speciesTimestamp.AverageFitness);
                        }

                        Dispatcher.Invoke(() =>
                        {
                            //Dein Code der synchronisiert zur GUI läuft
                            UpdateGrid();
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
                UpdateGrid();
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

        private void TestNetwork()
        {
            List<Neuron> neurons = GetNeurons();

            Network network = new Network(neurons, ActivationFunction.Tanh);

            Utils.WriteToFile(@"network_before_update.json", network.ToString(), false, false);

            Network netFromFile = Network.FromJObject(Utils.FileToJObject(@"C:\Users\t.stelzer\dev\C#\Repos\wpf\NeuralNetwork\bin\Debug\network_before.json"));

            List<double> inputs = new List<double>();
            inputs.Add(2.3);
            inputs.Add(-1.5);
            inputs.Add(-0.5);

            netFromFile.Update(inputs);

            Utils.WriteToFile(@"network_after_update.json", netFromFile.ToString(), false, false);
        }

        private DrawingImage GetGraph()
        {
            PathFigure pfSpecies = new PathFigure();
            pfSpecies.StartPoint = new Point(0, 0);
            pfSpecies.IsClosed = true;
            pfSpecies.Segments.Add(new LineSegment(new Point(100, 0), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(100, 20), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(80, 20), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(80, 50), true));

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(pfSpecies);

            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Brush = Brushes.Orange;
            geometryDrawing.Pen = new Pen(Brushes.Black, 3);
            geometryDrawing.Geometry = geometry;

            return new DrawingImage(geometryDrawing);
        }

        private List<Neuron> GetNeurons()
        {
            List<Neuron> neurons = new List<Neuron>();

            Neuron in1 = new Neuron("in1", NeuronType.Input);
            Neuron in2 = new Neuron("in2", NeuronType.Input);
            Neuron in3 = new Neuron("in3", NeuronType.Input);

            Neuron h1 = new Neuron("h1", NeuronType.Hidden);
            Neuron h2 = new Neuron("h2", NeuronType.Hidden);
            Neuron h3 = new Neuron("h3", NeuronType.Hidden);
            Neuron h4 = new Neuron("h4", NeuronType.Hidden);
            Neuron h5 = new Neuron("h5", NeuronType.Hidden);

            Neuron out1 = new Neuron("out", NeuronType.Output);

            Neuron bias = new Neuron("bias", NeuronType.Bias);

            Connection c1 = new Connection("c1");
            c1.NeuronFrom = in1;
            c1.NeuronTo = h4;
            c1.Weight = 0.7;

            Connection c2 = new Connection("c2");
            c2.NeuronFrom = in1;
            c2.NeuronTo = h1;
            c2.Weight = 0.9;

            Connection c3 = new Connection("c3");
            c3.NeuronFrom = h1;
            c3.NeuronTo = h3;
            c3.Weight = -0.7;

            Connection c4 = new Connection("c4");
            c4.NeuronFrom = h3;
            c4.NeuronTo = h4;
            c4.Weight = 0.3;

            Connection c5 = new Connection("c5");
            c5.NeuronFrom = in3;
            c5.NeuronTo = out1;
            c5.Weight = -0.5;

            Connection c6 = new Connection("c6");
            c6.NeuronFrom = bias;
            c6.NeuronTo = h2;
            c6.Weight = 0.4;

            Connection c7 = new Connection("c7");
            c7.NeuronFrom = bias;
            c7.NeuronTo = h5;
            c7.Weight = -0.2;

            Connection c8 = new Connection("c8");
            c8.NeuronFrom = h5;
            c8.NeuronTo = h4;
            c8.Weight = -0.5;

            Connection c9 = new Connection("c9");
            c9.NeuronFrom = h5;
            c9.NeuronTo = out1;
            c9.Weight = 0.8;

            Connection c10 = new Connection("c10");
            c10.NeuronFrom = h4;
            c10.NeuronTo = out1;
            c10.Weight = 0.7;

            Connection c11 = new Connection("c11");
            c11.NeuronFrom = h2;
            c11.NeuronTo = h1;
            c11.Weight = -0.4;

            Connection c12 = new Connection("c12");
            c12.NeuronFrom = h3;
            c12.NeuronTo = h2;
            c12.Weight = 0.6;

            Connection c13 = new Connection("c13");
            c13.NeuronFrom = h4;
            c13.NeuronTo = h4;
            c13.Weight = 0.6;

            in1.OutgoingConnections.Add(c1);
            in1.OutgoingConnections.Add(c2);
            in3.OutgoingConnections.Add(c5);
            bias.OutgoingConnections.Add(c6);
            bias.OutgoingConnections.Add(c7);
            h1.IncomingConnections.Add(c2);
            h1.IncomingConnections.Add(c11);
            h1.OutgoingConnections.Add(c3);
            h2.IncomingConnections.Add(c6);
            h2.IncomingConnections.Add(c12);
            h2.OutgoingConnections.Add(c11);
            h3.IncomingConnections.Add(c3);
            h3.OutgoingConnections.Add(c4);
            h3.OutgoingConnections.Add(c12);
            h4.IncomingConnections.Add(c1);
            h4.IncomingConnections.Add(c4);
            h4.IncomingConnections.Add(c8);
            h4.IncomingConnections.Add(c13);
            h4.OutgoingConnections.Add(c10);
            h4.OutgoingConnections.Add(c13);
            h5.IncomingConnections.Add(c7);
            h5.OutgoingConnections.Add(c8);
            h5.OutgoingConnections.Add(c9);
            out1.IncomingConnections.Add(c5);
            out1.IncomingConnections.Add(c9);
            out1.IncomingConnections.Add(c10);

            neurons.Add(in1);
            neurons.Add(in2);
            neurons.Add(in3);
            neurons.Add(bias);

            neurons.Add(h1);
            neurons.Add(h2);
            neurons.Add(h3);
            neurons.Add(h4);
            neurons.Add(h5);

            neurons.Add(out1);

            return neurons;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(1000);
            await Task.Run(() => TestGeneticAlgorithm());
        }
        
    }
}
