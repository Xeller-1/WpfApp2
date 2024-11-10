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
    public partial class EditUnitWindow : Window
    {
        private Units _unit;

        public EditUnitWindow(Units selectedUnit)
        {
            InitializeComponent();
            _unit = selectedUnit;

            // Проверка, есть ли у текущей единицы Location, если нет — создаем новый
            if (_unit.Location == null)
            {
                _unit.Location = new Location();
            }

            DataContext = _unit; // Устанавливаем DataContext для привязки данных
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = MillitaryEntities.GetContext();

                // Убедитесь, что Location также отслеживается контекстом
                if (_unit.Location.Location_ID == 0)
                {
                    context.Location.Add(_unit.Location);
                }

                // Сохраняем изменения в Unit и Location
                context.Entry(_unit).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                MessageBox.Show("Изменения успешно сохранены.");
                Close(); // Закрываем окно после сохранения
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}