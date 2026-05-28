using System.Windows;
using System.Windows.Controls;
using MolochnyKombinat.Classes;
using MolochnyKombinat.Models;

namespace MolochnyKombinat.Views
{
    public partial class UserEditWindow : Window
    {
        private Users _user;
        private bool _isEditMode;

        public UserEditWindow(Users user)
        {
            InitializeComponent();  // ЭТА СТРОКА ОЧЕНЬ ВАЖНА!
            _user = user;
            _isEditMode = (user != null);

            if (_isEditMode)
            {
                Title = "Редактирование пользователя";
                txtLogin.Text = user.login;
                txtPassword.Password = user.password;
                cmbRole.SelectedIndex = user.role == "Admin" ? 1 : 0;
                chkBlocked.IsChecked = user.is_blocked == true;
            }
            else
            {
                Title = "Добавление пользователя";
                chkBlocked.IsChecked = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";
            bool isBlocked = chkBlocked.IsChecked == true;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин и пароль обязательны для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {
                // Обновление существующего
                _user.login = login;
                _user.password = password;
                _user.role = role;
                _user.is_blocked = isBlocked;
                BD.UpdateUser(_user);
            }
            else
            {
                // Проверка на дубликат логина
                var existingUser = BD.AuthenticateUser(login, "");
                if (existingUser != null && existingUser.login == login)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание нового
                Users newUser = new Users
                {
                    login = login,
                    password = password,
                    role = role,
                    is_blocked = isBlocked
                };
                BD.AddUser(newUser);
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}