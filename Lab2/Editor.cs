using System.Windows.Input;
using System.Windows.Controls;

namespace Lab2
{
    public abstract class Editor
    {
        public abstract void OnMouseDown(Canvas canvas, MouseButtonEventArgs e);
        public abstract void OnMouseMove(Canvas canvas, MouseEventArgs e);
        public abstract void OnMouseUp(Canvas canvas, MouseButtonEventArgs e);
    }

    public class ShapeEditor : Editor
    {
        private Shape[] pcshape = new Shape[111];
        private int shapeCount = 0;
        protected double x1, y1, x2, y2;
        protected bool isDrawing = false;
        private System.Windows.UIElement previewElement;
       
        private Shape tempShape; 
        
        public string CurrentShapeType { get; set; } = "Line";

        public override void OnMouseDown(Canvas canvas, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(canvas);
            x1 = pos.X;
            y1 = pos.Y;
            x2 = x1;
            y2 = y1;
            isDrawing = true;

            tempShape = CurrentShapeType switch
            {
                "Point" => new PointShape(),
                "Line" => new LineShape(),
                "Rect" => new RectShape(),
                "Ellipse" => new EllipseShape(),
                _ => new LineShape()
            };

            tempShape.Set(x1, y1, x2, y2);
            
            previewElement = CreateWpfElement(tempShape);
             
            if (previewElement is System.Windows.Shapes.Shape wpfShape)
            {
                wpfShape.StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 };
            }
            
            if (previewElement != null)
            {
                canvas.Children.Add(previewElement);
            }
        }

        public override void OnMouseMove(Canvas canvas, MouseEventArgs e)
        {
            if (!isDrawing || previewElement == null || tempShape == null) return;
            
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

            if (previewElement != null)
            {
                canvas.Children.Remove(previewElement);
                previewElement = null;
            }

            tempShape.Set(x1, y1, x2, y2);
            tempShape.Draw(canvas);
            
            if (shapeCount < 111)
            {
                pcshape[shapeCount] = tempShape;
                shapeCount++;
            }
         
            tempShape = null; 
        }

        private System.Windows.UIElement CreateWpfElement(Shape shape)
        {
            var tempCanvas = new Canvas();
            shape.Draw(tempCanvas);
            if (tempCanvas.Children.Count > 0)
            {
                var el = tempCanvas.Children[0];
                tempCanvas.Children.Clear();
                return el;
            }
            return null;
        }
    }
}
