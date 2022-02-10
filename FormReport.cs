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
using PdfSharp.Pdf.Annotations;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Security;

namespace DACQViewer
{
    public partial class FormReport : Form
    {
        public FormReport()
        {
            InitializeComponent();
            //ambil path default (lokasi file exe)
            string loc = System.Reflection.Assembly.GetEntryAssembly().Location;    //ambil Main Path saat program exe dibuka plus file.exe
            string execLoc = Path.GetDirectoryName(loc);                            // ambil path saja tanpa file.exe
            
            //ambil path usform_template (form kosongan)
            path_us_form_kosong = String.Concat(execLoc, "\\Resources\\usformv3.daqp");        //file usform.daqp sudah diinclude di Resources (properties: Copy Always)
            //tampilkan form kosongan
            open_default_form_us();
        }

        private void cek()
        {
            string kodeOri="123456789";
            string kodeOk;
            kodeOk = fill_sisa(kodeOri, 15);
        }

        private String fill_sisa(string str, int full)
        {
            string strOk="";

            if(str.Length < full)       //9<15
            {
                for (int b = 0; b < full; b++)  //0 < 15
                {
                    strOk = String.Concat(str, " ");
                    if (strOk.Length < full)
                        b++;

                }
            }
            return strOk;
        }

        string path_us_form_kosong;

        //baca csv database
        string[] csv_path = new string[]
        {
            "D:/temp0.daqc",    //dtHasil0 = header DAQ Scope
            "D:/temp1.daqc",    //sensor value  (dgv)
            "D:/temp2.daqc",    //parameter daq (dgv)
            "D:/temp3.daqc",    //parameter firing
            "D:/temp4.daqp",            //pdf checklist gabungan (file pdf)
            "D:/temp5.daqc",    //time sequence us
            "D:/temp6.daqc",    //hasil hitung
            "D:/temp7.daqp",            //pdf form data us yg terisi
        };

        string pdf_path = "D:/temp8.daqp";  //pdf hasil merged USFORM & CHECKLIST

        DataTable[] dt = new DataTable[6]; 
        /*
         * {
            dt[0]   => temp0
            dt[1]   => temp1
            dt[2]   => temp2
            dt[3]   => temp3
            dt[4]   => temp5
            dt[5]   => temp6
        }
        */

        private DataTable baca_csv(int a)
        {
            DataTable dtt = new DataTable();
            //isi ke dt
            File.ReadLines(csv_path[a]).Take(1)
                .SelectMany(x => x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dtt.Columns.Add(x.Trim()));

            File.ReadLines(csv_path[a]).Skip(1)
                .Select(x => x.Split(';'))
                .ToList()
                .ForEach(line => dtt.Rows.Add(line));

            return dtt;
        }

        //field namelist  ada 37 field
        string[] nameFld_header = new string[3]
                {
                    "text_kodeus",
                    "text_tanggal",
                    "text_waktu"
                };
        string[] nameFld_daq = new string[10]
                {
                    //Tipe, merk
                    "text_scope1",      
                    "text_loadcell1",
                    "text_pressure1",
                    "text_suhu1",
                    "text_strain1",
                
                    //Serial Number
                    "text_scope2",      
                    "text_loadcell2",
                    "text_pressure2",
                    "text_suhu2",
                    "text_strain2",
                };
        string[] nameFld_firing = new string[3]
        {
            "text_volt",
            "text_ohm1",
            "text_ohm2"
        };
        string[] nameFld_motor = new string[7]
        {
            "text_kodemotor",
            "text_kodetabung",
            "text_kodenosel",
            "text_kodecap",
            "text_kodeign",
            "text_kodeprop",
            "text_kodeasm"
        };
        string[] nameFld_operasi = new string[6]
        {
            //before uji
            "text_berat1",
            "text_diameter1",
            "text_cog1",
            //after uji
            "text_berat2",
            "text_diameter2",
            "text_cog2",
        };
        string[] nameFld_hasil = new string[8]
        {
            "text_tign",
            "text_tburn",
            "text_fmax",
            "text_fave",
            "text_itot",
            "text_isp",
            "text_pmax",
            "text_pave"
        };

