using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1
{
    public static class Utils
    {
        private static Random random = new Random();

        public static double RandDouble(double inclusiveLowerBound, double exclusiveUpperBound)
        {
            return random.NextDouble() * (exclusiveUpperBound - inclusiveLowerBound) + inclusiveLowerBound;
        }

        public static JObject FileToJObject(string filepath)
        {
            return JObject.Parse(ReadFromFile(filepath));
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

        public static double Map(double value, double istart, double istop, double ostart, double ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }

        public static Color HSBtoRGB(double hue, double saturation, double brightness, double maxValue)
        {
            int r = 0, g = 0, b = 0;
            hue /= maxValue;
            saturation /= maxValue;
            brightness /= maxValue;

            if (saturation == 0)
            {
                r = g = b = (int)(brightness * 255.0f + 0.5f);
            }
            else
            {
                double h = (hue - (double)Math.Floor(hue)) * 6.0f;
                double f = h - (double)Math.Floor(h);
                double p = brightness * (1.0f - saturation);
                double q = brightness * (1.0f - saturation * f);
                double t = brightness * (1.0f - (saturation * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(t * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int)(q * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(brightness * 255.0f + 0.5f);
                        b = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int)(p * 255.0f + 0.5f);
                        g = (int)(q * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int)(t * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        r = (int)(brightness * 255.0f + 0.5f);
                        g = (int)(p * 255.0f + 0.5f);
                        b = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
            return Color.FromArgb(Convert.ToByte(255), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }
    }
}
