namespace MagneticSuspension
{
    partial class polarForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(polarForm));
            this.axTChartDpolar = new AxTeeChart.AxTChart();
            this.axTChartUpolar = new AxTeeChart.AxTChart();
            ((System.ComponentModel.ISupportInitialize)(this.axTChartDpolar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTChartUpolar)).BeginInit();
            this.SuspendLayout();
            // 
            // axTChartDpolar
            // 
            this.axTChartDpolar.Enabled = true;
            this.axTChartDpolar.Location = new System.Drawing.Point(454, 4);
            this.axTChartDpolar.Margin = new System.Windows.Forms.Padding(2);
            this.axTChartDpolar.Name = "axTChartDpolar";
            this.axTChartDpolar.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTChartDpolar.OcxState")));
            this.axTChartDpolar.Size = new System.Drawing.Size(447, 447);
            this.axTChartDpolar.TabIndex = 34;
            // 
            // axTChartUpolar
            // 
            this.axTChartUpolar.Enabled = true;
            this.axTChartUpolar.Location = new System.Drawing.Point(4, 4);
            this.axTChartUpolar.Margin = new System.Windows.Forms.Padding(2);
            this.axTChartUpolar.Name = "axTChartUpolar";
            this.axTChartUpolar.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTChartUpolar.OcxState")));
            this.axTChartUpolar.Size = new System.Drawing.Size(447, 447);
            this.axTChartUpolar.TabIndex = 33;
            // 
            // polarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 455);
            this.Controls.Add(this.axTChartDpolar);
            this.Controls.Add(this.axTChartUpolar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "polarForm";
            this.Text = "polarForm";
            ((System.ComponentModel.ISupportInitialize)(this.axTChartDpolar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTChartUpolar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public AxTeeChart.AxTChart axTChartUpolar;
        public AxTeeChart.AxTChart axTChartDpolar;
    }
}