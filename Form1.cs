using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
/*
 * TODO:
 *  - 
*/

/*
 *  Char map 
 *  a: base
 *  b: s1
 *  c: s2
 *  d: s3
 *  e: apri
 *  f: chiudi
 *  g: demo1
 *  h: s4 (braccio2)
 *  i:
 *  l:
 *  m:
 */
namespace RocketArm_3._0
{
    public partial class Form1 : Form
    {
        public SerialPort arduinoPort;          // Oggetto per la comunicazione seriale con Arduino
        public int[,] m;                            // Matrice per salvare i valori dei trackbar
        private bool init = false;
        int statpinza = 0;            // Flag per indicare se l'interfaccia è inizializzata
        private readonly string sessionId = Guid.NewGuid().ToString();
        //statpinza true = open, false = chiuso
        public Form1()
        {
            InitializeComponent();
            init = false;

            using (StreamWriter sw = new StreamWriter("log_errori.txt", append: true))
            {
                sw.WriteLine($"\n--- Nuova sessione: {sessionId} --- {DateTime.Now}");
            }

            this.DoubleBuffered = true;

            // Inizializzazione dei valori dei trackbar
            //trackBar1.Value = trackBar2.Value = 90;
            //trackBar3.Value = trackBar4.Value = trackBar5.Value = -90;

            // Eventi associati allo scroll dei trackbar

            trackBar1.Scroll += trackBar1_Scroll_1;
            trackBar2.Scroll += TrackBar2_Scroll;
            trackBar3.Scroll += TrackBar3_Scroll;
            trackBar4.Scroll += TrackBar4_Scroll;
            trackBar6.Scroll += trackBar6_Scroll;

            dataGridView1.Rows.Clear();

            // Aggiunta delle porte seriali disponibili alla comboBox
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }

            // Seleziona la prima porta se disponibile
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedItem = comboBox1.Items[0];

            // Inizializza le colonne della tabella se non presenti
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("Col1", "TrackBar 1");
                dataGridView1.Columns.Add("Col2", "TrackBar 2");
                dataGridView1.Columns.Add("Col3", "TrackBar 3");
                dataGridView1.Columns.Add("Col4", "TrackBar 4");
                dataGridView1.Columns.Add("Col5", "TrackBar 5");
                dataGridView1.Columns.Add("Col6", "Status pinza");
            }

