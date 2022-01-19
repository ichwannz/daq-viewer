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
        DataTable dtRekapF2 = new DataTable();
        DataTable dtParam_old = new DataTable();
        DataTable dtParam_upd = new DataTable();
        DataTable dtFiringParam = new DataTable();

       

        
        //private string roketID, daqID, dateID, timeID;
        private string[] idChannels, unitChannels;
        private int jumChannel;

        public FormParams(DataTable dtRekapF1, string daqID, string roketID, string dateID, string timeID, int sampleRate, int jumCh, string[] idCh, string[] unitCh)
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
            idChannels = idCh;
            unitChannels = unitCh;

            // Tabel 1
            //clear tabel first
            dtRekapF2.Clear();
            dtRekapF2 = dtRekapF1;          // sudah ketambahan KOLOM0 NOMOR
            create_Tabel1();

            // Tabel 2
            create_ChannelParams();
        }

        private void create_Tabel1()
        {
            //Inisialisasi tabel
            if (dataGridView1 != null)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
            }

            //dtRekapF2.Columns.Add("No.", typeof(int)).SetOrdinal(0);  //sudah di Form1
            dataGridView1.DataSource = dtRekapF2;
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

        #region TABEL CH PARAMS
        DataGridViewComboBoxColumn cbSensorType = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn cbSensorSign = new DataGridViewComboBoxColumn();
        private void create_ChannelParams()
        {
            //clear dgv2
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.Refresh();


            //bikin comboBox
            create_comboBox_ChParams();

            //bikin data table parameter
            try
            {
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
                dataGridView2.Columns[0].Width = 30;
                dataGridView2.Columns[0].ReadOnly = true;
                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.Columns[1].Width = 100;
                dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ((DataGridViewTextBoxColumn)dataGridView2.Columns[2]).MaxInputLength = 2;

                dataGridView2.Columns[3].ReadOnly = true;
                
                //bikin style setiap Header Column sama identik
                foreach(DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new System.Drawing.Font("HP Simplified", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;                    
                }

                //masukkan data tiap rows
                if (DAQCH_SAVED)
                {
                    load_dbChannelParams();

                    //dataGridView2.DataSource = dtParam_upd;



                    //load yg sudah ada
                    using (Form1 F1 = new Form1())
                    {
                        dataGridView2.DataSource = F1.dtParamsChannel;
                    }

                }
                else
                {
                    //bikin dari awal
                    for (int a=0; a< jumChannel;a++)
                    {
                        dataGridView2.Rows.Add((a + 1), idChannels[a + 1],0, unitChannels[a+1]);
                        //dataGridView2.Rows[a].Cells[5].Value = unitChannels[a + 1];
                    }
                }

                //isi dengan data di tiap row
                
                
                dataGridView2.Refresh();
            }
            catch(Exception)
            {
                MessageBox.Show("Tabel Param Error");
            }
        }

        private void load_dbChannelParams()
        {/*
            var dt = new DataTable();
            foreach (DataGridViewColumn col in dataGridView2.Columns)
            {
                if (col.Visible)
                {
                    dt.Columns.Add();
                }
            }

            var cell = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cell[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cell);
            }
        */

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

        int seqClick = 0;
        public int getSeqClick_FTabelz()
        {
            return seqClick;
        }
        #region SAVE UPDATE TABEL CH PARAMS
        private void button2_Click(object sender, EventArgs e)
        {
            seqClick = 1;

            dtParam_upd.Clear();
            dtParam_upd.Rows.Clear();
            dtParam_upd.Columns.Clear();
            
            /* CARA-2 : pakai fungsi konvert*/
            dtParam_upd = updateDataParams(dataGridView2);


            /* CARA-1 : pindah data dari datagridview2 ke datatable*/
           // dtParam_upd= (DataTable)(dataGridView2.DataSource);

            using (Form1 F1 = new Form1())
            {
                //F1.dtParamsChannel = (DataTable)(dataGridView2.DataSource);
                F1.dtParamsChannel = dtParam_upd;
            }


            DAQCH_SAVED = true;


            //Form1 f1 = new Form1();
            //f1.Show();
            //this.Hide();
        }

        private DataTable updateDataParams(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                {
                    dt.Columns.Add();
                }
            }

            var cell = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cell[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cell);
            }
            return dt;
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

        private bool DAQCH_SAVED, FIRING_SAVED = false;

        //BUTTON save firing params
        private void button3_Click(object sender, EventArgs e)
        {
            //lebih enak pakai dataset saja?
            dtFiringParam.Clear();
            dtFiringParam.Columns.Clear();
            dtFiringParam.Rows.Clear();

            dtFiringParam.Columns.Add("Volt Trigger");
            dtFiringParam.Columns.Add("Ohm Igniter");
            dtFiringParam.Columns.Add("Ohm Cable");
            dtFiringParam.Columns.Add("Duration Trigger");

            dtFiringParam.Rows.Add(textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text);

            MessageBox.Show("Parameter Firing Ignition sudah berhasil tersimpan !");
            FIRING_SAVED = true;

            if(DAQCH_SAVED && FIRING_SAVED )
            {
                //button1.Enabled = true;
            }

        }

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


        // fungsi screen capture 
        private int autonaming;
        private void button4_Click(object sender, EventArgs e)
        {
           

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

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
