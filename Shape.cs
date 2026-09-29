using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab3
{
    public enum ShapeType
    {
        Point,
        Line,
        Rect,
        Ellipse
    }

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

        public abstract UIElement CreateElement();

        public abstract void Update(UIElement element);

        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(CreateElement());
        }

        public virtual void ApplyRubberStyle(System.Windows.Shapes.Shape element)
        {
            element.Stroke = Brushes.Black;
            element.StrokeThickness = 1;
            element.StrokeDashArray = null;
            element.Fill = null;
        }
    }

    public class PointShape : Shape
    {
        private const double Size = 4;

        public override UIElement CreateElement()
        {
            var ellipse = new Ellipse
            {
                Width = Size,
                Height = Size,
                Fill = Brushes.Black
            };
            Update(ellipse);
            return ellipse;
        }

        public override void Update(UIElement element)
        {
            Canvas.SetLeft(element, x1 - Size / 2);
            Canvas.SetTop(element, y1 - Size / 2);
        }

        public override void ApplyRubberStyle(System.Windows.Shapes.Shape element)
        {
            element.Stroke = Brushes.Black;
            element.StrokeThickness = 1;
            element.Fill = Brushes.Black;
        }
    }

    public class LineShape : Shape
    {
        public override UIElement CreateElement()
        {
            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Update(line);
            return line;
        }

        public override void Update(UIElement element)
        {
            if (element is Line line)
            {
                line.X1 = x1;
                line.Y1 = y1;
                line.X2 = x2;
                line.Y2 = y2;
            }
        }
    }

    public class RectShape : Shape
    {
        public override UIElement CreateElement()
        {
            var rect = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Yellow,
                StrokeThickness = 2
            };
            Update(rect);
            return rect;
        }

        public override void Update(UIElement element)
        {
            if (element is Rectangle rect)
            {
                rect.Width = Math.Abs(x2 - x1);
                rect.Height = Math.Abs(y2 - y1);
                Canvas.SetLeft(rect, Math.Min(x1, x2));
                Canvas.SetTop(rect, Math.Min(y1, y2));
            }
        }
    }

    public class EllipseShape : Shape
    {
        public override UIElement CreateElement()
        {
            var ellipse = new Ellipse
            {
                Stroke = Brushes.Black,
                Fill = null,
                StrokeThickness = 2
            };
            Update(ellipse);
            return ellipse;
        }

        public override void Update(UIElement element)
        {
            if (element is Ellipse ellipse)
            {
                double dx = Math.Abs(x2 - x1);
                double dy = Math.Abs(y2 - y1);
                ellipse.Width = dx * 2;
                ellipse.Height = dy * 2;
                Canvas.SetLeft(ellipse, x1 - dx);
                Canvas.SetTop(ellipse, y1 - dy);
            }
        }
    }
}