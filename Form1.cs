using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;
using System.Globalization;

using System.IO;

/*
A-SELASA, 10 Dzulhijjah 1442 / 20 Juli 2021 
* 
* Bismillah 
* 1. Aplikasi "DACQViewer" ===> Data Acq Viewer Uji Statis Solid ROcket Motor ::: ini sebagai aplikasi post-processing (data US sudah tersedia)
* 2. Aplikasi "ACQ Runner" ===> next sebagai aplikasi pre-processing :
*    > Input database pre-US : Process & Manufacture [Propelan, Cap, Tabung, Nosel, Liner]
*                              Testing Verify [X-Ray, DT, NDT, Visual Inspection]
*                              Static Test Parameters [DAQ : ScopeCorder & CDA : Channel, Sampling, Scaling, Filtering, Amplifying, Excitaion]
*                                                     [Sensors : Type, SN, FS/Capacity, Sensitivity, Sign ADC/STRAIN/mAMP]  /// biar penamaan channel gakperlu pakai kapasitas lagi :D
*                                                     [Firing Ignition : Volt, Ampere, Ohm, Countdown time] 
*                                                     [Visual Recorder : Camera, Resolution, Dimension, Duration]
* 
* 
*     
*/

namespace DACQViewer
{
    public partial class Form1 : Form
    {
        baca_csv_ke_tabel bc;

        //untuk akses seqclick dari masing2 Formz
        FormParams ftabel;
        FormDataMotor fmotor;
        FormChart0 fchart0;
        //formchart1 fgrafik2;
        FormReport freport;
        //formZipFInal fzip;

        public Form1()
        {
            //tes sukses clear synchronize SF3<>github<>GSXLAPAN
            //Alhamdulillah

            InitializeComponent();
            showSubMenu(false);
            hideButton(true);
            //btnLoad.Enabled = false;

            //Judul (panel atas)
            label17.Visible = false;
            label18.Visible = false;

            panWelcome.Visible = false;
            panEngineerID.Visible = false;
            panFooter.Size = new Size(1069, 50);

            textBox5.CharacterCasing = CharacterCasing.Upper;
        }

        public DataTable dtDAQSampling = new DataTable();
        public DataTable dtParamsChannel = new DataTable(); //untuk save hasil saving
        public DataTable dtParamsFiring = new DataTable();

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

     #region FUNGSI_UTAMA_DACQ

        /* Variabel untuk store Data Table <=== CSV */
        DataTable dtRekap = new DataTable();
        DataTable dtRekapNum = new DataTable();

        private string[] idChannel;
        private string[] unitChannelStr;

        private int jumChannel;
        private int jumDataRow;
        private int sampleRate;

        private string daqID, roketID, dateID, timeID;

        
        private void ambil_csv_table()
        {
            Stream mystream = null;
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "CSV Files (*.csv)|*.csv|All Filess|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((mystream = ofd.OpenFile()) != null)
                    {
                        using (mystream)
                        {
                            bc = null;
                            bc = new baca_csv_ke_tabel(mystream);
                            mystream.Close();
                        }
                    }
                }

                catch (Exception err) { MessageBox.Show(err.Message); }
            }

            try
            {
                //Ambil DataTable : Binding
                dtRekap = bc.get_dataRekap();
                
                //HEADER
                daqID = bc.get_daqID();
                roketID = bc.get_roketID();
                dateID = bc.get_dateID();
                timeID = bc.get_timeID();
                
                //Ambil DATA_HEADER
                jumDataRow = bc.get_jumlahData() - 1;
                unitChannelStr = bc.get_channelUNIT();
                idChannel = bc.get_channelID();
                unitChannelStr = bc.get_channelUNIT();
                jumChannel = bc.get_jumlahCh();
                sampleRate = int.Parse(bc.get_Sps());

                //tambah/insert kolom Nomer urut di Column0
                dtRekapNum = dtRekap.Copy();
                dtRekapNum.Columns.Add("No.Data", typeof(int)).SetOrdinal(0);
                for (int a = 0; a < jumDataRow; a++)
                    dtRekapNum.Rows[a][0] = a;

                //debung value via textbox
                string csvPath = ofd.FileName;

            }
            catch (Exception) //err
            {
                //MessageBox.Show("Isi dulu bagian Comment D850 ! <ERROR_PARSE>");     // err.Message);
                MessageBox.Show("TIDAK BERHASIL MENGAMBIL FILE ! <ERROR_STREAM>","Wassalam..",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                Application.Exit();
                Environment.Exit(0);
            }
        }
     # endregion FUNGSI_UTAMA_DACQ
       
