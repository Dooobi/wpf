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
    /// Interaktionslogik für RoundButton.xaml
    /// </summary>
    public partial class RoundButton : UserControl
    {
        public RoundButton()
        {
            FontSize = 0.1;
            InitializeComponent();
        }

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(RoundButton), new PropertyMetadata(Brushes.Transparent));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(RoundButton), new PropertyMetadata());

        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(RoundButton), new PropertyMetadata());

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(RoundButton), new PropertyMetadata());


        public event MouseButtonEventHandler RoundButtonMouseDown;
        private void RoundButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RoundButtonMouseDown != null) RoundButtonMouseDown(this, e);
            e.Handled = true;
        }
    }
}
