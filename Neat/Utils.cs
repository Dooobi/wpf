using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat
{
    public static class Utils
    {
        private static Random random = new Random();

        public static double RandDouble(double inclusiveLowerBound, double exclusiveUpperBound)
        {
            return random.NextDouble() * (exclusiveUpperBound - inclusiveLowerBound) + inclusiveLowerBound;
        }

        public static void WriteToFile(string filepath, string text, bool endWithNewLine, bool startWithNewLine)
        {
            if (endWithNewLine)
            {
                text += Environment.NewLine;
            }
            if (startWithNewLine)
            {
                text = Environment.NewLine + text;
            }
            File.WriteAllText(filepath, text);
        }

        public static string ReadFromFile(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        public static void AppendToFile(string filepath, string text, bool endWithNewLine, bool startWithNewLine)
        {
            string fullText = ReadFromFile(filepath) + text;
            WriteToFile(filepath, fullText, endWithNewLine, startWithNewLine);
        }
    }
}