        //value
        string[] valueFld_header=new string[3];
        string[] valueFld_daq=new string[10];
        string[] valueFld_firing=new string[3];
        string[] valueFld_motor=new string[7];
        string[] valueFld_operasi=new string[6];
        string[] valueFld_hasil=new string[8];

        //string path_pdf_us_filled = "D:/temp7.daqp";// untuk setor file pdf Form Data US yg sudah terisi

        private void isi_str()
        {
            //1. cek apakah value di datatable bernilai 'null'? konvert ke nilai "N/A" (string)
            //2. masukkan ke textfield
            //3. jika textfield tidak ada di datatable? konvert ke "N/A" (string)
            DataTable[] dtx=new DataTable[6];

            dtx[0] = baca_csv(0);
            dtx[1] = baca_csv(1);
            dtx[2] = baca_csv(2);
            dtx[3] = baca_csv(3);
            dtx[4] = baca_csv(5);
            dtx[5] = baca_csv(6);

            //header    
            valueFld_header[0] = dtx[0].Rows[1][1].ToString();  //kode us
            valueFld_header[1] = dtx[0].Rows[2][1].ToString();  //date
            valueFld_header[2] = dtx[0].Rows[3][1].ToString();  //time

            //daq
            valueFld_daq[0] = dtx[0].Rows[0][1].ToString();         //scope
            valueFld_daq[1] = dtx[2].Rows[0][4].ToString();         //loadcell
            valueFld_daq[2] = dtx[2].Rows[1][4].ToString();         //pressure
            valueFld_daq[3] = "N/A";         //suhu
            valueFld_daq[4] = "N/A";         //strain

            valueFld_daq[5] = "N/A";
            valueFld_daq[6] = dtx[2].Rows[0][5].ToString();
            valueFld_daq[7] = dtx[2].Rows[1][5].ToString();
            valueFld_daq[8] = "N/A";
            valueFld_daq[9] = "N/A";

            //firing
            valueFld_firing[0] = dtx[3].Rows[1][1].ToString();  //volt
            valueFld_firing[1] = dtx[3].Rows[3][1].ToString();  //ohm wire
            valueFld_firing[2] = dtx[3].Rows[2][1].ToString();  //ohm ign

            //data motor
            valueFld_motor[0] = ""; 
            valueFld_motor[1] = "";
            valueFld_motor[2] = "";
            valueFld_motor[3] = "";
            valueFld_motor[4] = "";
            valueFld_motor[5] = "";
            valueFld_motor[6] = "";

            //data operasi uji
            valueFld_operasi[0] = "N/A";
            valueFld_operasi[1] = "N/A";
            valueFld_operasi[2] = "N/A";
            valueFld_operasi[3] = "N/A";
            valueFld_operasi[4] = "N/A";
            valueFld_operasi[5] = "N/A";

            //data hasil analisis
            valueFld_hasil[0] = dtx[5].Rows[0][1].ToString();
            valueFld_hasil[1] = dtx[5].Rows[1][1].ToString();
            valueFld_hasil[2] = dtx[5].Rows[5][1].ToString();
            valueFld_hasil[3] = dtx[5].Rows[6][1].ToString();
            valueFld_hasil[4] = dtx[5].Rows[3][1].ToString();
            valueFld_hasil[5] = dtx[5].Rows[4][1].ToString();
            valueFld_hasil[6] = dtx[5].Rows[7][1].ToString();
            valueFld_hasil[7] = dtx[5].Rows[8][1].ToString();
        }

        bool _EDITABLE;
        private void isi_form_data_us()
        {
            PdfDocument usform = PdfReader.Open(path_us_form_kosong, PdfDocumentOpenMode.Modify);
            PdfAcroForm form = usform.AcroForm;
            PdfPage pageGrafik = usform.Pages[1];   //letak img chart nantinya
            PdfSecuritySettings security_form = usform.SecuritySettings;

            //cek elemen NeedAppearance dari file activeform pdf (harus true)
            if (form.Elements.ContainsKey("/NeedAppearances"))
                form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
            else
                form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));

            //binding ke string
            isi_str();

