using System.Windows;

namespace Lab1
{
    public partial class Work2Window : Window
    {
        public string EnteredText { get; private set; } = string.Empty;

        public Work2Window()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            EnteredText = InputTextBox.Text;
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