using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5
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
        public double X1 { get; protected set; }
        public double Y1 { get; protected set; }
        public double X2 { get; protected set; }
        public double Y2 { get; protected set; }

        public abstract string Name { get; }

        public virtual (double minX, double minY, double maxX, double maxY) GetBounds()
        {
            return (Math.Min(X1, X2), Math.Min(Y1, Y2), Math.Max(X1, X2), Math.Max(Y1, Y2));
        }

        public virtual void Set(double startX, double startY, double endX, double endY)
        {
            X1 = startX;
            Y1 = startY;
            X2 = endX;
            Y2 = endY;
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
        
        public virtual string ToCsv()
        {
            return $"{Name};{X1.ToString(CultureInfo.InvariantCulture)};{Y1.ToString(CultureInfo.InvariantCulture)};{X2.ToString(CultureInfo.InvariantCulture)};{Y2.ToString(CultureInfo.InvariantCulture)}";
        }
    }

    public class PointShape : Shape
    {
        private const double Size = 4;
        public override string Name => "Точка";

        public override UIElement CreateElement()
        {
            var ellipse = new Ellipse { Width = Size, Height = Size, Fill = Brushes.Black };
            Update(ellipse);
            return ellipse;
        }

        public override void Update(UIElement element)
        {
            Canvas.SetLeft(element, X1 - Size / 2);
            Canvas.SetTop(element, Y1 - Size / 2);
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
        public override string Name => "Лінія";

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
                line.X1 = X1;
                line.Y1 = Y1;
                line.X2 = X2;
                line.Y2 = Y2;
            }
        }
    }

    public class RectShape : Shape
    {
        public override string Name => "Прямокутник";

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
                rect.Width = Math.Abs(X2 - X1);
                rect.Height = Math.Abs(Y2 - Y1);
                Canvas.SetLeft(rect, Math.Min(X1, X2));
                Canvas.SetTop(rect, Math.Min(Y1, Y2));
            }
        }
    }

    public class EllipseShape : Shape
    {
        public override string Name => "Еліпс";

        public override (double minX, double minY, double maxX, double maxY) GetBounds()
        {
            double dx = Math.Abs(X2 - X1);
            double dy = Math.Abs(Y2 - Y1);
            return (X1 - dx, Y1 - dy, X1 + dx, Y1 + dy);
        }

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
                double dx = Math.Abs(X2 - X1);
                double dy = Math.Abs(Y2 - Y1);
                ellipse.Width = dx * 2;
                ellipse.Height = dy * 2;
                Canvas.SetLeft(ellipse, X1 - dx);
                Canvas.SetTop(ellipse, Y1 - dy);
            }
        }
    }

    public class LineWithCirclesShape : Shape
    {
        private const double CircleRadius = 4;
        private readonly LineShape _lineShape = new LineShape();
        private readonly EllipseShape _ellipse1 = new EllipseShape();
        private readonly EllipseShape _ellipse2 = new EllipseShape();

        public override string Name => "Лінія з кружечками";

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
                _lineShape.Set(X1, Y1, X2, Y2);
                _lineShape.Update(group.Children[0]);

                _ellipse1.Set(X1 - CircleRadius, Y1 - CircleRadius, X1 + CircleRadius, Y1 + CircleRadius);
                _ellipse1.Update(group.Children[1]);

                _ellipse2.Set(X2 - CircleRadius, Y2 - CircleRadius, X2 + CircleRadius, Y2 + CircleRadius);
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

        public override string Name => "Каркас куба";

        public override (double minX, double minY, double maxX, double maxY) GetBounds()
        {
            double minX = Math.Min(X1, X2);
            double maxX = Math.Max(X1, X2);
            double minY = Math.Min(Y1, Y2);
            double maxY = Math.Max(Y1, Y2);

            double offset = Math.Min(Math.Abs(X2 - X1), Math.Abs(Y2 - Y1)) * 0.3;
            if (offset < 5) offset = 10;

            return (minX, minY - offset, maxX + offset, maxY);
        }

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
                double minX = Math.Min(X1, X2);
                double maxX = Math.Max(X1, X2);
                double minY = Math.Min(Y1, Y2);
                double maxY = Math.Max(Y1, Y2);

                double offset = Math.Min(Math.Abs(X2 - X1), Math.Abs(Y2 - Y1)) * 0.3;
                if (offset < 5) offset = 10;

                _frontRect.Set(minX, minY, maxX, maxY);
                _frontRect.Update(group.Children[0]);

                _backRect.Set(minX + offset, minY - offset, maxX + offset, maxY - offset);
                _backRect.Update(group.Children[1]);

                double[,] coords = new double[4, 4]
                {
                    { minX, minY, minX + offset, minY - offset },
                    { maxX, minY, maxX + offset, minY - offset },
                    { minX, maxY, minX + offset, maxY - offset },
                    { maxX, maxY, maxX + offset, maxY - offset }
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