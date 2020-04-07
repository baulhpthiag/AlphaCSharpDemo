using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MysqlDemo
{
    public partial class Form1 : Form
    {
        ParentDao dao = new ParentDao();

        public Form1()
        {
            InitializeComponent();
        }

        private void findButton_Click(object sender, EventArgs e)
        {


        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Parent p = new Parent()
            {
                id = Guid.NewGuid(),
                name = "测试Parent",
                age = 10,
                birthday = DateTime.Now,
                isWorking = true,
                tall = 1.8

            };
            dao.AddParent(p);

            MessageBox.Show("添加完成");
        }

        private void updateButton_Click(object sender, EventArgs e)
        {

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {

        }

        private void lastPageButton_Click(object sender, EventArgs e)
        {

        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {

        }
    }
}
