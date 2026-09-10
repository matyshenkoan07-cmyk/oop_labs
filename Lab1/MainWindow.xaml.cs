using System.Windows;

namespace Lab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Work1_Click(object sender, RoutedEventArgs e)
        {
            Work1Window work1 = new Work1Window();
            work1.Owner = this;
            
            bool? result = work1.ShowDialog();

            if (result == true)
            {
                int chosenNumber = work1.SelectedValue;
                ResultTextBlock.Text = $"Вибрано число з повзунка: {chosenNumber}";
            }
            else
            {
                ResultTextBlock.Text = "Дію скасовано (Робота 1)";
            }
        }
        
        private void Work2_Click(object sender, RoutedEventArgs e)
        {
            Work2Window work2 = new Work2Window();
            work2.Owner = this;
            
            bool? result = work2.ShowDialog();

            if (result == true)
            {
                string text = work2.EnteredText;
                ResultTextBlock.Text = $"Введений текст: {text}";
            }
            else
            {
                ResultTextBlock.Text = "Дію скасовано (Робота 2)";
            }
        }
    }
}