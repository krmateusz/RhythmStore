using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;

namespace WpfApp3
{

    public partial class MainWindow : Window
    {
        float saldo = getBalance.userBalance;
        private int tabIndex = 0;
        public MainWindow()
        {
            InitializeComponent();
            WypelnijComboBoxGatunek();
            WypelnijComboBoxWykonawca();
            WypelnijComboBoxSklep();
            WypelnijDataGridUsun();
            WypelnijDataGridSklep();
            WypelnijDataGridZakupy();
            WypelnijDataGridAdmin();
            tb_saldo.Text = saldo.ToString() + " zł";
            if (czyAdmin.admin != 0)
            {
                a_usun.Visibility = Visibility.Visible;
                a_dodaj.Visibility = Visibility.Visible;
                a_zarzadzaj.Visibility = Visibility.Visible;
            }
            InitializeKeyBindings();
            
        }
        // dodawanie skrotow klawiszowych
        private void InitializeKeyBindings()
        {
            // ctrl + 1 
            KeyGesture keyGestureCtrl1 = new KeyGesture(Key.D1, ModifierKeys.Control);
            InputBinding inputBindingCtrl1 = new InputBinding(MyCommands.Ctrl1Command, keyGestureCtrl1);
            CommandBindings.Add(new CommandBinding(MyCommands.Ctrl1Command, Ctrl1));
            InputBindings.Add(inputBindingCtrl1);

            // ctrl + 2
            KeyGesture keyGestureCtrl2 = new KeyGesture(Key.D2, ModifierKeys.Control);
            InputBinding inputBindingCtrl2 = new InputBinding(MyCommands.Ctrl2Command, keyGestureCtrl2);
            CommandBindings.Add(new CommandBinding(MyCommands.Ctrl2Command, Ctrl2));
            InputBindings.Add(inputBindingCtrl2);

            // ctrl+3
            KeyGesture keyGestureCtrl3 = new KeyGesture(Key.D3, ModifierKeys.Control);
            InputBinding inputBindingCtrl3 = new InputBinding(MyCommands.Ctrl3Command, keyGestureCtrl3);
            CommandBindings.Add(new CommandBinding(MyCommands.Ctrl3Command, Ctrl3));
            InputBindings.Add(inputBindingCtrl3);

            // ctrl+4
            KeyGesture keyGestureCtrl4 = new KeyGesture(Key.D4, ModifierKeys.Control);
            InputBinding inputBindingCtrl4 = new InputBinding(MyCommands.Ctrl4Command, keyGestureCtrl4);
            CommandBindings.Add(new CommandBinding(MyCommands.Ctrl4Command, Ctrl4));
            InputBindings.Add(inputBindingCtrl4);
        }

        public static class MyCommands
        {
            public static RoutedUICommand Ctrl1Command = new RoutedUICommand("Ctrl1Command", "Ctrl1Command", typeof(MainWindow));
            public static RoutedUICommand Ctrl2Command = new RoutedUICommand("Ctrl2Command", "Ctrl2Command", typeof(MainWindow));
            public static RoutedUICommand Ctrl3Command = new RoutedUICommand("Ctrl3Command", "Ctrl3Command", typeof(MainWindow));
            public static RoutedUICommand Ctrl4Command = new RoutedUICommand("Ctrl4Command", "Ctrl4Command", typeof(MainWindow));
        }


        private void Ctrl1(object sender, ExecutedRoutedEventArgs e)
        {
            ButtonSklep_Click(sender, e);
        }

        private void Ctrl2(object sender, ExecutedRoutedEventArgs e)
        {
            ButtonMojeZakupy_Click(sender, e);
        }

        private void Ctrl3(object sender, ExecutedRoutedEventArgs e)
        {
            Button_ClickKonto(sender, e);
        }

        private void Ctrl4(object sender, ExecutedRoutedEventArgs e)
        {
            Button_Click_Wyloguj(sender, e);
        }

        private SqlConnection connection = new SqlConnection(@"Data Source=x;Initial Catalog=x;Integrated Security=x;");

