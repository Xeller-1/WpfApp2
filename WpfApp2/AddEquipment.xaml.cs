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
    /// Логика взаимодействия для AddEquipment.xaml
    /// </summary>
    public partial class AddEquipment : Window
    {
        int UnitID;
        public AddEquipment(int unitId)
        {
            InitializeComponent();
            UnitID = unitId;
        }

        public Action OnWeaponAdded { get; internal set; }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(Description.Text))
            {
                errors.AppendLine("Введите описание");
            }
            if (string.IsNullOrEmpty(Name.Text))
            {
                errors.AppendLine("Введите название вооружения");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Equipment equipment= new Equipment
            {
                Equipment_ID = MillitaryEntities.GetContext().Weapons.Max(f => f.Weapon_ID) + 1,
                Equipment_Name = Name.Text,
                Unit_ID = UnitID,
                Specifications = Description.Text
            };

            try
            {
                MillitaryEntities.GetContext().Equipment.Add(equipment);

                MillitaryEntities.GetContext().SaveChanges();

                MessageBox.Show("Транспорт успешно сохранен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                OnWeaponAdded?.Invoke();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
