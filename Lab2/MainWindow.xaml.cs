using System.Windows;
using System.Windows.Input;

namespace Lab2
{
    public partial class MainWindow : Window
    {
        private ShapeEditor shapeEditor = new ShapeEditor();

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void Point_Click(object sender, RoutedEventArgs e)
        {
            shapeEditor.CurrentShapeType = "Point";
            Title = "Лабораторна робота №2 - Режим: Крапка";
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            shapeEditor.CurrentShapeType = "Line";
            Title = "Лабораторна робота №2 - Режим: Лінія";
        }

        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            shapeEditor.CurrentShapeType = "Ellipse";
            Title = "Лабораторна робота №2 - Режим: Еліпс";
        }
        
        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            shapeEditor.CurrentShapeType = "Rect";
            Title = "Лабораторна робота №2 - Режим: Прямокутник";
        }
        
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Лабораторна робота №2\nВиконала: Матюшенко Анастасія\nВаріант 11", 
                "Довідка", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }

        private void drawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            shapeEditor.OnMouseDown(drawingCanvas, e);
        }

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            shapeEditor.OnMouseMove(drawingCanvas, e);
        }

        private void drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            shapeEditor.OnMouseUp(drawingCanvas, e);
        }
    }
}
