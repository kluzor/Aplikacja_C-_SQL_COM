using System;
using System.Data;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.Odbc;

namespace programCOM
{
    public partial class programCOM : Form
    {
        public OdbcConnection OdbcConnect;
        // Deklaracja zmiennych globalnych

        char[] tab_imie = new char[20];
        char[] tab_nazwisko = new char[20];
        char[] tab_miasto = new char[20];
        char[] tab_uczelnia = new char[20];

        char[] seria = new char[10];
        public int seria_int = 0;
        public int seria_int_p = 0;

        char[] liczba_bialych = new char[10];
        char[] liczba_czarnych = new char[10];
        char[] ile_krazkow = new char[10];

        string czas_rozpoczecia;
        string czas_zakonczenia;

        char[] godzina_rozpoczecia = new char[10];
        char[] minuta_rozpoczecia = new char[10];
        char[] sekunda_rozpoczecia = new char[10];

        char[] godzina_zakonczenia = new char[10];
        char[] minuta_zakonczenia = new char[10];
        char[] sekunda_zakonczenia = new char[10];

        System.IO.Ports.SerialPort port;
        delegate void Delegat1();
        Delegat1 moj_del1;
        //public MySqlConnection polaczenie;

        public void Loguj()
        {
            try
            {
                var connectionString = @"DSN=PLC_SQL;DatabaseName=PLC_DB;UID=PLC;PWD=plc_pwd;";
                OdbcConnect = new OdbcConnection(connectionString);
                OdbcConnect.Open();
                MessageBox.Show("Nawiązano połączenie z bazą danych", "OK");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Brak połączenia z bazą danych\n" + exc.Message, "Błąd");
            }

            /*
            //Funkcja pobierająca dane logowania z formularza i przypisująca je do zmiennej 'połączenie'
            string mojePol =
            "SERVER=" + nazwaServeratb.Text + ";" +
            "DATABASE=" + nazwaBazyDanychtb.Text + ";" +
            "UID=" + uzytkowniktb.Text + ";" +
            "PASSWORD=" + haslotb.Text + ";";
            polaczenie = new MySqlConnection(mojePol);
            polaczenie.Open();
            */
        }

        public static string Caesar(string value, int shift)
        {
            char[] buffer = value.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                char letter = buffer[i];

                letter = (char)(letter + shift);

                if (letter > 'z')
                {
                    letter = (char)(letter - 26);
                }

                else if (letter < 'a')
                {
                    letter = (char)(letter + 26);
                }

                buffer[i] = letter;
            }
            return new string(buffer);
        }

        /*
                public static string caesar_decipher(int key, string ct)
                {
                    int size = ct.Length;
                    char[] value = new char[size];
                    char[] cipher = new char[size];
                    for (int r = 0; r < size; r++)
                    {
                        cipher[r] = Convert.ToChar(ct.Substring(r, 1));
                    }

                    for (int re = 0; re < size; re++)
                    {
                        int count = 0;
                        int a = Convert.ToInt32(cipher[re]);
                        for (int y = 1; y <= key; y++)
                        {
                            if (count == 0)
                            {
                                if (a == 65)
                                { a = 91; }
                                else if (a == 97)
                                { a = 123; }
                                value[re] = Convert.ToChar(a - y);
                                count++;
                            }
                            else
                            {
                                int b = Convert.ToInt32(value[re]);
                                if (b == 65)
                                { b = 91; }
                                else if (b == 97)
                                { b = 123; }
                                value[re] = Convert.ToChar(b - 1);

                            }
                        }
                    }
                    string plaintext = "";

                    for (int p = 0; p < size; p++)
                    {
                        plaintext = plaintext + value[p].ToString();
                    }

                    return plaintext.ToLower();
                }
        */
        public programCOM()
        {
            InitializeComponent();
            //inicjalizacja zmiennej port z domyślnymi wartościami
            port = new SerialPort();
            //ustawienie timeoutów aby program się nie wieszał
            port.ReadTimeout = 100;
            port.WriteTimeout = 100;
            Opcje.Enter += new EventHandler(Opcje_Enter);
            port.DataReceived += new SerialDataReceivedEventHandler(DataRecievedHandler);
            moj_del1 = new Delegat1(WpiszOdebrane);
        }

        private void DataRecievedHandler(object sender, SerialDataReceivedEventArgs e)

        {
            rtbTerminal.Invoke(moj_del1);
        }

        private void WpiszOdebrane()
        {
            //uruchomienie timera do automatycznego wysyłania danych z COM do bazy MySQL wraz z pojawieniem się nowej zmiennej po wyczyszczeniu portu COMN
            timer1.Start();
            DodajKolorowy(rtbTerminal, Convert.ToChar(port.ReadByte()) + "", System.Drawing.Color.Blue);
        }

        private void DodajKolorowy(System.Windows.Forms.RichTextBox RichTextBox, string Text, System.Drawing.Color Color)
        {
            var StartIndex = RichTextBox.TextLength;
            RichTextBox.AppendText(Text);
            var EndIndex = RichTextBox.TextLength;
            RichTextBox.Select(StartIndex, EndIndex - StartIndex);
            RichTextBox.SelectionColor = Color;
        }

