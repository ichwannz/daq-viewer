﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;
using ZedGraph;

using System.IO;

namespace DACQViewer
{
    public partial class FormChart0 : Form
    {
        public FormChart0(DataTable dtRekapF1, int dt1, string[] dt2, string[] dt3, int dt4, int dt5, string dt6)
        {
            InitializeComponent();

            //bikin array textbox hasil hitungan
            TextBox[] textboxArray = new TextBox[]
            {
                textBox8,   //ign
                textBox2,   //tburneff
                textBox3,   //tburntot

                textBox4,   //impuls_tot
                textBox5,
                textBox6,
                textBox7,
                textBox1,
                textBox10,

                textBox11,  //sampling
                textBox13
            };
            tb_hasil_hitung = textboxArray; //dibikin public

            dtRekapF3 = dtRekapF1;  //ambil data tabel full
            jumDataRow = dt1;       //jumlah baris data
            unitChannelArr = dt2;   //nama satuan
            idChannelArr = dt3;     //nama channel
            jumChannel = dt4;       //jumlah channel yg dipakai
            sampleRate = dt5;
            roketID = dt6;

            btnCurStart.BackColor = Color.Gold;
            btnHitung.Enabled = false;
            panelSQ.Visible = false;
            panelSQ.Location = lblSQ1.Location;

            //mulai form
            //showAstm.Show();

            inisialisasi_grafik();
            plot_grafik();

            cek_eksisting();        //cek apakah ada file analisis tersimpan
        }

        DataTable dtRekapF3 = new DataTable();  //objek tabel
        DataTable dtAnalytic = new DataTable();
        Color[] warnaKurva = new Color[10]
        {
            Color.Crimson,          // sebisa mungkin untuk igniter signal
            Color.Gold,
            Color.DeepSkyBlue,
            Color.LimeGreen,
            Color.MediumOrchid,
            Color.DarkGray,
            Color.Pink,
            Color.Aqua,
            Color.DarkOrange,
            Color.YellowGreen
        };
        Color[] warnaCursor = new Color[5]
        {
            Color.Black,
            Color.Blue,
            Color.Red,
            Color.Red,
            Color.Blue
        };
        String[] idxLabel = new string[5]
        {
            "T0", "A", "B","E","F"
        };

        TextBox[] tb_hasil_hitung;      //array textbox hasil hitung

        //variabel data header tabel parameter daq
        private string roketID;
        private string[] dataChX, dataChDg, idChannelArr, unitChannelArr;
        private int jumChannel, jumDataRow, sampleRate;

        _form_astm showAstm = new _form_astm();
        _form_w_propelan formwp = new _form_w_propelan();

     #region FORM_CHART
        GraphPane myChart = new GraphPane();    //objek zedgraph
        LineItem kurvaCHX;

        int pembagi = 10000;

