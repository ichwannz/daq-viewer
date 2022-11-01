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

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.AcroForms;

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

        string[] pdfPathDataMotorRoket=new string[8];   // array file pdf & path    atau 9?

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
            ofd.Title = "Open file pdf checklist motor roket";
            ofd.Filter = "pdf filesx (*.pdf) |*.pdf;";
            ofd.RestoreDirectory = true;
            ofd.FilterIndex = 1;
            //ofd.ShowDialog(); //dibukak manual

            if (ofd.ShowDialog()==DialogResult.OK)      //dibuka otomatis, sambil cek apakah sudah klik OK
            {
                pdfPathDataMotorRoket[pdfPathIndex] = ofd.FileName;
                pdfStatus[pdfPathIndex].BackColor = Color.LimeGreen;
                pdfStatus[pdfPathIndex].Text = Path.GetFileName(ofd.FileName);

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

        //tombol merge-save pdf checklist
        string pdf_merged_path = "D:/temp4.daqp";
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //FUNGSI MERGE PDF
                PdfDocument pdfMerged = new PdfDocument();

                foreach(string f in pdfPathDataMotorRoket)
                {
                    PdfDocument pdf = PdfReader.Open(f, PdfDocumentOpenMode.Import);

                    int sumPage = pdf.PageCount;
                    for(int a=0; a< sumPage; a++)
                    {
                        PdfPage pg = pdf.Pages[a];
                        pdfMerged.AddPage(pg);
                    }
                }

                //save pdf file (otomatis pakai temp folder)
                pdfMerged.Save(pdf_merged_path);
                MessageBox.Show("File Pdf Checklist Motor Roket sudah berhasil digabung & simpan !");

            }
            catch(Exception err)
            {
                MessageBox.Show("File Pdf Checklist Motor Roket gagal disimpan, Wajib dilengkapi semua, Mohon dicek lagi, bro !" + Environment.NewLine + Environment.NewLine + "Err.Code : " + err.Message, "Hasil", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            //save file ke pakai save dialog (folder)
            //save_file_dialog(pdfMerged);
        }

        private void save_file_dialog(PdfDocument pdfmergedz)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Pdf filez (.pdf)|*.pdf";
                sfd.RestoreDirectory = true;
                sfd.FilterIndex = 1;

                //save dialog : pilih path, nama, ekstensi...
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string fnamePath = sfd.FileName;
                    string fname = Path.GetFileNameWithoutExtension(sfd.FileName);

                    //save pdf
                    pdfmergedz.Save(fnamePath);       // pakai full path

                    MessageBox.Show("File " + fname + ".pdf sudah berhasil disimpan !", "Saving", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }
        }

        private void copy_file_pdf()
        {
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

        //fungsi, tombol & method untuk hide/show panel
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
