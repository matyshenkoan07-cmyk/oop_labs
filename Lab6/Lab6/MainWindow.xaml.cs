using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Lab6
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private uint MSG_DATA_READY;
        private uint MSG_PLOT_GRAPH;

        private Process _procObject2;
        private Process _procObject3;
        private HwndSource _hwndSource;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MSG_DATA_READY = RegisterWindowMessage("LAB6_DATA_READY");
            MSG_PLOT_GRAPH = RegisterWindowMessage("LAB6_PLOT_GRAPH");

            var interopHelper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(interopHelper.Handle);
            _hwndSource.AddHook(WndProc);

            // Початкова позиція Lab6
            Left = 50;
            Top = 100;
            Width = 350;
            Height = 250;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == MSG_DATA_READY)
            {
                // Отримали сигнал від Object2 -> передаємо команду Object3 побудувати графік
                IntPtr hObj3 = GetHWnd(_procObject3, "Object3");
                if (hObj3 != IntPtr.Zero)
                {
                    SendMessage(hObj3, MSG_PLOT_GRAPH, IntPtr.Zero, IntPtr.Zero);
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        private async void ExecuteTask_Click(object sender, RoutedEventArgs e)
        {
            InputDialog dlg = new InputDialog { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                // Запускаємо/перезапускаємо з новими параметрами
                StartOrRestartProcesses(dlg.N, dlg.Min, dlg.Max);
                await Task.Delay(600); // Даємо час вікнам ініціалізуватися

                ArrangeWindows();
            }
        }

        private void StartOrRestartProcesses(int n, double min, double max)
        {
            string args = $"{n} {min.ToString(System.Globalization.CultureInfo.InvariantCulture)} {max.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            _procObject2 = StartProcess("Object2", args);
            _procObject3 = StartProcess("Object3", "");
        }

        private Process StartProcess(string name, string args)
        {
            foreach (var p in Process.GetProcessesByName(name))
            {
                try { p.Kill(); } catch { }
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name}.exe");
            if (File.Exists(path))
            {
                return Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = args,
                    UseShellExecute = true
                });
            }
            return null;
        }

        private IntPtr GetHWnd(Process p, string processName)
        {
            if (p != null && !p.HasExited && p.MainWindowHandle != IntPtr.Zero)
                return p.MainWindowHandle;

            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0) return processes[0].MainWindowHandle;

            return IntPtr.Zero;
        }

        private void ArrangeWindows()
        {
            IntPtr hLab6 = new WindowInteropHelper(this).Handle;
            IntPtr hObj2 = GetHWnd(_procObject2, "Object2");
            IntPtr hObj3 = GetHWnd(_procObject3, "Object3");

            MoveWindow(hLab6, 50, 100, 350, 250, true);
            if (hObj2 != IntPtr.Zero) MoveWindow(hObj2, 410, 100, 450, 350, true);
            if (hObj3 != IntPtr.Zero) MoveWindow(hObj3, 870, 100, 450, 350, true);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            KillProcess("Object2");
            KillProcess("Object3");
            Application.Current.Shutdown();
        }

        private void KillProcess(string name)
        {
            foreach (var proc in Process.GetProcessesByName(name))
            {
                try { proc.Kill(); } catch { }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
    }
}