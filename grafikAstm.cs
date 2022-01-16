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
    public partial class grafikAstm : Form
    {
        public grafikAstm()
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
