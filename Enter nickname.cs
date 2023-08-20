using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_AutomatiX
{
    public partial class Enter_nickname : Form
    {
        public Enter_nickname()
        {
            InitializeComponent();
        }

        private void create_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string getNickName()
        {
            return textBox1.Text;
        }

        public void setNickName(string nick)
        {
            textBox1.Text = nick;
        }

        //private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        if (!string.IsNullOrEmpty(textBox1.Text))
        //        {
        //            // Здесь можно добавить дополнительные действия по обработке
        //            введенного текста, если необходимо
        //            Close(); // Закрытие формы
        //        }
        //    }
        //}
    }
}
