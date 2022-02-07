namespace DACQViewer
{
    partial class FormReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReport));
            this.btnSavePdfReport = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnViewPdfRreport = new System.Windows.Forms.Button();
            this.btnMakeZip = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSavePdfReport
            // 
            this.btnSavePdfReport.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSavePdfReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSavePdfReport.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePdfReport.Location = new System.Drawing.Point(16, 64);
            this.btnSavePdfReport.Name = "btnSavePdfReport";
            this.btnSavePdfReport.Size = new System.Drawing.Size(198, 45);
            this.btnSavePdfReport.TabIndex = 0;
            this.btnSavePdfReport.Text = "SIMPAN  FORM-US  (pdf)";
            this.btnSavePdfReport.UseVisualStyleBackColor = false;
            this.btnSavePdfReport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(16, 143);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(198, 141);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "- Tabel Konfirmasi nilai hasil penghitungan ISP, Impuls, dkk\n- Hasil Report FORM-" +
    "UJI-STATIS\n\n- lampiran photo gambar2 penting";
            // 
            // btnViewPdfRreport
            // 
            this.btnViewPdfRreport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewPdfRreport.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold);
            this.btnViewPdfRreport.Location = new System.Drawing.Point(16, 12);
            this.btnViewPdfRreport.Name = "btnViewPdfRreport";
            this.btnViewPdfRreport.Size = new System.Drawing.Size(198, 46);
            this.btnViewPdfRreport.TabIndex = 2;
            this.btnViewPdfRreport.Text = "ISI  FORM-US";
            this.btnViewPdfRreport.UseVisualStyleBackColor = true;
            this.btnViewPdfRreport.Click += new System.EventHandler(this.btnViewPdfRreport_Click);
            // 
            // btnMakeZip
            // 
            this.btnMakeZip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMakeZip.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMakeZip.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold);
            this.btnMakeZip.Location = new System.Drawing.Point(16, 413);
            this.btnMakeZip.Name = "btnMakeZip";
            this.btnMakeZip.Size = new System.Drawing.Size(198, 46);
            this.btnMakeZip.TabIndex = 2;
            this.btnMakeZip.Text = "BUAT ZIP FINAL REPORT";
            this.btnMakeZip.UseVisualStyleBackColor = true;
            this.btnMakeZip.Click += new System.EventHandler(this.btnMakeZip_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMakeZip);
            this.panel1.Controls.Add(this.btnViewPdfRreport);
            this.panel1.Controls.Add(this.btnSavePdfReport);
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(258, 472);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.axAcroPDF1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(258, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(795, 472);
            this.panel2.TabIndex = 4;
            // 
            // axAcroPDF1
            // 
            this.axAcroPDF1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAcroPDF1.Enabled = true;
            this.axAcroPDF1.Location = new System.Drawing.Point(0, 0);
            this.axAcroPDF1.Name = "axAcroPDF1";
            this.axAcroPDF1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF1.OcxState")));
            this.axAcroPDF1.Size = new System.Drawing.Size(795, 472);
            this.axAcroPDF1.TabIndex = 0;
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 472);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormReport";
            this.Text = "FormPerformance";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSavePdfReport;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnViewPdfRreport;
        private System.Windows.Forms.Button btnMakeZip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private AxAcroPDFLib.AxAcroPDF axAcroPDF1;
    }
}