using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void taskDemoButton_Click(object sender, EventArgs e)
        {

            Task tempTask = Task.Factory.StartNew(() =>
            {
               
                for (int i = 0; i < 1000; i++)
                {
                    Console.WriteLine("task"+i.ToString());
                    this.Invoke((Action)delegate ()
                    {
                        this.textBox1.Text = "task " + i.ToString() + Environment.NewLine + this.textBox1.Text;
                    });
                }
               
            });

            tempTask.ContinueWith((r) =>
            {
                this.Invoke((Action)delegate ()
                {
                    //MessageBox.Show("task continueWith");
                    this.textBox1.Text = "task continueWith" + Environment.NewLine + this.textBox1.Text;
                });

            });

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("main" + i.ToString());
                this.textBox1.Text = "main " + i.ToString() + Environment.NewLine + this.textBox1.Text;
            }

        }
    }
}
