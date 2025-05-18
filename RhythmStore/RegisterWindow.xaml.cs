using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
using System.Data.SqlClient;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.Threading;

namespace WpfApp3
{

    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Zarejestruj(object sender, RoutedEventArgs e)
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

            string imie = ImieTextBox.Text;
            string nazwisko = NazwiskoTextBox.Text;
            string email = EmailTextBox.Text;
            string powtorzEmail = PowtorzEmailTextBox.Text;
            string haslo = HasloPasswordBox.Password;
            string powtorzHaslo = PowtorzHasloPasswordBox.Password;

            if (haslo != powtorzHaslo || HasloPasswordBox.Password.Length == 0)
            {
                BladHasloTextBlox.Visibility = Visibility.Visible;
            }
            else if (haslo == powtorzHaslo && HasloPasswordBox.Password.Length != 0)
            {
                BladHasloTextBlox.Visibility = Visibility.Hidden;
            }
            if (email != powtorzEmail)
            {
                BladEmailTextBox.Visibility = Visibility.Visible;
            }
            else if (email == powtorzEmail)
            {
                BladEmailTextBox.Visibility = Visibility.Hidden;
            }
            if (RegulaminCheckBox.IsChecked != true)
            {
                BladRegulaminTextBlock.Visibility = Visibility.Visible;
            }
            else if (RegulaminCheckBox.IsChecked == true)
            {
                BladRegulaminTextBlock.Visibility = Visibility.Hidden;
            }

            if (email == powtorzEmail && haslo == powtorzHaslo && RegulaminCheckBox.IsChecked == true)
            {
                haslo = Sha1Hash(haslo);
                SqlConnection conn;
                string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
                using (conn = new SqlConnection(connetionString))
                {
                    string query = "INSERT INTO Uzytkownicy (Imie, Nazwisko, Email, Haslo) VALUES (@imie, @nazwisko, @email, @haslo)";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@imie", imie);
                        command.Parameters.AddWithValue("@nazwisko", nazwisko);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@haslo", haslo);
                        conn.Open();
                        int result = command.ExecuteNonQuery();

                        //sprawdzanie polaczenia
                        if (result < 0)
                        {
                            MessageBox.Show("Błąd w tworzeniu konta. Spróbuj jeszcze raz.");
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie zarejestrowano konto.");
                        }
                    }
                }
                conn.Close();
                LoginWindow loginWindow = new LoginWindow();
                this.Close();
                loginWindow.Show();
            }
        }

        private void ButtonPowrot_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
        }


        private void TextBox_GotFocus_Imie(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus_Imie;
        }

        private void TextBox_LostFocus_Imie(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = (string)FindResource("TextBoxText1");
                box.GotFocus += TextBox_GotFocus_Imie;
            }
        }

        private void TextBox_GotFocus_Nazwisko(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus_Nazwisko;
        }

        private void TextBox_LostFocus_Nazwisko(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = (string)FindResource("TextBoxText2");
                box.GotFocus += TextBox_GotFocus_Nazwisko;
            }
        }

        private void TextBox_GotFocus_Email(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus_Email;
        }

        private void TextBox_LostFocus_Email(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = "E-mail";
                box.GotFocus += TextBox_GotFocus_Email;
            }
        }

        private void TextBox_GotFocus_PowtorzEmail(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus_PowtorzEmail;
        }

        private void TextBox_LostFocus_PowtorzEmail(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = (string)FindResource("TextBoxText3");
                box.GotFocus += TextBox_GotFocus_PowtorzEmail;
            }
        }

        private void TextBlock_GotFocus_Haslo(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBlock_GotFocus_Haslo;
        }

        private void TextBlock_LostFocus_Haslo(Object sender, RoutedEventArgs e)
        {
            TextBlock box = sender as TextBlock;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.Text = "Haslo";
                box.GotFocus += TextBox_GotFocus_PowtorzEmail;
            }
        }
        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HasloPasswordBox.Focus();
            textPassword.Visibility = Visibility.Collapsed;

        }
        
        private void textPassword_MouseUp(object sender, MouseButtonEventArgs e)
        {
            textPassword.Visibility = Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            
                
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
        
        private void TextPasswordRepeat_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextPasswordRepeat.Visibility = Visibility.Visible;
        }
        private void TextBlock_GotFocus_PowtorzHaslo(object sender, RoutedEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBlock_GotFocus_Haslo;
            TextPasswordRepeat.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_LostFocus_PowtorzHaslo(Object sender, RoutedEventArgs e)
        {
            TextBlock box = sender as TextBlock;
            if (box.Text.Trim().Equals(string.Empty))
            {
                box.GotFocus += TextBox_GotFocus_PowtorzEmail;
                TextPasswordRepeat.Visibility = Visibility.Visible;
            }
        }


        private void TextPasswordRepeat_MouseDown(object sender, MouseEventArgs e)
        {
            PowtorzHasloPasswordBox.Focus();
        }

        private void PowtorzHasloPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
          
        }


        private void PasswordBox_GotFocus_PowtorzHaslo(object sender, RoutedEventArgs e)
        {
            PasswordBox tb = (PasswordBox)sender;
            tb.Password = string.Empty;
            tb.GotFocus -= PasswordBox_GotFocus_PowtorzHaslo;
            TextPasswordRepeat.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus_PowtorzHaslo(object sender, RoutedEventArgs e)
        {
            PasswordBox? box = sender as PasswordBox;
            if (box.Password.Trim().Equals(string.Empty))
            {
                box.GotFocus += PasswordBox_GotFocus_PowtorzHaslo;
                TextPasswordRepeat.Visibility = Visibility.Visible;
            }
        }

        private void RegulaminCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void HasloTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RegulaminLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            RegulaminLabel.Foreground = Brushes.Black;
            string lang = Thread.CurrentThread.CurrentUICulture.Name;
            string folderAplikacji = AppDomain.CurrentDomain.BaseDirectory;
            string sciezkaDoPliku = folderAplikacji + $"/RegulaminApki-{lang}.txt";
            TekstRegulaminu.Text = System.IO.File.ReadAllText(sciezkaDoPliku);
            Mouse.OverrideCursor = null;
            Regulamin.Visibility = Visibility.Visible;
            RegulaminRoot.Visibility = Visibility.Collapsed;

        }

        private void RegulaminLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            RegulaminLabel.Foreground = Brushes.Red;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void RegulaminLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            RegulaminLabel.Foreground = (Brush)FindResource("Text");
            Mouse.OverrideCursor = null;
        }

        private void ButtonPowrotZRegulaminu_Click(object sender, RoutedEventArgs e)
        {
            RegulaminRoot.Visibility = Visibility.Visible;
            Regulamin.Visibility = Visibility.Collapsed;
            BladHasloTextBlox.Visibility = Visibility.Hidden;
            BladEmailTextBox.Visibility = Visibility.Hidden;
            BladRegulaminTextBlock.Visibility = Visibility.Hidden;

        }
    }
}

