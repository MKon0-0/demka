using System;
using System.Windows;
using MolochnyKombinat.Classes;
using MolochnyKombinat.Models;

namespace MolochnyKombinat.Views
{
    public partial class LoginWindow : Window
    {
        private int captchaErrorCount = 0;
        private bool captchaPassed = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            // 1. Проверка на пустые поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Пожалуйста, заполните все поля!";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // 2. Проверка капчи (если еще не пройдена)
            if (!captchaPassed)
            {
                txtStatus.Text = "Пожалуйста, пройдите проверку (соберите пазл)";
                txtStatus.Foreground = System.Windows.Media.Brushes.Orange;
                OpenCaptchaWindow();
                return;
            }

            // 3. Поиск пользователя в базе данных
            var user = BD.AuthenticateUser(login, password);

            if (user == null)
            {
                txtStatus.Text = "Вы ввели неверный логин или пароль. Пожалуйста, проверьте ещё раз введенные данные.";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;

                captchaErrorCount++;

                if (captchaErrorCount >= 3)
                {
                    BD.BlockUser(login);
                    MessageBox.Show($"Пользователь {login} заблокирован. Обратитесь к администратору.",
                                    "Блокировка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    captchaErrorCount = 0;
                }
                return;
            }

            // 4. Проверка, не заблокирован ли пользователь
            if (user.is_blocked == true)
            {
                MessageBox.Show("Вы заблокированы. Обратитесь к администратору.",
                                "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 5. Успешная авторизация
            MessageBox.Show($"Вы успешно авторизовались!\nДобро пожаловать, {user.login}!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // 6. Открываем главное окно
            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }

        private void OpenCaptchaWindow()
        {
            CaptchaWindow captchaWindow = new CaptchaWindow();
            captchaWindow.Owner = this;
            bool? result = captchaWindow.ShowDialog();

            if (result == true && captchaWindow.IsCaptchaPassed)
            {
                captchaPassed = true;
                txtStatus.Text = "Капча пройдена! Теперь можете войти.";
                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                captchaErrorCount++;
                if (captchaErrorCount >= 3)
                {
                    MessageBox.Show("Превышено количество попыток капчи. Попробуйте позже.",
                                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                }
                else
                {
                    txtStatus.Text = $"Капча не пройдена. Осталось попыток: {3 - captchaErrorCount}";
                    txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }
    }
}