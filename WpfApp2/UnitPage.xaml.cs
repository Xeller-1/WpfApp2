using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static WpfApp2.MillitaryEntities;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для UnitPage.xaml
    /// </summary>
    public partial class UnitPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Units> CurrentPageList = new List<Units>();
        List<Units> TableList;

        public UnitPage()
        {
            InitializeComponent();

            // Загружаем данные из контекста и привязываем к ListView
            var currentUnits = MillitaryEntities.GetContext().Units
                .Include("Location") // Связь с таблицей местоположений    
                .Include("Equipment")
                .Include("Military_Branch")
                .Include("Weapons")
                .ToList();

            AgentListView.ItemsSource = currentUnits;

            ComboType.SelectedIndex = 0;

            var unitTypes = MillitaryEntities.GetContext().Military_Branch.ToList();
            foreach (var unitType in unitTypes)
            {
                ComboUnitType.Items.Add(new ComboBoxItem { Content = unitType.Branch_Name });
            }
            ComboUnitType.SelectedIndex = 0;

        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUnits();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUnits();
        }

        private void UpdateUnits()
        {
            var currentUnits = MillitaryEntities.GetContext().Units
                .Include("Location")
                .Include("Equipment")
                .Include("Military_Branch")
                .Include("Weapons")
                .ToList();

            // Сортировка по типу войск
            if (ComboType.SelectedIndex == 1)
            {
                currentUnits = currentUnits.OrderBy(u => u.Unit_Number).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentUnits = currentUnits.OrderByDescending(u => u.Unit_Number).ToList();
            }


            if (ComboUnitType.SelectedIndex > 0)
{
    var selectedUnitType = ComboUnitType.SelectedItem as ComboBoxItem;

    if (selectedUnitType != null)
    {
        var unitTypeName = selectedUnitType.Content.ToString();

        // Понимание структуры Military_Branch и правильный доступ к свойствам
        currentUnits = currentUnits.Where(u => u.Military_Branch != null && u.Military_Branch.Branch_Name == unitTypeName).ToList();
    }
}


            // Фильтрация по поисковому запросу
            currentUnits = currentUnits
                .Where(u =>
        (u.Unit_Number != null && u.Unit_Number.ToString().Contains(TBoxSearch.Text) ||
         u.Location?.City?.ToLower().Contains(TBoxSearch.Text.ToLower()) == true ||
         u.Location?.Country?.ToLower().Contains(TBoxSearch.Text.ToLower()) == true)
    ).ToList();



            // Привязка обновленных данных к ListView
            AgentListView.ItemsSource = currentUnits;

            TableList = currentUnits;
        }

       

        private void PageListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Переключаем страницу
            UpdateUnits();


        }


        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUnits();
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUnits();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddUnitPage(null));
            UpdateUnits();
        }


       

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUnit = (sender as Button).DataContext as Units;

            if (currentUnit == null)
            {
                MessageBox.Show("Не удалось найти объект для удаления.");
                return;
            }

            // Подтверждаем удаление
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем сначала Location, если не используются другими записями
                    var locationToDelete = currentUnit.Location;

                    

                    // Удаляем саму единицу
                    MillitaryEntities.GetContext().Units.Remove(currentUnit);

                    // Сохраняем изменения в базе данных
                    MillitaryEntities.GetContext().SaveChanges();

                    MessageBox.Show("Запись успешно удалена.");

                    // Обновляем данные на странице
                    AgentListView.ItemsSource = MillitaryEntities.GetContext().Units.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
            UpdateUnits();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var selectedUnit = (sender as Button).DataContext as Units;
            if (selectedUnit != null)
            {
                // Передаем ID выбранной части на страницу NextInfo
                var nextInfoPage = new NextInfo(selectedUnit.Unit_ID);

                // Открываем страницу NextInfo
                NavigationService.Navigate(nextInfoPage);
            }
            else
            {
                MessageBox.Show("Ошибка: не удалось найти выбранную часть.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateUnits();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboUnitType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUnits();
        }

        private void EditUnit_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddUnitPage((sender as Button).DataContext as Units));
        }
    }
}