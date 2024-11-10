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
                .ToList();

            AgentListView.ItemsSource = currentUnits;

            ComboType.SelectedIndex = 0;
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

            // Фильтрация по поисковому запросу
            currentUnits = currentUnits
    .Where(u =>
        (u.Unit_Number != null && u.Unit_Number.ToString().Contains(TBoxSearch.Text) ||
         u.Location?.City?.ToLower().Contains(TBoxSearch.Text.ToLower()) == true ||
         u.Location?.Country?.ToLower().Contains(TBoxSearch.Text.ToLower()) == true)
    )
    .ToList();



            // Привязка обновленных данных к ListView
            AgentListView.ItemsSource = currentUnits;

            TableList = currentUnits;
            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            CountPage = (CountRecords + 9) / 10;

            int min;
            if (selectedPage.HasValue)
            {
                CurrentPage = selectedPage.Value;
            }
            else
            {
                if (direction == 1 && CurrentPage > 0)
                {
                    CurrentPage--;
                }
                else if (direction == 2 && CurrentPage < CountPage - 1)
                {
                    CurrentPage++;
                }
            }

            min = Math.Min((CurrentPage + 1) * 10, CountRecords);
            for (int i = CurrentPage * 10; i < min; i++)
            {
                CurrentPageList.Add(TableList[i]);
            }

            PageListBox.Items.Clear();
            for (int i = 1; i <= CountPage; i++)
            {
                PageListBox.Items.Add(i);
            }
            PageListBox.SelectedIndex = CurrentPage;

            TBCount.Text = min.ToString();
            TBAllRecords.Text = $" из {CountRecords}";

            AgentListView.ItemsSource = CurrentPageList;
        }

        private void PageListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Переключаем страницу
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem) - 1);
            UpdateUnits();


        }


        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
            UpdateUnits();
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
            UpdateUnits();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddUnitPage(null));
            UpdateUnits();
        }


        private void AgentListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Units selectedUnit = AgentListView.SelectedItem as Units;

            if (selectedUnit != null)
            {
                // Передаем selectedUnit в конструктор
                DetailsWindow detailsWindow = new DetailsWindow(selectedUnit);
                detailsWindow.Show();
            }
            else
            {
                MessageBox.Show("Не удалось получить данные для выбранной части.");
            }
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




    }
}