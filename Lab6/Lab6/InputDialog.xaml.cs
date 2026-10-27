using System.Windows;

namespace Lab6
{
    public partial class InputDialog : Window
    {
        public int N { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }

        public InputDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtN.Text, out int n) &&
                double.TryParse(txtMin.Text, out double min) &&
                double.TryParse(txtMax.Text, out double max) && n > 1 && max > min)
            {
                N = n;
                Min = min;
                Max = max;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректні числові дані!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}