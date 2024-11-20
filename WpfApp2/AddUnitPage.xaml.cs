using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для AddUnitPage.xaml
    /// </summary>
    public partial class AddUnitPage : Page
    {
        private Units _currentUnit = new Units();


        public AddUnitPage(Units selectedUnit = null)
        {
            InitializeComponent();
            selectMilitaryBrach.ItemsSource = MillitaryEntities.GetContext().Military_Branch.Select(f => f.Branch_Name).ToList();
            // Если передан выбранный объект, значит мы редактируем существующую единицу
            if (selectedUnit != null)
            {
                _currentUnit = selectedUnit;
                // Загружаем объект Location для выбранной единицы
                _currentUnit.Location = MillitaryEntities.GetContext().Location
                    .FirstOrDefault(l => l.Location_ID == _currentUnit.Location_ID);
                var currentEqi = MillitaryEntities.GetContext().Equipment.Where(f => f.Unit_ID == _currentUnit.Unit_ID).ToList();
                selectMilitaryBrach.SelectedIndex = (int)(selectedUnit.Branch_ID - 1);
            }

            var militaryBranches = MillitaryEntities.GetContext().Military_Branch.ToList();
            

            // Понимание структуры Military_Branch и правильный доступ к свойствам


            // Если Location не инициализировано, создаём новый объект Location
            if (_currentUnit.Location == null)
            {
                _currentUnit.Location = new Location();
            }

            // Устанавливаем DataContext для привязки данных
            DataContext = _currentUnit;
        }


        // Обработчик нажатия кнопки "Выбрать файл" для выбора фото
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Открыть диалоговое окно для выбора файла
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                // Сохраняем путь к файлу в объекте
                _currentUnit.UnitPhoto = openFileDialog.FileName;

                // Отображаем изображение в Image
                UnitImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        // Обработчик нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация данных
            if (_currentUnit.Unit_Number == null)
            {
                errors.AppendLine("Укажите номер части");
            }

            // Если Location пустой, то инициализируем или обновляем его
            if (_currentUnit.Location == null)
            {
                _currentUnit.Location = new Location(); // Создаем новый объект Location
            }

            // Проверяем остальные поля
            if (string.IsNullOrWhiteSpace(_currentUnit.Location.City))
            {
                errors.AppendLine("Укажите город");
            }

            if (string.IsNullOrWhiteSpace(_currentUnit.Location.Country))
            {
                errors.AppendLine("Укажите страну");
            }

            if (string.IsNullOrWhiteSpace(_currentUnit.Location.Address))
            {
                errors.AppendLine("Укажите адрес");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка на существование части с таким номером
            var existingUnit = MillitaryEntities.GetContext().Units
                .FirstOrDefault(p => p.Unit_Number == _currentUnit.Unit_Number);

            if (existingUnit != null && _currentUnit.Unit_ID == 0) // Если такая часть уже существует
            {
                MessageBox.Show("Такая часть уже существует.");
                return;
            }


            _currentUnit.Branch_ID = selectMilitaryBrach.SelectedIndex + 1;
            // Если это новая запись, добавляем в базу данных
            if (_currentUnit.Unit_ID == 0)
            {
                MillitaryEntities.GetContext().Units.Add(_currentUnit);
                
            }
            else
            {
                // Если редактируем, обновляем данные в базе
                MillitaryEntities.GetContext().Entry(_currentUnit).State = EntityState.Modified;
            } 

            try
            {
                // Сохраняем изменения
                MillitaryEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");

                // Переход на страницу с единицами
                NavigationService.Navigate(new UnitPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void selectMilitaryBrach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}