     #region FUNGSI_UI-VIEW-HIDE-UX
        private void hideButton(bool val)
        {
            if(val)
                seqClick = 0;

            btnParamsInput.ForeColor = Color.DimGray;
            btnDataMotor.ForeColor = Color.DimGray;
            btnGrafik.ForeColor = Color.DimGray;
            btnReport.ForeColor = Color.DimGray;
            btnZip.ForeColor = Color.DimGray;
        }

        private void showSubMenu(bool val)
        {
            panSubAnalyze.Visible = val;
        }

        private void hidingSubMenu()
        {
            if (panSubAnalyze.Visible) panSubAnalyze.Visible = false;
        }

        private void showingSubMenu(Panel subMenu)
        {
            if (!subMenu.Visible)
            {
                hidingSubMenu();
                subMenu.Visible = true;

            }
            else
                subMenu.Visible = false;
        }

        private void coloring(Button tombol)
        {
            Font ftBold = new Font(tombol.Font.Name, tombol.Font.Size, FontStyle.Bold);
            Font ftNorm = new Font(tombol.Font.Name, tombol.Font.Size, FontStyle.Regular);

            btnLoad.ForeColor = Color.DimGray;
            btnParamsInput.ForeColor = Color.DimGray;
            btnDataMotor.ForeColor = Color.DimGray;
            btnGrafik.ForeColor = Color.DimGray;
            btnGrafIgn.ForeColor = Color.DimGray;
            btnGrafThrust.ForeColor = Color.DimGray;
            btnReport.ForeColor = Color.DimGray;
            btnZip.ForeColor = Color.DimGray;

            tombol.ForeColor = Color.Gold;
            tombol.Font = ftBold;

        }
  
     #endregion FUNGSI_VIEW-HIDE-UX

     #region MAIN_BUTTON
        // MAIN-BUTTON
        private int seqClick = 0;
        private bool flag_logged_in = false;

        private void btnLoad_Click(object sender, EventArgs e)
        {
            coloring(btnLoad);


            //setting datetime lokal Indonesia
            CultureInfo lokalID = new System.Globalization.CultureInfo("id-ID");
            string hariUji, tglUji;
            
            if (flag_logged_in)
            {
                showSubMenu(false);
                hideButton(true);
                
                //Panggil fungsi Load_CSV
                ambil_csv_table();      
                
                //bikin windows auto maximize setelah load file csv
                this.WindowState = FormWindowState.Maximized;

                //show judul
                DateTime dayTime = DateTime.ParseExact(dateID, "yyyy/MM/dd",CultureInfo.InvariantCulture);
                hariUji = lokalID.DateTimeFormat.GetDayName(dayTime.DayOfWeek).ToString();
                tglUji = dayTime.ToString("dd MMM yyyy");
                
                label17.Text = "Motor Roket : "+roketID;
                label18.Text = "Waktu Uji : " + hariUji + ", " + tglUji + ", " + timeID;
                label17.Visible = true;
                label18.Visible = true;

                //langsung buka tabel
                seqClick = 1;

                btnParamsInput_Click(sender, e);
            }
            else
                MessageBox.Show("Anda belum login, Bro !");
        }
    
        private void btnParamsInput_Click(object sender, EventArgs e)
        {
            if(seqClick == 1)
            {
                coloring(btnParamsInput);
                openMainForm(new FormParams(dtRekapNum, daqID, roketID, dateID, timeID, sampleRate, jumChannel, idChannel, unitChannelStr));     // start form baru
            }
            else
                MessageBox.Show("File Data Akuisisi US (Csv) belum dipilih, bro !");
        }

        private void btnDataMotor_Click(object sender, EventArgs e)
        {
            //seqClick = ftabel.getSeqClick_FTabelz() + 1;
            if(seqClick==1)
            {
                coloring(btnDataMotor);
                openMainForm(new FormDataMotor());
            }
            else
                MessageBox.Show("Data Parameter US belum dilengkapi, bro !");
        }

        private void btnGrafik_Click(object sender, EventArgs e)
        {
            //seqClick = fmotor.getSeqClick_FMotor() + 1;
            if(seqClick==1)
            {
                coloring(btnGrafik);
                showingSubMenu(panSubAnalyze);
                btnGrafIgn_Click(sender, e);
            }
            else
                MessageBox.Show("Data Motor Roket (Checklist) belum lengkap, bro !");
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            //seqClick = fchart0.getSeqClick_FChart0() + 1;
            if(seqClick==1)
            {
                coloring(btnReport);
                openMainForm(new FormReport());
            }
            else
                MessageBox.Show("Grafik Analisis belum diselesaikan, bro !");
        }
        private void btnZip_Click(object sender, EventArgs e)
        {
            //seqClick = freport.getSeqClick_FReport() + 1;
            if (seqClick == 1)
            {
                hidingSubMenu();
                coloring(btnZip);
                openMainForm(new FormArsipZip());
            }
            else
                MessageBox.Show("Form Laporan Uji Statis (pdf) belum selesai, bro !");
        }

