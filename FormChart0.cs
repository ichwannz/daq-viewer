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

        //variabel data header tabel
        private string[] dataChX, dataChDg;
        private string[] idChannelArr;
        private int jumChannel;
        private int jumDataRow;
        private int sampleRate;
        private string[] unitChannelArr;
        private string roketID;



        public FormChart0(DataTable dtRekapF1, int dt1, string[] dt2, string[] dt3, int dt4, int dt5, string dt6)
        {
            InitializeComponent();
            
            dtRekapF3 = dtRekapF1;  //ambil data tabel full
            jumDataRow = dt1;       //jumlah baris data
            unitChannelArr = dt2;   //nama satuan
            idChannelArr = dt3;     //nama channel
            jumChannel = dt4;       //jumlah channel yg dipakai
            sampleRate = dt5;
            roketID = dt6;

            //mulai form
            inisialisasi_grafik();
            plot_grafik();
        }
#region FORM_CHART
        LineItem kurvaCHX;

        private void plot_grafik()
        { 
            /*clearing area grafik dulu*/
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();
            myChart.CurveList.Clear();
            
            /*Pointpairlist kurva listCHX*/
            double[] xVal = new double[jumDataRow];
            double[] yVal = new double[jumDataRow];

            int pembagi = 10000;
            int pembagiVolt = 1000000;

            /*isi kurva (a) tiap Channel (a)  ===> tiap Kurva*/
            for (int a = 0; a < jumChannel; a++)
            {

                bool isVolt = false;    bool isBar = false;
                float b = 0;

                //cek Unit satuan >> tentukan pembagi Value
                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "bar")
                {
                    isBar = true;
                }

                if (String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "volt" || String.Format(unitChannelArr[a + 1], StringComparison.InvariantCultureIgnoreCase) == "V")
                {
                    isVolt = true;
                    pembagi = pembagiVolt;          //pengecualian untuk Volt, pembagi pakai 1000000
                }
                
                //isi data pointpair untuk kurva
                dataChX = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(a)).ToArray();
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
                if(isVolt)
                {
                    kurvaCHX.IsY2Axis = true;
                    myChart.Y2Axis.IsVisible = true;
                }

                if(isBar)
                {
                    kurvaCHX.YAxisIndex= 1 ;        //YAxis0
                    myChart.YAxisList[1].IsVisible = true;
                    //YAxis0.IsVisible = true;
                }
                
                myChart.CurveList.Add(kurvaCHX);
                
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                //zedGraphControl1.Refresh();
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
            myChart.XAxis.Title.FontSpec.Border = new Border(false,Color.Black,0);

            myChart.XAxis.Scale.Max = jumDataRow / sampleRate;
            myChart.XAxis.Scale.Min = 0;
            myChart.XAxis.MajorGrid.IsVisible = true;
            myChart.XAxis.MinorGrid.IsVisible = false;

            //setup Y-Axis
            myChart.YAxis.Title.Text = "Gaya dorong (Kgf)";
            myChart.YAxis.Title.FontSpec = new FontSpec("HP Simplified", 15, Color.Black,true,false,false);
            myChart.YAxis.Title.FontSpec.Border = new Border(false, Color.Black, 0);

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

            /*---------------------------------------------------------------------------ZGC_CONTROL_SETUP-------*/
            // setup draw plotter
            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.IsShowHScrollBar = true;
            zedGraphControl1.IsShowVScrollBar = true;
            zedGraphControl1.IsAutoScrollRange = true;

            zedGraphControl1.AxisChange();     
        }

#endregion FORM_CHART


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
                dtAnalytic.Columns.Add("NO", typeof(int));      //0
                dtAnalytic.Columns.Add("VIS", typeof(bool));    //1
                dtAnalytic.Columns.Add("CHANNEL", typeof(string));  //2
                dtAnalytic.Columns.Add("X1", typeof(string));       //3
                dtAnalytic.Columns.Add("X2", typeof(string));       //4
                dtAnalytic.Columns.Add("Unit Satuan", typeof(string)); //5

                //isi auto idChannel
                for(int a=0; a<jumChannel; a++)
                {
                    dtAnalytic.Rows.Add((a+1), true, idChannelArr[a+1],0,0,unitChannelArr[a+1]);
                }

                //binding dgv1 dengan dtAnalytic yg telah dibuat
                dataGridView1.DataSource = dtAnalytic;

                //style centang curve default 
                for(int a=0; a<jumChannel; a++)
                {
                    aksi_centang(a, true);
                }

                //style
                dataGridView1.Columns[0].Width = 25;
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].Width = 25;


                // auto header style
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);

                    col.ReadOnly = true;
                }
                dataGridView1.Columns[1].ReadOnly = false;

                
                // auto color style & isi nomor rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.DefaultCellStyle.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Regular, GraphicsUnit.Pixel);

                    //isi row number & style cell 
                    row.Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    row.Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    
                    //dataGridView1.Rows[1].Cells[1].Style.BackColor = warnaKurva[0];

                    //set color cell
                    for (int k = 0; k < jumChannel; k++)  // cara 1
                    {
                        dataGridView1.Rows[k].Cells[1].Style.BackColor = warnaKurva[k]; // kolom 1 (ID)
                        dataGridView1.Rows[k].Cells[2].Style.BackColor = warnaKurva[k]; // kolom 2 (channelName)
                        dataGridView1.Rows[k].Cells[1].Style.SelectionBackColor = warnaKurva[k];
                    }
                }
                dataGridView1.Refresh();
            }
            catch(Exception errz)
            {
                MessageBox.Show("Gagal menampilkan Tabel Analitik! "+errz.Message);
            }
        }
        private void create_cursor_vertical()
        {

        }

#region TABEL_ANALITIK_CELL_KLIK_EVENT
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

# endregion TABEL_ANALITIK_CELL_KLIK_EVENT
    
#region CHART_CLICK_EVENTSSS
        //point value waktu hovering
        PointPair ppCross;
        string strUnit;
        private string zedGraphControl1_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            ppCross = curve[iPt];
            /*
            for(int xx=0;xx<jumChannel;xx++)
            {
                if (curve.Label.Text == myChart.CurveList[xx].Label.Text)
                    strUnit = unitChannelArr[xx+1];
                else
                    strUnit = " ~";
            }

            */
            return curve.Label.Text + " : " + ppCross.X.ToString("#.##") + " detik; " + ppCross.Y.ToString("#.##") + " " + unitChannelArr[curve.YAxisIndex+1];  //strUnit;//
        }

        //fungsi klik baca value >> dgv1

        
#endregion CHART_CLICK_EVENTSS

#region Fungsi Hide/show Panel
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
               button9.BackgroundImage = Properties.Resources.upBtn;
            }
            else
            {
                Transition.run(panel4, "Height", 232, new TransitionType_EaseInEaseOut(500));
                button9.BackgroundImage = Properties.Resources.downBtn;
            }
        }
        #endregion Fungsi Hide/show Panel

        private void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            double X1, Y1;
            int X1int,idxData, pembagiVolt;

            myChart.ReverseTransform(e.Location, out X1, out Y1);

            X1 = X1 * 100;
            X1int = Convert.ToInt32(X1);

            for(int idx=0; idx<jumChannel; idx++)
            {
                if (String.Format(unitChannelArr[idx],StringComparison.InvariantCultureIgnoreCase) == "volt")
                {
                    pembagiVolt = 1000000;
                }
                else
                    pembagiVolt = 10000;

                dataChDg = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(idx)).ToArray();


                //idxData = dataChDg[X1int];
                dataGridView1.Rows[idx].Cells[3].Value = idxData.ToString();
            }
        }
    }
}
