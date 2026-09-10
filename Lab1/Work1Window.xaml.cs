using System.Windows;
using System.Windows.Controls;

namespace Lab1
{
    public partial class Work1Window : Window
    {
        public int SelectedValue { get; private set; } = 50;

        public Work1Window()
        {
            InitializeComponent();
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueLabel != null)
            {
                int val = (int)MySlider.Value;
                ValueLabel.Text = val.ToString();
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = (int)MySlider.Value;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}