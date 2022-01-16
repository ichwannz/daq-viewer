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
        //create objek dulu
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
        Color[] warnaCursor = new Color[6]
        {
            Color.Silver,
            Color.Black,
            Color.Blue,
            Color.Red,
            Color.Red,
            Color.Blue
        };

        //variabel data header tabel parameter daq
        private string roketID;
        private string[] dataChX, dataChDg, idChannelArr, unitChannelArr;
        private int jumChannel, jumDataRow, sampleRate;

        grafikAstm showAstm = new grafikAstm();

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

            panelSQ.Visible = false;
            //mulai form
            showAstm.Show();

            inisialisasi_grafik();
            plot_grafik();

            //if (showAstm.getAck())
            //    tampilkan_tabel_analitik();
        }

    #region FORM_CHART
        GraphPane myChart = new GraphPane();    //objek zedgraph
        LineItem kurvaCHX;
        bool tag_f, tag_p, tag_v;

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

                bool isVolt = false;    bool isBar = false; bool isKgf = false;
                float b = 0;

                //cek Unit satuan >> tentukan pembagi Value
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
                    pembagi = pembagiVolt;          //pengecualian untuk Volt, pembagi pakai 1000000
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
                if(isVolt)
                {
                    kurvaCHX.IsY2Axis = true;
                    myChart.Y2Axis.IsVisible = true;

                    idxColIgn = a;
                }

                if(isBar)
                {
                    kurvaCHX.YAxisIndex= 1 ;        //YAxis0
                    myChart.YAxisList[1].IsVisible = true;
                    //YAxis0.IsVisible = true;
                    idxColBar = a;
                }

                if(isKgf)
                {
                    idxColKgf = a;          //posisi index kolom kgf (F Thrust)
                }
                
                myChart.CurveList.Add(kurvaCHX);
                
                zedGraphControl1.AxisChange();
                //zedGraphControl1.Invalidate();
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

            /*---------------------------------------------------------------------------ZGC_CONTROL_SETUP-------*/
            // setup draw plotter
            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.IsShowHScrollBar = true;
            zedGraphControl1.IsShowVScrollBar = true;
            zedGraphControl1.IsAutoScrollRange = true;

            zedGraphControl1.AxisChange();     
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //replotting dengan data refining
        }
        #endregion FORM_CHART

        #region TABEL_ANALITIK
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
                dtAnalytic.Columns.Add("KURVA", typeof(bool));    //1
                dtAnalytic.Columns.Add("CHANNEL", typeof(string));  //2
                dtAnalytic.Columns.Add("NILAI (detik-X)", typeof(string));       //3
                dtAnalytic.Columns.Add("UNIT", typeof(string)); //4

                //isi auto idChannel
                for(int a=0; a<jumChannel; a++)
                {
                    dtAnalytic.Rows.Add((a+1), true, idChannelArr[a+1],0,unitChannelArr[a+1]);
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
            catch(Exception errz)
            {
                MessageBox.Show("Gagal menampilkan Tabel Analitik! "+ Environment.NewLine + errz.Message);
            }
        }
    #endregion TABEL_ANALITIK

    #region BUTTON
        private void btnMakeReport_Click(object sender, EventArgs e)
        {
            seqClick = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            grafikAstm astm = new grafikAstm();
            astm.Show();
        }

        int seqClick = 0;
        public int getSeqClick_FChart0()
        {
            return seqClick;
        }

        #endregion BUTTON


