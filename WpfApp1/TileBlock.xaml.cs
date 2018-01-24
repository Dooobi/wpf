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
    /// Interaktionslogik für TileBlock.xaml
    /// </summary>
    public partial class TileBlock : UserControl
    {
        public TileBlock()
        {
            InitializeComponent();
        }

        //Dependency properties for backgrounds
        // Left
        public Brush LeftBlockBackground
        {
            get { return (Brush)GetValue(LeftBlockBackgroundProperty); }
            set { SetValue(LeftBlockBackgroundProperty, value); }
        }
        public static readonly DependencyProperty LeftBlockBackgroundProperty =
            DependencyProperty.Register("LeftBlockBackground", typeof(Brush), typeof(TileBlock), new PropertyMetadata(Brushes.Transparent));
        //... repeat for Top, Right, Bottom and Central

        public event MouseButtonEventHandler LeftBlockMouseDown;
        private void LeftBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LeftBlockMouseDown != null) LeftBlockMouseDown(this, e);
            e.Handled = true;
        }

        // Right
        public Brush RightBlockBackground
        {
            get { return (Brush)GetValue(RightBlockBackgroundProperty); }
            set { SetValue(RightBlockBackgroundProperty, value); }
        }
        public static readonly DependencyProperty RightBlockBackgroundProperty =
            DependencyProperty.Register("RightBlockBackground", typeof(Brush), typeof(TileBlock), new PropertyMetadata(Brushes.Transparent));
        //... repeat for Top, Right, Bottom and Central

        public event MouseButtonEventHandler RightBlockMouseDown;
        private void RightBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RightBlockMouseDown != null) RightBlockMouseDown(this, e);
            e.Handled = true;
        }

        // Bottom
        public Brush BottomBlockBackground
        {
            get { return (Brush)GetValue(BottomBlockBackgroundProperty); }
            set { SetValue(BottomBlockBackgroundProperty, value); }
        }
        public static readonly DependencyProperty BottomBlockBackgroundProperty =
            DependencyProperty.Register("BottomBlockBackground", typeof(Brush), typeof(TileBlock), new PropertyMetadata(Brushes.Transparent));
        //... repeat for Top, Right, Bottom and Central

        public event MouseButtonEventHandler BottomBlockMouseDown;
        private void BottomBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BottomBlockMouseDown != null) BottomBlockMouseDown(this, e);
            e.Handled = true;
        }

        // Top
        public Brush TopBlockBackground
        {
            get { return (Brush)GetValue(TopBlockBackgroundProperty); }
            set { SetValue(TopBlockBackgroundProperty, value); }
        }
        public static readonly DependencyProperty TopBlockBackgroundProperty =
            DependencyProperty.Register("TopBlockBackground", typeof(Brush), typeof(TileBlock), new PropertyMetadata(Brushes.Transparent));
        //... repeat for Top, Right, Bottom and Central

        public event MouseButtonEventHandler TopBlockMouseDown;
        private void TopBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TopBlockMouseDown != null) TopBlockMouseDown(this, e);
            e.Handled = true;
        }

        // Central 
        public Brush CentralBlockBackground
        {
            get { return (Brush)GetValue(CentralBlockBackgroundProperty); }
            set { SetValue(CentralBlockBackgroundProperty, value); }
        }
        public static readonly DependencyProperty CentralBlockBackgroundProperty =
            DependencyProperty.Register("CentralBlockBackground", typeof(Brush), typeof(TileBlock), new PropertyMetadata(Brushes.Transparent));
        //... repeat for Top, Right, Bottom and Central

        public event MouseButtonEventHandler CentralBlockMouseDown;
        private void CentralBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CentralBlockMouseDown != null) CentralBlockMouseDown(this, e);
            e.Handled = true;
        }
        //... repeat for Top, Right, Bottom and Central

        //... repeat for MouseEnter, MouseLeave, MouseMove etc. if necessary
    }
}
