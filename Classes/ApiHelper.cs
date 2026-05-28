using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MolochnyKombinat.Classes
{
    public class ApiHelper
    {
        public static async Task<string> GetFullNameAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Используем WebClient (проще и надежнее)
                    using (WebClient client = new WebClient())
                    {
                        // Увеличиваем таймаут
                        client.Headers.Add("User-Agent", "Mozilla/5.0");

                        string url = "http://127.0.0.1:4444/TransferSimulator/fullName";
                        string json = client.DownloadString(url);

                        JObject obj = JObject.Parse(json);
                        string value = obj["value"]?.ToString();

                        if (!string.IsNullOrEmpty(value))
                        {
                            return value;
                        }

                        return "Не удалось получить ФИО из ответа";
                    }
                }
                catch (WebException ex)
                {
                    // Подробная информация об ошибке
                    string errorMsg = ex.Message;
                    if (ex.Response != null)
                    {
                        using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                        {
                            errorMsg += "\n" + reader.ReadToEnd();
                        }
                    }
                    return $"Ошибка подключения: {errorMsg}";
                }
                catch (Exception ex)
                {
                    return $"Ошибка: {ex.Message}";
                }
            });
        }

        public static bool ValidateFullName(string fullName, out string invalidSymbols)
        {
            invalidSymbols = "";

            if (string.IsNullOrEmpty(fullName) || fullName.Contains("Ошибка"))
            {
                invalidSymbols = "ошибка подключения";
                return false;
            }

            char[] forbiddenSymbols = { '&', '%', '#', '*', '@', '!', '$', '^', '(', ')', '<', '>', '/', '\\', '|', '"', ';', '=', '+', '[', ']', '{', '}', '?', '`', '~' };

            foreach (char c in forbiddenSymbols)
            {
                if (fullName.Contains(c))
                {
                    invalidSymbols += c + " ";
                }
            }

            return string.IsNullOrEmpty(invalidSymbols);
        }
    }
}