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

namespace DACQViewer
{
    public partial class FormParams : Form
    {
        DataTable dtSensorNumbering = new DataTable();

        //private string roketID, daqID, dateID, timeID;
        private string[] idChannels, unitChannels;
        private int jumChannel, jumDataRow;

        public FormParams(DataTable dtRekapF1, string daqID, string roketID, string dateID, string timeID, int sampleRate, int jumCh, string[] idCh, string[] unitCh, int jumlahDataRow)
        {
            InitializeComponent();
            
            //Baca Header
            textBox1.Text = roketID;
            textBox2.Text = dateID;
            textBox10.Text = timeID;
            textBox3.Text = sampleRate.ToString();
            textBox9.Text = daqID;
            textBox11.Text = jumCh.ToString();

            jumChannel = jumCh;
            jumDataRow = jumlahDataRow;
            idChannels = idCh;
            unitChannels = unitCh;

            // Tabel 1
            //clear tabel first
            dtSensorNumbering.Clear();
            dtSensorNumbering.Rows.Clear();
            dtSensorNumbering.Columns.Clear();

            dtSensorNumbering = dtRekapF1.Copy();          // sudah ketambahan KOLOM0 NOMOR
            create_tabel1_sensor();

            // Tabel 2
            create_tabel2_params();
        }
     #region CREATE DGV
        //tabel sensor value
        private void create_tabel1_sensor()
        {
            //Inisialisasi tabel
            //isi kolom nomer
            dtSensorNumbering.Columns.Add("No.Data", typeof(int)).SetOrdinal(0);

            for (int a = 0; a < jumDataRow; a++)
                dtSensorNumbering.Rows[a][0] = a;

            //init dgv
            if (dataGridView1 != null)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
            }

            //dtRekapF2.Columns.Add("No.", typeof(int)).SetOrdinal(0);  //sudah di Form1
            dataGridView1.DataSource = dtSensorNumbering;
            dataGridView1.Columns[0].Width = 55;

