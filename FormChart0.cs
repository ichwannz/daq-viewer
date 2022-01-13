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
using ZedGraph;

namespace DACQViewer
{
    public partial class FormChart0 : Form
    {
        /* CREATE OBJEK*/
        DataTable dtRekapF3 = new DataTable();  //objek tabel
        DataTable dtAnalytic = new DataTable();
        GraphPane myChart = new GraphPane();    //objek zedgraph
      
        Color[] warnaKurva = new Color[10]
        {
            Color.Crimson,          // sebisa mungin tuntuk igniter signal
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

        //variabel data header tabel
        private string[] dataChX;
        private string[] idChannelArr;
        private int jumChannel;
        private int jumDataRow;
        private int sampleRate;
        private string[] unitChannelArr;

        public FormChart0(DataTable dtRekapF1, int dt1, string[] dt2, string[] dt3, int dt4, int dt5)
        {
            InitializeComponent();


            dtRekapF3 = dtRekapF1;  //ambil data tabel full
            jumDataRow = dt1;       //jumlah baris data
            unitChannelArr = dt2;   //nama satuan
            idChannelArr = dt3;     //nama channel
            jumChannel = dt4;       //jumlah channel yg dipakai
            sampleRate = dt5;

            //mulai form
            inisialisasi_grafik();
            plot_grafik();

        }


        private int indexIgn, indexKgf;

        // cari nomor urut-index Channel dgn KGF, VOLT
        private void cekIndexKgf()
        {
            //apakah semua satuan sama? if no..activate Y2 axis: Bar & KGF
            //indexKgf = Array.IndexOf(unitChannel, "kgf");
            indexKgf = Array.FindIndex(unitChannelArr, a => a.Equals("kgf", StringComparison.InvariantCultureIgnoreCase));

            //indexIgn = Array.IndexOf(unitChannel, "V");
            indexIgn = Array.FindIndex(unitChannelArr, b => b.Equals("v", StringComparison.InvariantCultureIgnoreCase));

            if (indexKgf < 0) indexKgf = 0;
            if (indexIgn < 0) indexIgn = 0;
        }

        // bool apakah iTag(urutan kurvaCH0 cocok dengan nomor urut index KGF
        private int isKgfExist(int indexKurva)
        {
            int pembagiBar = 10000;
            int pembagiKgf = 10000;

            int pembagiSatuan;
            int tagz = indexKurva + 1;

            if (tagz == indexKgf || tagz == indexIgn)    // if channel kurva ini Thrust(KgF)
            {
                myChart.CurveList[indexKurva].IsY2Axis = true;  //kalau ada Volt atau Kgf
                pembagiSatuan = pembagiKgf;
            }
            else //if channel ini normal, pressure (bar)
            {
                myChart.CurveList[indexKurva].IsY2Axis = false; // kalau cuma Bar 
                pembagiSatuan = pembagiBar;
            }
            return pembagiSatuan;
        }

        private void plot_grafik()
        {
            cekIndexKgf();

            myChart.CurveList.Clear();

            if (indexKgf > 0)
            {
                myChart.Y2Axis.IsVisible = true;
                myChart.Y2Axis.Title.Text = "kgf";
            }

            /*clearing area grafik dulu*/
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();

            myChart.Legend.IsVisible = true;
            myChart.Legend.Position = ZedGraph.LegendPos.InsideTopRight;
            //zedGraphControl1.GraphPane.Chart.Fill.Color = Color.Black;    //chart
            //zedGraphControl1.GraphPane.Fill.Color = Color.Black;            //keseluruhan

            /*setup untuk tampilan chart*/
            myChart.XAxis.Scale.MajorStepAuto = true;
            myChart.XAxis.Scale.MinorStepAuto = true;

            //myChart.YAxis.Scale.MagAuto = false;
            myChart.XAxis.Scale.Max = jumDataRow / sampleRate;
            myChart.YAxis.Cross = 0.0;

            /*Pointpairlist kurva listCHX*/
            LineItem kurvaCHX;

            double[] xVal = new double[jumDataRow];
            double[] yVal = new double[jumDataRow];

            /*isi kurva (a) tiap channel (a)*/
            for (int a = 0; a < jumChannel; a++)
            {
                /*isi data(c) di tiap channel (a)*/
                int pembagi = 10000;
                float b = 0;

                dataChX = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(a)).ToArray();
                for (int c = 0; c < jumDataRow; c++)
                {
                    xVal[c] = b / sampleRate;
                    yVal[c] = float.Parse(dataChX[c]) / pembagi;
                    b++;
                }

                kurvaCHX = new LineItem(idChannelArr[a + 1], xVal, yVal, warnaKurva[a], SymbolType.None, 2.0f);
                kurvaCHX.Line.IsAntiAlias = true;
                myChart.CurveList.Add(kurvaCHX);

                isKgfExist(a);

                zedGraphControl1.AxisChange();
                //zedGraphControl1.Refresh();
            }
        }

