using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DIP_FinalProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            _inputImages = new List<Bitmap>();
            _openFile = new OpenFileDialog();
            _openFile.InitialDirectory = "C:";
            _openFile.Filter = "Bitmap Files (.bmp)|*.bmp|JPEG (.jpg)|*.jpg|PNG (.png)|*.png|All Files|*.*";
            _thresholdingLevel = 3;
        }

        private void _buttonLoadImage_Click(object sender, EventArgs e)
        {
            if (_openFile.ShowDialog() == DialogResult.OK)
            {
                int index = _listBoxInputImage.FindString(_openFile.FileName);
                if (index == ListBox.NoMatches)
                {
                    _listBoxInputImage.Items.Add(_openFile.FileName);
                    _inputImages.Add(new Bitmap(_openFile.FileName));
                    index = _listBoxInputImage.Items.Count - 1;
                }
                _listBoxInputImage.SetSelected(index, true);
                _openFile.FileName = "";
                _openFile.InitialDirectory = _openFile.FileName.Substring(0, _openFile.FileName.Length - _openFile.SafeFileName.Length);
            }
        }

        private void _listBoxInputImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap inputImage = _inputImages[_listBoxInputImage.SelectedIndex];
            _pictureBoxInputImage.Image = inputImage;
            _pictureBoxResult.Image = inputImage;

            _thresholdingRange = ImageProcessing.ImageProcessing.MultilevelThresholding(ref inputImage, out _thresholdingImage, _thresholdingLevel);
        }

        private void _pictureBoxInputImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (_radioButtonSegmentationMode.Checked == false)
            {
                return;
            }
            Bitmap image = _inputImages[_listBoxInputImage.SelectedIndex];
            _result = new Bitmap(image);
            Point seedPosition = new Point(e.X, e.Y);
            Point regionRange=new Point();
            int seedIntensity = image.GetPixel(seedPosition.X, seedPosition.Y).R;
            if (seedIntensity <= _thresholdingRange[0])
            {
                regionRange.X = -1;
                regionRange.Y = _thresholdingRange[0];
            }
            else if (seedIntensity > _thresholdingRange[_thresholdingLevel - 2])
            {
                regionRange.X = _thresholdingRange[_thresholdingLevel - 2];
                regionRange.Y = 255;
            }
            else
            {
                for (int i = 0; i < _thresholdingLevel - 2; i++)
                {
                    if (seedIntensity > _thresholdingRange[i] && seedIntensity <= _thresholdingRange[i + 1])
                    {
                        regionRange.X = _thresholdingRange[i];
                        regionRange.Y = _thresholdingRange[i + 1];
                        break;
                    }
                }
            }
            ImageProcessing.ImageProcessing.RegionGrowing(ref _thresholdingImage, out _region, out _contour, seedPosition, regionRange);
            for (int i = 0; i < _contour.Count; i++)
            {
                _result.SetPixel(_contour[i].X, _contour[i].Y, Color.FromArgb(255, 0, 0));
            }
            _pictureBoxResult.Image = _result;
        }

        private void _radioButtonMode_CheckedChanged(object sender, EventArgs e)
        {
            if (_radioButtonSegmentationMode.Enabled == true && _radioButtonSegmentationMode.Checked == true)
            {
                _listBoxInputImage.Enabled = true;
                _pictureBoxInputImage.Image = _inputImages[_listBoxInputImage.SelectedIndex];
            }
            else if (_radioButtonMeasurementMode.Enabled == true && _radioButtonMeasurementMode.Checked == true)
            {
                _pictureBoxInputImage.Image = null;
                _listBoxInputImage.Enabled = false;
            }
        }

        private void _buttonGroundTruth_Click(object sender, EventArgs e)
        {
            if (_openFile.ShowDialog() == DialogResult.OK)
            {
                
                _groundTruthImage=new Bitmap(_openFile.FileName);
                _openFile.FileName = "";
                _openFile.InitialDirectory = _openFile.FileName.Substring(0, _openFile.FileName.Length - _openFile.SafeFileName.Length);
                _pictureBoxInputImage.Image = _groundTruthImage;
                List<Point> contour = new List<Point>();
                List<Point> region = new List<Point>();
                for (int y = 0; y < _groundTruthImage.Height; y++)
                {
                    List<Point> rangePoint = new List<Point>();
                    for (int x = 0; x < _groundTruthImage.Width; x++)
                    {
                        Color RGB = _groundTruthImage.GetPixel(x, y);
                        if (RGB.R != RGB.G || RGB.R != RGB.B || RGB.G != RGB.B)
                        {
                            contour.Add(new Point(x, y));
                            rangePoint.Add(new Point(x, y));
                        }
                    }
                    if (rangePoint.Count != 0)
                    {
                        int leftX = rangePoint[0].X;
                        int rightX = rangePoint[rangePoint.Count - 1].X;
                        for (int i = leftX; i <= rightX; i++)
                        {
                            region.Add(new Point(i, y));
                        }
                    }
                }

                _labelMAD.Text = ImageProcessing.ImageProcessing.MeanOfAbsoluteDifference(ref _contour, ref  contour, Math.Sqrt(Math.Pow(_groundTruthImage.Width, 2) + Math.Pow(_groundTruthImage.Height, 2))).ToString("0.00");
                _labelDSC.Text = ImageProcessing.ImageProcessing.DiceSimilarityCoefficient(ref _region, ref region).ToString("0.00");
            }

        }
    }
}
