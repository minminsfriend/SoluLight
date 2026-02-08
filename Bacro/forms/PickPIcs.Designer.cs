namespace Bacro
{
    partial class PickPIcs
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
            this.picsView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // picsView
            // 
            this.picsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picsView.HideSelection = false;
            this.picsView.Location = new System.Drawing.Point(0, 0);
            this.picsView.Name = "picsView";
            this.picsView.Size = new System.Drawing.Size(532, 450);
            this.picsView.TabIndex = 0;
            this.picsView.UseCompatibleStateImageBehavior = false;
            // 
            // PickPIcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 450);
            this.Controls.Add(this.picsView);
            this.Location = new System.Drawing.Point(900, 100);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PickPIcs";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "픽픽스";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView picsView;
    }
}