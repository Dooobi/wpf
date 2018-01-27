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
    /// Interaktionslogik für SpeciesTimestampIcon.xaml
    /// </summary>
    public partial class SpeciesTimestampIcon : UserControl
    {
        public SpeciesTimestampIcon()
        {
            InitializeComponent();
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(SpeciesTimestampIcon), new PropertyMetadata(Brushes.Transparent));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(SpeciesTimestampIcon), new PropertyMetadata());

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(SpeciesTimestampIcon), new PropertyMetadata());

        public double TextSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(SpeciesTimestampIcon), new PropertyMetadata());


        public event MouseButtonEventHandler SpeciesTimestampIconMouseDown;
        private void SpeciesTimestampIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SpeciesTimestampIconMouseDown != null) SpeciesTimestampIconMouseDown(this, e);
            e.Handled = true;
        }
    }
}
