using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1
{
    [ValueConversion(typeof(double), typeof(Brush))]
    public class FitnessToColorConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fitness = (double)value;
            string fitnessRange = parameter as string;

            if (!string.IsNullOrEmpty(fitnessRange))
            {
                string[] fitnessBounds = fitnessRange.Split(';');
                double minFitness = 0.0, maxFitness = 0.0;
                bool success = Double.TryParse(fitnessBounds[0], out minFitness) && Double.TryParse(fitnessBounds[1], out maxFitness);
                
                if (success && fitness >= minFitness && fitness <= maxFitness)
                {
                    double hue = Utils.Map(fitness, minFitness, maxFitness, 00.0, 70.0);
                    return new SolidColorBrush(Utils.HSBtoRGB(hue, 255.0, 255.0, 255.0));
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
