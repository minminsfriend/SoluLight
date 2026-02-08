namespace Bacro
{
    partial class GameInput
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
            this.lbLoopTarget = new System.Windows.Forms.Label();
            this.tbLoopTarget = new System.Windows.Forms.TextBox();
            this.tbFullPower = new System.Windows.Forms.TextBox();
            this.lbFullPower = new System.Windows.Forms.Label();
            this.tbPizzaPower = new System.Windows.Forms.TextBox();
            this.lbPizzaPower = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbLoopTarget
            // 
            this.lbLoopTarget.AutoSize = true;
            this.lbLoopTarget.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbLoopTarget.Location = new System.Drawing.Point(114, 20);
            this.lbLoopTarget.Name = "lbLoopTarget";
            this.lbLoopTarget.Size = new System.Drawing.Size(63, 13);
            this.lbLoopTarget.TabIndex = 0;
            this.lbLoopTarget.Text = "생산 루프";
            // 
            // tbLoopTarget
            // 
            this.tbLoopTarget.Location = new System.Drawing.Point(117, 50);
            this.tbLoopTarget.Name = "tbLoopTarget";
            this.tbLoopTarget.Size = new System.Drawing.Size(66, 21);
            this.tbLoopTarget.TabIndex = 1;
            this.tbLoopTarget.Text = "1";
            this.tbLoopTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbFullPower
            // 
            this.tbFullPower.Location = new System.Drawing.Point(23, 50);
            this.tbFullPower.Name = "tbFullPower";
            this.tbFullPower.Size = new System.Drawing.Size(66, 21);
            this.tbFullPower.TabIndex = 3;
            this.tbFullPower.Text = "650";
            this.tbFullPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbFullPower
            // 
            this.lbFullPower.AutoSize = true;
            this.lbFullPower.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFullPower.Location = new System.Drawing.Point(20, 20);
            this.lbFullPower.Name = "lbFullPower";
            this.lbFullPower.Size = new System.Drawing.Size(59, 13);
            this.lbFullPower.TabIndex = 2;
            this.lbFullPower.Text = "총에너지";
            // 
            // tbPizzaPower
            // 
            this.tbPizzaPower.Location = new System.Drawing.Point(23, 120);
            this.tbPizzaPower.Name = "tbPizzaPower";
            this.tbPizzaPower.Size = new System.Drawing.Size(66, 21);
            this.tbPizzaPower.TabIndex = 5;
            this.tbPizzaPower.Text = "25";
            this.tbPizzaPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPizzaPower
            // 
            this.lbPizzaPower.AutoSize = true;
            this.lbPizzaPower.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbPizzaPower.Location = new System.Drawing.Point(20, 90);
            this.lbPizzaPower.Name = "lbPizzaPower";
            this.lbPizzaPower.Size = new System.Drawing.Size(72, 13);
            this.lbPizzaPower.TabIndex = 4;
            this.lbPizzaPower.Text = "한개에너지";
            // 
            // GameInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 170);
            this.Controls.Add(this.tbPizzaPower);
            this.Controls.Add(this.lbPizzaPower);
            this.Controls.Add(this.tbFullPower);
            this.Controls.Add(this.lbFullPower);
            this.Controls.Add(this.tbLoopTarget);
            this.Controls.Add(this.lbLoopTarget);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GameInput";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "게임 입력";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbLoopTarget;
        private System.Windows.Forms.TextBox tbLoopTarget;
        private System.Windows.Forms.TextBox tbFullPower;
        private System.Windows.Forms.Label lbFullPower;
        private System.Windows.Forms.TextBox tbPizzaPower;
        private System.Windows.Forms.Label lbPizzaPower;
    }
}