using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Lab2
{
    public abstract class Shape
    {
        protected double x1, y1, x2, y2;

        public void Set(double startX, double startY, double endX, double endY)
        {
            x1 = startX;
            y1 = startY;
            x2 = endX;
            y2 = endY;
        }

        public abstract void Draw(Canvas canvas);
        
        public abstract void Update(System.Windows.UIElement element);
    }

    public class PointShape : Shape
    {
        public override void Draw(Canvas canvas)
        {
            var ellipse = new Ellipse()
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Black
            };
           
            Canvas.SetLeft(ellipse, x1);
            Canvas.SetTop(ellipse, y1);
            
            canvas.Children.Add(ellipse);
        }

        public override void Update(System.Windows.UIElement element)
        {
        }
    }

    public class LineShape : Shape
    {
        public override void Draw(Canvas canvas)
        {
            var line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(line);
        }
            
        public override void Update(System.Windows.UIElement element)
        {
            if (element is Line line)
            {
                line.X2 = x2; 
                line.Y2 = y2;
            }
        }
    }
    
    public class EllipseShape : Shape
    {
        public override void Draw(Canvas canvas)
        {
            var ellipse = new Ellipse()
            {
                Width = Math.Abs(x2 - x1),
                Height = Math.Abs(y2 - y1),
                Stroke = Brushes.Black,
                Fill = Brushes.White,
                StrokeThickness = 2
            };
            
            Canvas.SetLeft(ellipse, Math.Min(x1, x2));
            Canvas.SetTop(ellipse, Math.Min(y1, y2));
            
            canvas.Children.Add(ellipse);
        }

        public override void Update(System.Windows.UIElement element)
        {
            if (element is Ellipse ellipse)
            {
                ellipse.Width = Math.Abs(x2 - x1);
                ellipse.Height = Math.Abs(y2 - y1);
                Canvas.SetLeft(ellipse, Math.Min(x1, x2));
                Canvas.SetTop(ellipse, Math.Min(y1, y2));
            }
        }
    }
    
    public class RectShape : Shape
    {
        public override void Draw(Canvas canvas)
        {
            double width = Math.Abs(x2 - x1) * 2;
            double height = Math.Abs(y2 - y1) * 2;
            
            var rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Black,
                Fill = new SolidColorBrush(Color.FromRgb(192, 192, 192)),
                StrokeThickness = 2
            };
            
            Canvas.SetLeft(rect, x1 - Math.Abs(x2 - x1));
            Canvas.SetTop(rect, y1 - Math.Abs(y2 - y1));
            
            canvas.Children.Add(rect);
        }

        public override void Update(System.Windows.UIElement element)
        {
            if (element is Rectangle rect)
            {
                rect.Width = Math.Abs(x2 - x1) * 2;
                rect.Height = Math.Abs(y2 - y1) * 2;
                Canvas.SetLeft(rect, x1 - Math.Abs(x2 - x1));
                Canvas.SetTop(rect, y1 - Math.Abs(y2 - y1));
            }
        }
    }
}
