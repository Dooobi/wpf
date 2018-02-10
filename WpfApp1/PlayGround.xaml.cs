using System;
using NeuralNetwork;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für PlayGround.xaml
    /// </summary>
    public partial class PlayGround : Window
    {
        private List<List<double>> inputPatterns;
        private List<double> answers;

        public PlayGround()
        {
            InitializeComponent();
            Initialize();

            value1.Text = "" + slider1.Value;
            value2.Text = "" + slider2.Value;
            value3.Text = "" + slider3.Value;
            value4.Text = "" + slider4.Value;
            value5.Text = "" + slider5.Value;
            value6.Text = "" + slider6.Value;
            value7.Text = "" + slider7.Value;

            TestXorNetwork();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            value1.Text = "" + slider1.Value;
            value2.Text = "" + slider2.Value;
            value3.Text = "" + slider3.Value;
            value4.Text = "" + slider4.Value;
            value5.Text = "" + slider5.Value;
            value6.Text = "" + slider6.Value;
            value7.Text = "" + slider7.Value;

            TestXorNetwork();
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

        private void TestXorNetwork()
        {
            Network network = CreateXorNetwork();

            for (int i = 0; i < inputPatterns.Count; i++)
            {
                List<double> inputPattern = inputPatterns[i];
                List<double> output = network.Update(inputPattern);
                double answer = answers[i];

                TextBox outputTextBox = null;
                Ellipse ellipse = null;
                switch (i)
                {
                    case 0:
                        outputTextBox = out1;
                        ellipse = ellipse1;
                        break;
                    case 1:
                        outputTextBox = out2;
                        ellipse = ellipse2;
                        break;
                    case 2:
                        outputTextBox = out3;
                        ellipse = ellipse3;
                        break;
                    case 3:
                        outputTextBox = out4;
                        ellipse = ellipse4;
                        break;
                }

                if (outputTextBox != null) outputTextBox.Text = "" + output[0];
                if (ellipse != null) ellipse.Fill = Math.Abs(answer - output[0]) < 0.5 ? Brushes.Green : Brushes.Red;

                string isOk = Math.Abs(answer - output[0]) < 0.5 ? "good" : "bad";
                Console.WriteLine("[{0},{1}]: {2} ({3}) [{4}]", inputPattern[0], inputPattern[1], output[0], answer, isOk);
            }

            double fitness = EvaluateFitness(network);
            textBoxFitness.Text = "" + fitness;
            Console.WriteLine("Fitness: {0}", fitness);
        }

        private Network CreateXorNetwork()
        {
            List<Neuron> neurons = new List<Neuron>();

            Neuron in1 = new Neuron("in1", NeuronType.Input);
            Neuron in2 = new Neuron("in2", NeuronType.Input);
            Neuron h1 = new Neuron("h1", NeuronType.Hidden);
            Neuron out1 = new Neuron("out", NeuronType.Output);
            Neuron bias = new Neuron("bias", NeuronType.Bias);

            Connection in1h1 = new Connection("in1h1");
            in1h1.NeuronFrom = in1;
            in1h1.NeuronTo = h1;
            Connection in2h1 = new Connection("in2h1");
            in2h1.NeuronFrom = in2;
            in2h1.NeuronTo = h1;
            Connection biash1 = new Connection("biash1");
            biash1.NeuronFrom = bias;
            biash1.NeuronTo = h1;
            Connection in1out1 = new Connection("in1out1");
            in1out1.NeuronFrom = in1;
            in1out1.NeuronTo = out1;
            Connection in2out1 = new Connection("in2out1");
            in2out1.NeuronFrom = in2;
            in2out1.NeuronTo = out1;
            Connection biasout1 = new Connection("biasout1");
            biasout1.NeuronFrom = bias;
            biasout1.NeuronTo = out1;
            Connection h1out1 = new Connection("h1out1");
            h1out1.NeuronFrom = h1;
            h1out1.NeuronTo = out1;

            in1h1.Weight = slider1.Value;
            in2h1.Weight = slider2.Value;
            biash1.Weight = slider3.Value;
            in1out1.Weight = slider4.Value;
            in2out1.Weight = slider5.Value;
            biasout1.Weight = slider6.Value;
            h1out1.Weight = slider7.Value;

            in1.OutgoingConnections.Add(in1h1);
            in1.OutgoingConnections.Add(in1out1);
            in2.OutgoingConnections.Add(in2h1);
            in2.OutgoingConnections.Add(in2out1);
            bias.OutgoingConnections.Add(biash1);
            bias.OutgoingConnections.Add(biasout1);
            h1.OutgoingConnections.Add(h1out1);
            h1.IncomingConnections.Add(in1h1);
            h1.IncomingConnections.Add(in2h1);
            h1.IncomingConnections.Add(biash1);
            out1.IncomingConnections.Add(in1out1);
            out1.IncomingConnections.Add(in2out1);
            out1.IncomingConnections.Add(biasout1);
            out1.IncomingConnections.Add(h1out1);

            neurons.Add(in1);
            neurons.Add(in2);
            neurons.Add(bias);
            neurons.Add(h1);
            neurons.Add(out1);

            return new Network(neurons, ActivationFunction.SigmoidDerivative);
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
