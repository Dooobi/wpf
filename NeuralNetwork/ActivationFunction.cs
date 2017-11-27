using System;

namespace NeuralNetwork
{
    public delegate double DelegateActivationFunction(double sum);

    public class ActivationFunction
    {
        public static double Passthrough(double input)
        {
            return input;
        }

        public static double Sigmoid(double input)
        {
            return 2 / (1 + Math.Exp(-2 * input)) - 1;
        }

        public static double SigmoidDerivative(double input)
        {
            double s = Sigmoid(input);
            return 1 - (Math.Pow(s, 2));
        }

        public static double Tanh(double input)
        {
            return Math.Tanh(input);
        }

        public static double SigmoidWithActivationResponseFactor(double input, double activationResponseFactor)
        {
            return (1 / (1 + Math.Exp(-input / activationResponseFactor)));
        }
    }
}
