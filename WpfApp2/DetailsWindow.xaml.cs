using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        private Units _selectedUnit;

        public DetailsWindow(Units selectedUnit)
        {
            InitializeComponent();
            _selectedUnit = selectedUnit;
            // Отображаем данные в окне
            CompanyCountText.Text = _selectedUnit.Company_Count.ToString();
            EquipmentCountText.Text = _selectedUnit.Equipment_Count.ToString();
            WeaponCountText.Text = _selectedUnit.Weapon_Count.ToString();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализуйте логику редактирования данных
            // Например, откроем окно с формой для редактирования данных
            EditUnitWindow editWindow = new EditUnitWindow(_selectedUnit);
            editWindow.Show();
        }
    }
}
