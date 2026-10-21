using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab5
{
    public abstract class Editor
    {
        public abstract void OnMouseDown(Canvas canvas, MouseButtonEventArgs e);
        public abstract void OnMouseMove(Canvas canvas, MouseEventArgs e);
        public abstract void OnMouseUp(Canvas canvas, MouseButtonEventArgs e);
    }

    public class ShapeEditor : Editor
    {
        private static readonly Lazy<ShapeEditor> instance = new Lazy<ShapeEditor>(() => new ShapeEditor());
        public static ShapeEditor Instance => instance.Value;

        private const int Zh = 11;
        private readonly int N = Zh + 100;

        private readonly List<Shape> pcshape;
        private double x1, y1, x2, y2;
        private bool isDrawing = false;

        private Shape tempShape;
        private UIElement previewElement;

        public ShapeType CurrentShapeType { get; set; } = ShapeType.Line;
        public IReadOnlyList<Shape> Shapes => pcshape.AsReadOnly();

        public event Action OnShapesChanged;

        private ShapeEditor()
        {
            pcshape = new List<Shape>(N);
        }

        public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
        {
            if (pcshape.Count >= N) return;

            var pos = e.GetPosition(canvas);
            x1 = x2 = pos.X;
            y1 = y2 = pos.Y;
            isDrawing = true;
            canvas.CaptureMouse();

            tempShape = CreateShape(CurrentShapeType);
            tempShape.Set(x1, y1, x2, y2);

            previewElement = tempShape.CreateElement();
            tempShape.ApplyRubberStyleToElement(previewElement);
            canvas.Children.Add(previewElement);
        }

        public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
        {
            if (!isDrawing || tempShape == null) return;

            var pos = e.GetPosition(canvas);
            x2 = pos.X;
            y2 = pos.Y;

            tempShape.Set(x1, y1, x2, y2);
            tempShape.Update(previewElement);
        }

        public override void OnMouseUp(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!isDrawing) return;

            var pos = e.GetPosition(canvas);
            x2 = pos.X;
            y2 = pos.Y;
            isDrawing = false;

            if (canvas.IsMouseCaptured) canvas.ReleaseMouseCapture();

            if (previewElement != null)
            {
                canvas.Children.Remove(previewElement);
                previewElement = null;
            }

            tempShape.Set(x1, y1, x2, y2);
            pcshape.Add(tempShape);
            tempShape = null;

            RedrawAll(canvas);
            OnShapesChanged?.Invoke();
        }

        public void RemoveAt(int index, Canvas canvas)
        {
            if (index >= 0 && index < pcshape.Count)
            {
                pcshape.RemoveAt(index);
                RedrawAll(canvas);
                OnShapesChanged?.Invoke();
            }
        }

        public void RedrawAll(Canvas canvas)
        {
            canvas.Children.Clear();
            foreach (var shape in pcshape)
            {
                shape.Draw(canvas);
            }
        }

        public void SaveToCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var shape in pcshape)
                {
                    writer.WriteLine(shape.ToCsv());
                }
            }
        }

        public void LoadFromCsv(string filePath, Canvas canvas)
        {
            if (!File.Exists(filePath)) return;

            pcshape.Clear();
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 5)
                {
                    string name = parts[0];
                    if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double x1) &&
                        double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double y1) &&
                        double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double x2) &&
                        double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double y2))
                    {
                        Shape shape = CreateShapeByName(name);
                        if (shape != null)
                        {
                            shape.Set(x1, y1, x2, y2);
                            pcshape.Add(shape);
                        }
                    }
                }
            }
            RedrawAll(canvas);
            OnShapesChanged?.Invoke();
        }

        public static Shape CreateShape(ShapeType type) => type switch
        {
            ShapeType.Point => new PointShape(),
            ShapeType.Line => new LineShape(),
            ShapeType.Rect => new RectShape(),
            ShapeType.Ellipse => new EllipseShape(),
            ShapeType.LineWithCircles => new LineWithCirclesShape(),
            ShapeType.Cube => new CubeShape(),
            _ => new LineShape()
        };

        private static Shape CreateShapeByName(string name) => name switch
        {
            "Точка" => new PointShape(),
            "Лінія" => new LineShape(),
            "Прямокутник" => new RectShape(),
            "Еліпс" => new EllipseShape(),
            "Лінія з кружечками" => new LineWithCirclesShape(),
            "Каркас куба" => new CubeShape(),
            _ => null
        };
    }
}