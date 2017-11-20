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
    /// Interaktionslogik für Graph.xaml
    /// </summary>
    public partial class Graph : UserControl
    {
        public Graph()
        {
            InitializeComponent();
        }

        public Graph(DrawingImage drawingImage) : this()
        {
            myImage.Source = drawingImage;
        }
    }
}
