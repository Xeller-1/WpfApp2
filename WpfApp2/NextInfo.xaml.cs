using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для NextInfo.xaml
    /// </summary>
    public partial class NextInfo : Page
    {
        private int unitId; 
        public event Action OnWeaponAdded;
        public NextInfo(int unitId)
        {
            InitializeComponent();
            this.unitId = unitId;
            LoadUnitDetails();
        }

        private void LoadUnitDetails()
        {
            try
            {
                var unitDetails = (from unit in MillitaryEntities.GetContext().Units
                                   where unit.Unit_ID == unitId
                                   select new
                                   {
                                       Weapon_Name = unit.Weapons.Select(w => w.Weapon_Name).ToList(),
                                       Weapon_Specifications = unit.Weapons.Select(w => w.Specifications).ToList(),
                                       Equipment_Name = unit.Equipment.Select(e => e.Equipment_Name).ToList(),
                                       Equipment_Specifications = unit.Equipment.Select(e => e.Specifications).ToList()
                                   }).FirstOrDefault();

                if (unitDetails != null)
                {
                    var weapons = unitDetails.Weapon_Name.Zip(unitDetails.Weapon_Specifications, (name, specifications) => new
                    {
                        Weapon_Name = name,
                        Weapon_Specifications = specifications
                    }).ToList();

                    DataListView.ItemsSource = weapons;

                    var equipment = unitDetails.Equipment_Name.Zip(unitDetails.Equipment_Specifications, (name, specifications) => new
                    {
                        Equipment_Name = name,
                        Equipment_Specifications = specifications
                    }).ToList();

                    DataListViewEquipment.ItemsSource = equipment;
                }
                else
                {
                    MessageBox.Show("Не удалось найти информацию о технике и вооружении для выбранной части.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWeapons_Click(object sender, RoutedEventArgs e)
        {
            AddWeapon addWeaponWindow = new AddWeapon(unitId);
            addWeaponWindow.OnWeaponAdded += OnWeaponAddedHandler;
            addWeaponWindow.Show();
            LoadUnitDetails();
        }
        private void OnWeaponAddedHandler()
        {
            LoadUnitDetails();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoadUnitDetails();
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            AddEquipment addEQUIPMENTWindow = new AddEquipment(unitId);
            addEQUIPMENTWindow.OnWeaponAdded += OnWeaponAddedHandler;
            addEQUIPMENTWindow.Show();
            LoadUnitDetails();
        }
    }
}
