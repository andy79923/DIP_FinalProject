﻿using System.Windows.Forms;
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
            this._listBoxInputImage = new System.Windows.Forms.ListBox();
            this._pictureBoxInputImage = new System.Windows.Forms.PictureBox();
            this._pictureBoxResult = new System.Windows.Forms.PictureBox();
            this._radioButtonSegmentationMode = new System.Windows.Forms.RadioButton();
            this._radioButtonMeasurementMode = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxInputImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxResult)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonLoadImage
            // 
            this._buttonLoadImage.Location = new System.Drawing.Point(29, 227);
            this._buttonLoadImage.Name = "_buttonLoadImage";
            this._buttonLoadImage.Size = new System.Drawing.Size(75, 23);
            this._buttonLoadImage.TabIndex = 0;
            this._buttonLoadImage.Text = "Load Image";
            this._buttonLoadImage.UseVisualStyleBackColor = true;
            this._buttonLoadImage.Click += new System.EventHandler(this._buttonLoadImage_Click);
            // 
            // _listBoxInputImage
            // 
            this._listBoxInputImage.FormattingEnabled = true;
            this._listBoxInputImage.HorizontalScrollbar = true;
            this._listBoxInputImage.ItemHeight = 12;
            this._listBoxInputImage.Location = new System.Drawing.Point(10, 10);
            this._listBoxInputImage.Name = "_listBoxInputImage";
            this._listBoxInputImage.Size = new System.Drawing.Size(120, 196);
            this._listBoxInputImage.TabIndex = 1;
            this._listBoxInputImage.SelectedIndexChanged += new System.EventHandler(this._listBoxInputImage_SelectedIndexChanged);
            // 
            // _pictureBoxInputImage
            // 
            this._pictureBoxInputImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pictureBoxInputImage.Cursor = System.Windows.Forms.Cursors.Cross;
            this._pictureBoxInputImage.Location = new System.Drawing.Point(146, 10);
            this._pictureBoxInputImage.Name = "_pictureBoxInputImage";
            this._pictureBoxInputImage.Size = new System.Drawing.Size(512, 512);
            this._pictureBoxInputImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._pictureBoxInputImage.TabIndex = 2;
            this._pictureBoxInputImage.TabStop = false;
            this._pictureBoxInputImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this._pictureBoxInputImage_MouseClick);
            // 
            // _pictureBoxResult
            // 
            this._pictureBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pictureBoxResult.Location = new System.Drawing.Point(679, 10);
            this._pictureBoxResult.Name = "_pictureBoxResult";
            this._pictureBoxResult.Size = new System.Drawing.Size(512, 512);
            this._pictureBoxResult.TabIndex = 3;
            this._pictureBoxResult.TabStop = false;
            // 
            // _radioButtonSegmentationMode
            // 
            this._radioButtonSegmentationMode.AutoSize = true;
            this._radioButtonSegmentationMode.Checked = true;
            this._radioButtonSegmentationMode.Location = new System.Drawing.Point(6, 21);
            this._radioButtonSegmentationMode.Name = "_radioButtonSegmentationMode";
            this._radioButtonSegmentationMode.Size = new System.Drawing.Size(86, 16);
            this._radioButtonSegmentationMode.TabIndex = 4;
            this._radioButtonSegmentationMode.TabStop = true;
            this._radioButtonSegmentationMode.Text = "Segmentation";
            this._radioButtonSegmentationMode.UseVisualStyleBackColor = true;
            this._radioButtonSegmentationMode.CheckedChanged += new System.EventHandler(this._radioButtonMode_CheckedChanged);
            // 
            // _radioButtonMeasurementMode
            // 
            this._radioButtonMeasurementMode.AutoSize = true;
            this._radioButtonMeasurementMode.Location = new System.Drawing.Point(6, 43);
            this._radioButtonMeasurementMode.Name = "_radioButtonMeasurementMode";
            this._radioButtonMeasurementMode.Size = new System.Drawing.Size(85, 16);
            this._radioButtonMeasurementMode.TabIndex = 5;
            this._radioButtonMeasurementMode.Text = "Measurement";
            this._radioButtonMeasurementMode.UseVisualStyleBackColor = true;
            this._radioButtonMeasurementMode.CheckedChanged += new System.EventHandler(this._radioButtonMode_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._radioButtonSegmentationMode);
            this.groupBox1.Controls.Add(this._radioButtonMeasurementMode);
            this.groupBox1.Location = new System.Drawing.Point(29, 274);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(101, 72);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mode";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1286, 543);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._pictureBoxResult);
            this.Controls.Add(this._pictureBoxInputImage);
            this.Controls.Add(this._listBoxInputImage);
            this.Controls.Add(this._buttonLoadImage);
            this.Name = "MainForm";
            this.Text = "Trapezium Segmentation";
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxInputImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxResult)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenFileDialog _openFile;
        private List<Bitmap> _inputImages;
        List<Point> _region;
        List<Point> _contour;
        Bitmap _result;
        Bitmap _thresholdingImage;
        int[] _thresholdingRange;
        int _thresholdingLevel;
        private System.Windows.Forms.Button _buttonLoadImage;
        private ListBox _listBoxInputImage;
        private PictureBox _pictureBoxInputImage;
        private PictureBox _pictureBoxResult;
        private RadioButton _radioButtonSegmentationMode;
        private RadioButton _radioButtonMeasurementMode;
        private GroupBox groupBox1;
    }
}