/*-------------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------------------- EVENT HANDLER ------*/
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
            
            return curve.Label.Text + " : " + ppCross.X.ToString("#.##") + " detik; " + ppCross.Y.ToString("#.##") + " " + unitChannelArr[curve.YAxisIndex+1];  //strUnit;//
        }

    //fungsi klik baca value >> dgv1
        private void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            double X1, Y1, pembagiVolt;
            if(e.Button==MouseButtons.Left)
            {
                if(idxCursor==0)
                    tampilkan_tabel_analitik();

                idxCursor++; //urutan cursor, warna, 

                myChart.ReverseTransform(e.Location, out X1, out Y1);

                create_cursorx_lineObj(X1);

                X1 = X1 * 100;

                for (int idx = 0; idx < jumChannel; idx++)
                {
                    if (String.Format(unitChannelArr[idx + 1], StringComparison.InvariantCultureIgnoreCase) == "volt")
                    {
                        pembagiVolt = 1000000;
                    }
                    else
                        pembagiVolt = 10000;

                    dataChDg = dtRekapF3.AsEnumerable().Select(r => r.Field<string>(idx)).ToArray();

                    string dat0 = dataChDg[Convert.ToInt32(X1)];
                    double dat1 = double.Parse(dat0);
                    double dat2 = dat1 / pembagiVolt;

                    dataGridView1.Rows[idx].Cells[3].Value = dat2.ToString("#.000");
                }
                    
                //ambil nilai CursorX, tergantung button 
                if(btnSQ1cek)
                {
                    tZero = X1 / 100;   //time Zero, saat sinyal diberikan

                    btnSQ1.BackColor = Color.Lime;
                    btnSQ1cek = false;
                }

                if(btnSQ2cek)
                {
                    tA = X1 / 100;      //time A, saat ignition dimulai
                    tign=(tA-tZero);
                    textBox8.Text = tign.ToString("#.000")+"  ";

                    btnSQ2.BackColor = Color.Lime;
                    btnSQ2cek = false;
                }

                if(btnSQ3cek)
                {
                    tB = X1/100;

                    btnSQ3.BackColor = Color.Lime;
                    btnSQ3cek = false;
                }

                if(btnSQ4cek)
                {
                    tE = X1/sampleRate;
                    tburnEff = (tE - tB);
                    textBox2.Text = tburnEff.ToString("#.000") + "  ";

                    btnSQ4.BackColor = Color.Lime;
                    btnSQ4cek = false;                               
                }

                if(btnSQ5cek)
                {
                    tF = X1/100;
                    taction = (tF - tB);
                    textBox3.Text = taction.ToString("#.000") + "  ";

                    btnSQ5.BackColor = Color.Lime;
                    btnSQ5cek = false;
                }
                
            }
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
        int idxCursor = 0;
        private void create_cursorx_lineObj(double xval)
        {                                    
            //reset cursor jumlah
            if (idxCursor > 5)      
            {
                idxCursor = 0;
                myChart.GraphObjList.Clear();
                zedGraphControl1.GraphPane.GraphObjList.Clear();
                zedGraphControl1.Refresh();
            }  

            //bikin line cursorX
            //LineObj cursorx1 = new LineObj(Color.Black, x1, myChart.YAxis.Scale.Min, x1, myChart.YAxis.Scale.Max);
            LineObj cursorx1 = new LineObj(warnaCursor[idxCursor], xval, 0, xval,1 );   //index idxCursor langsung nilai 1 ...untuk warnaCursor
            cursorx1.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
            cursorx1.IsClippedToChartRect = true;
            cursorx1.Line.Width = 2;
            cursorx1.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            cursorx1.ZOrder = ZOrder.E_BehindCurves;

            myChart.GraphObjList.Add(cursorx1);
            zedGraphControl1.Refresh();
        }

        private void create_textObj()
        {
            TextObj labelA = new TextObj("1) Klik sekali untuk memilih titik awal \n 2)Klik kedua...",
                1f, 1f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom);

            labelA.FontSpec.StringAlignment = StringAlignment.Near;

            myChart.GraphObjList.Add(labelA);
        }

    //fungsi box burning area
        double x1box, x2box;
        private void create_burning_area()
        {
            x1box = tB;
            BoxObj boxBurning = new BoxObj(x1box,15000,tburnEff,15000, Color.Empty, Color.FromArgb(50, Color.Crimson));
            boxBurning.Location.CoordinateFrame = CoordType.AxisXYScale;
            boxBurning.Location.AlignV = AlignV.Top;
            boxBurning.Location.AlignH = AlignH.Left;
            boxBurning.ZOrder = ZOrder.E_BehindCurves;
            myChart.GraphObjList.Add(boxBurning);

            TextObj texBurning = new TextObj("Area waktu bakar efektif", (x1box+(tburnEff*0.1)),10000);
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


        #endregion CHART_CLICK_EVENTSS
        
    #region FUNGSI AMBIL DATA CURSOR

        //variabel hitung-hitungan
        bool btnSQ1cek, btnSQ2cek, btnSQ3cek, btnSQ4cek, btnSQ5cek;
        double tZero, tA, tB, tE, tF;           // titik2 dalam skema ASTM (A, B, C...)dalam detik, untuk dapet data-urut, perlu dikali 10
        double tign, tburnEff, taction;

        List<string[]> dataus = new List<string[]>();

        int idxColKgf, idxColBar, idxColIgn;
        double impuls_tot, impuls_sp, w_prop;

        private void hitung_analisis_astm()
        {
            double[] data_f, data_p, data_v;
            double[] data_f_fine_eff, data_p_fine_eff;                  //burning effective
            double[] data_f_fine_tot, data_p_fine_tot, data_v_fine_tot; //burning total/action

            double f_max, f_ave, p_max, p_ave;

            int refineSkip, refineTake;             //refining data untuk effective burning
            int refineSkip_tot, refineTake_tot;     //refining data untuk total burning
            
            //convert string[] ke double
                //data_f = Array.ConvertAll(dataus[idxColKgf], s => double.Parse(s));   //pakai LAMBDA :convert String[] to double[]
                //data_f = Array.ConvertAll(dataus[idxColKgf],double.Parse);            //convert String[] to double[]
            data_f = dataus[idxColKgf].Select(double.Parse).ToArray();  // THRUST_F       //pakai LINQ : convert String[] to double[]
            data_p = dataus[idxColBar].Select(double.Parse).ToArray();  // PRESSURE_P
            data_v = dataus[idxColIgn].Select(double.Parse).ToArray();  //IGNITION_V
            
            //cari parameter
            f_max = data_f.Max();
            f_max = f_max / 10000;
            p_max = data_p.Max();
            p_max = p_max / 10000;

            //ACTION AREA ~ total burning
            refineSkip_tot = Convert.ToInt32(tA * sampleRate);
            refineTake_tot = Convert.ToInt32(taction * sampleRate);

            data_f_fine_tot = data_f.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
            data_p_fine_tot = data_p.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
            data_v_fine_tot = data_v.Skip(refineSkip_tot).Take(refineTake_tot).ToArray();
            


            //BURNING AREA - effective buning
            refineSkip = Convert.ToInt32(tB * sampleRate);
            refineTake = Convert.ToInt32(tburnEff * sampleRate);

            data_f_fine_eff = data_f.Skip(refineSkip).Take(refineTake).ToArray();
            data_p_fine_eff = data_p.Skip(refineSkip).Take(refineTake).ToArray();

            //rerata dari BURNING TIME EFFECTIVE !
            f_ave = data_f_fine_eff.Average();
            p_ave = data_p_fine_eff.Average();

          if(tbWpropelan!=null)
            {
                w_prop = double.Parse(tbWpropelan.Text);

                impuls_tot = (f_ave/10000 * tburnEff);            //impuls total= Thrust rerata*TimeBurning effective
                impuls_sp = (impuls_tot / w_prop);          //impuls sp=impuls total/berat propelan
            }
          
            //tampilkan
            textBox4.Text = impuls_tot.ToString("#.000") + "  ";
            textBox5.Text = impuls_sp.ToString("#.000") + "  ";

            textBox7.Text = (f_ave / 10000).ToString("#.000") + "  ";
            textBox10.Text = (p_ave / 10000).ToString("#.000") + "  ";

            textBox6.Text = f_max.ToString("#.000")+"  ";
            textBox1.Text = p_max.ToString("#.000")+"  ";

            textBox11.Text = jumDataRow.ToString();
            textBox13.Text = sampleRate.ToString();
        }

        private void btnSQ1_Click(object sender, EventArgs e)
        {
            panel7.Visible = true;
            btnSQ1.BackColor = Color.Crimson;
            btnSQ1cek = true;
        }

        private void btnSQ2_Click(object sender, EventArgs e)
        {
            panel8.Visible = true;
            btnSQ2.BackColor = Color.Crimson;
            btnSQ2cek = true;
        }

        private void btnSQ3_Click(object sender, EventArgs e)
        {
            panel9.Visible = true;
            btnSQ3.BackColor = Color.Crimson;
            btnSQ3cek = true;
        }

        private void btnSQ4_Click(object sender, EventArgs e)
        {
            panel10.Visible = true;
            btnSQ4.BackColor = Color.Crimson;
            btnSQ4cek = true;
        }

        private void btnSQ5_Click(object sender, EventArgs e)
        {
            panel5.Visible = true;
            btnSQ5.BackColor = Color.Crimson;
            btnSQ5cek = true;
        }

        private void btnSQ6_Click(object sender, EventArgs e)
        {
            hitung_analisis_astm();
            create_burning_area();
        }
        
    #endregion FUNGS AMBIL DATA CURSOR

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