            //style
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                
                col.ReadOnly = true;        //semua readonly..
            }
            dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Refresh();
        }

        //tabel parameter
        DataGridViewComboBoxColumn cbSensorType = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn cbSensorSign = new DataGridViewComboBoxColumn();
        private void create_tabel2_params()
        {
            //bikin comboBox
            create_comboBox_ChParams();

            //bikin data table parameter
            try
            {
                //clear dgv2
                dataGridView2.DataSource = null;
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                dataGridView2.Refresh();

                //bikin datagridview2 column header
                dataGridView2.Columns.Add("nM", "No");            //0
                dataGridView2.Columns.Add("chName", "Channel Name");        //1
                dataGridView2.Columns.Add("chNo", "Channel No.");           //2
                dataGridView2.Columns.Add("unSat", "Unit Satuan");      //3
                dataGridView2.Columns.Add(cbSensorType);                //4
                dataGridView2.Columns.Add("sn", "Sensor Serial No.");   //5
                dataGridView2.Columns.Add("fs", "Sensor Full Scale");   //6
                dataGridView2.Columns.Add(cbSensorSign);            //7
                dataGridView2.Columns.Add("date", "Last Calibration Date");     //8

                //style
                //dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[0].Width = 30;
                dataGridView2.Columns[0].ReadOnly = true;
                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.Columns[1].Width = 100;
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[3].Width = 65;
                dataGridView2.Columns[3].ReadOnly = true;
                dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ((DataGridViewTextBoxColumn)dataGridView2.Columns[2]).MaxInputLength = 2;

                
                //bikin style setiap Header Column sama identik
                foreach(DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;                    
                }

                //masukkan data tiap rows
                for (int a=0; a< jumChannel;a++)
                    dataGridView2.Rows.Add((a + 1), idChannels[a + 1],0, unitChannels[a+1]);
                
                dataGridView2.Refresh();
            }
            catch(Exception)
            {
                MessageBox.Show("Tabel Param Error");
            }
        }

        private void create_comboBox_ChParams()
        {

            cbSensorType.HeaderText = "Sensor Type";
            cbSensorType.Items.Add("TML LoadCell");
            cbSensorType.Items.Add("PT750 Pressure");
            cbSensorType.Items.Add("HPT200 Pressure");
            cbSensorType.Items.Add("HPT902 Pressure");
            cbSensorType.Items.Add("lain-lain");
            cbSensorType.Name = "cbSenType";

            cbSensorSign.HeaderText = "Sensor Signal";
            cbSensorSign.Items.Add("Strain");
            cbSensorSign.Items.Add("4-20 mAmp (ADC)");
            cbSensorSign.Items.Add("DC Voltage");
            cbSensorSign.Name = "cbSenSign";


            //dataGridView2.Columns.Add(dgv2ColorColumn); // dipakai di tabel parameter chart saja

        }

        #endregion


        #region FUNGSI SAVE PARAMETER

        string logPath = "D:/uslog.daqt";
        private void make_txt_test_log()
        {
            File.WriteAllLines(logPath, rtbTestLog.Lines);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //create tag kalau sudah oke tabelnya
            //create csv file
            make_csv_from_dgv();    // untuk dtHasil1 & dtHasil2
            make_dtt_from_tb();     // untuk dtHasil0 & dtHasil3
            make_txt_test_log();

            MessageBox.Show("Sudah berhasil disimpan !");

        }



        /*------------------------------------------------------------------------------------------------------- create CSV untuk tiap2 cluster --*/

        string[] csv_path_tabel = new string[4]
        {
            "D:/temp0.daqc",    //dtHasil0
            "D:/temp1.daqc",    
            "D:/temp2.daqc",
            "D:/temp3.daqc"     //dtHasil3
        };
        
        //temporary path windows, naming nya random *.tmp
        //string path_temporary = Path.GetTempFileName(); 

        private void make_csv_from_dgv() //export dgv2 ke csv
        {
            StringBuilder sb = new StringBuilder();

            //dgv1
            //ambil header kolom
            var header1 = dataGridView1.Columns.Cast<DataGridViewColumn>();
            sb.AppendLine(string.Join(";", header1.Select(col1 => col1.HeaderText).ToArray()));

            //ambil tiap baris
            foreach(DataGridViewRow dr in dataGridView1.Rows)
            {
                var cellz = dr.Cells.Cast<DataGridViewCell>();
                sb.AppendLine(string.Join(";", cellz.Select(celz => celz.Value).ToArray()));
            }
            File.WriteAllText(csv_path_tabel[1], sb.ToString());

            //clear sb
            sb.Clear();

            //dgv2
            //ambil judul kolom
            var header2 = dataGridView2.Columns.Cast<DataGridViewColumn>();
            //sb.AppendLine(string.Join(";", headers.Select(col => "\"" + col.HeaderText + "\"").ToArray()));

            sb.AppendLine(string.Join(";", header2.Select(col2 => col2.HeaderText).ToArray()));

            //ambil tiap row
            foreach (DataGridViewRow dr in dataGridView2.Rows)
            {
                var cellz = dr.Cells.Cast<DataGridViewCell>();
                sb.AppendLine(string.Join(";", cellz.Select(celz => celz.Value).ToArray()));
            }

            //tulis ke file csv
            File.WriteAllText(csv_path_tabel[2], sb.ToString());

        }
        private void make_dtt_from_tb()
        {
            DataTable dtHasil0 = new DataTable();
            DataTable dtHasil3 = new DataTable();

            dtHasil0.Clear();
            dtHasil0.Rows.Clear();
            dtHasil3.Clear();
            dtHasil3.Rows.Clear();
            
            //Parameter DAQ Scope
            dtHasil0.Columns.Add("variabel", typeof(string)).SetOrdinal(0);
            dtHasil0.Columns.Add("value", typeof(string)).SetOrdinal(1);

            dtHasil0.Rows.Add("scopeID", textBox9.Text);
            dtHasil0.Rows.Add("roketID", textBox1.Text);
            dtHasil0.Rows.Add("dateID", textBox2.Text);
            dtHasil0.Rows.Add("timeID", textBox10.Text);
            dtHasil0.Rows.Add("spRate", textBox3.Text);
            dtHasil0.Rows.Add("channelJum", textBox11.Text);

            //Parameter Firing Ignition
            dtHasil3.Columns.Add("variabel", typeof(string)).SetOrdinal(0);
            dtHasil3.Columns.Add("value", typeof(string)).SetOrdinal(1);

            dtHasil3.Rows.Add("volt", textBox4.Text);
            dtHasil3.Rows.Add("iamp", textBox8.Text);
            dtHasil3.Rows.Add("ignOhm", textBox5.Text);
            dtHasil3.Rows.Add("wireOhm", textBox6.Text);
            dtHasil3.Rows.Add("trigLength", textBox7.Text);

            make_csv_from_dtt(dtHasil0, 0);
            make_csv_from_dtt(dtHasil3, 3);
        }

        private void make_csv_from_dtt(DataTable dt,int idxPath)
        {
            StringBuilder sb = new StringBuilder();

            string[] headers = dt.Columns.Cast<DataColumn>()
                .Select(col => col.ColumnName)
                .ToArray();
            sb.AppendLine(string.Join(";", headers));

            foreach(DataRow dr in dt.Rows)
            {
                string[] cells = dr.ItemArray.Select(celz => celz.ToString()).ToArray();
                sb.AppendLine(string.Join(";", cells));
            }

            File.WriteAllText(csv_path_tabel[idxPath], sb.ToString());
        }


        /*------------------------------------------------------------------------------------------------------- reload CSV untuk tiap2 cluster --*/

        //reload dari save2an csv, bikin datatablem masukkan ke dgv
        private void make_dgv_from_csv(DataGridView dgv, int idxCsv)
        {
            DataTable dtHasilx = new DataTable();
                        
            //isi ke DT
            File.ReadLines(csv_path_tabel[idxCsv]).Take(1)
                .SelectMany(x => x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dtHasilx.Columns.Add(x.Trim()));
           

            File.ReadLines(csv_path_tabel[idxCsv]).Skip(1)
                .Select(x => x.Split(';'))
                .ToList()
                .ForEach(line => dtHasilx.Rows.Add(line));

            //isi ke DGV2
            foreach(DataRow dr in dtHasilx.Rows)
            {
                dgv.Rows.Add(dr.ItemArray);
            }
        }

        private void make_tb_from_csv(int idxCsv)
        {
            //baca csv masukkan ke dt
            DataTable dtHasily = new DataTable();

            //isi ke DT
            File.ReadLines(csv_path_tabel[idxCsv]).Take(1)
                .SelectMany(x => x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dtHasily.Columns.Add(x.Trim()));


            File.ReadLines(csv_path_tabel[idxCsv]).Skip(1)
                .Select(x => x.Split(';'))
                .ToList()
                .ForEach(line => dtHasily.Rows.Add(line));

            //Isi Parameter Firing Ignition
            textBox4.Text = dtHasily.Rows[0][1].ToString(); //volt
            textBox8.Text = dtHasily.Rows[1][1].ToString(); //iamp
            textBox5.Text = dtHasily.Rows[2][1].ToString(); //ignOhm
            textBox6.Text = dtHasily.Rows[3][1].ToString(); //wireOhm
            textBox7.Text = dtHasily.Rows[4][1].ToString(); //trigLength


            //Isi Parameter DAQ Scope
            /*
            textBox9.Text       //scopeID
            textBox1.Text       //roketID
            textBox2.Text       //dateID
            textBox10.Text      //timeID
            textBox3.Text       //spRate
            textBox11.Text      //channelJum
            */
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = null;
            //dataGridView1.Rows.Clear();
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();

            //make_dgv_from_csv(dataGridView1, 1);  //dgv sensorval
            make_dgv_from_csv(dataGridView2, 2);    //dgv params daq

            dataGridView1.Refresh();
            dataGridView2.Refresh();

            make_tb_from_csv(3);        //params firing

            //hapus file csv temporary
            //File.Delete(csv_dgv_path);
        }

     #endregion

     #region Fungsi DateTimePicker di kolom 9

        //Initialized a new DateTimePicker Control
        DateTimePicker calendarPicker = new DateTimePicker();  
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If any cell is clicked on the Second column which is our date Column  
            if (e.ColumnIndex == 8 && e.RowIndex>=0)
            {
                //Adding DateTimePicker control into DataGridView   
                dataGridView2.Controls.Add(calendarPicker);

                // Setting the format (i.e. 2014-10-10)  
                calendarPicker.Format = DateTimePickerFormat.Short;

                // It returns the retangular area that represents the Display area for a cell  
                Rectangle calendarBox = dataGridView2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                //Setting area for DateTimePicker Control  
                calendarPicker.Size = new Size(calendarBox.Width, calendarBox.Height);

                // Setting Location  
                calendarPicker.Location = new Point(calendarBox.X, calendarBox.Y);

                // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
                calendarPicker.CloseUp += new EventHandler(calendarPicker_CloseUp);

                // An event attached to dateTimePicker Control which is fired when any date is selected  
                calendarPicker.TextChanged += new EventHandler(calendarPicker_OnTextChange);

                // Now make it visible  
                calendarPicker.Visible = true;
            }
        }
        private void calendarPicker_OnTextChange(object sender, EventArgs e)
        {
            // Saving the 'Selected Date on Calendar' into DataGridView current cell  
            dataGridView2.CurrentCell.Value = calendarPicker.Text.ToString();
        }

        private void calendarPicker_CloseUp(object sender, EventArgs e)
        {
            calendarPicker.Visible = false;
        }
        #endregion


        /*fungsi untuk FORCE HURUF kAPITAL & NUMERIC ACCEPTANCE DI SPESIFIK KOLOM*/
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Force untuk Capslock
            if (dataGridView2.CurrentCell.ColumnIndex == 5)     // KOLOM 6 input force to capslock
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
                }
            }

            // Hanya terima input numeric
            e.Control.KeyPress -= new KeyPressEventHandler(col_Keypress);       //bikin event baru

            if (dataGridView2.CurrentCell.ColumnIndex == 2 || dataGridView2.CurrentCell.ColumnIndex == 6) //kolom yg diinginkan
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(col_Keypress);
                }
            }
        }
 
        //sub-fungsi untuk otomatis klik cell combobox
        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);

        }

        //sub-fungsi untuk cek numeric
        private void col_Keypress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
