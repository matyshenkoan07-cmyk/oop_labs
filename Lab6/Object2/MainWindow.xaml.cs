using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Object2
{
    public class DataItem
    {
        public int Index { get; set; }
        public double Value { get; set; }
    }

    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private uint MSG_DATA_READY;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MSG_DATA_READY = RegisterWindowMessage("LAB6_DATA_READY");

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 4 &&
                int.TryParse(args[1], out int n) &&
                double.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double min) &&
                double.TryParse(args[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double max))
            {
                GenerateAndCopyToClipboard(n, min, max);

                IntPtr lab6HWnd = FindLab6Window();
                if (lab6HWnd != IntPtr.Zero)
                {
                    SendMessage(lab6HWnd, MSG_DATA_READY, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        public void GenerateAndCopyToClipboard(int n, double min, double max)
        {
            Random rand = new Random();
            List<DataItem> items = new List<DataItem>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                double val = min + (rand.NextDouble() * (max - min));
                items.Add(new DataItem { Index = i, Value = val });
                sb.AppendLine($"{i}\t{val.ToString(CultureInfo.InvariantCulture)}");
            }

            dataGrid.ItemsSource = items;
            Clipboard.SetText(sb.ToString());
        }

        private IntPtr FindLab6Window()
        {
            foreach (var p in System.Diagnostics.Process.GetProcessesByName("Lab6"))
            {
                if (p.MainWindowHandle != IntPtr.Zero) return p.MainWindowHandle;
            }
            return IntPtr.Zero;
        }
    }
}