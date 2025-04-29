using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
/*
 * TODO
 *  - Aggiungere un id sessione ai log di errore
 *  - 
*/
namespace RocketArm_3._0
{
    public partial class Form1 : Form
    {
        public SerialPort arduinoPort;        // Oggetto per la comunicazione seriale con Arduino
        public int[,] m;                      // Matrice per salvare i valori dei trackbar
        private bool init = false;            // Flag per indicare se l'interfaccia è inizializzata
        public Form1()   // costruttore
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // Inizializzazione dei valori dei trackbar
            trackBar1.Value = trackBar2.Value = 90;
            trackBar3.Value = trackBar4.Value = trackBar5.Value = -90;

            // Eventi associati allo scroll dei trackbar
            trackBar1.Scroll += trackBar1_Scroll_1;
            trackBar2.Scroll += TrackBar2_Scroll;
            trackBar3.Scroll += TrackBar3_Scroll;
            trackBar4.Scroll += TrackBar4_Scroll;
            trackBar5.Scroll += trackBar5_Scroll;

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
            }

            // Salva i valori attuali nella prima riga della tabella
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar5.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5);

            m = CreateMatrix();  // Crea la matrice con i dati iniziali
            init = true;         //controllo sulla prima inizializzazione del form
        }

        private void File_ER()
        {
            using(StreamWriter sw = new StreamWriter("log_errori.txt", append: true))
            {
                sw.WriteLine("ERRORE : " + label7.Text);
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

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            SendAngle("e", -trackBar5.Value);  // Invia valore del trackBar5
        }

        private void SendAngle(string command, int angle)   // invio dati alla seriale
        {
            if (arduinoPort != null && arduinoPort.IsOpen)
            {
                try
                {
                    arduinoPort.WriteLine($"{command}{angle}");  // Comando + angolo
                }
                catch (TimeoutException ex)
                {
                    label7.Text = "Tempo scaduto per inviare il comando: " + ex.Message;
                    File_ER();
                }
                catch (IOException ex)
                {                    label7.Text="Errore di I/O durante l'invio del comando: " + ex.Message + "\nref SendAngle";
                    File_ER();
                }
            }
            else if (!init)
                label7.Text="La porta seriale non è aperta.\nref SendAngle";
                File_ER();
        }

        private void button1_Click(object sender, EventArgs e)  // collegamento all'arduino
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
                    label7.Text=ex.Message + "\nref button1 connection";
                    File_ER();
                }
            }
            try
            {
                arduinoPort.Open();   // Apertura porta seriale
                MessageBox.Show("connected");
            }
            catch (Exception ex)
            {
                label7.Text=ex.Message + "\nref button1 connection";
                File_ER();
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
            SendAngle("f", 0);   // Invia comando di demo
        }

        private void button3_Click(object sender, EventArgs e)  // registra valori
        {
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar5.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5);   // Aggiunge i valori alla tabella
            m = CreateMatrix();   // Ricrea la matrice aggiornata
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)  // invio riga scelta
        {
            if (init)
            {
                IList l = dataGridView1.CurrentRow.Cells;
                int i = 97;  // ASCII 'a'
                foreach (DataGridViewTextBoxCell o in l)
                    SendAngle((char)i++ + "", (int)o.Value);  // Invia ogni valore della riga
            }
        }

        private int[,] CreateMatrix()   // creazione matrice dei valori della tabella
        {
            int[,] m = new int[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                {
                    DataGridViewRow row = dataGridView1.Rows[j];
                    int? v = (int?)row.Cells[k].Value;
                    if (v == null)
                    {
                        dataGridView1.Rows.RemoveAt(j);  // Rimuove righe incomplete
                        break;
                    }
                    else
                        m[j, k] = (int)v;
                }
            return m;
        }

        private void button4_Click(object sender, EventArgs e)  // send grid
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                    SendAngle((char)(k + 97) + "", m[j, k]);  // Invia l’intera matrice ad Arduino
        }

        private void timer1_Tick(object sender, EventArgs e)  // lettura dati da Arduino        DA RIMUOVERE PRIMA DELLA RELEASE
        {
            if (arduinoPort != null && arduinoPort.IsOpen)
                try
                {
                    if (arduinoPort.BytesToRead > 0)
                    {
                        string data = arduinoPort.ReadLine();
                        label6.Text = $"Data received: {data}";  // Mostra dati ricevuti
                    }
                }
                catch (Exception ex)
                {
                    label7.Text = "Error reading from the serial port: " + ex.Message + "\nref timer1";
                    File_ER();
                }
        }

        private void button5_Click(object sender, EventArgs e)  // cancella righe tabella
        {
            dataGridView1.Rows.Clear();
        }
    }
}
