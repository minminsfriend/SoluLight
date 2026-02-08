using System.Windows.Forms;

namespace Geminapi
{
    partial class Geminapi
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.textWords = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textWords
            // 
            this.textWords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textWords.BackColor = System.Drawing.Color.Black;
            this.textWords.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textWords.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textWords.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.textWords.Location = new System.Drawing.Point(19, 19);
            this.textWords.Margin = new System.Windows.Forms.Padding(10);
            this.textWords.Name = "textWords";
            this.textWords.Size = new System.Drawing.Size(768, 544);
            this.textWords.TabIndex = 0;
            this.textWords.Text = "◇ ";
            // 
            // Geminapi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(800, 582);
            this.Controls.Add(this.textWords);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(600, 100);
            this.MaximizeBox = false;
            this.Name = "Geminapi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "제미나이 Api";
            this.ResumeLayout(false);

        }


        #endregion

        private RichTextBox textWords;
    }
}

