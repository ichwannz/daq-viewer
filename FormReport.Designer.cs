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
            this.btnSavePdfReport = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnViewPdfRreport = new System.Windows.Forms.Button();
            this.btnMakeZip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSavePdfReport
            // 
            this.btnSavePdfReport.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSavePdfReport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSavePdfReport.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePdfReport.Location = new System.Drawing.Point(12, 64);
            this.btnSavePdfReport.Name = "btnSavePdfReport";
            this.btnSavePdfReport.Size = new System.Drawing.Size(100, 45);
            this.btnSavePdfReport.TabIndex = 0;
            this.btnSavePdfReport.Text = "Simpan PDF FORM US";
            this.btnSavePdfReport.UseVisualStyleBackColor = false;
            this.btnSavePdfReport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(118, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(478, 141);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "- Tabel Konfirmasi nilai hasil penghitungan ISP, Impuls, dkk\n- Hasil Report FORM-" +
    "UJI-STATIS\n\n- lampiran photo gambar2 penting";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // btnViewPdfRreport
            // 
            this.btnViewPdfRreport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewPdfRreport.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold);
            this.btnViewPdfRreport.Location = new System.Drawing.Point(12, 12);
            this.btnViewPdfRreport.Name = "btnViewPdfRreport";
            this.btnViewPdfRreport.Size = new System.Drawing.Size(100, 46);
            this.btnViewPdfRreport.TabIndex = 2;
            this.btnViewPdfRreport.Text = "View PDF FORM HASIL US";
            this.btnViewPdfRreport.UseVisualStyleBackColor = true;
            // 
            // btnMakeZip
            // 
            this.btnMakeZip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMakeZip.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMakeZip.Font = new System.Drawing.Font("HP Simplified", 9.749999F, System.Drawing.FontStyle.Bold);
            this.btnMakeZip.Location = new System.Drawing.Point(12, 414);
            this.btnMakeZip.Name = "btnMakeZip";
            this.btnMakeZip.Size = new System.Drawing.Size(100, 46);
            this.btnMakeZip.TabIndex = 2;
            this.btnMakeZip.Text = "BUAT ZIP FINAL REPORT";
            this.btnMakeZip.UseVisualStyleBackColor = true;
            this.btnMakeZip.Click += new System.EventHandler(this.btnMakeZip_Click);
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 472);
            this.Controls.Add(this.btnMakeZip);
            this.Controls.Add(this.btnViewPdfRreport);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnSavePdfReport);
            this.Name = "FormReport";
            this.Text = "FormPerformance";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSavePdfReport;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnViewPdfRreport;
        private System.Windows.Forms.Button btnMakeZip;
    }
}