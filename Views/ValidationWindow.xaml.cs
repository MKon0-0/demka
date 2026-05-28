using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using MolochnyKombinat.Classes;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MolochnyKombinat.Views
{
    public partial class ValidationWindow : Window
    {
        private string currentFullName = "";
        private bool isValid = false;
        private string invalidSymbolsList = "";

        public ValidationWindow()
        {
            InitializeComponent();
        }

        private async void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            btnGetData.IsEnabled = false;
            txtStatus.Text = "Загрузка данных из эмулятора...";
            txtFullName.Text = "";
            txtValidationResult.Text = "";
            txtInvalidSymbols.Text = "";

            string result = await ApiHelper.GetFullNameAsync();
            currentFullName = result;
            txtFullName.Text = currentFullName;

            if (currentFullName.StartsWith("Эмулятор"))
            {
                txtStatus.Text = "Ошибка подключения к эмулятору";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                btnValidate.IsEnabled = false;
            }
            else
            {
                txtStatus.Text = "Данные получены. Нажмите 'Проверить' для валидации.";
                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
                btnValidate.IsEnabled = true;
            }

            btnGetData.IsEnabled = true;
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            // Валидация ФИО
            isValid = ApiHelper.ValidateFullName(currentFullName, out invalidSymbolsList);

            if (isValid)
            {
                txtValidationResult.Text = "✓ ФИО корректно. Запрещенных символов не найдено.";
                txtValidationResult.Foreground = System.Windows.Media.Brushes.Green;
                txtInvalidSymbols.Text = "Нет";
            }
            else
            {
                txtValidationResult.Text = "✗ ФИО содержит запрещенные символы!";
                txtValidationResult.Foreground = System.Windows.Media.Brushes.Red;
                txtInvalidSymbols.Text = invalidSymbolsList;
            }

            btnSendResult.IsEnabled = true;
            txtStatus.Text = "Проверка завершена. Можете отправить результат или сохранить в ТестКейс.";
        }

        private void btnSendResult_Click(object sender, RoutedEventArgs e)
        {
            // Эмуляция отправки результата
            string resultMessage = isValid ? "УСПЕШНО" : "НЕ УСПЕШНО";
            MessageBox.Show($"Результат проверки отправлен.\nСтатус: {resultMessage}\nДанные: {currentFullName}\nЗапрещенные символы: {(string.IsNullOrEmpty(invalidSymbolsList) ? "нет" : invalidSymbolsList)}",
                            "Результат отправлен", MessageBoxButton.OK, MessageBoxImage.Information);
            txtStatus.Text = $"Результат отправлен. Статус: {resultMessage}";
        }

        private void btnSaveToWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(currentFullName))
                {
                    MessageBox.Show("Нет данных для сохранения. Сначала получите и проверьте данные.",
                                    "Нет данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Сохраняем на рабочий стол
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"ТестКейс_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(desktopPath, fileName);

                // Формируем содержимое
                string content = "====================\n";
                content += "ТЕСТ-КЕЙС ВАЛИДАЦИИ ФИО\n";
                content += "====================\n\n";
                content += $"Дата и время: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
                content += $"Проверяемые данные: {currentFullName}\n";
                content += $"Результат проверки: {(isValid ? "УСПЕШНО" : "НЕ УСПЕШНО")}\n";
                content += $"Запрещенные символы: {(string.IsNullOrEmpty(invalidSymbolsList) ? "не обнаружены" : invalidSymbolsList)}\n";
                content += "\n====================\n";

                File.WriteAllText(filePath, content);

                MessageBox.Show($"Результат сохранен на рабочий стол!\n\nФайл: {fileName}",
                                "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);

                // Открываем папку с файлом
                System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{filePath}\"");

                txtStatus.Text = $"Результат сохранен: {fileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка сохранения";
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}