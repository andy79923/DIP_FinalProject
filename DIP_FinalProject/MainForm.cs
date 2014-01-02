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
            _saveFile = new SaveFileDialog();
            _saveFile.InitialDirectory = "C";
            _saveFile.Filter = "Bitmap Files (.bmp)|*.bmp";
            _thresholdingLevel = 3;
            _seedPosition = new Point();
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
            _groupBoxMode.Enabled = false;

            _thresholdingRange = ImageProcessing.ImageProcessing.OtsuMethod(ref inputImage, _thresholdingLevel);
        }

        private void _pictureBoxInputImage_MouseClick(object sender, MouseEventArgs e)
        {
            
            if (_radioButtonSegmentationMode.Checked == false || _pictureBoxInputImage.Image==null)
            {
                return;
            }
            _groupBoxMode.Enabled = true;
            Bitmap image = _inputImages[_listBoxInputImage.SelectedIndex];
            _result = new Bitmap(image);
            _seedPosition.X = e.X;
            _seedPosition.Y = e.Y;
            Point regionRange=new Point();
            int seedIntensity = image.GetPixel(_seedPosition.X, _seedPosition.Y).R;
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
            ImageProcessing.ImageProcessing.RegionGrowing(ref image, out _region, out _contour, ref _seedPosition, regionRange, false);

            int top = image.Height - 1, bottom = 0, left = image.Width - 1, right = 0;
            for (int i = 0; i < _contour.Count; i++)
            {
                if (_contour[i].X < left)
                {
                    left = _contour[i].X;
                }
                if (_contour[i].X > right)
                {
                    right = _contour[i].X;
                }
                if (_contour[i].Y < top)
                {
                    top = _contour[i].Y;
                }
                if (_contour[i].Y > bottom)
                {
                    bottom = _contour[i].Y;
                }
            }
            top = (top - 3 >= 0) ? top - 3 : top;
            bottom = (bottom + 3 < image.Height) ? bottom + 3 : bottom;
            right = (right + 3 < image.Width) ? right + 3 : right;
            left = (left - 3 >= 0) ? left - 3 : left;
            Bitmap ROI = new Bitmap(right - left + 1, bottom - top + 1);
            Bitmap smoothingROI;
            for (int y = 0; y < ROI.Height; y++)
            {
                for (int x = 0; x < ROI.Width; x++)
                {
                    int intensity = image.GetPixel(x + left, y + top).R;
                    ROI.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                }
            }
            ImageProcessing.ImageProcessing.Thresholding(ref ROI, out smoothingROI, _thresholdingRange, _thresholdingLevel);
            ROI = new Bitmap(smoothingROI);

            int smoothingTimes = (int)(((double)image.Width / (double)ROI.Width + (double)image.Height / (double)ROI.Height + 1) / 2);
            for (int i = 0; i < smoothingTimes; i++)
            {
                ImageProcessing.ImageProcessing.MedianSmoothing(ref ROI, out smoothingROI, 3);
                ROI = new Bitmap(smoothingROI);
            }
            List<Point> roiRegion, roiContour;
            _contour = new List<Point>();
            _region = new List<Point>();
            Point shiftPoint = new Point(_seedPosition.X - left, _seedPosition.Y - top);
            ImageProcessing.ImageProcessing.RegionGrowing(ref ROI, out roiRegion, out roiContour, ref shiftPoint, regionRange, true);

            bool[,] roiRegionCheck = new bool[ROI.Height, ROI.Width];
            List<Point> ehanceContour = new List<Point>();
            for (int i = 0; i < roiRegion.Count; i++)
            {
                roiRegionCheck[roiRegion[i].Y, roiRegion[i].X] = true;
            }

            for (int i = 0; i < roiContour.Count; i++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        int wX = roiContour[i].X + x - 1;
                        int wY = roiContour[i].Y + y - 1;
                        if (x == 1 && y == 1 || (wX < 0 || wX >= ROI.Width || wY < 0 || wY >= ROI.Height) || roiRegionCheck[wY, wX] == true) continue;
                        roiRegionCheck[wY, wX] = true;
                        ehanceContour.Add(new Point(wX, wY));
                        roiRegion.Add(new Point(wX, wY));
                    }
                }
            }
            for (int i = 0; i < ehanceContour.Count; i++)
            {
                roiContour.Add(new Point(ehanceContour[i].X, ehanceContour[i].Y));
            }
            for (int i = 0; i < roiContour.Count; i++)
            {
                _contour.Add(new Point(roiContour[i].X + left, roiContour[i].Y + top));
                _result.SetPixel(_contour[i].X, _contour[i].Y, Color.FromArgb(255, 0, 0));
            }
            for (int i = 0; i < roiRegion.Count; i++)
            {
                _region.Add(new Point(roiRegion[i].X + left, roiRegion[i].Y + top));
            }
            _pictureBoxResult.Image = _result;
        }

        private void _radioButtonMode_CheckedChanged(object sender, EventArgs e)
        {
            if (_groupBoxMode.Enabled == false)
            {
                return;
            }
            if (_radioButtonSegmentationMode.Checked == true)
            {
                _listBoxInputImage.Enabled = true;
                _buttonLoadImage.Enabled = true;
                _buttonGroundTruth.Enabled = false;
                _labelMAD.Text = "0";
                _labelDSC.Text = "0";
                _labelGroundTruth.Visible = false;
                _labelInput.Visible = true;
                _pictureBoxInputImage.Image = _inputImages[_listBoxInputImage.SelectedIndex];
            }
            else if (_radioButtonMeasurementMode.Checked == true)
            {
                _pictureBoxInputImage.Image = null;
                _listBoxInputImage.Enabled = false;
                _buttonLoadImage.Enabled = false;
                _buttonGroundTruth.Enabled = true;
                _labelInput.Visible = false;
                _labelGroundTruth.Visible = true;
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
                bool[,] contourCheck = new bool[_groundTruthImage.Height, _groundTruthImage.Width];
                for (int y = 0; y < _groundTruthImage.Height; y++)
                {
                    for (int x = 0; x < _groundTruthImage.Width; x++)
                    {
                        Color RGB = _groundTruthImage.GetPixel(x, y);
                        if (RGB.R != RGB.G || RGB.R != RGB.B || RGB.G != RGB.B)
                        {
                            contour.Add(new Point(x, y));
                            contourCheck[y, x] = true;
                        }
                    }
                }

                List<Point> region = new List<Point>(contour);
                bool[,] check = new bool[_groundTruthImage.Height, _groundTruthImage.Width];
                Queue<Point> seeds = new Queue<Point>();
                seeds.Enqueue(new Point(_seedPosition.X, _seedPosition.Y));
                check[_seedPosition.Y, _seedPosition.X] = true;
                while (seeds.Count != 0)
                {
                    Point seed = seeds.Dequeue();
                    region.Add(new Point(seed.X, seed.Y));
                    if (seed.X + 1 < _groundTruthImage.Width && contourCheck[seed.Y, seed.X + 1] == false && check[seed.Y, seed.X + 1] == false)
                    {
                        seeds.Enqueue(new Point(seed.X + 1, seed.Y));
                        check[seed.Y, seed.X + 1] = true;
                    }

                    if (seed.X - 1 >= 0 && contourCheck[seed.Y, seed.X - 1] == false && check[seed.Y, seed.X - 1] == false)
                    {
                        seeds.Enqueue(new Point(seed.X - 1, seed.Y));
                        check[seed.Y, seed.X - 1] = true;
                    }

                    if (seed.Y + 1 < _groundTruthImage.Height && contourCheck[seed.Y + 1, seed.X] == false && check[seed.Y + 1, seed.X] == false)
                    {
                        seeds.Enqueue(new Point(seed.X, seed.Y + 1));
                        check[seed.Y + 1, seed.X] = true;
                    }

                    if (seed.Y - 1 >= 0 && contourCheck[seed.Y - 1, seed.X] == false && check[seed.Y - 1, seed.X] == false)
                    {
                        seeds.Enqueue(new Point(seed.X, seed.Y - 1));
                        check[seed.Y - 1, seed.X] = true;
                    }
                }

                if (contour.Count == 0 || region.Count == 0)
                {
                    _labelMAD.Text = "0";
                    _labelDSC.Text = "0";
                    return;
                }

                _labelMAD.Text = ImageProcessing.ImageProcessing.MeanOfAbsoluteDifference(ref _contour, ref  contour, Math.Sqrt(Math.Pow(_groundTruthImage.Width, 2) + Math.Pow(_groundTruthImage.Height, 2))).ToString("0.00");
                _labelDSC.Text = ImageProcessing.ImageProcessing.DiceSimilarityCoefficient(ref _region, ref region).ToString("0.00");
            }

        }

        private void _buttonSaveResult_Click(object sender, EventArgs e)
        {
            if (_saveFile.ShowDialog() == DialogResult.OK)
            {
                _result.Save(_saveFile.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                _saveFile.InitialDirectory = _saveFile.FileName.Substring(0, _saveFile.FileName.Length - 4);
                _saveFile.FileName = "";
            }
        }
    }
}