        private void plot_grafik()
        {
            /*clearing area grafik dulu*/
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();
            myChart.CurveList.Clear();

            /*Pointpairlist kurva listCHX*/
            double[] xVal = new double[jumDataRow];
            double[] yVal = new double[jumDataRow];

            int pembagiVolt = (int)numv.Value;

            /*isi kurva (a) tiap Channel (a)  ===> tiap Kurva*/
            for (int a = 0; a < jumChannel; a++)
            {
                bool isVolt = false;
                bool isBar = false;
                bool isKgf = false;
                bool isCel = false;

                float b = 0;

                //cek Unit satuan di LOOP INI (a) >> tentukan pembagi Value, dll perlakuan tiap channel
                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "bar")
                {
                    isBar = true;
                }

                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "kgf")
                {
                    isKgf = true;
                }

                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "volt" || String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "V")
                {
                    isVolt = true;
                    pembagi = pembagiVolt;
                }

                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "C")
                {
                    isCel = true;
                }



                //isi data pointpair untuk kurva
                dataChX = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(a)).ToArray();
                dataus.Add(dataChX);

                for (int c = 0; c < jumDataRow; c++)
                {
                    xVal[c] = b / sampleRate;
                    yVal[c] = float.Parse(dataChX[c]) / pembagi;
                    b++;
                }
                

                //isi kurva line & style
                kurvaCHX = new LineItem(idChannelArr[a + 1], xVal, yVal, warnaKurva[a], SymbolType.None, 3.0f);
                kurvaCHX.Line.IsAntiAlias = true;
                kurvaCHX.Line.IsSmooth = true;


                //penentuan letak kurva dengan Y-Axis : Y2Axis, Yaxis1, YAxis2
                if (isKgf)
                {
                    idxColKgf = a;          //posisi index kolom kgf (F Thrust)
                }

                if (isVolt)
                {
                    /*
                    double[] arrVolt = dataChX.Select(double.Parse).ToArray();
                    maxVoltScale = arrVolt.Max();
                    myChart.Y2Axis.Scale.Max = (maxVoltScale * 3);
                    */

                    kurvaCHX.IsY2Axis = true;
                    myChart.Y2Axis.IsVisible = true;
                    idxColIgn = a;
                }

                if (isBar)
                {
                    kurvaCHX.YAxisIndex = 1;        //YAxis0
                    myChart.YAxisList[1].IsVisible = true;
                    idxColBar = a;
                }

                if(isCel)
                {
                    kurvaCHX.YAxisIndex = 2;    //YAxis1
                    myChart.YAxisList[2].IsVisible = true;
                    idxColCel = a;  // ada berapa dulu channel suhu nya? 4 channel? terus piye?
                }
              
                myChart.CurveList.Add(kurvaCHX);
                
                zedGraphControl1.AxisChange();
                //zedGraphControl1.Invalidate();
                zedGraphControl1.Refresh();
            }
        }

        private void inisialisasi_grafik()
        {
            /*---------------------------------------------------------------------------CHART_SETUP-------*/
            //inisiasi grafik plot area
            myChart = zedGraphControl1.GraphPane;

            //setup background
            //myChart.Fill = new Fill(Color.GreenYellow);  //outside chart
            //myChart.Chart.Fill = new Fill(Color.Black);   //inside chart   

            myChart.Legend.IsVisible = true;
            myChart.Legend.Position = ZedGraph.LegendPos.Top;
            myChart.IsFontsScaled = false;

            //judul grafik
            myChart.Title.Text = "Grafik Hasil Uji Statis " + roketID;
            myChart.Title.FontSpec.Family = "HP Simplified";
            myChart.Title.FontSpec.Size = 15;
            myChart.Title.FontSpec.IsBold = true;

            /*---------------------------------------------------------------------------AXIS_SETUP-------*/
            //setup X-Axis
            myChart.XAxis.Title.Text = "Waktu (Detik)";
            myChart.XAxis.Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black, true, false, false);
            myChart.XAxis.Title.FontSpec.Border = new Border(false, Color.Black, 0);

            myChart.XAxis.Scale.Max = jumDataRow / sampleRate;
            myChart.XAxis.Scale.Min = 0;
            myChart.XAxis.MajorGrid.IsVisible = true;
            myChart.XAxis.MajorGrid.Color = Color.Black;
            myChart.XAxis.MinorGrid.IsVisible = true;

            //setup Y-Axis
            myChart.YAxis.Title.Text = "Gaya dorong (Kgf)";
            myChart.YAxis.Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black, true, false, false);
            myChart.YAxis.Title.FontSpec.Border = new Border(false, Color.Black, 0);
            myChart.YAxis.Title.FontSpec.Angle = 180;

            myChart.YAxis.Scale.MaxAuto = true;
            myChart.YAxis.Scale.Min = 0;
            myChart.YAxis.MajorGrid.IsVisible = true;
            myChart.YAxis.MinorGrid.IsVisible = false;

            myChart.YAxis.MajorTic.IsOpposite = false;  //biar Tic tidak muncul di semua YAxis-Y2
            myChart.YAxis.MinorTic.IsOpposite = false;
            myChart.YAxis.MajorTic.IsInside = false;
            myChart.YAxis.MinorTic.IsInside = false;
            myChart.YAxis.Scale.Align = AlignP.Inside;
            myChart.YAxis.Cross = 0.0;

            //setup Y2-AXIS
            myChart.Y2Axis.Title.Text = "Ignition (Volt)";
            myChart.Y2Axis.Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black, true, false, false);
            myChart.Y2Axis.Title.FontSpec.Border = new Border(false, Color.Black, 0);
            myChart.Y2Axis.Title.FontSpec.Angle = 180;

            myChart.Y2Axis.Scale.Max = 30;      //30 volt...
            myChart.Y2Axis.Scale.Min = 0;

            myChart.Y2Axis.MajorTic.IsInside = false;
            myChart.Y2Axis.MajorTic.IsInside = false;
            myChart.Y2Axis.MinorTic.IsInside = false;
            myChart.Y2Axis.MajorTic.IsOpposite = false;
            myChart.Y2Axis.MinorTic.IsOpposite = false;
            myChart.Y2Axis.Scale.Align = AlignP.Inside;

            //setup Y-Axis 1
            YAxis YAxis0 = new YAxis("Bar");    //YAxisIndex nya = 1...
            myChart.YAxisList.Add(YAxis0);

            myChart.YAxisList[1].Title.Text = "Tekanan (Bar)";
            myChart.YAxisList[1].Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black, true, false, false);
            myChart.YAxisList[1].Title.FontSpec.Border = new Border(false, Color.Black, 0);
            myChart.YAxisList[1].Title.FontSpec.Angle = 180;

            myChart.YAxisList[1].Scale.Max = 150;       //bar max scale view
            myChart.YAxisList[1].Scale.Min = 0;
            myChart.YAxisList[1].MajorGrid.IsVisible = false;
            myChart.YAxisList[1].MinorGrid.IsVisible = false;

            myChart.YAxisList[1].MajorTic.IsOpposite = false;  //biar Tic tidak muncul di semua YAxis-Y2
            myChart.YAxisList[1].MinorTic.IsOpposite = false;
            myChart.YAxisList[1].MajorTic.IsInside = false;
            myChart.YAxisList[1].MinorTic.IsInside = false;
            myChart.YAxisList[1].Scale.Align = AlignP.Inside;
            myChart.YAxisList[1].Cross = 0.0;

            //setup Y-Axis 2
            YAxis YAxis1 = new YAxis("Celsius");
            myChart.YAxisList.Add(YAxis1);

            myChart.YAxisList[2].Title.Text = "Suhu (Celsius)";
            myChart.YAxisList[2].Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black, true, false, false);
            myChart.YAxisList[2].Title.FontSpec.Border = new Border(false, Color.Black, 0);
            myChart.YAxisList[2].Title.FontSpec.Angle = 180;

            myChart.YAxisList[2].Scale.Max = 1000;       //cel max scale view
            myChart.YAxisList[2].Scale.Min = 0;
            myChart.YAxisList[2].MajorGrid.IsVisible = false;
            myChart.YAxisList[1].MinorGrid.IsVisible = false;

            myChart.YAxisList[2].MajorTic.IsOpposite = false;  //biar Tic tidak muncul di semua YAxis-Y2
            myChart.YAxisList[2].MinorTic.IsOpposite = false;
            myChart.YAxisList[2].MajorTic.IsInside = false;
            myChart.YAxisList[2].MinorTic.IsInside = false;
            myChart.YAxisList[2].Scale.Align = AlignP.Inside;
            myChart.YAxisList[2].Cross = 0.0;

            /*---------------------------------------------------------------------------ZGC_CONTROL_SETUP-------*/
            // setup draw plotter
            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.IsShowHScrollBar = true;
            zedGraphControl1.IsShowVScrollBar = true;
            //zedGraphControl1.IsAutoScrollRange = true;

            zedGraphControl1.AxisChange();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //replotting dengan data refining
        }
        #endregion FORM_CHART

     #region TABEL_ANALITIK
        bool tabelShowed = false;
        private void tampilkan_tabel_analitik()
        {
            tabelShowed = true;
            //clear tabel
            dtAnalytic.Clear();
            dtAnalytic.Columns.Clear();
            dtAnalytic.Rows.Clear();
            //clear dgv1
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            try
            {
                //isi header column dulu
                dtAnalytic.Columns.Add("NO", typeof(int));      //0
                dtAnalytic.Columns.Add("KURVA", typeof(bool));    //1
                dtAnalytic.Columns.Add("CHANNEL", typeof(string));  //2
                dtAnalytic.Columns.Add("NILAI (detik-X)", typeof(string));       //3
                dtAnalytic.Columns.Add("UNIT", typeof(string)); //4

                //isi auto idChannel
                for (int a = 0; a < jumChannel; a++)
                {
                    dtAnalytic.Rows.Add((a + 1), true, idChannelArr[a + 1], 0, unitChannelArr[a + 1]);
                }

                //binding dgv1 dengan dtAnalytic yg telah dibuat
                dataGridView1.DataSource = dtAnalytic;

                //style centang curve default 
                for (int a = 0; a < jumChannel; a++)
                {
                    aksi_centang(a, true);
                }

                //style
                dataGridView1.Columns[0].Width = 25;
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].Width = 50;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].Width = 100;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                //style header column
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    col.ReadOnly = true;        //semua readonly..
                }
                dataGridView1.Columns[1].ReadOnly = false;  //..kecuali column centang


                //style rows color & isi nomor rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

                    //isi row number & style cell 
                    row.Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    row.Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    //set color cell
                    for (int k = 0; k < jumChannel; k++)  // cara 1
                    {
                        dataGridView1.Rows[k].Cells[0].Style.BackColor = warnaKurva[k]; // kolom 2 (ID)
                        dataGridView1.Rows[k].Cells[1].Style.BackColor = warnaKurva[k]; // kolom 2 (ID)
                        dataGridView1.Rows[k].Cells[2].Style.BackColor = warnaKurva[k]; // kolom 3 (channelName)
                        dataGridView1.Rows[k].Cells[1].Style.SelectionBackColor = warnaKurva[k];
                    }
                }

                dataGridView1.Refresh();
            }
            catch (Exception errz)
            {
                MessageBox.Show("Gagal menampilkan Tabel Analitik! " + Environment.NewLine + errz.Message);
            }
        }
        #endregion TABEL_ANALITIK

     #region BUTTON

        /*---------------------------------------------------------------------------------------------------- SAVE CSV ------*/
        string[] csv_path = new string[2]
            {
                "D:/temp5.daqc",        // csv untuk time sequence grafik
                "D:/temp6.daqc"         // csv untuk hasil hitungan analisis
            };

        //save csv tsequence
        private void save_csv_tsequence()
        {
            //untuk simpan variabel time sequence grafik
            DataTable dtHasil5 = new DataTable();

            //bikin dtt dari inputan
            dtHasil5.Clear();
            dtHasil5.Columns.Add("variabel", typeof(string)).SetOrdinal(0);
            dtHasil5.Columns.Add("value", typeof(string)).SetOrdinal(1);

            dtHasil5.Rows.Add("tzero", tZero.ToString());
            dtHasil5.Rows.Add("timea", tA.ToString());
            dtHasil5.Rows.Add("timeb", tB.ToString());
            dtHasil5.Rows.Add("timee", tE.ToString());
            dtHasil5.Rows.Add("timef", tF.ToString());
            dtHasil5.Rows.Add("wpropelan", tbWpropelan.Text);

            //bikin csv dari dtt
            make_csv_fr_dtt(dtHasil5, 0);   //tseq data
        }

        //save csv hasil hitungan
        private void save_csv_hasil()
        {
            //untuk simpan variabel hasil hitungan
            DataTable dtHasil6 = new DataTable();

            //bikin dtt dari hasil hitung (TextBox)
            dtHasil6.Clear();
            dtHasil6.Columns.Add("variabel", typeof(string)).SetOrdinal(0);
            dtHasil6.Columns.Add("value", typeof(string)).SetOrdinal(1);

            dtHasil6.Rows.Add("tign", textBox8.Text);
            dtHasil6.Rows.Add("tburn", textBox2.Text);
            dtHasil6.Rows.Add("taction", textBox3.Text);
            dtHasil6.Rows.Add("itot", textBox4.Text);
            dtHasil6.Rows.Add("isp", textBox5.Text);
            dtHasil6.Rows.Add("fmax", textBox6.Text);
            dtHasil6.Rows.Add("fave", textBox7.Text);
            dtHasil6.Rows.Add("pmax", textBox1.Text);
            dtHasil6.Rows.Add("pave", textBox10.Text);
            dtHasil6.Rows.Add("sample", textBox11.Text);
            dtHasil6.Rows.Add("sprate", textBox13.Text);

            //bikin csv dari dtt
            make_csv_fr_dtt(dtHasil6, 1);   //data hasil analisis
        }

        //fungsi bikin Csv dari DataTable
        private void make_csv_fr_dtt(DataTable dt, int idxPath)
        {
            StringBuilder sb = new StringBuilder();

            string[] headers = dt.Columns.Cast<DataColumn>()
                .Select(col => col.ColumnName)
                .ToArray();
            sb.AppendLine(string.Join(";", headers));

            foreach (DataRow dr in dt.Rows)
            {
                string[] cells = dr.ItemArray.Select(celz => celz.ToString()).ToArray();
                sb.AppendLine(string.Join(";", cells));
            }

            File.WriteAllText(csv_path[idxPath], sb.ToString());
        }

        /*---------------------------------------------------------------------------------------------------- LOAD CSV ------*/
        
        //load csv to datatable, binding ke tb/var
        double[] tseq = new double[5];  //tzero, tA, tB, tE, tF
        private void baca_csv(int idxPath)
        {
            //baca csv masukkan ke dt
            DataTable dtHasilz = new DataTable();

            //isi ke dt
            File.ReadLines(csv_path[idxPath]).Take(1)
                .SelectMany(x => x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dtHasilz.Columns.Add(x.Trim()));

            File.ReadLines(csv_path[idxPath]).Skip(1)
                .Select(x => x.Split(';'))
                .ToList()
                .ForEach(line => dtHasilz.Rows.Add(line));

            //Isi variabel tseq
            if(idxPath==0)
            {
                //otomatis, dibikin array
                for(int a=0; a<tseq.Count();a++)
                {
                    //tseq[a] = double.Parse(dtHasilz.Rows[a][1].ToString());
                    tseq[a] = Convert.ToDouble(dtHasilz.Rows[a][1]);
                }

                //manual
                /*
                tZero = double.Parse(dtHasilz.Rows[0][1].ToString());
                tA = (double)dtHasilz.Rows[1][1];
                tB = (double)dtHasilz.Rows[2][1]; 
                tE = (double)dtHasilz.Rows[3][1]; 
                tF = (double)dtHasilz.Rows[4][1]; 
                */
                tbWpropelan.Text = dtHasilz.Rows[5][1].ToString(); 
            }

            //Isi variabel hasil hitungan
            else if (idxPath==1)
            {
                int b = tb_hasil_hitung.Count();

                //otomatis
                for(int a=0; a<b ;a++)
                {
                    tb_hasil_hitung[a].Text = dtHasilz.Rows[a][1].ToString();
                }

                //manual
                /*
                textBox8.Text = dtHasilz.Rows[0][1].ToString(); 
                textBox2.Text = dtHasilz.Rows[1][1].ToString(); 
                textBox3.Text = dtHasilz.Rows[2][1].ToString(); 

                textBox4.Text = dtHasilz.Rows[3][1].ToString();
                textBox5.Text = dtHasilz.Rows[4][1].ToString();
                textBox6.Text = dtHasilz.Rows[5][1].ToString();
                textBox7.Text = dtHasilz.Rows[6][1].ToString();
                textBox1.Text = dtHasilz.Rows[7][1].ToString();
                textBox10.Text = dtHasilz.Rows[8][1].ToString();

                textBox11.Text = dtHasilz.Rows[9][1].ToString();
                textBox13.Text = dtHasilz.Rows[10][1].ToString();
                */
            }
        }

        /*---------------------------------------------------------------------------------------------------- BUTTON ------*/
        string imgChartPath = "D:\\img.png";
        //string imgChartPath = Path.Combine(@"D:\", "imgz.png");
        private void btnSaveAnalysis_Click(object sender, EventArgs e)
        {
            //save dtHasil5 : time sequence
            //save dtHasil1a : dt sensor refined
            //save dtHasil6 : hasil hitungan
            //buat form-us
            //save chart
            zedGraphControl1.BorderStyle = BorderStyle.FixedSingle;

            zedGraphControl1.MasterPane.GetImage().Save(imgChartPath,System.Drawing.Imaging.ImageFormat.Png);
            save_csv_tsequence();   
            save_csv_hasil();

            MessageBox.Show("Hasil analisis berhasil disimpan, bro!");
        }

        private void btAstm_Click(object sender, EventArgs e)
        {
            _form_astm astm = new _form_astm();
            astm.Show();
        }

        private void cek_eksisting()
        {
            if(File.Exists(csv_path[0]) && File.Exists(csv_path[1]))
            {
                //ambil return value dari dialog button yesno
                DialogResult dr= MessageBox.Show("Terdeteksi ada file Analisis DAQ sebelumnya,"+Environment.NewLine+
                    "Grafik chart akan diatur sesuai analisis sebelumnya, Anda mau pakai, bro?"
                    , "Reload file tersimpan"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Asterisk);

                if(dr==DialogResult.Yes)
                {
                    // baca csv, bikin dtt, binding ke tb/var
                    baca_csv(0);    //tseq
                    baca_csv(1);    //hasil
                    //bikin line/garis
                    idxCursor = 0;

                    for(int a=0 ; a<tseq.Count() ; a++)
                    {
                        create_cursorx_idx(tseq[a]);
                    }
                    //chart refresh
                    btnHitung.Enabled = false;
                }
                else if (dr==DialogResult.No)
                {
                    //bikin manual lagi
                    MessageBox.Show("Silahkan setel posisi Cursor Time Sequence uji statis <lihat skema ASTM>");
                }

                //tampilkan_tabel_analitik();

            }
        }
        
        #endregion BUTTON


