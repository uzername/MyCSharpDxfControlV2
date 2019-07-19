namespace WindowsFormsRotation
{
    partial class UserControlForPaint
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.realPaintingPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // realPaintingPanel
            // 
            this.realPaintingPanel.Location = new System.Drawing.Point(1, 1);
            this.realPaintingPanel.Margin = new System.Windows.Forms.Padding(0);
            this.realPaintingPanel.Name = "realPaintingPanel";
            this.realPaintingPanel.Size = new System.Drawing.Size(200, 100);
            this.realPaintingPanel.TabIndex = 0;
            this.realPaintingPanel.Click += new System.EventHandler(this.realPaintingPanel_Click);
            this.realPaintingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.RealPaintingPanel_Paint);
            // 
            // UserControlForPaint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.realPaintingPanel);
            this.Name = "UserControlForPaint";
            this.Size = new System.Drawing.Size(388, 314);
            this.Load += new System.EventHandler(this.UserControlForPaint_Load);
            this.Click += new System.EventHandler(this.UserControlForPaint_Click);
            this.Resize += new System.EventHandler(this.UserControlForPaint_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel realPaintingPanel;
    }
}
