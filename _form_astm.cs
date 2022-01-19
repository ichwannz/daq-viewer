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
    public partial class _form_astm : Form
    {
        public _form_astm()
        {
            InitializeComponent();
        }

        public bool setuju;

        private void button1_Click(object sender, EventArgs e)
        {
            setuju = true;
            this.Close();
        }

        public bool getAck()
        {
            return setuju;
        }
    }
}
