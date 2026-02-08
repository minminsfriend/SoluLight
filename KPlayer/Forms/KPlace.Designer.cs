namespace KPlayer
{
    partial class KPlace
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
            this.textInput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textInput
            // 
            this.textInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textInput.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textInput.Location = new System.Drawing.Point(315, 472);
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(275, 41);
            this.textInput.TabIndex = 0;
            this.textInput.Text = "";
            this.textInput.TextChanged += new System.EventHandler(this.textInput_TextChanged);
            // 
            // KPlace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 526);
            this.Controls.Add(this.textInput);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KPlace";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "무엇이든 입력해 보세요";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textInput;
    }
}