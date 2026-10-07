using System.Windows;
using System.Windows.Input;

namespace Lab4
{
    public partial class MainWindow : Window
    {
        // Для непарного варіанту (Ж=11) використовуємо глобальний/статичний екземпляр MyEditor (ShapeEditor)
        private static readonly ShapeEditor myEditor = new ShapeEditor();

        public MainWindow()
        {
            InitializeComponent();
            SetShapeType(ShapeType.Line);
        }

        private void SetShapeType(ShapeType type)
        {
            myEditor.CurrentShapeType = type;

            pointButton.IsChecked = type == ShapeType.Point;
            lineButton.IsChecked = type == ShapeType.Line;
            rectButton.IsChecked = type == ShapeType.Rect;
            ellipseButton.IsChecked = type == ShapeType.Ellipse;
            lineCirclesButton.IsChecked = type == ShapeType.LineWithCircles;
            cubeButton.IsChecked = type == ShapeType.Cube;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Point_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Point);
        private void Line_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Line);
        private void Rectangle_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Rect);
        private void Ellipse_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Ellipse);
        private void LineWithCircles_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.LineWithCircles);
        private void Cube_Click(object sender, RoutedEventArgs e) => SetShapeType(ShapeType.Cube);

        private void ObjectsMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var type = myEditor.CurrentShapeType;

            pointMenuItem.IsChecked = type == ShapeType.Point;
            lineMenuItem.IsChecked = type == ShapeType.Line;
            rectMenuItem.IsChecked = type == ShapeType.Rect;
            ellipseMenuItem.IsChecked = type == ShapeType.Ellipse;
            lineCirclesMenuItem.IsChecked = type == ShapeType.LineWithCircles;
            cubeMenuItem.IsChecked = type == ShapeType.Cube;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Лабораторна робота №4\nВиконала: Матюшенко Анастасія\nВаріант Ж = 11 (Статичний MyEditor)",
                "Довідка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void drawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => myEditor.OnMouseDown(drawingCanvas, e);

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
            => myEditor.OnMouseMove(drawingCanvas, e);

        private void drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            => myEditor.OnMouseUp(drawingCanvas, e);
    }
}