using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Lab5
{
    public partial class MainWindow : Window
    {
        private readonly ShapeEditor myEditor = ShapeEditor.Instance;
        private MyTableWindow? _tableWindow;
        private UIElement? _highlightElement;

        public MainWindow()
        {
            InitializeComponent();
            SetShapeType(ShapeType.Line);

            myEditor.OnShapesChanged += RefreshTableData;
        }

        private void RefreshTableData()
        {
            if (_tableWindow != null && _tableWindow.IsLoaded)
            {
                var items = myEditor.Shapes.Select(s => new TableItem
                {
                    Name = s.Name,
                    X1 = s.X1,
                    Y1 = s.Y1,
                    X2 = s.X2,
                    Y2 = s.Y2
                });
                _tableWindow.UpdateTableData(items);
            }
        }

        private void TableWindowMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_tableWindow == null || !_tableWindow.IsLoaded)
            {
                _tableWindow = new MyTableWindow();

                _tableWindow.OnShapeSelected += HighlightShapeOnCanvas;
                _tableWindow.OnShapeDeleted += DeleteShapeFromTable;

                _tableWindow.Closed += (s, args) =>
                {
                    _tableWindow = null;
                    tableWindowMenuItem.IsChecked = false;
                    RemoveHighlight();
                };

                _tableWindow.Show();
                tableWindowMenuItem.IsChecked = true;
                RefreshTableData();
            }
            else
            {
                _tableWindow.Close();
            }
        }

        private void HighlightShapeOnCanvas(int index)
        {
            RemoveHighlight();

            if (index >= 0 && index < myEditor.Shapes.Count)
            {
                var shape = myEditor.Shapes[index];
                var (minX, minY, maxX, maxY) = shape.GetBounds();

                double margin = 5;
                double rectX = minX - margin;
                double rectY = minY - margin;
                double width = (maxX - minX) + margin * 2;
                double height = (maxY - minY) + margin * 2;

                if (width < 10) width = 10;
                if (height < 10) height = 10;

                var highlightRect = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                };

                Canvas.SetLeft(highlightRect, rectX);
                Canvas.SetTop(highlightRect, rectY);

                drawingCanvas.Children.Add(highlightRect);
                _highlightElement = highlightRect;
            }
        }

        private void RemoveHighlight()
        {
            if (_highlightElement != null)
            {
                drawingCanvas.Children.Remove(_highlightElement);
                _highlightElement = null;
            }
        }

        private void DeleteShapeFromTable(int index)
        {
            RemoveHighlight();
            myEditor.RemoveAt(index, drawingCanvas);
        }

        private void SaveCsv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "CSV файли (*.csv)|*.csv|Усі файли (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
            {
                myEditor.SaveToCsv(dialog.FileName);
            }
        }

        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "CSV файли (*.csv)|*.csv|Усі файли (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
            {
                RemoveHighlight();
                myEditor.LoadFromCsv(dialog.FileName, drawingCanvas);
            }
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            _tableWindow?.Close();
            Application.Current.Shutdown();
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

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

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
            MessageBox.Show("Лабораторна робота №5\nВиконала: Матюшенко Анастасія\nВаріант Ж = 11 (Singleton Меєрса)",
                "Довідка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void drawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveHighlight();
            myEditor.OnMouseDown(drawingCanvas, e);
        }

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
            => myEditor.OnMouseMove(drawingCanvas, e);

        private void drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            => myEditor.OnMouseUp(drawingCanvas, e);
    }
}