/*-------------------------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------- EVENT HANDLER ------*/
     #region TABEL_ANALITIK_EVENTS : CLICK
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //pakai event cell value changed >> perlu masuk ke status checkboxnya
            // auto cek status CheckBox
            for (int a = 0; a < jumChannel; a++)
            {
                DataGridViewCheckBoxCell cBox = dataGridView1.Rows[a].Cells[1] as DataGridViewCheckBoxCell;
                if (e.ColumnIndex == dataGridView1.Columns[1].Index && e.RowIndex != -1)
                {
                    if (cBox != null && !DBNull.Value.Equals(cBox.Value) && (bool)cBox.Value == true)
                        aksi_centang(a, true);
                    else
                        aksi_centang(a, false);
                }
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //saat 1 event ngeklik dihitung 1 kali eksekusi (mouse up)
            if (e.ColumnIndex == dataGridView1.Columns[1].Index && e.RowIndex != -1)
                dataGridView1.EndEdit();
        }

        //fungsi untuk show/hide curve, via kolom centang di DGV1
        private void aksi_centang(int idx, bool status)
        {
            if (status)
            {
                //curve visiblity true
                myChart.CurveList[idx].IsVisible = true;
                //dataGridView1.Rows[idx].Cells[5].Style.BackColor = Color.GreenYellow;
            }

            else
            {
                //curve visibility false
                myChart.CurveList[idx].IsVisible = false;
                //dataGridView1.Rows[idx].Cells[5].Style.BackColor = Color.Crimson;
            }
            zedGraphControl1.Refresh();
        }

        #endregion TABEL_ANALITIK_CELL_KLIK_EVENT

     #region CHART_EVENT : HOVER, CLICK

    //point value waktu hovering
        PointPair ppCross;
        private string zedGraphControl1_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            ppCross = curve[iPt];

            return curve.Label.Text + " : " + ppCross.X.ToString("#.##") + " detik; " + ppCross.Y.ToString("#.##") + " " + unitChannelArr[curve.YAxisIndex + 1];  //strUnit;//
        }

    //fungsi klik baca value >> dgv1
        private void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            double X1, Y1, pembagiVolt;

            if (!tabelShowed)                       //jika tabel belum tampil, tampilkan !
                tampilkan_tabel_analitik();

            if (e.Button == MouseButtons.Left)
            {
            //0) ambil nilai mouse(X,Y)
                myChart.ReverseTransform(e.Location, out X1, out Y1);

            //1) masukkan ke tabel
                double xdata = (X1 * sampleRate);          //detik * 100
                for (int idx = 0; idx < jumChannel; idx++)
                {
                    if (String.Format(unitChannelArr[idx + 1], StringComparison.InvariantCultureIgnoreCase) == "volt")
                        pembagiVolt = 1000000;
                    else
                        pembagiVolt = 10000;

                    dataChDg = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(idx)).ToArray();

                    string dat0 = dataChDg[Convert.ToInt32(xdata)];
                    double dat1 = double.Parse(dat0);
                    double dat2 = dat1 / pembagiVolt;

                    dataGridView1.Rows[idx].Cells[3].Value = dat2.ToString("0.000");
                }

            //2) create cursorx_BEBAS  //jika tombol START belum dipilih
                if(!cursorxStart)           
                    create_cursorx_once(X1); 

            //3) create cursor_IDX & ambil nilai Cursorx ke variabel 
                if(cursorxStart)
                {

                    //mulai bikin cursorx_IDX
                    create_cursorx_idx(X1);

                    //jika klik sudah 5 kali
                    if (idxCursor > 4)
                    {
                        hapus_data_baca();
                    }

                    //mulai sequence peletakan cursor
                    if (idxCursor == 0)     //kondisi sudah kik cursorx TZero
                    {
                        tZero = X1;   //time Zero, saat sinyal diberikan

                        lblSQ1.BackColor = Color.FromArgb(240, 240, 240);

                        lblSQ2.BackColor = Color.Crimson;
                        panelSQ.Location = lblSQ2.Location; //pindah ke nomer 2
                    }

                        //ignition rise time
                    if (idxCursor == 1)
                    {
                        tA = X1;      //time A, saat ignition dimulai
                        tign = (tA - tZero);
                        textBox8.Text = tign.ToString("0.000") + "  ";

                        lblSQ2.BackColor = Color.FromArgb(240, 240, 240);

                        lblSQ3.BackColor = Color.Crimson;
                        panelSQ.Location = lblSQ3.Location;
                    }

                    if (idxCursor==2)
                    {
                        tB = X1;

                        lblSQ3.BackColor = Color.FromArgb(240, 240, 240);

                        lblSQ4.BackColor = Color.Crimson;
                        panelSQ.Location = lblSQ4.Location;
                    }

                        //eff burningtime
                    if (idxCursor==3)
                    {
                        tE = X1;
                        tburnEff = (tE - tB);
                        textBox2.Text = tburnEff.ToString("0.000") + "  ";

                        lblSQ4.BackColor = Color.FromArgb(240, 240, 240);
                        lblSQ5.BackColor = Color.Crimson;
                        panelSQ.Location = lblSQ5.Location;
                    }

                    //tot burning time/action time
                    if (idxCursor == 4)
                    {
                        tF = X1;
                        taction = (tF - tB);
                        textBox3.Text = taction.ToString("0.000") + "  ";

                        lblSQ5.BackColor = Color.FromArgb(240, 240, 240);
                        lblSQ6.BackColor = Color.Crimson;
                        panelSQ.Location = lblSQ6.Location;


                        tbWpropelan.Text = "0"; //reset
                        while(tbWpropelan.Text=="0")
                        {
                            _form_w_propelan fw = new _form_w_propelan();
                            using (fw)
                            {
                                if (fw.ShowDialog() == DialogResult.OK)
                                {
                                    //beratProp = formwp.get_w_prop;
                                    //beratProp = formwp.getWW();
                                }
                            }
                            tbWpropelan.Text = fw.getWW();

                        }

                        btnHitung.Enabled = true;
                        btnHitung.BackColor = Color.Gold;
                        btnCurStart.BackColor = Color.FromArgb(240, 240, 240);
                        
                        panelSQ.Location = lblSQ1.Location;
                        panelSQ.Visible = false;

                        btnCurStart_Click(sender, e);
                        btnHitung_Click(sender, e);

                    }

                    idxCursor++;
                }
            }

        }
        
        private void hapus_data_baca()
        {
            //clear semua nilai tersimpan
            idxCursor = 0;
            myChart.GraphObjList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();

            tZero = 0;
            tA = 0;
            tB = 0;
            tE = 0;
            tF = 0;
            tign = 0;
            tburnEff = 0;
            taction = 0;

            //clear textbox
            textBox8.Text = "0";
            textBox2.Text = "0";
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox5.Text = "0";
            textBox7.Text = "0";
            textBox10.Text = "0";
            textBox6.Text = "0";
            textBox1.Text = "0";
        }
        
    //fungsi hovering VCursor
        PointPairList listCursorX = new PointPairList();
        LineItem lineCursorX;

        private void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)
        {
            //ambil data koordinat mouse
            double mousex, mousey;
            myChart.ReverseTransform(e.Location, out mousex, out mousey);

            //add (x,y) garis cursor
            listCursorX.Add(mousex, myChart.YAxis.Scale.Min);
            listCursorX.Add(mousex, myChart.YAxis.Scale.Max);

            //add curve
            lineCursorX = myChart.AddCurve("", listCursorX, Color.Black, SymbolType.Square);
            lineCursorX.Line.Width = 1f;
            lineCursorX.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;

            zedGraphControl1.Refresh();

            listCursorX.Clear();


        }
        private void zedGraphControl1_MouseLeave(object sender, EventArgs e)
        {
            //listCursorX.Clear();
        }

    //fungsi letakkan garis VCursor
        private void create_cursorx_once(double xval)
        {            
            //create line CursorX nya satu tok, clear setiap kali klik
            myChart.GraphObjList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();

            //bikin line cursorX
            LineObj cursorx1 = new LineObj(Color.Black, xval, 0, xval, 1);   //index idxCursor langsung nilai 1 ...untuk warnaCursor
            cursorx1.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
            cursorx1.IsClippedToChartRect = true;
            cursorx1.Line.Width = 2;
            cursorx1.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            cursorx1.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(cursorx1);

            TextObj linex = new TextObj("X", xval, 0, CoordType.XScaleYChartFraction, AlignH.Right, AlignV.Top);
            linex.FontSpec = new FontSpec("HP Simplified", 17, Color.Gold, true, true, false, Color.Black, null, FillType.Solid);
            linex.FontSpec.Border.IsVisible = false;
            linex.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(linex);

            zedGraphControl1.Refresh();
        }

        int idxCursor = 0;
        private void create_cursorx_idx(double xval)
        {              
            //bikin line cursorX
            LineObj cursorx1 = new LineObj(warnaCursor[idxCursor], xval, 0, xval,1 );   //index idxCursor langsung nilai 1 ...untuk warnaCursor
            cursorx1.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
            cursorx1.IsClippedToChartRect = true;
            cursorx1.Line.Width = 2;
            cursorx1.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            cursorx1.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(cursorx1);

            TextObj linex = new TextObj(idxLabel[idxCursor], xval, 0, CoordType.XScaleYChartFraction, AlignH.Right, AlignV.Top);
            linex.FontSpec = new FontSpec("HP Simplified", 17, Color.White, true, true, false, warnaCursor[idxCursor], null, FillType.Solid);
            linex.FontSpec.Border.IsVisible = false;
            linex.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(linex);

            zedGraphControl1.Refresh();
        }


    //button start/stop cursor
        bool cursorxStart;
        int idxBtnCursorStart=0;
        private void btnCurStart_Click(object sender, EventArgs e)
        {
            btAstm_Click(sender, e);    //bukak astm first

            idxBtnCursorStart++;
            if (isOdd(idxBtnCursorStart))
            {
                hapus_data_baca();
    
                //clear dulu setelah single cursorx
                myChart.GraphObjList.Clear();
                zedGraphControl1.GraphPane.GraphObjList.Clear();
                zedGraphControl1.Refresh();

                btnHitung.Enabled = false;
                btnHitung.BackColor = Color.FromArgb(240, 240, 240);
                panelSQ.Visible = true;
                panelSQ.Size= new Size(475,20);
                
                lblSQ1.BackColor = Color.Crimson;
                btnCurStart.Text = "CLEAR";
                btnCurStart.BackColor = Color.Crimson;
                cursorxStart = true;
            }
            else
            {
                panelSQ.Visible = false;
                panelSQ.Size = new Size(475, 20);

                lblSQ6.BackColor = Color.FromArgb(240,240,240);
                btnCurStart.Text = "MULAI";
                cursorxStart = false;
            }
        }

        #endregion CHART_CLICK_EVENTSS
 /*--------------------------------------------------------------------------------------------------- FUNGSI HITUNG UTAMA-*/

     #region FUNGSI AMBIL DATA CURSOR, HITUNGAN ANALISIS

        //variabel hitung-hitungan
        double tZero, tA, tB, tE, tF;           // titik2 dalam skema ASTM (A, B, C...)dalam detik, untuk dapet data-urut, perlu dikali 10
        double tign, tburnEff, taction;

        List<string[]> dataus = new List<string[]>();
        int idxColKgf, idxColBar, idxColIgn, idxColCel;
        double impuls_tot, impuls_sp, w_prop;           //w_prop dalam input gram 
        double f_max, f_ave, p_max, p_ave;
        double sample_fine_tot;             //sampel data setelah direfine/split => action time
        private void hitung_analisis_astm()
        {
            double[] data_f, data_p, data_v;    //untuk store data raw (belum di split)
            double[] data_f_fine_eff, data_p_fine_eff;                  //untuk store burning effective after split
            double[] data_f_fine_tot, data_p_fine_tot, data_v_fine_tot; //utk store burning total/action after split


            int refineSkip_eff, refineTake_eff;             //refining data untuk effective burning
            int refineSkip_tot, refineTake_tot;     //refining data untuk total burning action
            
            try
            {
                //convert string[] ke double
                    //data_f = Array.ConvertAll(dataus[idxColKgf], s => double.Parse(s));   //pakai LAMBDA :convert String[] to double[]
                    //data_f = Array.ConvertAll(dataus[idxColKgf],double.Parse);            //convert String[] to double[]
                data_f = dataus[idxColKgf].Select(double.Parse).ToArray();  // THRUST_F       //pakai LINQ : convert String[] to double[]
                data_p = dataus[idxColBar].Select(double.Parse).ToArray();  // PRESSURE_P
                data_v = dataus[idxColIgn].Select(double.Parse).ToArray();  //IGNITION_V
            
                //cari parameter
                f_max = data_f.Max();
                f_max = f_max / (int)numf.Value;
                p_max = data_p.Max();
                p_max = p_max / (int)nump.Value;

                //ACTION AREA ~ total burning
                refineSkip_tot = Convert.ToInt32(tA * sampleRate);              //dikali sampleRate untuk mencari posisi "data urutan"
                refineTake_tot = Convert.ToInt32(taction * sampleRate);
                sample_fine_tot = refineTake_tot;       // ambil sebagai sampel total (split before & after time action)

                data_f_fine_tot = data_f.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
                data_p_fine_tot = data_p.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
                data_v_fine_tot = data_v.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
            


                //BURNING AREA - effective buning
                refineSkip_eff = Convert.ToInt32(tB * sampleRate);              //dikali sampleRate untuk mencari posisi "data urutan"
                refineTake_eff = Convert.ToInt32(tburnEff * sampleRate);

                data_f_fine_eff = data_f.Skip(refineSkip_eff).Take(refineTake_eff).ToArray();
                data_p_fine_eff = data_p.Skip(refineSkip_eff).Take(refineTake_eff).ToArray();

                //rerata dari BURNING TIME EFFECTIVE !
                f_ave = data_f_fine_eff.Average();
                p_ave = data_p_fine_eff.Average();

                if(tbWpropelan.Text!=null)
                {
                    w_prop = double.Parse(tbWpropelan.Text);    //dalam gram
                    w_prop = w_prop / 1000;         //dalam kg

                    impuls_tot = (f_ave/10000 * tburnEff);            //impuls total= Thrust rerata*TimeBurning effective
                    impuls_sp = (impuls_tot / w_prop);          //impuls sp=impuls total/berat propelan
                }
                
                //tampilkan
                textBox4.Text = impuls_tot.ToString("0.000") + "  ";
                textBox5.Text = impuls_sp.ToString("0.000") + "  ";

                textBox7.Text = (f_ave / 10000).ToString("0.000") + "  ";
                textBox10.Text = (p_ave / 10000).ToString("0.000") + "  ";

                textBox6.Text = f_max.ToString("0.000")+"  ";
                textBox1.Text = p_max.ToString("0.000")+"  ";

                textBox11.Text = refineTake_tot.ToString();
                textBox13.Text = sampleRate.ToString();
            }

            catch(Exception err)
            {
                MessageBox.Show("Berat Propelan mohon diisi dulu, bro !", "Input Nilai", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        
        //fungsi box burning area
        double x1box, x2box;
        private void create_burning_area()
        {
            x1box = tB;
            BoxObj boxBurning = new BoxObj(x1box, 15000, tburnEff, 15000, Color.Empty, Color.FromArgb(50, Color.Crimson));
            boxBurning.Location.CoordinateFrame = CoordType.AxisXYScale;
            boxBurning.Location.AlignV = AlignV.Top;
            boxBurning.Location.AlignH = AlignH.Left;
            boxBurning.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(boxBurning);

            TextObj texBurning = new TextObj("Area waktu bakar efektif", (x1box + (tburnEff * 0.1)), 10000);
            texBurning.Location.CoordinateFrame = CoordType.AxisXYScale;
            texBurning.Location.AlignH = AlignH.Left;
            texBurning.Location.AlignV = AlignV.Center;
            texBurning.FontSpec.IsItalic = true;
            texBurning.FontSpec.Family = "HP Simplified";
            texBurning.FontSpec.FontColor = Color.Black;
            texBurning.FontSpec.Size = 14f;
            myChart.GraphObjList.Add(texBurning);

            zedGraphControl1.Refresh();
        }

        private void btnHitung_Click(object sender, EventArgs e)
        {
            try
            {
                hitung_analisis_astm();
                //create_burning_area();      //BoxObj
            }
            catch(Exception err)
            {
                MessageBox.Show("Vertical CursorX tmohon diatur dulu, bro !");
            }

        }
       
    #endregion FUNGSI AMBIL DATA CURSOR

     #region Fungsi Hide/show Panel
        private static bool isOdd(int val)
        {
            return val % 2 != 0;
        }

        int lastHeight;
        int idxButton = 0;


        private void button9_Click(object sender, EventArgs e)
        {
            idxButton++;

            if(isOdd(idxButton))
            {
                lastHeight = panel4.Size.Height;
                Transition.run(panel4, "Height", 35, new TransitionType_EaseInEaseOut(500));
               button9.BackgroundImage = Properties.Resources.upBtn;
            }
            else
            {
                Transition.run(panel4, "Height", lastHeight, new TransitionType_EaseInEaseOut(500));
                button9.BackgroundImage = Properties.Resources.downBtn;
            }
        }
    #endregion Fungsi Hide/show Panel

        

    }
}
