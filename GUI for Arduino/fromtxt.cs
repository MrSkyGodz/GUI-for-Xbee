using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GUI_for_Arduino
{
    public partial class fromtxt : Form
    {
        string file;
        double speed = 1;
        double time = 0;
        Cmp cmp = new Cmp();
        
        // time interval
        int interval = 50;

        // datas
        List<Logs> datas = new List<Logs>();

        public fromtxt()
        {
            InitializeComponent();
            timer1.Enabled = false;
            timer1.Interval = interval;
        }

        private void fromtxt_Load(object sender, EventArgs e)
        {

        }
        //select file
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog1.FileName))
            {
                file = openFileDialog1.FileName;
            }

        }
        //read and save data to datas
        private void button2_Click(object sender, EventArgs e)
        {
            datas = new List<Logs>();

            string[] lines = System.IO.File.ReadAllLines(@file);

            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                Logs t = new Logs(data[0], data[1], data[2]);;

                datas.Add(t);
            }

            hScrollBar1.Maximum = datas[datas.Count - 1].time;
            hScrollBar1.Minimum = datas.First().time;

           
        }
        
        private void button4_Click(object sender, EventArgs e)
        { 
            timer1.Enabled = !timer1.Enabled;
        }
        bool flag = true;

        // time line
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            Logs temp = new Logs((int)time);
            
            time = Math.Max(hScrollBar1.Minimum,Math.Min(hScrollBar1.Maximum,(int)time));
            hScrollBar1.Value = (int)time;
            // binary searc for find where am i in data
            int index = datas.BinarySearch(item: temp, comparer: cmp);

            if (index < 0)
            {

                Console.WriteLine(time.ToString() + " " + (~index).ToString() + "\n");
                index = ~index;
            }
            else
            {
                Console.WriteLine(time.ToString() + " " + index.ToString() + "\n");
            }

            if (time % 1000 >= 500 && flag == false)
            {
                flag = true;
                //Console.WriteLine(time.ToString() + " " + (~index).ToString() + "\n");

            }
            else if (time % 1000 < 500 && flag == true)
            {
                flag = false;
                //Console.WriteLine(time.ToString() + " " + (~index).ToString() + "\n");
            }

            time += (interval + Math.Log(datas.Count)/(1e8) + 17) * speed;
        }

        //set time line from scroll
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            time = hScrollBar1.Value;

        }
        //set speed from scroll
        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            //if(hScrollBar2.Value > 0)
            //    speed = Math.Log10(hScrollBar2.Value);
            //else
            //    speed = -Math.Log10(-hScrollBar2.Value);]
            speed = hScrollBar2.Value;
        }
        //default value for speed
        private void button6_Click(object sender, EventArgs e)
        {
            hScrollBar2.Value = (int)1;
        }

       
    }
}