            // Salva i valori attuali nella prima riga della tabella
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar6.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5, statpinza == 1 ? "Open" : "Closed");

            m = CreateMatrix();  // Crea la matrice con i dati iniziali
            init = true;         //controllo sulla prima inizializzazione del form
        }

        private void File_ER(String er) //Controllo errori  // OK
        {
            using (StreamWriter sw = new StreamWriter("log_errori.txt", append: true))
            {
                sw.WriteLine($"[{DateTime.Now}] Sessione {sessionId} - ERRORE: {er}");
                sw.WriteLine("ERRORE : " + er);
            }
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            SendAngle("a", trackBar1.Value);  // Invia valore del trackBar1
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            SendAngle("b", trackBar2.Value);  // Invia valore del trackBar2
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            SendAngle("c", -trackBar3.Value);  // Invia valore del trackBar3
        }

        private void TrackBar4_Scroll(object sender, EventArgs e)
        {
            SendAngle("d", -trackBar4.Value);  // Invia valore del trackBar4
        }

        private void SendAngle(string command, int angle)   //Manda input   // OK
        {
            if (!init || arduinoPort == null || !arduinoPort.IsOpen)
            {
                return;     // Ignora se non pronto
            }

            try
            {
                arduinoPort.WriteLine($"{command}{angle}");
            }
            catch (TimeoutException ex)
            {
                label7.Text = "Tempo scaduto per inviare il comando: " + ex.Message;
                File_ER(ex.Message);
            }
            catch (IOException ex)
            {
                label7.Text = "Errore di I/O durante l'invio del comando: " + ex.Message + "\nref SendAngle";
                File_ER(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)  // collegamento all'arduino // OK
        {
            string com = (string)comboBox1.SelectedItem;
            if (com != "")
            {
                try
                {
                    arduinoPort = new SerialPort
                    {
                        PortName = com,
                        BaudRate = 9600,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One
                    };
                }
                catch (Exception ex)
                {
                    label7.Text = ex.Message + "\nref button1 connection";
                    File_ER(ex.Message);
                }
            }
            try
            {
                arduinoPort.Open();   // Apertura porta seriale
                MessageBox.Show("connected");
            }
            catch (Exception ex)
            {
                label7.Text = ex.Message + "\nref button1 connection";
                File_ER(ex.Message);
            }

            // Rileva nuovamente le porte disponibili
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }

        private void button2_Click(object sender, EventArgs e)  // demo
        {
            SendAngle("g", 0);   // Invia comando di demo
        }

        private void button3_Click(object sender, EventArgs e)  // registra valori  // OK
        {
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar6.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5, statpinza);   // Aggiunge i valori alla tabella
            m = CreateMatrix();   // Ricrea la matrice aggiornata
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)  // invio riga scelta  //DA PROVARE
        {
            if (init && 1 == 0)
            {
                IList l = dataGridView1.CurrentRow.Cells;
                int i = 97;  // ASCII 'a'
                try
                {
                    foreach (DataGridViewTextBoxCell o in l)
                        SendAngle((char)i++ + "", (int)o.Value);  // Invia ogni valore della riga
                }
                catch (Exception ex)
                {
                    label7.Text = ex.Message + "\nref grid cell enter";
                    File_ER(ex.Message);
                }
            }
        }

        private int[,] CreateMatrix()   // creazione matrice dei valori della tabella   // OK
        {
            int[,] m = new int[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                {
                    DataGridViewRow row = dataGridView1.Rows[j];
                    int? v;
                    if (row.Cells[k].Value is string)
                    {
                        if ((String)(row.Cells[k].Value) == "Open")
                            v = 1;
                        else
                            v = 0;
                    }
                    else
                    {
                        v = (int?)row.Cells[k].Value;
                        if (v == null)
                        {
                            dataGridView1.Rows.RemoveAt(j);  // Rimuove righe incomplete
                            break;
                        }
                        else
                            m[j, k] = (int)v;
                    }
                }
            return m;
        }

        private void button4_Click(object sender, EventArgs e)  // send grid    //DA PROVARE
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                    SendAngle((char)(k + 97) + "", m[j, k]);  // Invia l’intera matrice ad Arduino

        }

        private void timer1_Tick(object sender, EventArgs e)  // lettura dati da Arduino    // DA RIMUOVERE PRIMA DELLA RELEASE  // OK
        {
            if (arduinoPort != null && arduinoPort.IsOpen && 1 == 0)
                try
                {
                    if (arduinoPort.IsOpen)
                        label8.Text = "Connected " + arduinoPort.PortName;
                    if (arduinoPort.BytesToRead > 0)
                    {
                        string data = arduinoPort.ReadLine();
                        label6.Text = $"Data received: {data}";  // Mostra dati ricevuti
                    }
                }
                catch (Exception ex)
                {
                    label7.Text = "Error reading from the serial port: " + ex.Message + "\nref timer1";
                    File_ER(ex.Message);
                }
        }

        private void button5_Click(object sender, EventArgs e)  // cancella righe tabella   // OK
        {
            dataGridView1.Rows.Clear();
        }

        private void button6_Click(object sender, EventArgs e)  // pinza    // OK
        {
            SendAngle("e", 0);
            statpinza = 1;
        }

        private void button7_Click(object sender, EventArgs e)  // pinza    // OK
        {
            SendAngle("f", 0);
            statpinza = 0;
        }

        private void button8_Click(object sender, EventArgs e)  // show trackbar braccio2   // OK
        {
            if (trackBar6.Visible)
            {
                trackBar6.Visible = false;
                label9.Visible = false;
            }
            else
            {
                trackBar6.Visible = true;
                label9.Visible = true;
            }
        }

        private void trackBar6_Scroll(object sender, EventArgs e)   // Braccio2 Servo4
        {
            SendAngle("k", trackBar6.Value);
        }

        private void button9_Click(object sender, EventArgs e)  //cambio mod invio
        {
            if (!init || arduinoPort == null || !arduinoPort.IsOpen)
            {
                return;     // Ignora se non tutto pronto
            }
            bool Button_State = true; 
            try
            {
                char a = 'z';
                if (Button_State)
                {
                    arduinoPort.WriteLine(a + "");
                    Button_State = false;
                }
                else
                {
                    arduinoPort.WriteLine('w' + "");
                    Button_State = true;
                }
            }
            catch (TimeoutException ex)
            {
                label7.Text = "Tempo scaduto per inviare il comando: " + ex.Message;
                File_ER(ex.Message);
            }
            catch (IOException ex)
            {
                label7.Text = "Errore di I/O durante l'invio del comando: " + ex.Message + "\nref SendAngle";
                File_ER(ex.Message);
            }
        }
    }
}