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
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            World myWorld = new World();
            
            myPanel.Children.Add(new Graph(GetGraph()));
            myPanel.Children.Add(new Graph(GetGraph()));

            Grid.SetRow(myPanel.Children[0], 1);
            Grid.SetRow(myPanel.Children[0], 1);
        }

        private DrawingImage GetGraph()
        {
            PathFigure pfSpecies = new PathFigure();
            pfSpecies.StartPoint = new Point(0, 0);
            pfSpecies.IsClosed = true;
            pfSpecies.Segments.Add(new LineSegment(new Point(100, 0), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(100, 20), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(80, 20), true));
            pfSpecies.Segments.Add(new LineSegment(new Point(80, 50), true));

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(pfSpecies);

            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Brush = Brushes.Orange;
            geometryDrawing.Pen = new Pen(Brushes.Black, 3);
            geometryDrawing.Geometry = geometry;

            return new DrawingImage(geometryDrawing);
        }
    }
}
