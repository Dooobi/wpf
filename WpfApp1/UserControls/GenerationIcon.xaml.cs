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
    /// Interaktionslogik für GenerationIcon.xaml
    /// </summary>
    public partial class GenerationIcon : UserControl
    {
        public GenerationIcon()
        {
            InitializeComponent();
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(GenerationIcon), new PropertyMetadata(Brushes.Transparent));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(GenerationIcon), new PropertyMetadata());

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(GenerationIcon), new PropertyMetadata());

        public event MouseButtonEventHandler GenerationIconMouseDown;
        private void GenerationIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GenerationIconMouseDown != null) GenerationIconMouseDown(this, e);
            e.Handled = true;
        }
    }
}
