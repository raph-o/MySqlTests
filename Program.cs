using System;
using System.Linq;
using Omu.Encrypto;

namespace MySqlTests
{
    class Options
    {
        public static void SelectOption ()
        {
            Console.Clear();

            Console.WriteLine("Choose an option by writing the corresponding number:");
            Console.WriteLine("1. Create an user");
            Console.WriteLine("2. Delete an user");
            Console.WriteLine("3. Perform an update on a user");
            Console.WriteLine("4. Exit");
            Console.WriteLine("");

            int option;

            do
            {
                Console.Write("Option: ");
                option = Convert.ToInt32(Console.ReadLine());               

                switch (option)
                {
                    case 1:
                        Create();
                        break;
                    case 2:
                        Delete();
                        break;
                    case 3:
                        Update();
                        break;
                    case 4:
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("You have to choose an available option");
                        break;
                }

            } while (option < 1 || option > 4);

            Console.ReadKey();
        }

        private static void Create ()
        {
            Console.Clear();
            User user = new User();
            string tempPwd = "";

            SqlUtils sql = new SqlUtils("localhost", 3306, "trysql", "root", "");

            Console.Write("Enter your username: ");
            user.Username = Convert.ToString(Console.ReadLine());

            while (sql.userInfos.Keys.Contains(user.Username))
            {
                Console.Write("This username already exists in our database, enter another: ");
                user.Username = Convert.ToString(Console.ReadLine());
            }

            Console.Write("Enter your password: ");
            tempPwd = Convert.ToString(Console.ReadLine());

            Console.Write("Enter your email: ");
            user.Mail = Convert.ToString(Console.ReadLine());

            while (sql.userInfos.Values.Contains(user.Mail))
            {
                Console.Write("This mail already exists in our database, enter another: ");
                user.Mail = Convert.ToString(Console.ReadLine());
            }

            var hasher = new Hasher();
            user.Password = hasher.Encrypt(tempPwd);

            user.DateJoined = DateTime.Today.ToShortDateString();

            sql.CreateUser(user);
            sql.Disconnect();
            Console.ReadKey();
        }

        private static void Delete ()
        {
            Console.Clear();

            SqlUtils sql = new SqlUtils("localhost", 3306, "trysql", "root", "");

            Console.Write("Enter the username of the user you want to delete: ");
            string username = Convert.ToString(Console.ReadLine());

            if (!sql.userInfos.Keys.Contains(username))
            {
                Console.WriteLine("This user doesn't exist");
                return;
            }

            sql.DeleteUser(username);
            sql.Disconnect();
            Console.ReadKey();
        }

        private static void Update ()
        {
            Console.Clear();
            User user = new User();
            string tempPwd = "";

            SqlUtils sql = new SqlUtils("localhost", 3306, "trysql", "root", "");

            Console.Write("Enter the username of the user you want to update: ");
            string username = Convert.ToString(Console.ReadLine());

            if (!sql.userInfos.Keys.Contains(username))
            {
                Console.WriteLine("This user doesn't exist");
                return;
            }

            Console.Write("Enter the new username you want: ");
            user.Username = Convert.ToString(Console.ReadLine());

            Console.Write("Enter the new password you want: ");
            tempPwd = Convert.ToString(Console.ReadLine());

            Console.Write("Enter the new mail you want: ");
            user.Mail = Convert.ToString(Console.ReadLine());

            var hasher = new Hasher();
            user.Password = hasher.Encrypt(tempPwd);

            sql.UpdateUser(username, sql.userInfos[username], user);
            sql.Disconnect();
            Console.ReadKey();
        }
    }

    class Program
    {
        static void Main (string[] args)
        {
            Options.SelectOption();
        }
    }
}