        void Opcje_Enter(object sender, EventArgs e)
        {
            //aktualizacja list
            this.cbName.Items.Clear();
            this.cbParity.Items.Clear();
            this.cbStop.Items.Clear();
            foreach (String s in SerialPort.GetPortNames()) this.cbName.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(Parity))) this.cbParity.Items.Add(s);
            foreach (String s in Enum.GetNames(typeof(StopBits))) this.cbStop.Items.Add(s);

            //aktualizacja nazw
            cbName.Text = port.PortName.ToString();
            cbBaud.Text = port.BaudRate.ToString();
            cbData.Text = port.DataBits.ToString();
            cbParity.Text = port.Parity.ToString();
            cbStop.Text = port.StopBits.ToString();
        }

        private void butSend_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                DodajKolorowy(rtbTerminal, ((Int32)numericSend.Value).ToString("X") + "", System.Drawing.Color.Black);
                Byte[] tosend = { (Byte)numericSend.Value };
                port.Write(tosend, 0, 1);
            }
            else System.Windows.Forms.MessageBox.Show("Aby wysłać bajt musisz ustanowić połączenie");
        }

        //Metoda ustawiająca domyślne wartości komunikacji UART 
        private void butDomyslne_Click(object sender, EventArgs e)
        {
            this.cbName.Text = "COM1";
            this.cbBaud.Text = "9600";
            this.cbData.Text = "8";
            this.cbParity.Text = "None";
            this.cbStop.Text = "One";
        }

        private void pbStatus_Click(object sender, EventArgs e)
        {
            //jeżeli połączenie jest aktywne to je kończymy, zmieniamy kolor na red i zmieniamy napis
            if (port.IsOpen)
            {
                pbStatus.BackColor = System.Drawing.Color.Red;
                port.Close();
                labStatus.Text = "Brak połączenia (teraz można zmieniać opcje połączenia)";
                DodajKolorowy(rtbTerminal, "\nZakończono połączenie z " + port.PortName + "\n", System.Drawing.Color.Orange);
                timer1.Stop();
            }
            //w przeciwnym wypadku włączamy połączenie, zmieniamy kolor na zielony i zmieniamy napis
            else
            {
                //połączenie może nie być możliwe dlatego należy się zabezpieczyć na wypadek błędu
                try
                {
                    //najpierw przepisujemy do portu parametry z opcji
                    port.PortName = this.cbName.Text;
                    port.BaudRate = Int32.Parse(this.cbBaud.Text);
                    port.DataBits = Int32.Parse(this.cbData.Text);
                    port.Parity = (Parity)Enum.Parse(typeof(Parity), this.cbParity.Text);
                    port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.cbStop.Text);
                    //a następnie uruchamiamy port
                    port.Open();
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    //po uruchomieniu zmieniamy elementy graficzne interfejsu
                    pbStatus.BackColor = System.Drawing.Color.Green;
                    labStatus.Text = "Aktywne połączenie (port:" + port.PortName.ToString() + ", prędkość: " + port.BaudRate.ToString() + ", bity danych: " +
                    port.DataBits.ToString() + "\n bity stopu: " + port.StopBits.ToString() + ", parzystość: " + port.Parity.ToString() + ")";
                    DodajKolorowy(rtbTerminal, "Rozpoczęto połączenie z " + port.PortName + "\n", System.Drawing.Color.Orange);
                }
                //jeżeli nastąpi błąd to go przechwycimy i wyświetlimy stosowny komunikat
                catch (Exception exc)
                {
                    MessageBox.Show("Błąd połączenia:\n" + exc.Message);
                }
            }
        }

        private void logowanieBtn_Click(object sender, EventArgs e)
        {
            pobierzDane();
        }

        //Metoda nawiązuję połączenie z bazą danych MySQL
        //jeżeli wystąpi błąd logowania pokaże się stosowany komunikat
        public void pobierzDane()
        {
            //pobierz dane logowania z formularza i przypisz
            string mojePolaczenie =
            "SERVER=" + nazwaServeratb.Text + ";" +
            "DATABASE=" + nazwaBazyDanychtb.Text + ";" +
            "UID=" + uzytkowniktb.Text + ";" +
            "PASSWORD=" + haslotb.Text + ";";

            //wykonaj polecenie języka SQL
            //string sql = "SELECT * FROM uzytkownicy";
            //string sql = "SELECT * FROM dane";
            //wyświetlenie testowanej tabeli
            /*
                        MySqlConnection polaczenie = new MySqlConnection(mojePolaczenie);
                        //blok try-catch przechwytuje błędy
                        try
                        {
                            //otwórz połączenie z bazą danych
                            polaczenie.Open();

                            //wykonaj polecenie języka SQL na danych połączeniu
                            using (MySqlCommand cmdSel = new MySqlCommand(sql, polaczenie))
                            {
                                DataTable dt = new DataTable();
                                //Pobierz dane i zapisz w strukturze DataTable
                                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                                da.Fill(dt);
                                //wpisz dane do kontrolki DATAGRID
                                dataGridView1.DataSource = dt.DefaultView;
                                MessageBox.Show("Zalogowano do bazy danych MySQL", "Informacja");
                            }
                        }
                        //Jeżeli wystąpi wyjątek wyrzuć go i pokaż informacje
                        catch (MySql.Data.MySqlClient.MySqlException)
                        {
                            MessageBox.Show("Błąd logowania do bazy danych MySQL", "Błąd");
                        }
                        //Zamknij połączenie po wyświetleniu danych
                        //polaczenie.Close();
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                //tworzenie nowego użytkownika, zapisywanie jego danych i nadawanie uprawnień
                Loguj();
                if (tHaslo.Text == tPhaslo.Text)
                {
                    MySqlCommand k =
                    new MySqlCommand("CREATE USER ?u @'127.0.0.1'IDENTIFIED BY ?m", polaczenie);
                    k.Parameters.Add(new MySqlParameter("u", tLogin.Text));
                    k.Parameters.Add(new MySqlParameter("m", tHaslo.Text));
                    k.ExecuteNonQuery();

                    MySqlCommand x =
                    new MySqlCommand("GRANT ALL PRIVILEGES ON *.* TO ?n @'127.0.0.1'WITH GRANT OPTION; ", polaczenie);
                    x.Parameters.Add(new MySqlParameter("n", tLogin.Text));
                    x.ExecuteNonQuery();

                    MySqlCommand d =
                    new MySqlCommand("INSERT INTO uzytkownik(login, haslo, email, imie, nazwisko) VALUES(?l,?h,?e,?i,?n)", polaczenie);
                    d.Parameters.Add(new MySqlParameter("l", tLogin.Text));
                    d.Parameters.Add(new MySqlParameter("h", tHaslo.Text));
                    d.Parameters.Add(new MySqlParameter("e", tEmail.Text));
                    d.Parameters.Add(new MySqlParameter("i", tImie.Text));
                    d.Parameters.Add(new MySqlParameter("n", tNazwisko.Text));
                    d.ExecuteNonQuery();
                }
                else
                {   
                    //Sprawdzanie zgodności podanych haseł
                    MessageBox.Show("Podane hasła róźnią sie od siebie.", "Błąd");
                }
            }

            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Brak połączenia z bazą danych MySql lub użytkownik już istnieje.", "Błąd");
            }
            */
        }

        private void butClear_Click(object sender, EventArgs e)
        {
            //Czyszczenie naszego terminala
            rtbTerminal.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            int j = 1;
            int t = 0;

            int b = 1;
            int d = 0;

            int w = 1;
            int r = 0;

            int y = 1;
            int u = 0;

            try
            {

                for (int q = 0; q < rtbTerminal.Text.Length; q = q + 1)
                {
                    if (rtbTerminal.Text[q] == '%')
                    {
                        for (i = q; i < rtbTerminal.Text.Length; i = i + 1)
                        {
                            if (rtbTerminal.Text[i] == '%')
                            {
                                while (rtbTerminal.Text[i + j] != '*')
                                {
                                    tab_imie[t] = rtbTerminal.Text[i + j];
                                    j = j + 1;
                                    t++;
                                }

                            }

                            if (rtbTerminal.Text[i] == ';')
                            {
                                while (rtbTerminal.Text[i + b] != ')')
                                {
                                    tab_nazwisko[d] = rtbTerminal.Text[i + b];
                                    b = b + 1;
                                    d++;
                                }
                            }

                            if (rtbTerminal.Text[i] == '?')
                            {
                                while (rtbTerminal.Text[i + w] != '=')
                                {
                                    tab_miasto[r] = rtbTerminal.Text[i + w];
                                    w = w + 1;
                                    r++;
                                }
                            }

                            if (rtbTerminal.Text[i] == '/')
                            {
                                while (rtbTerminal.Text[i + y] != '^')
                                {
                                    tab_uczelnia[u] = rtbTerminal.Text[i + y];
                                    y = y + 1;
                                    u++;
                                }

                                i = rtbTerminal.Text.Length;
                                q = rtbTerminal.Text.Length;

                                Loguj();

                                //Konwersja char na string

                                string imie_s = new string(tab_imie);
                                string nazwisko_s = new string(tab_nazwisko);
                                string miasto_s = new string(tab_miasto);
                                string uczelnia_s = new string(tab_uczelnia);

                                char[] charsToTrim = { '￡' };

                                //Deszyfrowanie   
                                // string imie = caesar_decipher(-5, imie_c);
                                // string nazwisko = caesar_decipher(-5, nazwisko_c);
                                // string miasto = caesar_decipher(-5, miasto_c);
                                // string uczelnia = caesar_decipher(-5, uczelnia_c);

                                string imie = Caesar(imie_s, -5);
                                string nazwisko = Caesar(nazwisko_s, -5);
                                string miasto = Caesar(miasto_s, -5);
                                string uczelnia = Caesar(uczelnia_s, -5);

                                string imie_t = imie.Trim(charsToTrim);
                                string nazwisko_t = nazwisko.Trim(charsToTrim);
                                string miasto_t = miasto.Trim(charsToTrim);
                                string uczelnia_t = uczelnia.Trim(charsToTrim);
                                /*
                                                                MySqlCommand c =
                                                                new MySqlCommand("INSERT INTO dane(imie,nazwisko,miasto,uczelnia)VALUES(?q,?w,?k,?z)", polaczenie);
                                                                c.Parameters.Add(new MySqlParameter("q", imie_t));
                                                                c.Parameters.Add(new MySqlParameter("w", nazwisko_t));
                                                                c.Parameters.Add(new MySqlParameter("k", miasto_t));
                                                                c.Parameters.Add(new MySqlParameter("z", uczelnia_t));
                                                                c.ExecuteNonQuery();

                                                                rtbTerminal.Text = "";
                                                                richTextBox2.Text = "" + imie_t + "";
                                */
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Nieoczekiway błąd aplikacji\n" + exc.Message, "Error");
            }
        }

        //Funkcja do wykonywania operacji SELECT w naszej bazie danych. Wysyła polecenie w języku MySQL, które jest wykonywane w bazie danych.
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT " + textBox1.Text + " FROM " + textBox2.Text;
                OdbcCommand select = new OdbcCommand(sql, OdbcConnect);
                select.ExecuteNonQuery();
                DataTable d = new DataTable();
                OdbcDataAdapter di = new OdbcDataAdapter(select);
                di.Fill(d);
                dataGridView1.DataSource = d.DefaultView;
            }

            catch (OdbcException exc)
            {
                MessageBox.Show("Brak połączenia z bazą danych lub niepoprawne polecenie selekcji.\n" + exc, "Błąd");
            }
        }

        //Najważniejsza funkcja naszego programu. Odpowiada za cykliczne przesłanie otrzymanych z sortownicy danych do bazy danych.
        // Przesyłanie pobranych zmiennych z portu COM do bazy MySQL realizowane jest co 8 sekund  
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Tworzenie zmiennych odpowiadających za indeksowanie tablic oraz pętle 'for'.
            int i = 0;
            int j = 4;
            int t = 0;

            int b = 4;
            int d = 0;

            int w = 4;
            int r = 0;

            int w1 = 4;
            int r1 = 0;

            int w2 = 4;
            int r2 = 0;

            int w3 = 4;
            int r3 = 0;

            int w4 = 4;
            int r4 = 0;

            int y = 4;
            int u = 0;

            int z = 4;
            int x = 0;

            int p = 4;
            int v = 0;

            // Otrzymany komunikat to cała porcja danych, którą po obróbce wpisujemy do bazy danych. Każda dana wysyłana jest między unikalnymi znakami początku i końca.
            // Znaki te to duże litery alfabetu, które są wysyłane bitowo, dla przykładu litera A w kodzie ASCII odpowiada liczbie 65 i ta liczba jest początkiem naszego komunikatu.
            // Gdy pojawi się liczba początku, program wpisuje do tablicy znakowej 'seria' odpowiadające jej dane. Po odczytaniu znaku końca, program oczekuje na znak początku następnej zmiennej.

            try
            {

                for (int q = 0; q < rtbTerminal.Text.Length; q = q + 1)
                {
                    if ((rtbTerminal.Text[q] == '6') && (rtbTerminal.Text[q + 1] == '5'))
                    {
                        for (i = q; i < rtbTerminal.Text.Length; i = i + 1)
                        {
                            if ((rtbTerminal.Text[i] == '6') && (rtbTerminal.Text[i + 1] == '5'))
                            {
                                while ((rtbTerminal.Text[i + j + 2] != '6') && (rtbTerminal.Text[i + j + 3] != '7'))
                                {
                                    seria[t] = rtbTerminal.Text[i + j];
                                    j = j + 1;
                                    t++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j] == '6') && (rtbTerminal.Text[i + j + 1] == '8'))
                            {
                                while ((rtbTerminal.Text[i + j + b + 2] != '6') && (rtbTerminal.Text[i + j + b + 3] != '9'))
                                {
                                    godzina_rozpoczecia[d] = rtbTerminal.Text[i + j + b];
                                    b = b + 1;
                                    d++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b] == '7') && (rtbTerminal.Text[i + j + b + 1] == '0'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + 2] != '7') && (rtbTerminal.Text[i + j + b + w + 3] != '1'))
                                {
                                    minuta_rozpoczecia[r] = rtbTerminal.Text[i + j + b + w];
                                    w = w + 1;
                                    r++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b + w] == '7') && (rtbTerminal.Text[i + j + b + w + 1] == '2'))
                            {
                                while (rtbTerminal.Text[i + j + b + w + w1 + 2] != '7' && rtbTerminal.Text[i + j + b + w + w1 + 3] != '3')
                                {
                                    sekunda_rozpoczecia[r1] = rtbTerminal.Text[i + j + b + w + w1];
                                    w1 = w1 + 1;
                                    r1++;
                                }

                            }
                            /// czas zakonczenia                        
                            if (rtbTerminal.Text[i + j + b + w + w1] == '7' && rtbTerminal.Text[i + j + b + w + w1 + 1] == '4')
                            {
                                while (rtbTerminal.Text[i + j + b + w + w1 + w2 + 2] != '7' && rtbTerminal.Text[i + j + b + w + w1 + w2 + 3] != '5')
                                {
                                    godzina_zakonczenia[r2] = rtbTerminal.Text[i + j + b + w + w1 + w2];
                                    w2 = w2 + 1;
                                    r2++;
                                }
                            }
                            //2 seria danych
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2] == '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + 1] == '6'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 2] != '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 3] != '8'))
                                {
                                    minuta_zakonczenia[r3] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3];
                                    w3 = w3 + 1;
                                    r3++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3] == '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 1] == '9'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 3] != '0'))
                                {
                                    sekunda_zakonczenia[r4] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4];
                                    w4 = w4 + 1;
                                    r4++;
                                }

                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 1] == '1'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 3] != '2'))
                                {
                                    liczba_bialych[u] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y];
                                    y = y + 1;
                                    u++;
                                }
                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 1] == '3'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 3] != '4'))
                                {
                                    liczba_czarnych[x] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z];
                                    z = z + 1;
                                    x++;
                                }
                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 1] == '5'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p + 3] != '6'))
                                {
                                    ile_krazkow[v] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p];
                                    p = p + 1;
                                    v++;
                                }

                                i = rtbTerminal.Text.Length;
                                q = rtbTerminal.Text.Length;

                                Loguj();

                                //W celu pomyślnego wpisania danych do bazy MySQL konieczna jest konwersja typów danych.

                                //Konwersja tablicy char do string

                                string seria_s = new string(seria);

                                string godzina_rozpoczecia_s = new string(godzina_rozpoczecia);
                                string minuta_rozpoczecia_s = new string(minuta_rozpoczecia);
                                string sekunda_rozpoczecia_s = new string(sekunda_rozpoczecia);

                                string godzina_zakonczenia_s = new string(godzina_zakonczenia);
                                string minuta_zakonczenia_s = new string(minuta_zakonczenia);
                                string sekunda_zakonczenia_s = new string(sekunda_zakonczenia);

                                string liczba_bialych_s = new string(liczba_bialych);
                                string liczba_czarnych_s = new string(liczba_czarnych);
                                string ile_krazkow_s = new string(ile_krazkow);

                                //Konwersja string do int

                                int seria_int = Int32.Parse(seria_s);

                                int godzina_rozpoczecia_int = Int32.Parse(godzina_rozpoczecia_s);
                                int minuta_rozpoczecia_int = Int32.Parse(minuta_rozpoczecia_s);
                                int sekunda_rozpoczecia_int = Int32.Parse(sekunda_rozpoczecia_s);

                                int godzina_zakonczenia_int = Int32.Parse(godzina_zakonczenia_s);
                                int minuta_zakonczenia_int = Int32.Parse(minuta_zakonczenia_s);
                                int sekunda_zakonczenia_int = Int32.Parse(sekunda_zakonczenia_s);

                                int liczba_bialych_int = Int32.Parse(liczba_bialych_s);
                                int liczba_czarnych_int = Int32.Parse(liczba_czarnych_s);
                                int ile_krazkow_int = Int32.Parse(ile_krazkow_s);

                                //Tworzenie zmiennej dt do której pobierana jest aktualna data w formacie "dzień/miesiąc/rok"

                                string dt = DateTime.Now.ToString("dd MM yyyy");

                                //Warunki dopisujące znak '0' na początku zmiennych reprezentujących minuty i sekundy mniejsze od 10.

                                if (minuta_rozpoczecia_int < 10 && sekunda_rozpoczecia_int >= 10)
                                {
                                    czas_rozpoczecia = godzina_rozpoczecia_int + ":0" + minuta_rozpoczecia_int + ":" + sekunda_rozpoczecia_int;
                                }
                                if (minuta_zakonczenia_int < 10 && sekunda_zakonczenia_int >= 10)
                                {
                                    czas_zakonczenia = godzina_zakonczenia_int + ":0" + minuta_zakonczenia_int + ":" + sekunda_zakonczenia_int;
                                }


                                if (sekunda_rozpoczecia_int < 10 && minuta_rozpoczecia_int >= 10)
                                {
                                    czas_rozpoczecia = godzina_rozpoczecia_int + ":" + minuta_rozpoczecia_int + ":0" + sekunda_rozpoczecia_int;
                                }
                                if (sekunda_zakonczenia_int < 10 && minuta_zakonczenia_int >= 10)
                                {
                                    czas_zakonczenia = godzina_zakonczenia_int + ":" + minuta_zakonczenia_int + ":0" + sekunda_zakonczenia_int;
                                }

                                if (minuta_rozpoczecia_int < 10 && sekunda_rozpoczecia_int < 10)
                                {
                                    czas_rozpoczecia = godzina_rozpoczecia_int + ":0" + minuta_rozpoczecia_int + ":0" + sekunda_rozpoczecia_int;
                                }
                                if (minuta_zakonczenia_int < 10 && sekunda_zakonczenia_int < 10)
                                {
                                    czas_zakonczenia = godzina_zakonczenia_int + ":0" + minuta_zakonczenia_int + ":0" + sekunda_zakonczenia_int;
                                }

                                if (minuta_rozpoczecia_int >= 10 && sekunda_rozpoczecia_int >= 10)
                                {
                                    czas_rozpoczecia = godzina_rozpoczecia_int + ":" + minuta_rozpoczecia_int + ":" + sekunda_rozpoczecia_int;
                                }
                                if (minuta_zakonczenia_int >= 10 && sekunda_zakonczenia_int >= 10)
                                {
                                    czas_zakonczenia = godzina_zakonczenia_int + ":" + minuta_zakonczenia_int + ":" + sekunda_zakonczenia_int;
                                }

                                // czas_rozpoczecia = godzina_rozpoczecia_int + ":" + minuta_rozpoczecia_int + ":" + sekunda_rozpoczecia_int;
                                // czas_zakonczenia = godzina_zakonczenia_int + ":" + minuta_zakonczenia_int + ":" + sekunda_zakonczenia_int;

                                richTextBox2.Text = "" + sekunda_zakonczenia_int + "";

                                // Z racji tego, że dane do bazy danych wysyłane są cyklicznie, dodano zabezpieczenie przeciwko tworzeniu nowych, takich samych krotek w bazie danych.
                                // Wprowadzono zmienną 'seria_int_p', która przechowuje informacje o poprzednim numerze serii. Jeśli numer serii aktualny jest różny od numeru poprzedniego,
                                // wpisywana jest nowa krotka za pomocą polecenia "INSERT INTO ... VALUES ..." (wpisywanie wartości do podanych kolumn danej tabeli) 

                                if (seria_int != seria_int_p)
                                {
                                    /*
                                                                        MySqlCommand c =
                                                                        new MySqlCommand("INSERT INTO seria(seria,czas_rozpoczecia,data_rozpoczecia,czas_zakonczenia,data_zakonczenia,login)VALUES(?q,?w,?k,?z,?j,?o)", polaczenie);
                                                                        c.Parameters.Add(new MySqlParameter("q", seria_int));
                                                                        c.Parameters.Add(new MySqlParameter("w", czas_rozpoczecia));
                                                                        c.Parameters.Add(new MySqlParameter("k", dt));
                                                                        c.Parameters.Add(new MySqlParameter("z", czas_zakonczenia));
                                                                        c.Parameters.Add(new MySqlParameter("j", dt));
                                                                        c.Parameters.Add(new MySqlParameter("o", uzytkowniktb.Text));
                                                                        c.ExecuteNonQuery();

                                                                        MySqlCommand h =
                                                                        new MySqlCommand("INSERT INTO dane_serii(seria, ile_krazkow, liczba_bialych, liczba_czarnych)VALUES(?v,?b,?n,?m)", polaczenie);
                                                                        h.Parameters.Add(new MySqlParameter("v", seria_int));
                                                                        h.Parameters.Add(new MySqlParameter("b", ile_krazkow));
                                                                        h.Parameters.Add(new MySqlParameter("n", liczba_bialych));
                                                                        h.Parameters.Add(new MySqlParameter("m", liczba_czarnych));
                                                                        h.ExecuteNonQuery();

                                                                        seria_int_p = seria_int;
                                                                        rtbTerminal.Text = "";
                                                                        timer1.Stop();
                                                                    }

                                                                    // Z kolei, jeśli numer serii aktualny jest równy numerowi poprzedniemu dane krotki odpowiadającej danemu numerowi serii są uaktalniane za pomocą polecenia "UPDATE tabela SET ..."

                                                                    else
                                                                    {
                                                                        MySqlCommand c =
                                                                        new MySqlCommand("UPDATE seria SET czas_rozpoczecia=?w, data_rozpoczecia=?k, czas_zakonczenia=?z, data_zakonczenia=?j, login=?o where seria =?q", polaczenie);
                                                                        c.Parameters.Add(new MySqlParameter("q", seria_int));
                                                                        c.Parameters.Add(new MySqlParameter("w", czas_rozpoczecia));
                                                                        c.Parameters.Add(new MySqlParameter("k", dt));
                                                                        c.Parameters.Add(new MySqlParameter("z", czas_zakonczenia));
                                                                        c.Parameters.Add(new MySqlParameter("j", dt));
                                                                        c.Parameters.Add(new MySqlParameter("o", uzytkowniktb.Text));
                                                                        c.ExecuteNonQuery();

                                                                        MySqlCommand h =
                                                                        new MySqlCommand("UPDATE dane_serii SET ile_krazkow=?b, liczba_bialych=?n, liczba_czarnych=?m where seria=?v", polaczenie);
                                                                        h.Parameters.Add(new MySqlParameter("v", seria_int));
                                                                        h.Parameters.Add(new MySqlParameter("b", ile_krazkow));
                                                                        h.Parameters.Add(new MySqlParameter("n", liczba_bialych));
                                                                        h.Parameters.Add(new MySqlParameter("m", liczba_czarnych));
                                                                        h.ExecuteNonQuery();
                                    */
                                    rtbTerminal.Text = "";
                                    timer1.Stop();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Nieoczekiway błąd aplikacji\n" + exc.Message, "Error");
            }
        }

        //Metoda za pomocą której realizowana jest operacja usunięcia stworzonego wczęsniej użytkownika w bazie danych MySQL wraz z jego danymi przechowywanymi w tabeli uzytkownik. 
        private void button4_Click_1(object sender, EventArgs e)
        {
            /*
            try
            {
                Loguj();
                MySqlCommand u =
                new MySqlCommand("DROP USER ?q @'127.0.0.1'", polaczenie);
                u.Parameters.Add(new MySqlParameter("q", textBox3.Text));
                u.ExecuteNonQuery();
                MySqlCommand d =
                new MySqlCommand("DELETE FROM uzytkownik WHERE login = ?d ;", polaczenie);
                d.Parameters.Add(new MySqlParameter("d", textBox3.Text));
                d.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Brak połączenia z bazą danych MySql lub niepoprawne dane.", "Błąd");
            }
            */
        }

        //Metoda uruchamiana za pomocą przycisku usuwająca tabele z bazy danych MySQL
        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                Loguj();
                string sql = "DROP TABLE " + textBox6.Text;
                OdbcCommand a = new OdbcCommand(sql, OdbcConnect);
                a.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Brak połączenia z bazą danych MySql lub niepoprawna nazwa tabeli.", "Błąd");
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i = 0;
            int j = 4;
            int t = 0;

            int b = 4;
            int d = 0;

            int w = 4;
            int r = 0;

            int w1 = 4;
            int r1 = 0;

            int w2 = 4;
            int r2 = 0;

            int w3 = 4;
            int r3 = 0;

            int w4 = 4;
            int r4 = 0;

            int y = 4;
            int u = 0;

            int z = 4;
            int x = 0;

            int p = 4;
            int v = 0;

            try
            {
                for (int q = 0; q < rtbTerminal.Text.Length; q = q + 1)
                {
                    if ((rtbTerminal.Text[q] == '6') && (rtbTerminal.Text[q + 1] == '5'))
                    {
                        for (i = q; i < rtbTerminal.Text.Length; i = i + 1)
                        {
                            if ((rtbTerminal.Text[i] == '6') && (rtbTerminal.Text[i + 1] == '5'))
                            {
                                while ((rtbTerminal.Text[i + j + 2] != '6') && (rtbTerminal.Text[i + j + 3] != '7'))
                                {
                                    seria[t] = rtbTerminal.Text[i + j];
                                    j = j + 1;
                                    t++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j] == '6') && (rtbTerminal.Text[i + j + 1] == '8'))
                            {
                                while ((rtbTerminal.Text[i + j + b + 2] != '6') && (rtbTerminal.Text[i + j + b + 3] != '9'))
                                {
                                    godzina_rozpoczecia[d] = rtbTerminal.Text[i + j + b];
                                    b = b + 1;
                                    d++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b] == '7') && (rtbTerminal.Text[i + j + b + 1] == '0'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + 2] != '7') && (rtbTerminal.Text[i + j + b + w + 3] != '1'))
                                {
                                    minuta_rozpoczecia[r] = rtbTerminal.Text[i + j + b + w];
                                    w = w + 1;
                                    r++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b + w] == '7') && (rtbTerminal.Text[i + j + b + w + 1] == '2'))
                            {
                                while (rtbTerminal.Text[i + j + b + w + w1 + 2] != '7' && rtbTerminal.Text[i + j + b + w + w1 + 3] != '3')
                                {
                                    sekunda_rozpoczecia[r1] = rtbTerminal.Text[i + j + b + w + w1];
                                    w1 = w1 + 1;
                                    r1++;
                                }

                            }
                            // czas zakonczenia                        
                            if (rtbTerminal.Text[i + j + b + w + w1] == '7' && rtbTerminal.Text[i + j + b + w + w1 + 1] == '4')
                            {
                                while (rtbTerminal.Text[i + j + b + w + w1 + w2 + 2] != '7' && rtbTerminal.Text[i + j + b + w + w1 + w2 + 3] != '5')
                                {
                                    godzina_zakonczenia[r2] = rtbTerminal.Text[i + j + b + w + w1 + w2];
                                    w2 = w2 + 1;
                                    r2++;
                                }
                            }
                            //2 seria danych
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2] == '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + 1] == '6'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 2] != '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 3] != '8'))
                                {
                                    minuta_zakonczenia[r3] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3];
                                    w3 = w3 + 1;
                                    r3++;
                                }
                            }

                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3] == '7') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + 1] == '9'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 3] != '0'))
                                {
                                    sekunda_zakonczenia[r4] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4];
                                    w4 = w4 + 1;
                                    r4++;
                                }

                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + 1] == '1'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 3] != '2'))
                                {
                                    liczba_bialych[u] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y];
                                    y = y + 1;
                                    u++;
                                }
                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + 1] == '3'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 3] != '4'))
                                {
                                    liczba_czarnych[x] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z];
                                    z = z + 1;
                                    x++;
                                }
                            }
                            if ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z] == '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + 1] == '5'))
                            {
                                while ((rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p + 2] != '8') && (rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p + 3] != '6'))
                                {
                                    ile_krazkow[v] = rtbTerminal.Text[i + j + b + w + w1 + w2 + w3 + w4 + y + z + p];
                                    p = p + 1;
                                    v++;
                                }

                                i = rtbTerminal.Text.Length;
                                q = rtbTerminal.Text.Length;

                                Loguj();

                                //Konwersja tablicy char do string

                                string seria_s = new string(seria);

                                string godzina_rozpoczecia_s = new string(godzina_rozpoczecia);
                                string minuta_rozpoczecia_s = new string(minuta_rozpoczecia);
                                string sekunda_rozpoczecia_s = new string(sekunda_rozpoczecia);

                                string godzina_zakonczenia_s = new string(godzina_zakonczenia);
                                string minuta_zakonczenia_s = new string(minuta_zakonczenia);
                                string sekunda_zakonczenia_s = new string(sekunda_zakonczenia);

                                string liczba_bialych_s = new string(liczba_bialych);
                                string liczba_czarnych_s = new string(liczba_czarnych);
                                string ile_krazkow_s = new string(ile_krazkow);

                                //Konwersja string do int

                                int seria_int = Int32.Parse(seria_s);

                                int godzina_rozpoczecia_int = Int32.Parse(godzina_rozpoczecia_s);
                                int minuta_rozpoczecia_int = Int32.Parse(minuta_rozpoczecia_s);
                                int sekunda_rozpoczecia_int = Int32.Parse(sekunda_rozpoczecia_s);

                                int godzina_zakonczenia_int = Int32.Parse(godzina_zakonczenia_s);
                                int minuta_zakonczenia_int = Int32.Parse(minuta_zakonczenia_s);
                                int sekunda_zakonczenia_int = Int32.Parse(sekunda_zakonczenia_s);

                                int liczba_bialych_int = Int32.Parse(liczba_bialych_s);
                                int liczba_czarnych_int = Int32.Parse(liczba_czarnych_s);
                                int ile_krazkow_int = Int32.Parse(ile_krazkow_s);

                                string dt = DateTime.Now.ToString("dd MMM yyyy");

                                czas_rozpoczecia = godzina_rozpoczecia_int + ":" + minuta_rozpoczecia_int + ":" + sekunda_rozpoczecia_int;
                                czas_zakonczenia = godzina_zakonczenia_int + ":" + minuta_zakonczenia_int + ":" + sekunda_zakonczenia_int;

                                // Wypisanie debugowanej zmiennej
                                richTextBox2.Text = "" + sekunda_zakonczenia_int + "";

                                if (seria_int != seria_int_p)
                                {
                                    /*
                                                                        MySqlCommand c =
                                                                        new MySqlCommand("INSERT INTO seria(seria,czas_rozpoczecia,data_rozpoczecia,czas_zakonczenia,data_zakonczenia,login)VALUES(?q,?w,?k,?z,?j,?o)", polaczenie);
                                                                        c.Parameters.Add(new MySqlParameter("q", seria_int));
                                                                        c.Parameters.Add(new MySqlParameter("w", czas_rozpoczecia));
                                                                        c.Parameters.Add(new MySqlParameter("k", dt));
                                                                        c.Parameters.Add(new MySqlParameter("z", czas_zakonczenia));
                                                                        c.Parameters.Add(new MySqlParameter("j", dt));
                                                                        c.Parameters.Add(new MySqlParameter("o", uzytkowniktb.Text));
                                                                        c.ExecuteNonQuery();

                                                                        MySqlCommand h =
                                                                        new MySqlCommand("INSERT INTO dane_serii(seria, ile_krazkow, liczba_bialych, liczba_czarnych)VALUES(?v,?b,?n,?m)", polaczenie);
                                                                        h.Parameters.Add(new MySqlParameter("v", seria_int));
                                                                        h.Parameters.Add(new MySqlParameter("b", ile_krazkow));
                                                                        h.Parameters.Add(new MySqlParameter("n", liczba_bialych));
                                                                        h.Parameters.Add(new MySqlParameter("m", liczba_czarnych));
                                                                        h.ExecuteNonQuery();

                                                                        seria_int_p = seria_int;
                                                                        rtbTerminal.Text = "";
                                                                    }

                                                                    else
                                                                    {
                                                                        MySqlCommand c =
                                                                        new MySqlCommand("UPDATE seria SET czas_rozpoczecia=?w, data_rozpoczecia=?k, czas_zakonczenia=?z, data_zakonczenia=?j, login=?o where seria =?q", polaczenie);
                                                                        c.Parameters.Add(new MySqlParameter("q", seria_int));
                                                                        c.Parameters.Add(new MySqlParameter("w", czas_rozpoczecia));
                                                                        c.Parameters.Add(new MySqlParameter("k", dt));
                                                                        c.Parameters.Add(new MySqlParameter("z", czas_zakonczenia));
                                                                        c.Parameters.Add(new MySqlParameter("j", dt));
                                                                        c.Parameters.Add(new MySqlParameter("o", uzytkowniktb.Text));
                                                                        c.ExecuteNonQuery();

                                                                        MySqlCommand h =
                                                                        new MySqlCommand("UPDATE dane_serii SET ile_krazkow=?b, liczba_bialych=?n, liczba_czarnych=?m where seria=?v", polaczenie);
                                                                        h.Parameters.Add(new MySqlParameter("v", seria_int));
                                                                        h.Parameters.Add(new MySqlParameter("b", ile_krazkow));
                                                                        h.Parameters.Add(new MySqlParameter("n", liczba_bialych));
                                                                        h.Parameters.Add(new MySqlParameter("m", liczba_czarnych));
                                                                        h.ExecuteNonQuery();

                                                                        rtbTerminal.Text = "";

                                    */
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Nieoczekiway błąd aplikacji\n" + exc.Message, "Error");
            }
        }

        //Metoda usuwająca dane z danej tabeli, za pomocą polecenia w języku MySQL. 
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "DELETE FROM " + textBox7.Text;
                OdbcCommand del = new OdbcCommand(sql, OdbcConnect);
                del.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Brak połączenia z bazą danych lub niepoprawna nazwa tabeli\n" + exc, "Błąd");
            }
        }

        private void Debuger_Click(object sender, EventArgs e)
        {

        }

        private void logowanieBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                var connectionString = @"DSN=PLC_SQL;DatabaseName=PLC_DB;UID=PLC;PWD=plc_pwd;";
                OdbcConnect = new OdbcConnection(connectionString);
                OdbcConnect.Open();
                MessageBox.Show("Nawiązano połączenie z bazą danych", "OK");
            }
            catch (OdbcException exc)
            {
                MessageBox.Show("Brak połączenia z bazą danych\n" + exc, "Błąd");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "INSERT INTO CHAMBER(Temperature_Module_APAR, Humidity_Module_APAR, Temperature_Act, Humidity_Act, Temperature_Set, Humidity_Set, Date_Time) VALUES(1,2,3,4,5,6,7)";
                //string sql = "CREATE TABLE CHAMBER4(ID int IDENTITY(1,1) PRIMARY KEY, Temperature_Module_APAR DOUBLE(6), Humidity_Module_APAR DOUBLE(6), Temperature_Act DOUBLE(6), Humidity_Act DOUBLE(6), Temperature_Set DOUBLE(6), Humidity_Set DOUBLE(6), Date_Time DATETIME);";
                OdbcCommand del = new OdbcCommand(sql, OdbcConnect);
                del.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR:" + exc);
            }
        }

        private void rtbTerminal_TextChanged(object sender, EventArgs e)
        {

        }
    }
}