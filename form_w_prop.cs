using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DACQViewer
{
    public partial class form_w_prop : Form
    {
        public form_w_prop()
        {
            InitializeComponent();
        }

        private string w_prop_gram="0";
        private void button1_Click(object sender, EventArgs e)
        {
            w_prop_gram = textBox1.Text;
            this.Dispose();
        }

        public string get_w_prop
        {
            get { return textBox1.Text;}
            
        }
        public string getWW()
        {
            return w_prop_gram;
        }
    }
}
