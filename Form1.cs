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
        int psize_w, psize_h;

        public Form1()
        {
            InitializeComponent();

            bt_coloring(null); //bikin tombol jadi dimgray BG.Color
            psize_w = panelSideMenu.Width;
            psize_h = panelSideMenu.Width;
            panelSideMenu.Size = new Size(50, psize_h);
            panelSideMenu.Visible = false;

            btnDetach.Visible = false;
            panJudulSubForm.Visible = false;
            lblJudulSubForm.Visible = false;
            panFooterLeft.Visible = false;
            panel3.Visible = false;

            //Judul (panel atas)
            label17.Visible = false;
            label18.Visible = false;

            panWelcome.Visible = false;
            panEngineerID.Visible = false;
            panHeader.Size = new Size(1069, 25);
            panFooter.Size = new Size(1069, 25);

            textBox5.CharacterCasing = CharacterCasing.Upper;
        }

        /*
        public DataTable dtDAQSampling = new DataTable();
        public DataTable dtParamsChannel = new DataTable(); //untuk save hasil saving
        public DataTable dtParamsFiring = new DataTable();
        */

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

     #region FUNGSI_UTAMA_DACQ

        /* Variabel untuk store Data Table <=== CSV */

        //model manual
        DataTable dtSensor = new DataTable();
        DataTable dtSensorNumbering = new DataTable();    // ada kolom nomer di samping

        private string[] idChannel;
        private string[] unitChannelStr;

        private int jumChannel;
        private int jumDataRow;
        private int sampleRate;

        private string daqID;
        private string roketID;
        private string dateID;
        private string timeID;

        //model pakai struct
        struct data_us
        {
            public DataTable dtSensor;
            public string[] chName;
            public string[] chUnit;

            public int chJumlah;
            public int datarowJumlah;
            public int sampelRate;

            public string scopeId;
            public string motorId;
            public string dateId;
            public string timeId;
        }
        data_us daq=new data_us();
        

        bool loadFailed = false;
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
                    else
                    {
                        loadFailed = true;
                    }
                }
                catch (Exception err)
                {
                    loadFailed = true;
                    MessageBox.Show("(0) Gagal mengambil CSV, mohon diulangi, broo ! " + err.Message);
                    Application.Exit();     //restart program, exit dulu
                    Environment.Exit(0);
                }
            }

            if (!loadFailed)
            {
                try
                {
                    if (bc != null)
                    {
                        //Ambil DataTable
                        dtSensor = bc.get_dataRekap();

                        //HEADER
                        daqID = bc.get_daqID();
                        roketID = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                        roketID = refine_roket_id();
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
                        dtSensorNumbering = dtSensor.Copy();

                    }
                }

                catch (Exception) //err
                {
                    MessageBox.Show("(1) TIDAK BERHASIL MENGAMBIL FILE ! <ERROR_STREAM>", "Wassalam..", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }

        private string refine_roket_id()
        {
            //cari index huruf R > hapus index sebelumnya > copy ke string baru
            try
            {
                int idx = 0;
                idx = roketID.IndexOf("R", StringComparison.InvariantCultureIgnoreCase);
                roketID = roketID.Remove(0, idx);
            }
            catch(Exception err)
            {
                MessageBox.Show("(2)Nama roket (Nama file Csv) salah, mohon diperbaiki, bro!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Application.Exit();
                Environment.Exit(0);
            }
            return roketID;
        }

        #endregion FUNGSI_UTAMA_DACQ



     #region FUNGSI_UX_HIDING/SHOW

        //modifier untuk pergantian antar OpenForm
        private int seqLeftMenu = 0;   //klik menu sequence
        private bool fload_ok, ftabel_ok, fmotor_ok, fchart_ok, freport_ok;
        private bool flag_logged_in = false;

        FormParams ftabel;
        FormDataMotor fmotor;
        FormChart0 fchart0;
        //formchart1 fgrafik2;
        FormReport freport;
        FormArsipZip fzip;

        private void bt_coloring(Button tombol)
        {
            Font ftBold = new Font(tombol.Font.Name, tombol.Font.Size, FontStyle.Bold);
            Font ftNorm = new Font(tombol.Font.Name, tombol.Font.Size, FontStyle.Regular);

            //button bukan 'tombol' dibikin inactive
            btnLoad.ForeColor = Color.DimGray;
            btnParamsInput.ForeColor = Color.DimGray;
            btnDataMotor.ForeColor = Color.DimGray;
            btnGrafik.ForeColor = Color.DimGray;
            btnReport.ForeColor = Color.DimGray;
            btnZip.ForeColor = Color.DimGray;

            //button 'tombol' dibikin active
            tombol.ForeColor = Color.Gold;
            tombol.Font = ftBold;

            //panelbar
            //panel3.Location = new Point(192, tombol.Location.Y);
            panel3.Location = tombol.Location;
            panel3.Visible = true;
            panel3.Location = btnGrafik.Location;
        }

        //show FORM UTAMA
        List<Form> listForm = new List<Form>();
        List<Form> listFm = new List<Form>();
        int indexActForm = 0;
        int idxListForm = 0;

        private Form activeFormz = null;
        private Form prevForm = null;

        private void openform(Form formz)
        {
            listForm.Add(formz);
            idxListForm++;

            //manual disave dulu kondisi lastForm yg diload, pakai idxListForm
            //listForm.Insert(idxListForm, formz);

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

        private void open_form(Form f_input)
        {
            if(activeFormz!=null)
            {
                activeFormz.Close();    // hide?
            }
            else
            {
                activeFormz = f_input;  //save dulu active form nya
                
                //setting untuk form yg diklik (LeftMenu)
                f_input.TopLevel = false;
                f_input.FormBorderStyle = FormBorderStyle.None;
                f_input.Dock = DockStyle.Fill;

                //masukkan, filling ke panel 
                panMainView.Controls.Add(f_input);
                //panMainView.Tag = f_input;
                f_input.BringToFront();
                f_input.Show();

            }
            /*
            switch (seqLeftMenu)
            {
                case 1:
                    listFm.Insert(1, activeFormz);
                    break;
                case 2:
                    listFm.Insert(2, activeFormz);
                    break;
                case 3:
                    listFm.Insert(3, activeFormz);
                    break;
                case 4:
                    listFm.Insert(4, activeFormz);
                    break;
                case 5:
                    listFm.Insert(5, activeFormz);
                    break;
            }
            */
        }

    #endregion FUNGSI_VIEW-HIDE-UX


     #region MAIN_BUTTON
        
        CultureInfo lokalID = new System.Globalization.CultureInfo("id-ID");

        private void btnLoad_Click(object sender, EventArgs e)
        {           
            //setting datetime lokal Indonesia
            string hariUji, tglUji;
            bt_coloring(btnLoad);

            if (flag_logged_in)
            {
                //bikin windows auto maximize setelah load file csv
                this.WindowState = FormWindowState.Maximized;

                //Panggil fungsi Load_CSV
                ambil_csv_table();      

                try
                {
                    //show judul
                    DateTime dayTime = DateTime.ParseExact(dateID, "yyyy/MM/dd",CultureInfo.InvariantCulture);
                    hariUji = lokalID.DateTimeFormat.GetDayName(dayTime.DayOfWeek).ToString();
                    tglUji = dayTime.ToString("dd MMM yyyy");
                
                    label17.Text = roketID;
                    label18.Text = hariUji + ", " + tglUji + ", " + timeID;
                    label17.Visible = true;
                    label18.Visible = true;

                    //increment seq
                    fload_ok = true;
                    btnParamsInput_Click(sender, e);
                }
                catch(Exception err)
                {
                    MessageBox.Show("ERROR, broo !" + Environment.NewLine + err.Message);
                    Application.Exit();
                    Environment.Exit(0);
                }

            }
            else
                MessageBox.Show("Anda belum login, Bro !");
        }
    
        private void btnParamsInput_Click(object sender, EventArgs e)
        {
            if(fload_ok)
            {
                btnDetach.Visible = true;
                panJudulSubForm.Visible = true;
                panFooterLeft.Visible = true;
                lblJudulSubForm.Visible = true;

                lblJudulSubForm.Text = "PARAMETER DATA AKUISISI DAN FIRING SETUP";
                open_form(new FormParams(dtSensor, daqID, roketID, dateID, timeID, sampleRate, jumChannel, idChannel, unitChannelStr, jumDataRow));     // start form baru
                seqLeftMenu = 1;

                bt_coloring(btnParamsInput);
            }
            else
                MessageBox.Show("File Data Akuisisi US (Csv) belum dipilih, bro !");
        }

        private void btnDataMotor_Click(object sender, EventArgs e)
        {
            if(ftabel_ok)
            {
                lblJudulSubForm.Text = "DATA MOTOR ROKET (CHECKLIST)";

                open_form(new FormDataMotor());
                seqLeftMenu = 2;

                bt_coloring(btnDataMotor);
            }
            else
                MessageBox.Show("Data Parameter US belum dilengkapi, bro !");
        }

        private void btnGrafik_Click(object sender, EventArgs e)
        {
            if(fmotor_ok)
            {
                lblJudulSubForm.Text = "GRAFIK DATA AKUISISI US";

                open_form(new FormChart0(dtSensor, jumDataRow, unitChannelStr, idChannel, jumChannel, sampleRate, roketID));
                seqLeftMenu = 3;
                bt_coloring(btnGrafik);
            }
            else
                MessageBox.Show("Data Motor Roket (Checklist) belum lengkap, bro !");
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if(fchart_ok)
            {
                lblJudulSubForm.Text = "FORM LAPORAN HASIL UJI STATIS";

                open_form(new FormReport());
                seqLeftMenu = 4;
                bt_coloring(btnReport);
            }
            else
                MessageBox.Show("Grafik Analisis belum diselesaikan, bro !");
        }
        private void btnZip_Click(object sender, EventArgs e)
        {
            if (freport_ok)
            {
                lblJudulSubForm.Text = "BUNDEL ARSIP (zip) MOTOR ROKET UJI STATIS";

                open_form(new FormArsipZip());
                seqLeftMenu = 5;
                bt_coloring(btnZip);
            }
            else
                MessageBox.Show("Form Laporan Uji Statis (pdf) belum selesai, bro !");
        }

     #endregion MAIN_BUTTON


        

     #region BUTTON_NAVIGASI dan FITUR TAMBAHAN

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
                Transition.run(panHeader, "Height", 70, new TransitionType_EaseInEaseOut(1000));
                Transition.run(panFooter,"Height",100, new TransitionType_EaseInEaseOut(1000));
                Transition.run(panWelcome, "Height", 52, new TransitionType_EaseInEaseOut(300));
                panelSideMenu.Visible = true;
                Transition.run(panelSideMenu, "Width", psize_w, new TransitionType_EaseInEaseOut(300));

                //...


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

                MessageBox.Show($"Selamat datang, {textBox5.Text} !!","",MessageBoxButtons.OK, MessageBoxIcon.Information);
                flag_logged_in = true;
                
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

        private void panFooter_Paint(object sender, PaintEventArgs e)
        {

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
     
        /*Status waktu saat ini*/
        private void timer1_Tick(object sender, EventArgs e)
        {
            string strDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");//DateTime.Today.ToLongDateString();
            //label9.Text = strDate;
        }
     #endregion BUTTON_NAVIGASI
    }
}
