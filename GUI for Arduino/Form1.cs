using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace GUI_for_Arduino
{
    public partial class Form1 : Form
    {
        public string msg;
        public byte[] bytes_received = new byte[100];
        double time = 0;
        Timer myTimer = new Timer();
        Random random = new Random();

        string file;

        float timer, timenow;
        int dataCount;
        int time_stamp;
        int motor_rpm;
        int vsm;
        int internal_faults;
        int battery_percentage;
        int battery_voltage;
        int battery_current;
        int battery_heat;
        int motor_heat;
        int inverter_heat;
        int speed_ = 0;
        
        bool isLogging;


        public Form1()
        {
            InitializeComponent();
            Init();
        }
        void Init()
        {
            isLogging = false;
            
            // data count and time reset
            dataCount = 0;
            timer = (float)DateTime.Now.Millisecond / 1000 + (float)DateTime.Now.Second;
            //image box
            Image formula_img = pictureBox6.Image;
            //formula_img.RotateFlip(RotateFlipType.Rotate270FlipX);
            pictureBox6.Image = formula_img;


            //Baud Rates
            comboBox2.Items.Add(9600);
            comboBox2.Items.Add(115200);
            //Ports
            refresh_ports();

            //Event handler for serial port
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SeriPortCom_DataReceived);
            serialPort1.PortName = "COM4";

            //progress
            timer1.Enabled = true;
            timer1.Interval = 50;
            //Gauge
            timer2.Enabled = true;
            timer2.Interval = 10;


            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart3.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart4.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart4.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;


        }
        void clear()
        {

            time_stamp = ' ';
            motor_rpm = 0;
            vsm = 0;
            internal_faults = 0;
            battery_percentage = 0;
            battery_voltage = 0;
            battery_current = 0;
            battery_heat = 0;
            motor_heat = 0;
            inverter_heat = 0;
            speed_ = 0;

        }

        

        private void timer2_Tick(object sender, EventArgs e)
        {
            /* 
             * GAUGE VALUES
             */
            
            //speed gauge
            if(speed_ < 200)
                gauge1.Value = gauge1.Value + (speed_ - gauge1.Value) / 25;
            
            //battery percantage gauge
            if(battery_percentage < 200)
                gauge2.Value = gauge2.Value + (battery_percentage - gauge2.Value) / 50; ;

            //power gauge
            int _kw = (battery_voltage * battery_current);
            gauge3.Value = gauge3.Value + (_kw - gauge3.Value) / 50;


            //battery heat gauge
            battery_heat = inverter_heat; // gecici
            if (battery_heat < 200)
                gauge4.Value = gauge4.Value + ((int)battery_heat - gauge4.Value) / 50; ;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //chart 1
            chart1.Series[0].Points.AddXY(time, 3 * Math.Sin(5 + time) + 5 * Math.Cos(3 + time));

            if (chart1.Series[0].Points.Count > 100)
                chart1.Series[0].Points.RemoveAt(0);

            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = time;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart1.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;


            // chart 2
            chart2.Series[0].Points.AddXY(time, 3 * Math.Sin(5 + time) + 5 * Math.Cos(3 + time));

            if (chart2.Series[0].Points.Count > 100)
                chart2.Series[0].Points.RemoveAt(0);

            chart2.ChartAreas[0].AxisX.Minimum = (double)chart2.Series[0].Points[0].XValue;
            chart2.ChartAreas[0].AxisX.Maximum = (float)time;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart2.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;
            // chart 3
            chart3.Series[0].Points.AddXY(time, (double)battery_heat);

            if (chart3.Series[0].Points.Count > 1000)
                chart3.Series[0].Points.RemoveAt(0);

            chart3.ChartAreas[0].AxisY.Minimum = 0;
            chart3.ChartAreas[0].AxisY.Maximum = 100;
            chart3.ChartAreas[0].AxisX.Minimum = chart3.Series[0].Points[0].XValue;
            chart3.ChartAreas[0].AxisX.Maximum = time;
            chart3.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart3.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;

            // remove gridline y
            chart3.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            // chart 4
            chart4.Series[0].Points.AddXY(time, (double)speed_);

            if (chart4.Series[0].Points.Count > 500)
                chart4.Series[0].Points.RemoveAt(0);

            chart4.ChartAreas[0].AxisY.Minimum = 0;
            chart4.ChartAreas[0].AxisY.Maximum = 120;
            chart4.ChartAreas[0].AxisX.Minimum = chart4.Series[0].Points[0].XValue;
            chart4.ChartAreas[0].AxisX.Maximum = time;
            chart4.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart4.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;

            // remove gridlines
            chart4.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart4.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            //speed

            label13.Text = " " + speed_.ToString() + " km/h";
            //battery percentage

            label4.Text = "%" + battery_percentage.ToString();
            //battery heat
            label7.Text = battery_heat.ToString() + " °C";

            //battery voltage
            textBox2.Text = battery_voltage.ToString() + " V";
            //battery current
            textBox1.Text = battery_current.ToString() + " A";
            //motor heat
            textBox3.Text = motor_heat.ToString() + " °C";
            //inverter heat
            textBox4.Text = inverter_heat.ToString() + " °C";
            //VSM
            textBox5.Text = vsm.ToString();
            //kw
            label5.Text = ((float)(battery_voltage * battery_current)/1000).ToString() + " Kw";


            switch (vsm.ToString())
            {
                case "0":
                    textBox7.BackColor = Color.White;
                    break;
                case "4":
                    textBox7.BackColor = Color.Blue;
                    break;
                case "5":
                    textBox7.BackColor = Color.Yellow;
                    break;
                case "6":
                    textBox7.BackColor = Color.Green;
                    break;
                case "7":
                    textBox7.BackColor = Color.Red;
                    break;
                default:
                    textBox7.BackColor = Color.White;
                    break;
            }
            //internal fault
            textBox6.Text = internal_faults.ToString();

            // data count
            textBox8.Text = dataCount.ToString();

            // data timer
            timenow = (float)DateTime.Now.Millisecond / 1000 + (float)DateTime.Now.Second;
            textBox9.Text = (timenow - timer).ToString();

            // data timer visualizer
            
            
            if(dataCount % 2 == 1)
                textBox10.BackColor = Color.LightGreen;
            else
                textBox10.BackColor = Color.Yellow;

            // logging
            if(isLogging)
            {
                textBox11.Text = "Logging Started";
                textBox11.BackColor = Color.Green;
            }
            else
            {
                textBox11.Text = "Logging Stopped";
                textBox11.BackColor = Color.Red;
            }
            

            time += 0.1;
        }



        void refresh_ports()
        {
            string[] ports = SerialPort.GetPortNames();
            System.Console.WriteLine(ports);
            comboBox1.Items.Clear();
            // Display each port name to the console.
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            Console.WriteLine("Refreshed");
            refresh_ports();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = (string)comboBox1.SelectedItem;
            Console.WriteLine("Selected");
        }

        int trans(int x)
        {

            int t = 0;
            for (int i = x + 3; i >= x; i--)
            {
                t = t << 8;
                t = t | bytes_received[i];
            }
            return t;


        }

        private void logger()
        {
            if(isLogging)
            {
                string path = file;
                Console.WriteLine(path);
              
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd.mm.yyyy HH.mm.ss.ff") + " " + battery_voltage.ToString() + " " + battery_current.ToString() + " " + battery_heat.ToString() + " " + motor_heat.ToString() + " " + inverter_heat.ToString() + " " + speed_.ToString() + " " + battery_percentage.ToString() + " " + vsm.ToString() + " " + internal_faults.ToString());
                }
            
                
            }
        }
        

        private void SeriPortCom_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("Success");

                serialPort1.Read(bytes_received, 0, 47);
                

                time_stamp = trans(0);

                Console.WriteLine(time_stamp);

                Console.WriteLine(bytes_received[0]);

                if (time_stamp == -1)
                {
                    dataCount++;
                    timer = (float)DateTime.Now.Millisecond / 1000 + (float)DateTime.Now.Second;
                    motor_rpm = trans(4);
                    Console.WriteLine(motor_rpm);
                    //circularProgressBar1.Value = (int)motor_rpm;


                    
                    vsm = trans(8);
                    Console.WriteLine(vsm);

                    
                    internal_faults = trans(12);
                    Console.WriteLine(internal_faults);

                    
                    battery_percentage = trans(16);
                    Console.WriteLine(battery_percentage);

                   
                    battery_voltage = trans(20);
                    Console.WriteLine(battery_voltage);


                    
                    battery_current = trans(24);
                    Console.WriteLine(battery_current);


                    battery_heat = trans(28);
                    Console.WriteLine(battery_heat);



                    motor_heat = trans(32);
                    Console.WriteLine(motor_heat);

                    
                    inverter_heat = trans(36);
                    Console.WriteLine(inverter_heat);

                    
                    speed_ = trans(40);
                    Console.WriteLine(speed_);

                    richTextBox1.Invoke(new EventHandler(delegate
                    {

                        richTextBox1.AppendText(time_stamp + "  ");
                        richTextBox1.AppendText(motor_rpm.ToString() + " ");
                        richTextBox1.AppendText(vsm.ToString() + " ");
                        richTextBox1.AppendText(internal_faults.ToString() + " ");
                        richTextBox1.AppendText(battery_percentage.ToString() + " ");
                        richTextBox1.AppendText(battery_voltage.ToString() + " ");
                        richTextBox1.AppendText(battery_current.ToString() + " ");
                        richTextBox1.AppendText(battery_heat.ToString() + " ");
                        richTextBox1.AppendText(motor_heat.ToString() + " ");
                        richTextBox1.AppendText(inverter_heat.ToString() + " ");
                        richTextBox1.AppendText(speed_.ToString() + "\n");
                        //richTextBox1.AppendText(fault_message.ToString() + "\n");


                    }));

                    logger();
                }

                


                //Console.WriteLine(text[5]);
                //Console.WriteLine(text[7]);
                //Console.WriteLine(text[8]);

                

                //if ((int)text[text.Length - 1] == 10)
                //{
                //    msg = msg + text;
                //    Console.WriteLine(msg);
                //    msg = "";
                //}
                //else
                //{
                //    msg = msg + text;
                //}




            }
            catch
            {
                MessageBox.Show("Something gone wrong..", "Error-322EDA23");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (serialPort1.IsOpen == false)
            {
                Console.WriteLine("Starting connection..");
                richTextBox1.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText("Starting connection..\n");
                }));
                try { serialPort1.Open(); }
                catch
                {
                    MessageBox.Show("Couldn't open Port", "Error-322EDA23");
                }
                
            }
            else
            {
                richTextBox1.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText("Connection already estabilished\n");
                }));
                Console.WriteLine("Connection already estabilished");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            serialPort1.BaudRate = (int)comboBox2.SelectedItem;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void circularProgressBar1_Click_1(object sender, EventArgs e)
        {


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                Console.WriteLine("Connection is over..");
                serialPort1.Close();
                clear();

                richTextBox1.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText("Connection is over..\n");
                }));
            }
            else
            {
                richTextBox1.Invoke(new EventHandler(delegate
                {
                    richTextBox1.AppendText("Connection already closed\n");
                }));
                Console.WriteLine("Connection already closed");
            }

        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }


        private void chart2_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

      

        private void gauge2_Load(object sender, EventArgs e)
        {

        }

     

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void label13_Click_1(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {
            
        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            isLogging = false;
            button4.Enabled = true;
            button4.Cursor = Cursors.Hand;
            button5.Enabled = false;
            button5.Cursor = Cursors.No;
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            isLogging = true;          
            button4.Enabled = false;
            button4.Cursor = Cursors.No;
            button5.Enabled = true;
            button5.Cursor = Cursors.Hand;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                file = folderBrowserDialog1.SelectedPath;
                file = @file + "\\" + "log_" + DateTime.Now.ToString("dd_MMM_yyyy__HH_mm_ss") + ".txt";
            }
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
