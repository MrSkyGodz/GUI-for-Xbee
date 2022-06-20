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




namespace GUI_for_Arduino
{
    public partial class Form1 : Form
    {
        public string msg;
        public byte[] bytes_received = new byte[100];

        int angle = 0;
        double time = 0;
        Image img;

        Timer myTimer = new Timer();
        Random random = new Random();


        char time_stamp;
        uint motor_rpm;
        uint vsm;
        uint internal_faults;
        uint battery_percentage;
        uint battery_voltage;
        uint battery_current;
        uint battery_heat;
        uint motor_heat;
        uint inverter_heat;
        uint speed_ = 0; 
        
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        void Init()
        {
            //image box
            Image formula_img = pictureBox6.Image;
            formula_img.RotateFlip(RotateFlipType.Rotate270FlipX);
            pictureBox6.Image = formula_img;


            //Baud Rates
            comboBox2.Items.Add(9600);
            comboBox2.Items.Add(115200);
            //Ports
            refresh_ports();
            
            //Event handler for serial port
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SeriPortCom_DataReceived);
            serialPort1.PortName = "COM3";

            //chart
            timer1.Enabled = true;
            timer1.Interval = 50;

            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            
            
            chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            chart1.Series[0].Points.AddXY(time, 3 * Math.Sin(5 + time) + 5 * Math.Cos(3 + time));

            if (chart1.Series[0].Points.Count > 100)
                chart1.Series[0].Points.RemoveAt(0);

            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = time;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart1.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;


            chart2.Series[0].Points.AddXY(time, 3 * Math.Sin(5 + time) + 5 * Math.Cos(3 + time));

            if (chart2.Series[0].Points.Count > 100)
                chart2.Series[0].Points.RemoveAt(0);

            chart2.ChartAreas[0].AxisX.Minimum = (double)chart2.Series[0].Points[0].XValue;
            chart2.ChartAreas[0].AxisX.Maximum = (float)time;
            chart2.ChartAreas[0].AxisX.LabelStyle.Format = "0.0";
            chart2.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;


            //speed
            circularProgressBar2.Value = (int)speed_;
            label12.Text = speed_.ToString();
            //battery percentage
            label8.Text =  "%" + battery_percentage.ToString();
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
            //internal fault
            textBox6.Text = internal_faults.ToString();



            time += 0.1;
        }



        void refresh_ports()
        {
            string[] ports = SerialPort.GetPortNames();
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

        private void SeriPortCom_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("Success");

                serialPort1.Read(bytes_received, 0, 20);
                uint t = 0;

                time_stamp = (char)0;

                //Console.WriteLine(text);
                
                time_stamp = Convert.ToChar(bytes_received[0]);

                Console.WriteLine(time_stamp);

                if (time_stamp == '$')
                {
                    t = 0;
                    motor_rpm = 0;
                    t = t | bytes_received[2];
                    t = t << 8;
                    motor_rpm = t | bytes_received[1];
                    Console.WriteLine(motor_rpm);
                    //circularProgressBar1.Value = (int)motor_rpm;


                    vsm = 0;
                    vsm = bytes_received[3];
                    Console.WriteLine(vsm);

                    internal_faults = 0;
                    internal_faults = bytes_received[4];
                    Console.WriteLine(internal_faults);

                    battery_percentage = 0;
                    battery_percentage = bytes_received[5];
                    Console.WriteLine(battery_percentage);

                    t = 0;
                    battery_voltage = 0;
                    t = t | bytes_received[7];
                    t = t << 8;
                    battery_voltage = t | bytes_received[6];
                    Console.WriteLine(battery_voltage);

                    battery_current = 0;
                    battery_current = bytes_received[8];
                    Console.WriteLine(battery_current);

                    battery_heat = 0;
                    battery_heat = bytes_received[9];
                    Console.WriteLine(battery_heat);


                    motor_heat = 0;
                    motor_heat = bytes_received[10];
                    Console.WriteLine(motor_heat);

                    inverter_heat = 0;
                    inverter_heat = bytes_received[11];
                    Console.WriteLine(inverter_heat);

                    speed_ = 0;
                    speed_ = bytes_received[12];
                    Console.WriteLine(speed_);
                    

                    char fault_message = (char)0;
                    fault_message = (char)bytes_received[13];
                    Console.WriteLine(fault_message);


                    




                    richTextBox1.Invoke(new EventHandler(delegate
                    {
                        richTextBox1.AppendText(time_stamp + "  ");
                        richTextBox1.AppendText(motor_rpm.ToString() + " ");
                        richTextBox1.AppendText(vsm.ToString() + " ");
                        richTextBox1.AppendText(internal_faults.ToString() + " ");
                        richTextBox1.AppendText(battery_percentage.ToString() + " ");
                        richTextBox1.AppendText(battery_voltage.ToString() + " ");
                        richTextBox1.AppendText(battery_current.ToString() + " ");
                        richTextBox1.AppendText(motor_heat.ToString() + " ");
                        richTextBox1.AppendText(inverter_heat.ToString() + " ");
                        richTextBox1.AppendText(speed_.ToString() + " ");
                        richTextBox1.AppendText(fault_message.ToString() + "\n");


                    }));
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
                MessageBox.Show("Couldn't open Port", "Error-322EDA23");
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
                serialPort1.Open();
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


            //img = Image.FromFile(@"D:\Racing\GUI\GUI for Arduino\GUI for Arduino\imgs\ibre.png");

            ////Bitmap bitmap = new Bitmap(pictureBox1.Width,pictureBox1.Height);

            ////Graphics gfx = Graphics.FromImage(bitmap);

            ////gfx.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
            //////gfx.RotateTransform(100);
            ////gfx.DrawImage(img, 0, 0);



            //Bitmap bmp = new Bitmap(img);

            //// whatever your plans where, we don't know ;-)
            //// RectangleF rectf = new RectangleF(640F, 1100F, 0, 0);

            //Graphics g = Graphics.FromImage(bmp);
            //g.TranslateTransform(img.Width, img.Height);

            //// DrawImage needs an image, not a string
            //g.RotateTransform(150);
            //g.DrawImage(new Bitmap(img), new Point(0, 0));


            //// flush is for finishing write operations
            //// dispose is the command to get rid of GDI elements:
            //g.Dispose();

            //pictureBox1.Image = bmp;




        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void circularProgressBar2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart2_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
