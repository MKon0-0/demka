using System.Net;
using System.Windows;

namespace MolochnyKombinat
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Разрешаем HTTP-запросы (для эмулятора)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Views.LoginWindow loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}