        #endregion MAIN_BUTTON




        #region SUB_BUTTON
        //Setiap Sub-Button click_events langsung create Form Baru 
        //object form untuk simpan masing2 kondisi form, jadi gak perlu load eksekusi lagi dari awal (csv, tabel & chart)

        List<Form> listForm = new List<Form>();
        int indexActForm = 0;
        int indexListForm = 0;

        private Form activeFormz = null;
        private Form prevForm = null;

        
        private void openMainForm(Form formz)
        {
            listForm.Add(formz);
            indexListForm++;

            //manual disave dulu kondisi lastForm yg diload, pakai indexListForm
            //listForm.Insert(indexListForm, formz);

            //active form diclose dulu..kalau ada
            if (activeFormz != null)
            {
                activeFormz.Close();
                //activeFormz.Hide();                    
                if (prevForm == formz)
                    prevForm.Close();
            }

            //mainFormz dijadikan activeForm
            activeFormz = formz;

            formz.TopLevel = false;
            formz.FormBorderStyle = FormBorderStyle.None;
            formz.Dock = DockStyle.Fill;

            // tambah(~posisikan) form ke panelMainView
            panMainView.Controls.Add(formz);
            panMainView.Tag = formz;
            formz.BringToFront();
            formz.Show();
        }

        private void btnGrafIgn_Click(object sender, EventArgs e)
        {
            coloring(btnGrafIgn);
            openMainForm(new FormChart0(dtRekap, jumDataRow, unitChannelStr, idChannel, jumChannel, sampleRate));
        }

        private void btnGrafThrust_Click(object sender, EventArgs e)
        {
            coloring(btnGrafThrust);
            openMainForm(new ADMIN());
        }
     #endregion SUB_BUTTON


     #region BUTTON_NAVIGASI

        /* Button navigasi*/
        private const int PANEL_WELC_HIDE = 52;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (cbDEBUG_MODE.Checked)
            {
                textBox5.Text = "ADIT";
                textBox6.Text = "2040";
            }

            if (textBox6.Text == "2040" && textBox5.Text != " ")
            {
                // Transition.run(panMainView, "BackColor", Color.Green, new TransitionType_Linear(1000));
                Transition.run(panFooter,"Height",100, new TransitionType_EaseInEaseOut(1000));
                Transition.run(panWelcome, "Height", 52, new TransitionType_EaseInEaseOut(300));
                //...

                MessageBox.Show($"Selamat datang, {textBox5.Text} !!","",MessageBoxButtons.OK, MessageBoxIcon.Information);
                flag_logged_in = true;

                textBox1.Text = textBox5.Text;   //ID_User
                textBox4.Text = "Engineer Staff US-1";  //User positiion in OFK
                textBox3.Text = "Electronics Eng";// User Expertise
                textBox2.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");  // Tanggal & waktu sekarang, ambil dari PC

                label15.Text = $"Selamat datang, {textBox5.Text} ";
                label15.TextAlign = ContentAlignment.MiddleRight;

                panWelcome.Visible = true;
                panEngineerID.Visible = true;   //panel ID Engineer kanan bawah pojok
                //panFooter.Size = new Size(1069, 100); // Balikin ke semula
                gbLogin.Visible = false;
                btnLoad_Click(sender, e);
            }

            else
                MessageBox.Show("Password &/atau Username Salah !"," ",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        List<Form> formDetached = new List<Form>();
        private void btnDetach_Click(object sender, EventArgs e)
        {
            //Form Ftemp = new Form();
            //Ftemp = activeFormz;

            prevForm = activeFormz;
            indexActForm++;

            if(activeFormz!=null)
            {
                panMainView.Controls.Remove(prevForm);

                //setting hasil detach window
                prevForm.TopLevel = true;
                prevForm.FormBorderStyle = FormBorderStyle.Sizable;
                prevForm.Dock = DockStyle.Fill;
                prevForm.StartPosition = FormStartPosition.CenterScreen;

                prevForm.BringToFront();
                prevForm.Show();
                activeFormz = null;
            }
            formDetached.Add(prevForm);
        }
     
     #endregion BUTTON_NAVIGASI

        /*Status waktu saat ini*/
        private void timer1_Tick(object sender, EventArgs e)
        {
            label9.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");//DateTime.Today.ToLongDateString();
        }
    }
}
