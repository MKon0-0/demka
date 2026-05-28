using System.Windows;
using MolochnyKombinat.Classes;
using MolochnyKombinat.Models;

namespace MolochnyKombinat.Views
{
    public partial class MainWindow : Window
    {
        private Users currentUser;

        public MainWindow(Users user)
        {
            InitializeComponent();
            currentUser = user;

            txtUserInfo.Text = $"Пользователь: {user.login} | Роль: {user.role}";

            if (user.role == "Admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            var users = BD.GetAllUsers();
            dgUsers.ItemsSource = users;
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            UserEditWindow editWindow = new UserEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as Users;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserEditWindow editWindow = new UserEditWindow(selectedUser);
            if (editWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as Users;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить пользователя {selectedUser.login}?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                BD.DeleteUser(selectedUser.ID);
                LoadUsers();
            }
        }

        private void btnUnblockUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgUsers.SelectedItem as Users;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для разблокировки", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedUser.is_blocked = false;
            BD.UpdateUser(selectedUser);
            LoadUsers();
            MessageBox.Show("Пользователь разблокирован", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private void btnValidation_Click(object sender, RoutedEventArgs e)
        {
            ValidationWindow validationWindow = new ValidationWindow();
            validationWindow.ShowDialog();
        }
    }
}