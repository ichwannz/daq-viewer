using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

namespace DACQViewer
{
    public partial class FormReport : Form
    {
        public FormReport()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "daq files(*.daq)|*.daq|All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if(sfd.ShowDialog()==DialogResult.OK)
            {
                if((myStream=sfd.OpenFile()) != null)
                {
                    //Writing algorithm
                    myStream.Close();
                }
                string filename = Path.GetFileNameWithoutExtension(sfd.FileName);
                MessageBox.Show("File " + filename +" sudah berhasil disimpan !","Saving", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        int seqClick = 0;
        public int getSeqClick_FReport()
        {
            return seqClick;
        }
        private void btnMakeZip_Click(object sender, EventArgs e)
        {
            seqClick = 1;
            //...
        }
    }
}
