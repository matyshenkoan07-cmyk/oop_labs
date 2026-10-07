using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab4
{
    public enum ShapeType
    {
        Point,
        Line,
        Rect,
        Ellipse,
        LineWithCircles,
        Cube
    }

    public abstract class Shape
    {
        protected double x1, y1, x2, y2;

        public virtual void Set(double startX, double startY, double endX, double endY)
        {
            x1 = startX;
            y1 = startY;
            x2 = endX;
            y2 = endY;
        }

        public abstract UIElement CreateElement();
        public abstract void Update(UIElement element);

        public virtual void Draw(Canvas canvas)
        {
            canvas.Children.Add(CreateElement());
        }

        public virtual void ApplyRubberStyleToElement(UIElement element)
        {
            if (element is System.Windows.Shapes.Shape shape)
            {
                ApplyRubberStyleToShape(shape);
            }
            else if (element is Canvas group)
            {
                foreach (UIElement child in group.Children)
                {
                    ApplyRubberStyleToElement(child);
                }
            }
        }

        protected virtual void ApplyRubberStyleToShape(System.Windows.Shapes.Shape element)
        {
            element.Stroke = Brushes.Black;
            element.StrokeThickness = 1;
            element.StrokeDashArray = new DoubleCollection { 4, 2 };
            element.Fill = null;
        }
    }

    public class PointShape : Shape
    {
        private const double Size = 4;

        public override UIElement CreateElement()
        {
            var ellipse = new Ellipse { Width = Size, Height = Size, Fill = Brushes.Black };
            Update(ellipse);
            return ellipse;
        }

        public override void Update(UIElement element)
        {
            Canvas.SetLeft(element, x1 - Size / 2);
            Canvas.SetTop(element, y1 - Size / 2);
        }

        protected override void ApplyRubberStyleToShape(System.Windows.Shapes.Shape element)
        {
            element.Stroke = Brushes.Black;
            element.StrokeThickness = 1;
            element.Fill = Brushes.Black;
            element.StrokeDashArray = null;
        }
    }

    public class LineShape : Shape
    {
        public override UIElement CreateElement()
        {
            var line = new Line { Stroke = Brushes.Black, StrokeThickness = 2 };
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
            var rect = new Rectangle { Stroke = Brushes.Black, Fill = Brushes.Yellow, StrokeThickness = 2 };
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
            var ellipse = new Ellipse { Stroke = Brushes.Black, Fill = null, StrokeThickness = 2 };
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

    public class LineWithCirclesShape : Shape
    {
        private const double CircleRadius = 4;

        private readonly LineShape _lineShape = new LineShape();
        private readonly EllipseShape _ellipse1 = new EllipseShape();
        private readonly EllipseShape _ellipse2 = new EllipseShape();

        public override UIElement CreateElement()
        {
            var group = new Canvas();

            group.Children.Add(_lineShape.CreateElement());
            
            var c1 = (Ellipse)_ellipse1.CreateElement();
            c1.Fill = Brushes.White;
            group.Children.Add(c1);

            var c2 = (Ellipse)_ellipse2.CreateElement();
            c2.Fill = Brushes.White;
            group.Children.Add(c2);

            Update(group);
            return group;
        }

        public override void Update(UIElement element)
        {
            if (element is Canvas group && group.Children.Count == 3)
            {
                _lineShape.Set(x1, y1, x2, y2);
                _lineShape.Update(group.Children[0]);

                _ellipse1.Set(x1 - CircleRadius, y1 - CircleRadius, x1 + CircleRadius, y1 + CircleRadius);
                _ellipse1.Update(group.Children[1]);

                _ellipse2.Set(x2 - CircleRadius, y2 - CircleRadius, x2 + CircleRadius, y2 + CircleRadius);
                _ellipse2.Update(group.Children[2]);
            }
        }
    }

    public class CubeShape : Shape
    {
        private readonly RectShape _frontRect = new RectShape();
        private readonly RectShape _backRect = new RectShape();
        private readonly LineShape[] _lines = new LineShape[4] 
        { 
            new LineShape(), new LineShape(), new LineShape(), new LineShape() 
        };

        public override UIElement CreateElement()
        {
            var group = new Canvas();

            var frontElem = (Rectangle)_frontRect.CreateElement();
            frontElem.Fill = null;
            group.Children.Add(frontElem);

            var backElem = (Rectangle)_backRect.CreateElement();
            backElem.Fill = null;
            group.Children.Add(backElem);

            for (int i = 0; i < 4; i++)
            {
                group.Children.Add(_lines[i].CreateElement());
            }

            Update(group);
            return group;
        }

        public override void Update(UIElement element)
        {
            if (element is Canvas group && group.Children.Count == 6)
            {
                double dx = (x2 - x1) * 0.3;
                double dy = (y2 - y1) * 0.3;
                
                _frontRect.Set(x1, y1, x2, y2);
                _frontRect.Update(group.Children[0]);

                _backRect.Set(x1 + dx, y1 - dy, x2 + dx, y2 - dy);
                _backRect.Update(group.Children[1]);

                double[,] coords = new double[4, 4]
                {
                    { x1, y1, x1 + dx, y1 - dy },
                    { x2, y1, x2 + dx, y1 - dy },
                    { x1, y2, x1 + dx, y2 - dy },
                    { x2, y2, x2 + dx, y2 - dy }
                };

                for (int i = 0; i < 4; i++)
                {
                    _lines[i].Set(coords[i, 0], coords[i, 1], coords[i, 2], coords[i, 3]);
                    _lines[i].Update(group.Children[i + 2]);
                }
            }
        }
    }
}