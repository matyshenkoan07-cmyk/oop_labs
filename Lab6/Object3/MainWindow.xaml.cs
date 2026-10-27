using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Object3
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        private uint MSG_PLOT_GRAPH;
        private List<Point> _points = new List<Point>();
        private HwndSource _hwndSource;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MSG_PLOT_GRAPH = RegisterWindowMessage("LAB6_PLOT_GRAPH");

            var interopHelper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(interopHelper.Handle);
            _hwndSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MSG_PLOT_GRAPH)
            {
                ReadClipboardAndPlot();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void ReadClipboardAndPlot()
        {
            if (!Clipboard.ContainsText()) return;

            string text = Clipboard.GetText();
            string[] lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            _points.Clear();
            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                {
                    _points.Add(new Point(x, y));
                }
            }

            DrawGraph();
        }

        private void PlotCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
            plotCanvas.Children.Clear();
            if (_points.Count < 2) return;

            double width = plotCanvas.ActualWidth;
            double height = plotCanvas.ActualHeight;
            double margin = 40;

            if (width <= margin * 2 || height <= margin * 2) return;

            Line xAxis = new Line { X1 = margin, Y1 = height - margin, X2 = width - margin, Y2 = height - margin, Stroke = Brushes.Black, StrokeThickness = 2 };
            Line yAxis = new Line { X1 = margin, Y1 = margin, X2 = margin, Y2 = height - margin, Stroke = Brushes.Black, StrokeThickness = 2 };
            plotCanvas.Children.Add(xAxis);
            plotCanvas.Children.Add(yAxis);

            double minX = _points[0].X, maxX = _points[_points.Count - 1].X;
            double minY = double.MaxValue, maxY = double.MinValue;

            foreach (var p in _points)
            {
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            if (Math.Abs(maxY - minY) < 0.0001) maxY = minY + 1.0;

            Func<double, double> scaleX = x => margin + (x - minX) / (maxX - minX == 0 ? 1 : maxX - minX) * (width - 2 * margin);
            Func<double, double> scaleY = y => (height - margin) - (y - minY) / (maxY - minY) * (height - 2 * margin);

            TextBlock labelMinY = new TextBlock { Text = minY.ToString("F1"), FontSize = 10 };
            Canvas.SetLeft(labelMinY, 5); Canvas.SetTop(labelMinY, height - margin - 10);
            plotCanvas.Children.Add(labelMinY);

            TextBlock labelMaxY = new TextBlock { Text = maxY.ToString("F1"), FontSize = 10 };
            Canvas.SetLeft(labelMaxY, 5); Canvas.SetTop(labelMaxY, margin - 10);
            plotCanvas.Children.Add(labelMaxY);

            Polyline line = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };
            PointCollection collection = new PointCollection();

            foreach (var p in _points)
            {
                double cx = scaleX(p.X);
                double cy = scaleY(p.Y);
                collection.Add(new Point(cx, cy));

                Ellipse dot = new Ellipse { Width = 4, Height = 4, Fill = Brushes.Red };
                Canvas.SetLeft(dot, cx - 2);
                Canvas.SetTop(dot, cy - 2);
                plotCanvas.Children.Add(dot);
            }
            line.Points = collection;
            plotCanvas.Children.Add(line);
        }
    }
}