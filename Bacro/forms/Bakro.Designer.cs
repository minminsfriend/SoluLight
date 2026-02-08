namespace Bacro
{
    partial class Bakro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bakro));
            this.btnNavi = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnTemp = new System.Windows.Forms.Button();
            this.btnDho = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnNavi
            // 
            this.btnNavi.Location = new System.Drawing.Point(414, 14);
            this.btnNavi.Name = "btnNavi";
            this.btnNavi.Size = new System.Drawing.Size(120, 23);
            this.btnNavi.TabIndex = 0;
            this.btnNavi.Text = "네비";
            this.btnNavi.UseVisualStyleBackColor = true;
            this.btnNavi.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(272, 14);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 23);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "편집";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnTemp
            // 
            this.btnTemp.Location = new System.Drawing.Point(19, 14);
            this.btnTemp.Name = "btnTemp";
            this.btnTemp.Size = new System.Drawing.Size(80, 23);
            this.btnTemp.TabIndex = 2;
            this.btnTemp.Text = "해제";
            this.btnTemp.UseVisualStyleBackColor = true;
            this.btnTemp.Click += new System.EventHandler(this.btnTemp_Click);
            // 
            // btnDho
            // 
            this.btnDho.Location = new System.Drawing.Point(128, 14);
            this.btnDho.Name = "btnDho";
            this.btnDho.Size = new System.Drawing.Size(120, 23);
            this.btnDho.TabIndex = 3;
            this.btnDho.Text = "대항해시대";
            this.btnDho.UseVisualStyleBackColor = true;
            this.btnDho.Click += new System.EventHandler(this.btnDho_Click);
            // 
            // Bakro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 50);
            this.Controls.Add(this.btnDho);
            this.Controls.Add(this.btnTemp);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnNavi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(723, 942);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Bakro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "배크로";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnNavi;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnTemp;
        private System.Windows.Forms.Button btnDho;
    }
}