        private void WypelnijDataGridUsun()
        {
            try
            {
                connection.Open();

                string query = "SELECT * FROM Plyta";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dg_usun.ItemsSource = dataTable.DefaultView;
             
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych z bazy danych: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void WypelnijDataGridAdmin()
        {
            try
            {
                connection.Open();

                string query = "SELECT ID, Imie, Nazwisko, Email, Admin FROM Uzytkownicy WHERE Admin=0";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dg_admin.ItemsSource = dataTable.DefaultView;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych z bazy danych: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }



        private void WypelnijDataGridSklep()
        {
            try
            {
                connection.Open();

                string query = "SELECT p.Tytul, w.Wykonawca AS Wykonawca, g.Gatunek AS Gatunek, p.Utwory, p.Dlugosc, p.Cena AS \"Cena (PLN)\" FROM Plyta p, Wykonawcy w, Gatunki g WHERE p.Wykonawca_ID=w.ID AND p.Gatunek_ID=g.ID";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dg_sklep.ItemsSource = dataTable.DefaultView;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych z bazy danych: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void WypelnijDataGridZakupy()
        {
            int userID = getID.userID;
            try
            {
                connection.Open();

                string query = "SELECT p.Tytul AS Plyta, t.Suma AS \"Cena (PLN)\", t.Data AS \"Data zakupu\" FROM Plyta p, Transakcja t WHERE t.Plyta_ID=p.ID AND t.Uzytkownik_ID=@ID";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ID", userID);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dg_zakupy.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych z bazy danych: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void HideAll()
        {
            KontoGrid.Visibility = Visibility.Hidden;
            dodawanie_a.Visibility = Visibility.Hidden;
            a_usun_grid.Visibility = Visibility.Hidden;
            sklep_grid.Visibility = Visibility.Hidden;
            zakupy_grid.Visibility = Visibility.Hidden;
            zarzadzaj_grid.Visibility = Visibility.Hidden;
        }

        private void updateBalance()
        {
            int id = getID.userID;
            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "SELECT Stan_konta FROM Uzytkownicy WHERE ID=@ID";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    float result = Convert.ToSingle(command.ExecuteScalar());
                    tb_saldo.Text = result.ToString() + " zł";
                }
            }
            conn.Close();
        }

        private void WypelnijComboBoxGatunek()
        {
            ObservableCollection<ElementListyGatunek> elementy = PobierzGatunkiZBazyDanych();
            cb_gat.ItemsSource = elementy;
        }

        private ObservableCollection<ElementListyGatunek> PobierzGatunkiZBazyDanych()
        {
            string connectionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            ObservableCollection<ElementListyGatunek> elementy = new ObservableCollection<ElementListyGatunek>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Gatunek FROM Gatunki";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["ID"];
                            string gatunek = reader["Gatunek"].ToString();
                            elementy.Add(new ElementListyGatunek { ID = id, Gatunek = gatunek });
                        }
                    }
                }
            }

            return elementy;
        }

        SqlConnection con = new SqlConnection(@"Data Source=x;Initial Catalog=x;Integrated Security=x;");

        public class ElementListyGatunek
        {
            public int ID { get; set; }
            public string Gatunek { get; set; }

            public override string ToString()
            {
                return Gatunek;
            }
        }

        //teraz to samo tylko dla wykonawcy

        private void WypelnijComboBoxWykonawca()
        {
            ObservableCollection<ElementListyWykonawca> elementy = PobierzWykonawcowZBazyDanych();
            cb_wyk.ItemsSource = elementy;
        }

        private ObservableCollection<ElementListyWykonawca> PobierzWykonawcowZBazyDanych()
        {
            string connectionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            ObservableCollection<ElementListyWykonawca> elementy = new ObservableCollection<ElementListyWykonawca>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Wykonawca FROM Wykonawcy";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["ID"];
                            string wykonawca = reader["Wykonawca"].ToString();
                            elementy.Add(new ElementListyWykonawca { ID = id, Wykonawca = wykonawca });
                        }
                    }
                }
            }

            return elementy;
        }

        public class ElementListyWykonawca
        {
            public int ID { get; set; }
            public string Wykonawca { get; set; }

            public override string ToString()
            {
                return Wykonawca;
            }
        }

        private void Button_Click_Wyloguj(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void a_usun_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            a_usun_grid.Visibility = Visibility.Visible;
        }

        private void dod_gat_Click(object sender, RoutedEventArgs e)
        {
            string gatunek = tb_gatunek.Text;
            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x; ";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "INSERT INTO Gatunki (Gatunek) VALUES (@gatunek)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@gatunek", gatunek);
                    conn.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 0)
                    {
                        MessageBox.Show("Wystąpił bład. Spróbuj jeszcze raz.");
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie dodano wartość.");
                    }
                }
            }
            conn.Close();
        }

        private void dod_wyk_Click(object sender, RoutedEventArgs e)
        {
            string wykonawca = tb_wykonawca.Text;
            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "INSERT INTO Wykonawcy (Wykonawca) VALUES (@wykonawca)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@wykonawca", wykonawca);
                    conn.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 0)
                    {
                        MessageBox.Show("Wystąpił bład. Spróbuj jeszcze raz.");
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie dodano wartość.");
                    }
                }
            }
            conn.Close();
        }


        private void TextBoxImieKonto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_ClickKonto(object sender, RoutedEventArgs e)
        {
            HideAll();
            KontoGrid.Visibility = Visibility.Visible;
            int ID = getID.userID;
            con.Open();
            SqlCommand command = new SqlCommand("SELECT Imie, Nazwisko, Email, Miejscowosc, Ulica, Nr_domu, Nr_lokalu FROM Uzytkownicy WHERE ID = @ID", con);
            command.Parameters.AddWithValue("@ID", ID);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                TextBoxImieKonto.Text = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                TextBoxNazwiskoKonto.Text = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                TextBoxEmailKonto.Text = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                TextBoxMiejscowoscKonto.Text = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                TextBoxUlicaKonto.Text = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                TextBoxNrDomuKonto.Text = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                TextBoxNrLokaluKonto.Text = reader.IsDBNull(6) ? string.Empty : reader.GetInt32(6).ToString();
            }
            con.Close();


        }

        private void ButtonZatwierdz_Click(object sender, RoutedEventArgs e)
        {
            string imie = TextBoxImieKonto.Text;
            string nazwisko = TextBoxNazwiskoKonto.Text;
            string email = TextBoxEmailKonto.Text;
            string miejscowosc = TextBoxMiejscowoscKonto.Text;
            string ulica = TextBoxUlicaKonto.Text;
            string nrlokalu = TextBoxNrLokaluKonto.Text;
            string nrdomu = TextBoxNrDomuKonto.Text;
            int ID = getID.userID;
            con.Open();
            SqlCommand command = new SqlCommand("UPDATE Uzytkownicy SET Imie = @imie, Nazwisko = @nazwisko, Email = @email, Miejscowosc = @miejscowosc, Ulica = @ulica, Nr_domu = @nrdomu, Nr_lokalu = @nrlokalu WHERE ID = @ID", con);
            {
                command.Parameters.AddWithValue("@imie", imie);
                command.Parameters.AddWithValue("@nazwisko", nazwisko);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@miejscowosc", miejscowosc);
                command.Parameters.AddWithValue("@ulica", ulica);
                command.Parameters.AddWithValue("@nrdomu", nrdomu);
                command.Parameters.AddWithValue("@nrlokalu", nrlokalu);
                command.Parameters.AddWithValue("@ID", ID);

                int a = command.ExecuteNonQuery();
            }


            con.Close();
        }
            private void dodaj_plyte_Click(object sender, RoutedEventArgs e)
            {
                string tytul = Tytul.Text;
                int wykonawca = cb_wyk.SelectedIndex;
                wykonawca = wykonawca + 1;
                int gatunek = cb_gat.SelectedIndex;
                gatunek = gatunek + 1;
                string utwory = Utwory.Text;
                string dlugosc = Dlugosc.Text;
                string cena_s = Cena.Text;
                int cena = int.Parse(cena_s);

                SqlConnection conn;
                string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
                using (conn = new SqlConnection(connetionString))
                {
                    string query = "INSERT INTO Plyta (Tytul, Wykonawca_ID, Gatunek_ID, Utwory, Dlugosc, Cena) VALUES (@tytul, @wykonawca, @gatunek, @utwory, @dlugosc, @cena)";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@tytul", tytul);
                        command.Parameters.AddWithValue("@wykonawca", wykonawca);
                        command.Parameters.AddWithValue("@gatunek", gatunek);
                        command.Parameters.AddWithValue("@utwory", utwory);
                        command.Parameters.AddWithValue("@dlugosc", dlugosc);
                        command.Parameters.AddWithValue("@cena", cena);
                        conn.Open();
                        int result = command.ExecuteNonQuery();

                        if (result < 0)
                        {
                            MessageBox.Show("Wystąpił bład. Spróbuj jeszcze raz.");
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie dodano wartość.");
                        }
                    }
                }
                conn.Close();
                WypelnijDataGridSklep();
                WypelnijComboBoxSklep();
            }

            private void a_dodaj_Click(object sender, RoutedEventArgs e)
            {
                HideAll();
                dodawanie_a.Visibility = Visibility.Visible;
            }

        private void dg_usun_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void usun_utwor_Click(object sender, RoutedEventArgs e)
        {
            string id = usun_id.Text;
            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "DELETE FROM Plyta WHERE ID = @id";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    int result = command.ExecuteNonQuery();

                    if (result < 0)
                    {
                        MessageBox.Show("Wystąpił bład. Sprawdź parametry i spróbuj ponownie.");
                    }
                    else
                    {
                        MessageBox.Show("Pomyślnie usunięto wartość.");
                    }
                }
            }
            conn.Close();
        }

        private void usun_odswiez_Click(object sender, RoutedEventArgs e)
        {
            WypelnijDataGridUsun();
        }

        private void dodaj_odswiez_Click(object sender, RoutedEventArgs e)
        {
            WypelnijComboBoxGatunek();
            WypelnijComboBoxWykonawca();
        }

        private void ButtonMotywJasny(object sender, RoutedEventArgs e)
        {
            AppThemes.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
        }

        private void ButtonMotywCiemny(object sender, RoutedEventArgs e)
        {
            AppThemes.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
        }

        private void WypelnijComboBoxSklep()
        {
            ObservableCollection<ElementListySklep> elementy = PobierzElementyZBazyDanych();
            cb_plyta.ItemsSource = elementy;
        }

        private ObservableCollection<ElementListySklep> PobierzElementyZBazyDanych()
        {
            string connectionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            ObservableCollection<ElementListySklep> elementy = new ObservableCollection<ElementListySklep>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Tytul FROM Plyta";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["ID"];
                            string tytul = reader["Tytul"].ToString();
                            elementy.Add(new ElementListySklep { ID = id, Tytul = tytul });
                        }
                    }
                }
            }

            return elementy;
        }

        public class ElementListySklep
        {
            public int ID { get; set; }
            public string Tytul { get; set; }

            public override string ToString()
            {
                return Tytul;
            }
        }

        private void kup_button_Click(object sender, RoutedEventArgs e)
        {
            int userID = getID.userID;
            ElementListySklep wybranyElement = (ElementListySklep)cb_plyta.SelectedItem;
            int plytaID = wybranyElement.ID;
            float cena = 0;

            SqlConnection conn;
            string connetionString = @"Data Source=x;Initial Catalog=x;Integrated Security=x;";
            using (conn = new SqlConnection(connetionString))
            {
                string query = "SELECT Cena FROM Plyta WHERE ID = @ID";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ID", plytaID);
                    conn.Open();
                    float result = Convert.ToSingle(command.ExecuteScalar());
                    cena = result;
                }

                if(saldo >= cena)
                {
                    saldo = saldo - cena;
                    string query2 = "UPDATE Uzytkownicy SET Stan_konta = @saldo WHERE ID = @ID";
                    using (SqlCommand command2 = new SqlCommand(query2, conn))
                    {
                        command2.Parameters.AddWithValue("@ID", userID);
                        command2.Parameters.AddWithValue("@saldo", saldo);
                        int result2 = command2.ExecuteNonQuery();
                    }

                    string query3 = "INSERT INTO Transakcja (Uzytkownik_ID, Plyta_ID, Suma) VALUES (@uID, @pID, @suma)";
                    using (SqlCommand command3 = new SqlCommand(query3, conn))
                    {
                        command3.Parameters.AddWithValue("@uID", userID);
                        command3.Parameters.AddWithValue("@pID", plytaID);
                        command3.Parameters.AddWithValue("@suma", cena);
                        int result3 = command3.ExecuteNonQuery();
                        if (result3 < 0)
                        {
                            MessageBox.Show("Wystąpił bład. Spróbuj jeszcze raz.");
                        }
                        else
                        {
                            MessageBox.Show("Pomyślnie zakupiono płytę.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Brak wystarczających środków na koncie.");
                }
            }
            WypelnijDataGridZakupy();
            updateBalance();
        }

        private void ButtonSklep_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            sklep_grid.Visibility = Visibility.Visible;
        }

        private void ButtonMojeZakupy_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            zakupy_grid.Visibility = Visibility.Visible;
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
            UpdateDataGridHeaders();
        }

        private void ButtonLang_Click(object sender, RoutedEventArgs e)
        {
            SetLang(((Button)sender).Tag.ToString());
        }

        private static Dictionary<string, string> headerTranslations = new Dictionary<string, string>
        {
             { "Tytul", "Title" },
             { "Wykonawca", "Artist" },
             { "Gatunek", "Genre" },
             { "Utwory", "Songs" },
             { "Dlugosc", "Lenght" },
             { "Cena (PLN)", "Price (PLN)" },
             { "Data zakupu", "Date of purchase" },
             { "Plyta", "Disc" },
             { "Wykonawca_ID", "Artist_ID" },
             { "Gatunek_ID", "Genre_ID" },
             { "Cena", "Price" },
             { "Imie", "First name" },
             { "Nazwisko", "Last name" },


             { "Title", "Tytul" },
             { "Artist", "Wykonawca" },
             { "Genre", "Gatunek" },
             { "Songs", "Utwory" },
             { "Lenght", "Dlugosc" },
             { "Price", "Cena" },
             { "Date of purchase", "Data zakupu" },
             { "Disc", "Plyta" },
             { "Artist_ID", "Wykonawca_ID" },
             { "Genre_ID", "Gatunek_ID" },
             { "Price (PLN)", "Cena (PLN)" },
             { "First name", "Imie" },
             { "Last name", "Nazwisko" },

        };
        private void UpdateDataGridHeaders()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(dg_sklep.ItemsSource);
            foreach (DataGridColumn column in dg_zakupy.Columns)
            {
                string headerKey = column.Header as string;
                if (!string.IsNullOrEmpty(headerKey) && headerTranslations.ContainsKey(headerKey))
                {
                    column.Header = headerTranslations[headerKey];
                }
                else
                {
                    Debug.WriteLine($"Brak tłumaczenia dla nagłówka: {headerKey}");
                }
            }
            foreach (DataGridColumn column in dg_sklep.Columns)
            {
                string headerKey = column.Header as string;
                if (!string.IsNullOrEmpty(headerKey) && headerTranslations.ContainsKey(headerKey))
                {
                    column.Header = headerTranslations[headerKey];
                }
                else
                {
                    Debug.WriteLine($"Brak tłumaczenia dla nagłówka: {headerKey}");
                }
            }
            foreach (DataGridColumn column in dg_usun.Columns)
            {
                string headerKey = column.Header as string;
                if (!string.IsNullOrEmpty(headerKey) && headerTranslations.ContainsKey(headerKey))
                {
                    column.Header = headerTranslations[headerKey];
                }
                else
                {
                    Debug.WriteLine($"Brak tłumaczenia dla nagłówka: {headerKey}");
                }
            }
            foreach (DataGridColumn column in dg_admin.Columns)
            {
                string headerKey = column.Header as string;
                if (!string.IsNullOrEmpty(headerKey) && headerTranslations.ContainsKey(headerKey))
                {
                    column.Header = headerTranslations[headerKey];
                }
                else
                {
                    Debug.WriteLine($"Brak tłumaczenia dla nagłówka: {headerKey}");
                }
            }
            view.Refresh();
        }

        private void button_zmien_nazwe_Click(object sender, RoutedEventArgs e)
        {
            string nNazwa = tb_nazwa.Text;
            nazwa.Text = nNazwa;
        }

        private void a_zarzadzaj_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            zarzadzaj_grid.Visibility = Visibility.Visible;
        }

        private void button_dodaj_admina_Click(object sender, RoutedEventArgs e)
        {
            string strID = tb_admin.Text;
            int id = int.Parse(strID);
            con.Open();
            SqlCommand command = new SqlCommand("UPDATE Uzytkownicy SET Admin=1 WHERE ID = @ID", con);
            {
                command.Parameters.AddWithValue("@ID", id);
                int result = command.ExecuteNonQuery();
                if(result < 0)
                {
                    MessageBox.Show("Wystąpił bład. Spróbuj jeszcze raz.");
                }
                else
                {
                    MessageBox.Show("Pomyślnie awansowano administratora.");
                }
            }
            con.Close();
        }

        private void Tytul_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}