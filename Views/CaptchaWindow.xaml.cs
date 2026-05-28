using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MolochnyKombinat.Views
{
    public partial class CaptchaWindow : Window
    {
        // Фрагменты и правильный порядок
        private List<FragmentInfo> fragments = new List<FragmentInfo>();
        private List<int> clickOrder = new List<int>();
        private bool isCompleted = false;

        public bool IsCaptchaPassed { get; private set; } = false;

        public CaptchaWindow()
        {
            InitializeComponent();
            InitializeCaptcha();
        }

        private void InitializeCaptcha()
        {
            // Создаем фрагменты
            fragments.Clear();
            clickOrder.Clear();
            isCompleted = false;

            // Перемешиваем порядок отображения
            var shuffledOrder = new List<int> { 1, 2, 3, 4 };
            Random rnd = new Random();
            for (int i = 0; i < shuffledOrder.Count; i++)
            {
                int r = rnd.Next(i, shuffledOrder.Count);
                int temp = shuffledOrder[i];
                shuffledOrder[i] = shuffledOrder[r];
                shuffledOrder[r] = temp;
            }

            // Очищаем сетку
            PuzzleGrid.Children.Clear();

            // Добавляем фрагменты в случайном порядке
            foreach (int num in shuffledOrder)
            {
                var fragment = CreateFragment(num);
                fragments.Add(new FragmentInfo
                {
                    Number = num,
                    BorderControl = fragment.Border,
                    ImageControl = fragment.Image,
                    GridControl = fragment.Grid,
                    IsClicked = false
                });
                PuzzleGrid.Children.Add(fragment.Border);
            }

            txtOrder.Text = "";
            txtStatus.Text = "";
            btnConfirm.IsEnabled = false;

            // Явно подписываем кнопки (на случай, если в XAML не сработало)
            btnReset.Click += btnReset_Click;
            btnConfirm.Click += btnConfirm_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private FragmentControls CreateFragment(int number)
        {
            // Загружаем изображение
            string imagePath = $"/Images/{number}.png";
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Relative);
            bitmap.EndInit();

            Image image = new Image
            {
                Source = bitmap,
                Width = 180,
                Height = 180,
                Stretch = Stretch.UniformToFill
            };

            // Создаем рамку
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5),
                Background = Brushes.White,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Подпись с номером
            TextBlock numberLabel = new TextBlock
            {
                Text = $"№{number}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = Brushes.Yellow,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5),
                Padding = new Thickness(5, 2, 5, 2)
            };

            // Контейнер для изображения и подписи
            Grid grid = new Grid();
            grid.Children.Add(image);
            grid.Children.Add(numberLabel);

            border.Child = grid;
            border.Tag = number;

            // Подписываем на клик
            border.MouseLeftButtonDown += Fragment_Click;

            return new FragmentControls
            {
                Border = border,
                Image = image,
                Grid = grid,
                Number = number
            };
        }

        private void Fragment_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isCompleted) return;

            Border border = sender as Border;
            if (border == null) return;

            int number = (int)border.Tag;

            // Проверяем, не кликнули ли уже на этот фрагмент
            var fragmentInfo = fragments.Find(f => f.Number == number);
            if (fragmentInfo.IsClicked) return;

            // Ожидаемый следующий номер
            int expectedNext = clickOrder.Count + 1;

            if (number == expectedNext)
            {
                // Правильный клик
                fragmentInfo.IsClicked = true;
                clickOrder.Add(number);

                // Подсвечиваем зеленым
                fragmentInfo.BorderControl.BorderBrush = Brushes.Green;
                fragmentInfo.BorderControl.BorderThickness = new Thickness(3);

                txtOrder.Text = string.Join(" → ", clickOrder);

                // Проверяем, собран ли весь пазл
                if (clickOrder.Count == 4)
                {
                    isCompleted = true;
                    txtStatus.Text = "✓ Пазл собран правильно! Нажмите 'Подтвердить'.";
                    txtStatus.Foreground = Brushes.Green;
                    btnConfirm.IsEnabled = true;
                }
            }
            else
            {
                // Неправильный клик
                txtStatus.Text = $"Ошибка! Вы кликнули на фрагмент №{number}, а должны были на №{expectedNext}. Начните заново.";
                txtStatus.Foreground = Brushes.Red;

                // Сбрасываем все
                ResetCaptcha();
            }
        }

        private void ResetCaptcha()
        {
            // Сбрасываем все фрагменты
            foreach (var fragment in fragments)
            {
                fragment.IsClicked = false;
                fragment.BorderControl.BorderBrush = Brushes.Gray;
                fragment.BorderControl.BorderThickness = new Thickness(2);
            }

            clickOrder.Clear();
            txtOrder.Text = "";
            isCompleted = false;
            btnConfirm.IsEnabled = false;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            InitializeCaptcha();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (clickOrder.Count == 4 && clickOrder[0] == 1 && clickOrder[1] == 2 && clickOrder[2] == 3 && clickOrder[3] == 4)
            {
                IsCaptchaPassed = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                txtStatus.Text = "Пазл не собран правильно. Начните заново!";
                txtStatus.Foreground = Brushes.Red;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsCaptchaPassed = false;
            this.Close();  // Убираем DialogResult, просто закрываем окно
        }

        private class FragmentInfo
        {
            public int Number { get; set; }
            public Border BorderControl { get; set; }
            public Image ImageControl { get; set; }
            public Grid GridControl { get; set; }
            public bool IsClicked { get; set; }
        }

        private class FragmentControls
        {
            public Border Border { get; set; }
            public Image Image { get; set; }
            public Grid Grid { get; set; }
            public int Number { get; set; }
        }
    }
}