using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
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
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;

namespace WpfApp3
{

    public static class czyAdmin
    {
        public static int admin { get; set; }
    }

    public static class getID
    {
        public static int userID { get; set; }
    }

    public static class getBalance
    {
        public static float userBalance { get; set; }
    }
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_ButtonZaloguj(object sender, RoutedEventArgs e)
        {
            static string Sha1Hash(string input)
            {
                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                    StringBuilder sb = new StringBuilder();

                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            }

            string email = LoginTextBox.Text;
            string haslo = HasloPasswordBox.Password;
            haslo = Sha1Hash(haslo);
            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x; ";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "SELECT COUNT (*) FROM Uzytkownicy WHERE Email = @email AND Haslo = @haslo";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@haslo", haslo);
                    conn.Open();
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    if (result <= 0)
                    {
                        MessageBox.Show("Nieprawidłowe dane logowania. Spróbuj ponownie");
                    }
                    else
                    {
                        string query1 = "SELECT COUNT (*) FROM Uzytkownicy WHERE Email = @email AND Haslo = @haslo AND Admin = 1";
                        using (SqlCommand command1 = new SqlCommand(query1, conn))
                        {
                            command1.Parameters.AddWithValue("@email", email);
                            command1.Parameters.AddWithValue("@haslo", haslo);
                            int result1 = Convert.ToInt32(command1.ExecuteScalar());
                            czyAdmin.admin = result1;
                        }
                        string query2 = "SELECT ID FROM Uzytkownicy WHERE Email = @email AND Haslo = @haslo";
                        using (SqlCommand command2 = new SqlCommand(query2, conn))
                        {
                            command2.Parameters.AddWithValue("@email", email);
                            command2.Parameters.AddWithValue("@haslo", haslo);
                            int result2 = Convert.ToInt32(command2.ExecuteScalar());
                            getID.userID = result2;
                        }
                        string query3 = "SELECT Stan_konta FROM Uzytkownicy WHERE Email = @email AND Haslo = @haslo";
                        using (SqlCommand command3 = new SqlCommand(query3, conn))
                        {
                            command3.Parameters.AddWithValue("@email", email);
                            command3.Parameters.AddWithValue("@haslo", haslo);
                            float result3 = Convert.ToSingle(command3.ExecuteScalar());
                            getBalance.userBalance = result3;
                        }
                        MainWindow mainWindow = new MainWindow();
                        this.Close();
                        mainWindow.Show();
                    }
                }
            }
            conn.Close();
        }

        private void Button_Click_Zerejestruj(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            this.Close();
            registerWindow.Show();
            
        }

        private void TextBox_GotFocus_Login(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus_Login;
        }

        private void TextBox_LostFocus_Login(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = "Login";
                box.GotFocus += TextBox_GotFocus_Login;
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HasloPasswordBox.Focus();
            textPassword.Visibility = Visibility.Collapsed;

        }

        private void PasswordBox_GotFocus_Haslo(object sender, RoutedEventArgs e)
        {
            PasswordBox tb = (PasswordBox)sender;
            tb.Password = string.Empty;
            tb.GotFocus -= PasswordBox_GotFocus_Haslo;
            textPassword.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus_Haslo(object sender, RoutedEventArgs e)
        {
            PasswordBox? box = sender as PasswordBox;
            if (box.Password.Trim().Equals(string.Empty))
            {
                box.GotFocus += PasswordBox_GotFocus_Haslo;
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void SetLang(string lang)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            ResourceDictionary langDict = new ResourceDictionary()
            {
                Source = new Uri($"/Languages/Dictionary-{lang}.xaml", UriKind.Relative)
            };
            App.Current.Resources.MergedDictionaries[1] = langDict;

        }

        private void ButtonLang_Click(object sender, RoutedEventArgs e)
        {
            SetLang(((Button)sender).Tag.ToString());
        }


    }
}
