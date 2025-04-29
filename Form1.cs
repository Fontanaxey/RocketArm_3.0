using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
namespace RocketArm_3._0
{
    public partial class Form1 : Form
    {
        public SerialPort arduinoPort;
        public int[,] m;
        public Form1()
        {
            InitializeComponent();
            trackBar1.Scroll += trackBar1_Scroll_1;
            trackBar2.Scroll += TrackBar2_Scroll;
            trackBar3.Scroll += TrackBar3_Scroll;
            trackBar4.Scroll += TrackBar4_Scroll;
            trackBar5.Scroll += trackBar5_Scroll;
            dataGridView1.Rows.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedItem = comboBox1.Items[0];

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("Col1", "TrackBar 1");
                dataGridView1.Columns.Add("Col2", "TrackBar 2");
                dataGridView1.Columns.Add("Col3", "TrackBar 3");
                dataGridView1.Columns.Add("Col4", "TrackBar 4");
                dataGridView1.Columns.Add("Col5", "TrackBar 5");
            }
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar5.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5);
            m = createMatrix();
        }
        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            SendAngle("a", trackBar1.Value);
        }
        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            SendAngle("b", trackBar2.Value);
        }
        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            SendAngle("c", -trackBar3.Value);
        }
        private void TrackBar4_Scroll(object sender, EventArgs e)
        {
            SendAngle("d", -trackBar4.Value);
        }
        private void SendAngle(string command, int angle)   //invio dati
        {
            if (arduinoPort != null && arduinoPort.IsOpen)
            {
                try
                {
                    arduinoPort.WriteLine($"{command}{angle}");
                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show("Tempo scaduto per inviare il comando: " + ex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Errore di I/O durante l'invio del comando: " + ex.Message);
                }
            }
            else
                MessageBox.Show("La porta seriale non è aperta.");
        }
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            SendAngle("e", -trackBar5.Value);
        }
        private void button1_Click(object sender, EventArgs e)  //collegamento all'arduino
        {
            string com = (string)comboBox1.SelectedItem;
            if (com != "")
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
            try
            {
                arduinoPort.Open();
                MessageBox.Show("connected");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error to Arduino: " + ex.Message);
            }
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }
        private void button2_Click(object sender, EventArgs e)  //demo
        {
            SendAngle("f", 0);
        }
        private void button3_Click(object sender, EventArgs e)  //registra valori
        {
            int a1 = trackBar1.Value, a2 = trackBar2.Value, a3 = trackBar3.Value, a4 = trackBar4.Value, a5 = trackBar5.Value;
            dataGridView1.Rows.Add(a1, a2, a3, a4, a5);
            m = createMatrix();
        }
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)    //invio riga scelta
        {
            IList l = dataGridView1.CurrentRow.Cells;
            int i = 97;
            foreach (DataGridViewTextBoxCell o in l)
            {
                SendAngle((char)i++ + "", (int)o.Value);
            }
        }
        private int[,] createMatrix()   //creazione 
        {
            IList l = dataGridView1.Rows;
            int[,] m = new int[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                {
                    DataGridViewRow row = dataGridView1.Rows[j];
                    int? v = (int?) row.Cells[k].Value;
                    if (v == null)
                    {
                        dataGridView1.Rows.RemoveAt(j);
                        break;
                    }
                    else
                        m[j, k] = (int)v;
                }
            return m;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                    SendAngle((char)(k + 97) + "", m[j, k]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (arduinoPort != null && arduinoPort.IsOpen)
                try
                {
                    if (arduinoPort.BytesToRead > 0)
                    {
                        string data = arduinoPort.ReadLine();
                        label6.Text = $"Data received: {data}"; // Optional: Display the received data  
                    }
                }
                catch (Exception ex)
                {
                MessageBox.Show("Error reading from the serial port: " + ex.Message);
                }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
    }
}