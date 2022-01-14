using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transitions;
using System.Windows.Forms;

namespace DACQViewer
{
    public partial class FormDataMotor : Form
    {
        public FormDataMotor()
        {
            InitializeComponent();

            //bikin array control
            Button[] buttonpdfz = new Button[]
            {
                button10,
                button11,
                button12,
                button13,
                button14,
                button15,
                button16,
                button17,
                button18
            };
            TextBox[] textboxz = new TextBox[]
            {
                textBox1,
                textBox2,
                textBox3,
                textBox4,
                textBox5,
                textBox6,
                textBox7,
                textBox8,
                textBox9
            };
            Label[] labelz = new Label[]
            {
                label2,
                label3,
                label4,
                label5,
                label6,
                label8,
                label9,
                label10,
                label7
            };

            //dibikin public global var
            btnViewingPdf = buttonpdfz;
            pdfStatus = textboxz;
            dataLabel = labelz;
        }

        Button[] btnViewingPdf;
        Label[] dataLabel;  // array label text checklist data
        TextBox[] pdfStatus;    // array textbox status warna loaded pdf

        string[] pdfPathDataMotorRoket=new string[9];   // array file pdf & path

        #region LOAD_PDF
        private void button1_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(0);
        }
    
        private void button2_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            browse_pdf_data_srm(8);
        }
        #endregion LOAD_PDF


        #region VIEW_PDF

        private void hideViewPdf(Button btn)
        {
            if (btn.Text == "Hide (pdf)")
            {
                panel2.Hide();
                btnViewingPdf[0].Text = "View (pdf)";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //hideViewPdf(button10);
            view_pdf_data_srm(0);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(1);
            //hideViewPdf(button11);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(2);
            //hideViewPdf(button12);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(3);
            //hideViewPdf(button13);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(4);
            //hideViewPdf(button14);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(5);
            //hideViewPdf(button15);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(6);
            //hideViewPdf(button16);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(7);
            //hideViewPdf(button17);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            view_pdf_data_srm(8);
            //hideViewPdf(button18);
        }
        #endregion VIEW_PDF


        //fungsi load/view
        private void browse_pdf_data_srm(int pdfPathIndex)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "pdf filesx (*.pdf) |*.pdf;";
            ofd.ShowDialog();
            if (ofd.FileName != null)
            {
                pdfPathDataMotorRoket[pdfPathIndex] = ofd.FileName;
                pdfStatus[pdfPathIndex].BackColor = Color.LimeGreen;
            }
        }
        private void view_pdf_data_srm(int pdfPathIndexz)
        {
            if (pdfPathDataMotorRoket[pdfPathIndexz] != null)
            {
                panel2.Show();
                //btnViewingPdf[pdfPathIndexz].Text = "Hide (pdf)";
                axAcroPDF1.LoadFile(pdfPathDataMotorRoket[pdfPathIndexz]);
                //axAcroPDF1.Show();
            }

            else
                MessageBox.Show(dataLabel[pdfPathIndexz].Text + ", belum dipilih !");
        }

        int seqClick = 0;
        public int getSeqClick_FMotor()
        {
            return seqClick;
        }
        //tombol merge-save pdf checklost
        private void button19_Click(object sender, EventArgs e)
        {
            seqClick = 1;
            //copy file pdf ke folder baru
            // atau
            //merge semua pdf+pdf US_REPORT
            //bikin arsip zip isi PDF & CSV file asli

            //FUNGSI MERGE PDF
            //...


            //COPY PDF sajaa
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "pdf files(*.pdf)|*.pdf|All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string filePath = sfd.FileName;
                string filename = Path.GetFileNameWithoutExtension(sfd.FileName);

                File.Copy(pdfPathDataMotorRoket[0], filePath);
                MessageBox.Show("File " + filename + " sudah berhasil disimpan !", "Saving", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


       

       
        //buka pdf pakai aplikasi external
        public static void OpenWithDefaultProgram(string path)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private static bool isOdd(int val)
        {
            return val % 2 != 0;
        }

        int idxButton = 0;
 
        private void button20_Click(object sender, EventArgs e)
        {
            idxButton++;

            if (isOdd(idxButton))   //hiding
            {
                panel2.Visible = false;
                //Transition.run(panel2, "Width", 50, new TransitionType_EaseInEaseOut(500));
                button20.BackgroundImage = Properties.Resources.leftBtn;
            }
            else //showing
            {
                panel2.Visible = true;
                //Transition.run(panel2, "Width", 586, new TransitionType_EaseInEaseOut(500));
                button20.BackgroundImage = Properties.Resources.rightBtn;
            }
        }
    }
}
