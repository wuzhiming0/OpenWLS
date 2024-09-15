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

using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Drawing;
using System.IO;

using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Client.GView.Models;

namespace OpenWLS.Client.GView.GUI
{
    public class HLine
    {
        public double YPos
        {
            get
            {
                return Line.Y1;
            }
            set
            {
                Line.Y1 = value;
                Line.Y2 = value;
            }
        }

        bool selected;
        public bool Selected
        {
            get
            {
                return selected;

            }
            set
            {
                selected = value;
                Line.Stroke = selected ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Wheat;
            }
        }


        public Line Line { get; set; }

        public HLine()
        {
            Line = new Line();
            Line.StrokeThickness = 5;
            Line.X1 = 0;
            Line.X2 = 40;
            Selected = false;
        }
        public void HitTest(double y)
        {
            Selected = YPos + 4 >= y && YPos - 4 <= y;
        }
    }
    public class VLine
    {
        public double XPos
        {
            get
            {
                return Line.X1;
            }
            set
            {
                Line.X1 = value;
                Line.X2 = value;
            }
        }

        bool selected;
        public bool Selected
        {
            get
            {
                return selected;

            }
            set
            {
                selected = value;
                Line.Stroke = selected ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Wheat;
            }
        }

        public Line Line { get; set; }

        public VLine()
        {
            Line = new Line();
            Line.StrokeThickness = 5;
            Line.Y1 = 0;
            Line.Y2 = 40;
            Selected = false;
        }
        public void HitTest(double x)
        {
            Selected = XPos + 4 >= x && XPos - 4 <= x;
        }
    }

    public class SBRectangle
    {
        public System.Windows.Shapes.Rectangle Rect { get; set; }

        bool selected;
        public bool Selected
        {
            get
            {
                return selected;

            }
            set
            {
                selected = value;
                Rect.Opacity = selected ? 0.7 : 0.5;
            }
        }
        SolidColorBrush fill;
        public SolidColorBrush Fill
        {
            get { return fill; }
            set { fill = value;
                Rect.Fill = fill;
            }
        }

        public SBRectangle()
        {
            Rect = new System.Windows.Shapes.Rectangle(); 
            Selected = false;
        }

    }
    /// <summary>
    /// Interaction logic for GViewScrollbar.xaml
    /// </summary>
    public partial class GViewScrollbar : System.Windows.Controls.UserControl
    {
        public event System.Windows.Controls.Primitives.ScrollEventHandler Scroll;
        public event EventHandler ZoneChangedEvent;

        bool Vertical
        {
            get { return orientation == System.Windows.Controls.Orientation.Vertical; }
        }

        HLine h1, h2;
        VLine v1, v2;
        SBRectangle rect;

        double rect_x, rect_y;
        double maximum;
        double scale;

