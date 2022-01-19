using System;
using System.IO;
using System.Data;

namespace DACQViewer
{
    class baca_csv_ke_tabel
    {
      
        private string daqID;        //tipe DL850
        private string roketID;        //nama Roket ==> via comment
        private string sampleRate;
        private string timeID;
        private string dateID;
        private int nRow;   // jumlah baris
        private int nCol;   //jumlah kolom (banyak Channel+1)
        private int nChannel; //jumlah channel
        private int nData; //jumlah data sampling

        private string[] channelID;   //LIST judul Channel
        private string[] rowsDATA;
        private string[] channelUNIT;   //LIST satuan per channel

        private static DataTable dtRekap = new DataTable();

        //untuk debug
        //private string[] cekArrStr;
        //private string cekStr;

        public baca_csv_ke_tabel(Stream mystream)
        {
            string temp;
            string[] temps;

            DataTable dt = new DataTable();
            StreamReader sr = new StreamReader(mystream);

            #region DATA_HEADER
            //Ambil baris DAQ Device
            for (int a = 0; a < 1; a++)
                sr.ReadLine();
            temp = sr.ReadLine();
            temps = temp.Split(new Char[] { ',', '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            daqID = temps[2];

            //Ambil baris Nama Roket (kolom Comment)        ==> update per 2021-01-22: namaroket roketId diambil dari filename
            //langsung lanjut scanif
            temp = sr.ReadLine();
            //temps = temp.Split(new Char[] { ',', '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //roketID = temps[1];     // ==> comment harus diisi tipe roket
            
            //Ambil Judul tiap Channel (array)
            for (int a = 0; a < 1; a++) sr.ReadLine();             //lanjut ke baris 5, baris judul channel; angka '2' merupakan lanjutan dari scan#1 baris ke-3,4,5
            temp = sr.ReadLine();
            temps = temp.Split(new char[] { ' ', ',', '"' }, StringSplitOptions.RemoveEmptyEntries);
            channelID = temps;       //ARRAY string..berisi JUDUL Tiap CHANNEL

            nCol = channelID.Length;    //JUMLAH KOLOM
            nChannel = nCol - 1;        //JUMLAH CHANNEL

            //Ambil Nama Satuan (array)
            for (int a = 0; a < 1; a++) sr.ReadLine();
            temp = sr.ReadLine();
            temps = temp.Split(new char[] { ',', ' ', '"' }, StringSplitOptions.RemoveEmptyEntries);
            channelUNIT = temps;  

            //Ambil Sampling Rate
            temp = sr.ReadLine();
            temps = temp.Split(new char[] { ',', '"', '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            sampleRate = temps[1];

            //Ambil Tanggal Pengujian
            for (int a = 0; a < 5; a++) sr.ReadLine();
            temp = sr.ReadLine();
            temps = temp.Split(new Char[] { ',', ' ', '"' }, StringSplitOptions.RemoveEmptyEntries);
            dateID = temps[1];

            //Ambil Jam Pengujian
            temp = sr.ReadLine();
            temps = temp.Split(new Char[] { ',', '"', '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            timeID = temps[1];

            #endregion

            #region DATA_TABEL

            //AMBIL Jumlah Data Sampling (rows)
            // atau BISA DIGANTI DENGAN AMBIL kolom BLOCKSIZE .... jumlah data sampling uji
            temp = null;
            nRow = 0;
            while ((sr.ReadLine()) != null)     //ketika satu baris habis(ketemu null), maka dianggap Row=1
                if (nCol > 0) nRow++;           //menghitung jumlah Row ==> DARI ROW 1 !!
            nData = nRow;

            //Mulai Dari Awal..habis hitung nData
            sr.BaseStream.Seek(0, 0);                           //start readline dari awal
            for (int a = 0; a < 16; a++) sr.ReadLine();         //langsung ke baris 17

            //ENTRI KE DATATABLE 
            // Beri Judul tiap Kolom, ambil dari channelID
            for (int a = 0; a < nChannel; a++)
                dt.Columns.Add(channelID[a + 1]);

            // Ambil setiap baris-split-add rows DataTable
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                rowsDATA = temp.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                DataRow listRow = dt.NewRow();
                for (int a = 0; a < nChannel; a++)
                {
                    listRow[a] = float.Parse(rowsDATA[a]);          //string dibikin ke float (angka)
                }
                dt.Rows.Add(listRow);
            }
            sr.Close();
            dtRekap = dt.Copy();
            
            #endregion
        }

        public DataTable get_dataRekap() { return dtRekap; }
        public string get_daqID() { return daqID; }
        public string get_roketID()
        {
            //TIDAK DIPAKAI !
            if (roketID == null || roketID == "")
            {
                roketID = "[N/A]";
            }

            return roketID;
        }
        public string get_dateID() { return dateID; }
        public string get_timeID() { return timeID; }

        public string[] get_channelID() { return channelID; }   // ARRAY STRING judul2 CHANNEL
        public string[] get_channelUNIT() { return channelUNIT; } // ARRAY STRING  satuan tiap CHANNEL

        //fungsi debug
        public int get_jumlahCh() { return nChannel; }
        public int get_jumlahData() { return nData; }
        public string get_Sps() { return sampleRate; }

        //debug
        //public string[] get_cekArrStr() { return cekArrStr; }
        //public string get_cekStr() { return cekStr; }


    }
}
