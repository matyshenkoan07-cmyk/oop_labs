using System.Windows;
using System.Windows.Controls;

namespace Lab5
{
    public class TableItem
    {
        public string Name { get; set; } = string.Empty;
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }

    public partial class MyTableWindow : Window
    {
        public event Action<int>? OnShapeSelected;
        public event Action<int>? OnShapeDeleted;

        public MyTableWindow()
        {
            InitializeComponent();
        }

        public void UpdateTableData(IEnumerable<TableItem> items)
        {
            objectsDataGrid.ItemsSource = null;
            objectsDataGrid.ItemsSource = items;
        }

        private void ObjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = objectsDataGrid.SelectedIndex;
            if (index >= 0)
            {
                OnShapeSelected?.Invoke(index);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int index = objectsDataGrid.SelectedIndex;
            if (index >= 0)
            {
                OnShapeDeleted?.Invoke(index);
            }
        }
    }
}