        System.Windows.Controls.Orientation orientation;
        public System.Windows.Controls.Orientation Orientation { get { return orientation; }
            set { orientation = value;
                if(Vertical)
                {
                    canvas.Children.Clear();
                    h1 = new HLine();
                    h2 = new HLine();

                    canvas.Children.Add(h1.Line);
                    canvas.Children.Add(h2.Line);

                    rect = new SBRectangle();
                    rect.Rect.Opacity = 0.5;
                    rect.Rect.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0xff));
                    //      rect.Rect.OpacityMask = new System.Windows.Media.SolidColorBrush( System.Windows.Media.Color.FromRgb(0,0,0));
                    rect.Rect.Height = ActualHeight;
                    rect.Rect.Width = 40;
                    Width = 40;
                    Height = double.NaN;
                    canvas.Children.Add(rect.Rect);
                }
                else
                {
                    canvas.Children.Clear();
                    v1 = new VLine();
                    v2 = new VLine();

                    canvas.Children.Add(v1.Line);
                    canvas.Children.Add(v2.Line);

                    rect = new SBRectangle();
                    rect.Rect.Opacity = 0.5;
                    rect.Rect.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0xff));
                    //      rect.Rect.OpacityMask = new System.Windows.Media.SolidColorBrush( System.Windows.Media.Color.FromRgb(0,0,0));
                    rect.Rect.Height = 40;
                    Height = 40;
                    Width = double.NaN;
                    rect.Rect.Width = ActualWidth;
                    canvas.Children.Add(rect.Rect);
                }
            }
        }
        public GvView PlotView { get; set; }  
        public double Maximum
        {
            get { return maximum; }
            set
            {
   //             if (maximum == value)
   //                 return;
                maximum = value; 
                if(orientation == System.Windows.Controls.Orientation.Vertical)
                    Visibility = maximum <= ActualHeight? Visibility.Collapsed : Visibility.Visible;
                else
                    Visibility = maximum <= ActualWidth? Visibility.Collapsed : Visibility.Visible;
                if( Visibility == Visibility.Visible )
                {
                    if (indexAutoScroll)
                        SetIndexAutoPos();
                    UpdateBar();
                }

            }
        }
        public double Value
        {
            get
            {
                if (Vertical)
                {
                    scale = ActualHeight / maximum;
                    return h1.YPos / scale;
                }
                else
                {
                    scale = ActualWidth / maximum;
                    return v1.XPos / scale;
                }
            }
        }
        bool indexAutoScroll;
        public bool IndexAutoScroll
        {
            get { return indexAutoScroll; }
            set { indexAutoScroll = value; }
        }
        public GViewScrollbar()
        {         
            maximum = 1; 
            indexAutoScroll = false;
            InitializeComponent();

            //        PlotView view = new PlotView();
            //      host.Child = view;
        }
        public void UpdateBar()
        {
            if (double.IsNaN(maximum) || maximum == 0)
                return;
            UpdateLayout();
            //      Dispatcher.Invoke(() =>
            //      {
            if (Vertical)
                    UpdateYBar();
                else
                    UpdateXBar();
     //       });

        }
   //     delegate void UpdateColorMaskDelegate(System.Windows.Media.Color  c);
        public void UpdateColorMask(System.Windows.Media.Color c)
        {
            Dispatcher.Invoke(() =>
            {
                rect.Fill = new SolidColorBrush(c);
            });
        }

        public void SetIndexAutoPos()
        {
            if (Vertical)
            {
                h1.YPos = (maximum - ActualHeight) / maximum * ActualHeight;
                h2.YPos = ActualHeight-1; 
            }
            else
            {
                v1.XPos = (maximum - ActualWidth) / maximum * ActualWidth;
                v2.XPos = ActualWidth - 1;
            }         
        }
               
        public void UpdateYBar()
        {
            scale = ActualHeight / maximum ;
            double h = ActualHeight * scale;
            if (h < 10)
                h = 10;
            if (h1.YPos + 10 > ActualHeight)
                h1.YPos = ActualHeight - 10;
            scale = ActualHeight / maximum;
            double w = ActualWidth;

            double d2 = h1.YPos + h;
            canvas.Width = w;
            rect.Rect.Height = h;     
            
            h1.Line.X2 = w;
            h2.Line.Y1 = d2;
            h2.Line.Y2 = d2;
            h2.Line.X2 = w;
            rect.Rect.Width = w;

            Canvas.SetTop(rect.Rect, h1.YPos);
            canvas.SetValue(Canvas.TopProperty, 0.0);
            ReDrawCurve();
        }


        public void UpdateXBar()
        {
            scale = ActualWidth / maximum;
            double w = ActualWidth * scale;
            if (w < 10)
                w = 10;
            if (v1.XPos + 10 > ActualWidth)
                v1.XPos = ActualWidth - 10;
        
            double h = ActualHeight;

            canvas.Height = h;
            rect.Rect.Height = h;
            double d2 = v1.XPos + w;
            v1.Line.Y2 = h;
            v2.Line.X1 = d2;
            v2.Line.X2 = d2;
            v2.Line.Y2 = h;
            rect.Rect.Width = w;

            Canvas.SetLeft(rect.Rect, v1.XPos);
            canvas.SetValue(Canvas.TopProperty, 0.0);
            ReDrawCurve();
        }

 
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(this);
            if(Vertical)
            {
                double y = pos.Y;
                double y1 = Canvas.GetTop(rect.Rect);
                double y2 = y1 + rect.Rect.Height;
                if (y < y2 && y > y1)
                {
                    rect.Selected = true;
                    rect_y = pos.Y;
                    return;
                }

                h1.HitTest(pos.Y);
                if (h1.Selected)
                    return;
                h2.HitTest(pos.Y);
                if (h2.Selected)
                    return;
            }
            else
            {
                double x = pos.X;
                double x1 = Canvas.GetLeft(rect.Rect);

                double x2 = x1 + rect.Rect.Width;
                if (x < x2 && x > x1)
                {
                    rect.Selected = true;
                    rect_x = pos.X;
                    return;
                }

                v1.HitTest(pos.X);
                if (v1.Selected)
                    return;
                v2.HitTest(pos.X);
                if (v2.Selected)
                    return;
            }

        }

        void EndScroll()
        {
            if (Vertical)
            {
                h1.Selected = false;
                h2.Selected = false;
            }
            else
            {
                v1.Selected = false;
                v2.Selected = false;
            }
            rect.Selected = false;

            if (ZoneChangedEvent != null)
                ZoneChangedEvent(this, new EventArgs());
        }



        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            EndScroll();
        }


        void SetH1Pos(double y)
        {
            if (y > 0 && y < h2.YPos - 10)
            {
                rect.Rect.Height += h1.YPos - y;
                h1.YPos = y;
                Canvas.SetTop(rect.Rect, y);
            }
        }
        void SetH2Pos(double y)
        {
            if (y < ActualHeight && y > h1.YPos + 10)
            {
                rect.Rect.Height += y - h2.YPos;
                h2.YPos = y;
            }
        }

        void SetV1Pos(double x)
        {
            if (x > 0 && x < v2.XPos - 10)
            {
                rect.Rect.Width += v1.XPos - x;
                v1.XPos = x;
                Canvas.SetLeft(rect.Rect, x);
            }
        }

        void SetV2Pos(double x)
        {
            if (x < ActualWidth && x > v1.XPos + 10)
            {
                rect.Rect.Width += x - v2.XPos;
                v2.XPos = x;
            }
        }

        void SetRectPos(double y)
        {

            if (orientation == System.Windows.Controls.Orientation.Vertical)
            {
                double d = y - rect_y;
                double d1 = h1.YPos + d;
                double d2 = h2.YPos + d;
                if (d1 < 0 || d2 > canvas.ActualHeight)
                    return;
                rect_y = y;
                h1.YPos = d1;
                h2.YPos = d2;
                Canvas.SetTop(rect.Rect, h1.YPos);
            }else
            {
                double d = y - rect_x;
                double d1 = v1.XPos + d;
                double d2 = v2.XPos + d;
                if (d1 < 0 || d2 > canvas.ActualWidth)
                    return;
                rect_x = y;
                v1.XPos = d1;
                v2.XPos = d2;
                Canvas.SetLeft(rect.Rect, v1.XPos);
            }
        }
        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                System.Windows.Point pos = e.GetPosition(this);
                if(Vertical)
                {
                    if (h1.Selected)
                        SetH1Pos(pos.Y);
                    if (h2.Selected)
                        SetH2Pos(pos.Y);
                    if (rect.Selected)
                    {
                        SetRectPos(pos.Y);
                        if(Scroll != null)
                            Scroll(this, new System.Windows.Controls.Primitives.ScrollEventArgs(System.Windows.Controls.Primitives.ScrollEventType.Last, h1.YPos / scale ));
                    }
                }
                else
                {
                    if (v1.Selected)
                        SetV1Pos(pos.X);
                    if (v2.Selected)
                        SetV2Pos(pos.X);
                    if (rect.Selected)
                    {
                        SetRectPos(pos.X);
                        if(Scroll != null)
                            Scroll(this, new System.Windows.Controls.Primitives.ScrollEventArgs(System.Windows.Controls.Primitives.ScrollEventType.Last, v1.XPos / scale ));
                    }
                }
            }
            catch (Exception e1)
            {

            }
        }
        delegate void ReDrawCurveDelegate();
        public void ReDrawCurve()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new ReDrawCurveDelegate(ReDrawCurve), null);
            else
            {
                int w = (int)canvas.ActualWidth;
                int h = (int)canvas.ActualHeight;
                if (w == 0 || h == 0)
                    return;

                System.Drawing.Image imageBuf = Vertical ? new Bitmap(w, h) : new Bitmap(h, w);
                Graphics gBuf = Graphics.FromImage(imageBuf);
                gBuf.Clear(System.Drawing.Color.White);
                DrawXBarCurePoints(gBuf);

                using (MemoryStream ms = new MemoryStream())
                {
                    if (orientation == System.Windows.Controls.Orientation.Horizontal)
                        imageBuf.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    imageBuf.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    System.Windows.Controls.Image convertedImage = new System.Windows.Controls.Image { Source = decoder.Frames[0] };
                    if (canvas.Children.Count > 3)
                        canvas.Children.RemoveAt(0);
                    //                convertedImage.Height = h; 
                    canvas.Children.Insert(0, convertedImage);
                }

            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           if (PlotView != null)
                ReDrawCurve();
        }

       

        private void Canvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {


        }

        private void canvas_LostFocus(object sender, RoutedEventArgs e)
        {
            EndScroll();
        }

        void DrawXBarCurePoints(Graphics g)
        {
            try
            {
                float right = Vertical ? (float)ActualWidth : (float)ActualHeight;
                float sh =  Vertical ? (float)(ActualHeight / maximum) : (float)(ActualWidth / maximum);
                float x = 0;
                foreach (IGvItemC item in PlotView.Items)
                    x = item.DrawScrollbar(g, x, right, sh);
            }
            catch (Exception e)
            {

            }
        }
    }
}
