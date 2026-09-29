using System.Windows;
using System.Windows.Input;

namespace Lab3
{
    public partial class MainWindow : Window
    {
        private readonly ShapeEditor shapeEditor = new ShapeEditor();

        public MainWindow()
        {
            InitializeComponent();
            SetShapeType(ShapeType.Line);
        }

        private void SetShapeType(ShapeType type)
        {
            shapeEditor.CurrentShapeType = type;

            pointButton.IsChecked = type == ShapeType.Point;
            lineButton.IsChecked = type == ShapeType.Line;
            rectButton.IsChecked = type == ShapeType.Rect;
            ellipseButton.IsChecked = type == ShapeType.Ellipse;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Point_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Point);
        private void Line_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Line);
        private void Rectangle_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Rect);
        private void Ellipse_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Ellipse);

        private void ObjectsMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var type = shapeEditor.CurrentShapeType;
            pointMenuItem.IsChecked = type == ShapeType.Point;
            lineMenuItem.IsChecked = type == ShapeType.Line;
            rectMenuItem.IsChecked = type == ShapeType.Rect;
            ellipseMenuItem.IsChecked = type == ShapeType.Ellipse;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Лабораторна робота №3\nВиконала: Матюшенко Анастасія\nЖ = 12",
                "Довідка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void drawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => shapeEditor.OnMouseDown(drawingCanvas, e);

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
            => shapeEditor.OnMouseMove(drawingCanvas, e);

        private void drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            => shapeEditor.OnMouseUp(drawingCanvas, e);
    }
}