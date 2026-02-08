namespace KPlayer
{
    partial class MyInput
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
            this.richInputa = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richInputa
            // 
            this.richInputa.BackColor = System.Drawing.Color.MistyRose;
            this.richInputa.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richInputa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richInputa.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.richInputa.Location = new System.Drawing.Point(0, 0);
            this.richInputa.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richInputa.Name = "richInputa";
            this.richInputa.Size = new System.Drawing.Size(201, 32);
            this.richInputa.TabIndex = 0;
            this.richInputa.Text = "";
            // 
            // MyInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 32);
            this.Controls.Add(this.richInputa);
            this.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyInput";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "나으입력";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richInputa;
    }
}