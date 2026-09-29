using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3
{
    public abstract class Editor
    {
        public abstract void OnMouseDown(Canvas canvas, MouseButtonEventArgs e);
        public abstract void OnMouseMove(Canvas canvas, MouseEventArgs e);
        public abstract void OnMouseUp(Canvas canvas, MouseButtonEventArgs e);
    }

    public class ShapeEditor : Editor
    {
        private const int Zh = 12;
        private readonly int N = Zh + 100;

        private readonly List<Shape> pcshape;
        private int shapeCount = 0;

        private double x1, y1, x2, y2;
        private bool isDrawing = false;

        private Shape tempShape;
        private UIElement previewElement;

        public ShapeType CurrentShapeType { get; set; } = ShapeType.Line;

        public ShapeEditor()
        {
            pcshape = new List<Shape>(N);
        }

        public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
        {
            if (shapeCount >= N) return;

            var pos = e.GetPosition(canvas);
            x1 = x2 = pos.X;
            y1 = y2 = pos.Y;
            isDrawing = true;
            canvas.CaptureMouse();

            tempShape = CreateShape(CurrentShapeType);
            tempShape.Set(x1, y1, x2, y2);

            previewElement = tempShape.CreateElement();
            if (previewElement is System.Windows.Shapes.Shape wpfShape)
            {
                tempShape.ApplyRubberStyle(wpfShape);
            }
            canvas.Children.Add(previewElement);
        }

        public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
        {
            if (!isDrawing) return;

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
            canvas.ReleaseMouseCapture();

            canvas.Children.Remove(previewElement);
            previewElement = null;

            tempShape.Set(x1, y1, x2, y2);
            tempShape.Draw(canvas);

            pcshape.Add(tempShape);
            tempShape = null;
        }

        private static Shape CreateShape(ShapeType type) => type switch
        {
            ShapeType.Point => new PointShape(),
            ShapeType.Line => new LineShape(),
            ShapeType.Rect => new RectShape(),
            ShapeType.Ellipse => new EllipseShape(),
            _ => new LineShape()
        };
    }
}