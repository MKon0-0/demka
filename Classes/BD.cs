using System;
using System.Collections.Generic;
using System.Linq;
using MolochnyKombinat.Models;

namespace MolochnyKombinat.Classes
{
    public static class BD
    {
        public static DEMKAEntities db = new DEMKAEntities();

        // Аутентификация пользователя
        public static Users AuthenticateUser(string login, string password)
        {
            return db.Users.FirstOrDefault(u => u.login == login && u.password == password);
        }

        // Получить всех пользователей
        public static List<Users> GetAllUsers()
        {
            return db.Users.ToList();
        }

        // Добавить пользователя
        public static void AddUser(Users newUser)
        {
            db.Users.Add(newUser);
            db.SaveChanges();
        }

        // Обновить пользователя
        public static void UpdateUser(Users user)
        {
            var existingUser = db.Users.Find(user.ID);
            if (existingUser != null)
            {
                existingUser.login = user.login;
                existingUser.password = user.password;
                existingUser.role = user.role;
                existingUser.is_blocked = user.is_blocked;
                db.SaveChanges();
            }
        }

        // Удалить пользователя
        public static void DeleteUser(int userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        // Блокировка пользователя
        public static void BlockUser(string login)
        {
            var user = db.Users.FirstOrDefault(u => u.login == login);
            if (user != null)
            {
                user.is_blocked = true;
                db.SaveChanges();
            }
        }
    }
}