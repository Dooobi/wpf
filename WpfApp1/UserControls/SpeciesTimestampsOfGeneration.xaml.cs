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
    /// Interaktionslogik für SpeciesTimestampsOfGeneration.xaml
    /// </summary>
    public partial class SpeciesTimestampsOfGeneration : UserControl
    {
        public SpeciesTimestampsOfGeneration()
        {
            InitializeComponent();
        }

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(SpeciesTimestampsOfGeneration), new PropertyMetadata());
    }
}
