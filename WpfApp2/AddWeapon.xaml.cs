using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
    /// Логика взаимодействия для AddWeapon.xaml
    /// </summary>
    public partial class AddWeapon : Window
    {
        int UnitID;
        public event Action OnWeaponAdded;
        public AddWeapon(int unitId)
        {
            InitializeComponent();
            UnitID = unitId;
        }

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

            Weapons weapons = new Weapons
            {
                Weapon_ID = MillitaryEntities.GetContext().Weapons.Max(f => f.Weapon_ID) + 1,
                Weapon_Name = Name.Text,
                Unit_ID = UnitID,
                Specifications = Description.Text
            };

            try
            {
                MillitaryEntities.GetContext().Weapons.Add(weapons);

                MillitaryEntities.GetContext().SaveChanges();

                MessageBox.Show("Оружие успешно сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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