            //binding ke form
            baca_hh(form, nameFld_header, valueFld_header);
            baca_hh(form, nameFld_daq, valueFld_daq);
            baca_hh(form, nameFld_firing, valueFld_firing);
            baca_hh(form, nameFld_motor, valueFld_motor);
            baca_hh(form, nameFld_operasi, valueFld_operasi);
            baca_hh(form, nameFld_hasil, valueFld_hasil);
            

            //skip signFIeld
            PdfSignatureField signImg = (PdfSignatureField)(form.Fields["para_grafik"]);
            PdfString signImgStr = new PdfString("Grafik Data Uji Statik");
            signImg.Value = signImgStr;
            signImg.ReadOnly = true;

            //addimage
            draw_img(pageGrafik, int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text));

            //add catatan
            PdfTextField notePengujian = (PdfTextField)form.Fields["para_catatan"];
            PdfString noteStr = new PdfString("N/A");
            notePengujian.Value = noteStr;
            notePengujian.ReadOnly = true;

            /*--------------------------------------------------------------------------------Properties ---*/
            //pdf info
            usform.Info.Title = "Form Data Hasil Uji Statis Motor Roket Padat";
            usform.Info.Author = "Tim Data US Pustekroket LAPAN";
            usform.Info.Subject = "Generated by Aplikasi DACQViewer Pustekroket 2020";
            usform.Info.Keywords = "Uji Statis, Olah Data, Analisis, Roket, LAPAN";

            //security

            /*
             * Setting one of the passwords automatically sets the security level to 
             * PdfDocumentSecurityLevel.Encrypted128Bit;
            */

            //cek apakah textbox password kosong atau ada spasi
            string pw = String.Concat(textBox5.Text.Where(a => !Char.IsWhiteSpace(a)));

            if(pw!="")
            {
                security_form.UserPassword = textBox5.Text;
                security_form.OwnerPassword = textBox5.Text;
            }

            // Don't use 40 bit encryption unless needed for compatibility
            //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

            // Restrict some rights.

            security_form.PermitAccessibilityExtractContent = false;
            security_form.PermitAnnotations = false;
            security_form.PermitAssembleDocument = false;
            security_form.PermitExtractContent = false;
            security_form.PermitFormsFill = true;
            security_form.PermitFullQualityPrint = false;
            security_form.PermitModifyDocument = false;     // = true;
            security_form.PermitPrint = false;

            //save file
            usform.Save(csv_path[7]);
            //open with external
            //Process.Start(csv_path[7]);
        }

        
        private void baca_hh(PdfAcroForm formX, string[] grupField, string[] valueStr)
        {
            int sum = grupField.Count();

            //cek grup field yang bisa diedit/isi scr manual
            if (grupField == nameFld_motor)
                _EDITABLE = true;

            PdfTextField[] textFld = new PdfTextField[sum];
            PdfString[] textStr = new PdfString[sum];

            //binding otomatis
            for (int a = 0; a < sum; a++)
            {
                textFld[a] = (PdfTextField)(formX.Fields[grupField[a]]);                //field name
                textStr[a] = new PdfString(valueStr[a]);
                textFld[a].Value = textStr[a];

                if (_EDITABLE)
                    textFld[a].ReadOnly = false;
                else
                    textFld[a].ReadOnly = true;
            }

            _EDITABLE = false;  //kembalikan ke default (readonly)
        }
        private void draw_img(PdfPage pg, int x, int y, int width, int height)
        {
            XGraphics gfx = XGraphics.FromPdfPage(pg);

            //BeginBox(gfx, 1, "Grafik US");

            XImage img = XImage.FromFile(imgPath);
            gfx.DrawImage(img, x,y,width,height);

            //EndBox(gfx);

        }


        private void open_default_form_us()
        {
            try
            {
                axAcroPDF1.LoadFile(path_us_form_kosong);
                //axAcroPDF1.Show();
            }
            catch(Exception err)
            {
                MessageBox.Show("Gagal membuka Form Data Uji Statis !");
            }
            
        }

        //isi form data us DUMMY
        private void isi_form_dummy()
        {
            // 1-isi data total ke form aktif ( akses semua file temporary csv....)
            // 2-preview

            PdfDocument usform_template = PdfReader.Open(path_us_form_kosong, PdfDocumentOpenMode.Modify);
            PdfAcroForm form = usform_template.AcroForm;
            PdfPage pageGrafik = usform_template.Pages[2];   //letak img chart nantinya

            //cek elemen NeedAppearance dari file activeform pdf (harus true)
            if (form.Elements.ContainsKey("/NeedAppearances"))
                form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
            else
                form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));

            //isikan field ke PdfField
            PdfTextField field0 = (PdfTextField)(form.Fields["text_kodeus"]);
            field0.Font = new Font("HP Simplified", 10, FontStyle.Bold, GraphicsUnit.World);            //setel font
            PdfString fd0 = new PdfString("US.RX320.21.03");    //isikan nilai ke PdfString

            //binding string ke field
            field0.Value = fd0;
            field0.ReadOnly = true; //setel form R/O

            double[] re = new double[4];

            //image adding
            PdfSignatureField signf = (PdfSignatureField)form.Fields["sign_grafik"];
            PdfRectangle rect = signf.Elements.GetRectangle(PdfAnnotation.Keys.Rect);

            draw_img(pageGrafik, 10, 10, 16, 32);
                
            //document saving

            usform_template.Info.Title = "Form Data Hasil Uji Statis Motor Roket Padat";
            usform_template.Info.Author = "Tim Data US Pustekroket LAPAN";
            usform_template.Info.Subject = "Generated by Aplikasi DACQViewer Pustekroket 2020";
            usform_template.Info.Keywords = "Uji Statis, Olah Data, Analisis, Roket, LAPAN";
            usform_template.Save(csv_path[7]);
        }

        string imgPath = "D:/img.png";
       

        /*
        public void BeginBox(XGraphics gfx, int number, string title)
        {
            const int dEllipse = 15;
            XRect rect = new XRect(0, 20, 300, 200);
            if (number % 2 == 0)
                rect.X = 300 - 5;
            rect.Y = 40 + ((number - 1) / 2) * (200 - 5);
            rect.Inflate(-10, -10);
            XRect rect2 = rect;
            rect2.Offset(this.borderWidth, this.borderWidth);
            gfx.DrawRoundedRectangle(new XSolidBrush(this.shadowColor), rect2, new XSize(dEllipse + 8, dEllipse + 8));
            XLinearGradientBrush brush = new XLinearGradientBrush(rect, this.backColor, this.backColor2, XLinearGradientMode.Vertical);
            gfx.DrawRoundedRectangle(this.borderPen, brush, rect, new XSize(dEllipse, dEllipse));
            rect.Inflate(-5, -5);

            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            gfx.DrawString(title, font, XBrushes.Navy, rect, XStringFormats.TopCenter);

            rect.Inflate(-10, -5);
            rect.Y += 20;
            rect.Height -= 20;

            this.state = gfx.Save();
            gfx.TranslateTransform(rect.X, rect.Y);
        }
        
        public void EndBox(XGraphics gfx)
        {
            gfx.Restore(this.state);
        }
        */


        //preview form data us
        private void view_usform_pdf()
        {
            try
            {
                axAcroPDF1.LoadFile(csv_path[7]);
            }
            catch(Exception err)
            {
                MessageBox.Show("Form Data US (file pdf) gagal ditampilkan");
            }
        }
        

        //tombol tombol

        private void save_pdf_dialog()
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "daq files(*.daq)|*.daq|All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = sfd.OpenFile()) != null)
                {
                    //Writing algorithm
                    myStream.Close();
                }
                string filename = Path.GetFileNameWithoutExtension(sfd.FileName);
                MessageBox.Show("File " + filename + " sudah berhasil disimpan !", "Saving", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            //save_pdf_dialog();
            //update file pdf csv_path[7] dari adobe??
        }

        private void btnMakeZip_Click(object sender, EventArgs e)
        {

        }

        private void btnIsiForm_Click(object sender, EventArgs e)
        {
            //isi_form_us();
            isi_form_data_us();
            view_usform_pdf();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(csv_path[4]))
                    axAcroPDF2.LoadFile(csv_path[4]);   //checklist pdf
                else
                    MessageBox.Show("Data Checklist (file pdf) tidak ditemukan, bro!");

            }
            catch (Exception err)
            {
                MessageBox.Show("Data Checklist (file pdf) tidak dapat dibuka, bro!");
            }

        }
    }
}
