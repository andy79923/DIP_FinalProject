using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
namespace DIP_FinalProject
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this._buttonLoadImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonLoadImage
            // 
            this._buttonLoadImage.Location = new System.Drawing.Point(29, 205);
            this._buttonLoadImage.Name = "_buttonLoadImage";
            this._buttonLoadImage.Size = new System.Drawing.Size(75, 23);
            this._buttonLoadImage.TabIndex = 0;
            this._buttonLoadImage.Text = "Load Image";
            this._buttonLoadImage.UseVisualStyleBackColor = true;
            this._buttonLoadImage.Click += new System.EventHandler(this._buttonLoadImage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this._buttonLoadImage);
            this.Name = "MainForm";
            this.Text = "Trapezium Segmentation";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenFileDialog _openFile;
        private List<Bitmap> _inputImages;
        private System.Windows.Forms.Button _buttonLoadImage;
    }
}

