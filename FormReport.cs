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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.AcroForms;

namespace DACQViewer
{
    public partial class FormReport : Form
    {
        public FormReport()
        {
            InitializeComponent();

            open_default_form_us();
        }

        string path_us_form_def;
        private void open_default_form_us()
        {
            try
            {
                string loc = System.Reflection.Assembly.GetEntryAssembly().Location;    //ambil Main Path saat program exe dibuka plus file.exe
                string execLoc = Path.GetDirectoryName(loc);                            // ambil path saja tanpa file.exe
                path_us_form_def = string.Concat(execLoc,"\\Resources\\usformv3.daqp");        //file usform.daqp sudah diinclude di Resources (properties: Copy Always)
            
                //axAcroPDF1.LoadFile("D:/us_form.usdaq");
                axAcroPDF1.LoadFile(path_us_form_def);
                //axAcroPDF1.Show();
            }
            catch(Exception err)
            {
                MessageBox.Show("Gagal membuka Form Data Uji Statis !");
            }
            
        }

        //isi form data us
        string path_pdf_us_filled = "D:/temp7.daqp";// untuk setor file pdf Form Data US yg sudah terisi
        private void isi_form_us()
        {
            // 1-isi data total ke form aktif ( akses semua file temporary csv....)
            // 2-preview

            PdfDocument usform_template = PdfReader.Open(path_us_form_def, PdfDocumentOpenMode.Modify);
            PdfAcroForm form = usform_template.AcroForm;

            //cek elemen NeedAppearance dari file activeform pdf (harus true)
            if (form.Elements.ContainsKey("/NeedAppearances"))
                form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
            else
                form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));
            
           

            //isikan field ke PdfField
            PdfTextField field0 = (PdfTextField)(form.Fields["text_kodeus"]);
            PdfTextField field1 = (PdfTextField)(form.Fields["text_tanggal"]);
            PdfTextField field2 = (PdfTextField)(form.Fields["text_scope1"]);

            /*
            if(field0.Elements.ContainsKey("/DA")==false)
            {
                field0.Elements.Add("/DA", new PdfString("/CoBo 12 If 0 g"));
            }
            else
                field0.Elements["/DA"] = new PdfString("/CoBo 12 Tf 0 g");
            */
            //setel font
            field0.Font = new Font("HP Simplified", 10, FontStyle.Bold, GraphicsUnit.World);
            field1.Font = new Font("HP Simplified", 10, FontStyle.Bold, GraphicsUnit.World);
            field2.Font = new Font("HP Simplified", 12, FontStyle.Bold, GraphicsUnit.World);
 
            //isikan nilai ke PdfString
            PdfString fd0 = new PdfString("US.RX320.21.03");
            PdfString fd1 = new PdfString("Seloso");
            PdfString fd2 = new PdfString("DL850");
            //binding string ke field
            field0.Value = fd0;
            field1.Value = fd1;
            field2.Value = fd2;

            //setel form R/O
            field0.ReadOnly = true;
            field1.ReadOnly = true;
            field2.ReadOnly = true;
            
            usform_template.Save(path_pdf_us_filled);

            /*
            try
            {
                
            }
            catch(Exception err)
            {
                MessageBox.Show("Gagal mengisi dan membuka Form Data US (pdf)" + Environment.NewLine + Environment.NewLine + err.Message);
            }
            */





        }

        private void view_form_us_pdf()
        {
            try
            {
                axAcroPDF1.LoadFile(path_pdf_us_filled);
            }
            catch(Exception err)
            {
                MessageBox.Show("Form Data US (file pdf) gagal ditampilkan");
            }
        }







        //tombol tombol

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

       
        private void btnMakeZip_Click(object sender, EventArgs e)
        {

        }

        private void btnIsiForm_Click(object sender, EventArgs e)
        {
            isi_form_us();
            view_form_us_pdf();
        }
    }
}