        private void inisialisasi_grafik()
        {
            /* SETUP AWAL CHART */
            myChart = zedGraphControl1.GraphPane;

            //Font setupp
            myChart.IsFontsScaled = false;

            myChart.Title.FontSpec.Family = "HP Simplified";
            myChart.Title.FontSpec.Size = 15;
            myChart.Title.FontSpec.IsBold = true;

            myChart.XAxis.Title.FontSpec.Family = "HP Simplified";
            myChart.XAxis.Title.FontSpec.Size = 13;
            myChart.XAxis.Title.FontSpec.IsBold = true;

            myChart.YAxis.Title.FontSpec.Family = "HP Simplified";
            myChart.YAxis.Title.FontSpec.Size = 13;
            myChart.YAxis.Title.FontSpec.IsBold = true;

            myChart.Y2Axis.Title.FontSpec.Family = "HP Simplified";
            myChart.Y2Axis.Title.FontSpec.Size = 13;
            myChart.Y2Axis.Title.FontSpec.IsBold = true;

            //myChart.Fill = new Fill(Color.GreenYellow);  //outside chart
            //myChart.Chart.Fill = new Fill(Color.Black);   //inside chart   
            
            //Judul
            myChart.Title.Text = "Grafik Penyalaan & Gaya Dorong";// Debit (pressure)";                                 //VAR           

            /* SETUP AWAL AXIS */
            //Axis X
            myChart.XAxis.Title.Text = " detik";
            myChart.XAxis.Scale.Min = 0;
            myChart.YAxis.Scale.Min = 0;
            myChart.Y2Axis.Scale.Min = 0;

            //Axis Y
            myChart.YAxis.Title.Text = " bar ";     //VAR bisa: bar, kgf, Celcius

            // setup draw plotting
            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.AxisChange();     
        }

        private void tampilkan_tabel_analitik()
        {
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
                dtAnalytic.Columns.Add("SEL", typeof(bool));
                dtAnalytic.Columns.Add("CHANNEL", typeof(string));
                dtAnalytic.Columns.Add("X0", typeof(string));
                dtAnalytic.Columns.Add("X1", typeof(string));
                dtAnalytic.Columns.Add("<status>", typeof(string));

                //isi auto idChannel
                for(int a=0; a<jumChannel; a++)
                {
                    dtAnalytic.Rows.Add(true, idChannelArr[a+1]);
                }

                //binding dgv1 dengan dtAnalytic yg telah dibuat
                dataGridView1.DataSource = dtAnalytic;

                //style centang curve default 
                for(int a=0; a<jumChannel; a++)
                {
                    aksi_centang(a, true);
                }

                //style
                dataGridView1.Columns[0].Width = 28;

                // auto header style
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                }

                
                // auto color style & isi nomor rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.Font = new System.Drawing.Font("HP Simplified", 12F, FontStyle.Bold, GraphicsUnit.Pixel);

                    //isi row number & style cell 
                    row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    row.Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    row.HeaderCell.Value = (row.Index + 1).ToString();
                    
                    textBox1.Text = "here 3";

                    dataGridView1.Rows[0].Cells[0].Style.BackColor = warnaKurva[0];

                        textBox1.Text = "here 8";

                    //set color cell
                    for (int k = 0; k < jumChannel; k++)  // cara 1
                    {
                        dataGridView1.Rows[k].Cells[0].Style.BackColor = warnaKurva[k]; // kolom 0 (ID)
                        dataGridView1.Rows[k].Cells[1].Style.BackColor = warnaKurva[k]; // kolom 1 (channelName)
                        dataGridView1.Rows[k].Cells[0].Style.SelectionBackColor = warnaKurva[k];

                    }
                }
                dataGridView1.Refresh();
            }
            catch(Exception errz)
            {
                MessageBox.Show("Gagal menampilkan Tabel Analitik! "+errz.Message);
            }
        }

    #region TABEL_ANALITIK_CELL_KLIK_EVENT
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //pakai event cell value changed >> perlu masuk ke status checkboxnya
            // auto cek status CheckBox
            for (int a = 0; a < jumChannel; a++)
            {
                DataGridViewCheckBoxCell cBox = dataGridView1.Rows[a].Cells[0] as DataGridViewCheckBoxCell;
                if (e.ColumnIndex == dataGridView1.Columns[0].Index && e.RowIndex != -1)
                {
                    if (cBox != null && !DBNull.Value.Equals(cBox.Value) && (bool)cBox.Value == true)
                        aksi_centang(a, true);
                    else
                        aksi_centang(a, false);
                }
            }
        }

        private void create_cursor_vertical()
        {

        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //saat 1 event ngeklik dihitung 1 kali eksekusi (mouse up)
            if (e.ColumnIndex == dataGridView1.Columns[0].Index && e.RowIndex != -1)
                dataGridView1.EndEdit();
        }

        int seqClick = 0;
        public int getSeqClick_FChart0()
        {
            return seqClick;
        }
        private void btnMakeReport_Click(object sender, EventArgs e)
        {
            seqClick = 1;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            grafikAstm astm = new grafikAstm();
            astm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tampilkan_tabel_analitik();
        }

        private static bool isOdd(int val)
        {
            return val % 2 != 0;
        }

        int idxButton = 0;
        private void button9_Click(object sender, EventArgs e)
        {
            idxButton++;

            if(isOdd(idxButton))
            {
                Transition.run(panel4, "Height", 35, new TransitionType_EaseInEaseOut(500));
                //button9.Text = "show";
                button9.BackgroundImage = Properties.Resources.upBtn;
            }
            else
            {
                Transition.run(panel4, "Height", 232, new TransitionType_EaseInEaseOut(500));
                //button9.Text = "hide";
                button9.BackgroundImage = Properties.Resources.downBtn;
            }
        }


        //fungsi untuk show/hide curve, via kolom centang di DGV1
        private void aksi_centang(int idx, bool status)
        {
            if (status)
            {
                //curve visiblity true
                myChart.CurveList[idx].IsVisible = true;
                dataGridView1.Rows[idx].Cells[4].Style.BackColor = Color.GreenYellow;
            }

            else
            {
                //curve visibility false
                myChart.CurveList[idx].IsVisible = false;
                dataGridView1.Rows[idx].Cells[4].Style.BackColor = Color.Crimson;
            }
            zedGraphControl1.Refresh();
        }

    # endregion TABEL_ANALITIK_CELL_KLIK_EVENT
    }
}
