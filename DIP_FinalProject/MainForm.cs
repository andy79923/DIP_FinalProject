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
        }

        private void _pictureBoxInputImage_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap image = _inputImages[_listBoxInputImage.SelectedIndex];
            Bitmap result = new Bitmap(image);
            List<Point> region;
            List<Point> contour;
            Point seedPosition = new Point(e.X, e.Y);
            Bitmap thresholdImage;
            int thresholdLevel=3;
            int[] thresholdValue;
            thresholdValue = ImageProcessing.ImageProcessing.MultilevelThresholding(ref image, out thresholdImage, thresholdLevel);
            Point regionRange=new Point();
            int seedIntensity = image.GetPixel(seedPosition.X, seedPosition.Y).R;
            if (seedIntensity <= thresholdValue[0])
            {
                regionRange.X = -1;
                regionRange.Y = thresholdValue[0];
            }
            else if (seedIntensity > thresholdValue[thresholdLevel - 2])
            {
                regionRange.X = thresholdValue[thresholdLevel - 2];
                regionRange.Y = 255;
            }
            else
            {
                for (int i = 0; i < thresholdLevel - 2; i++)
                {
                    if (seedIntensity > thresholdValue[i] && seedIntensity <= thresholdValue[i + 1])
                    {
                        regionRange.X = thresholdValue[i];
                        regionRange.Y = thresholdValue[i + 1];
                        break;
                    }
                }
            }
            ImageProcessing.ImageProcessing.RegionGrowing(ref thresholdImage, out region, out contour, seedPosition, regionRange);
            for (int i = 0; i < contour.Count; i++)
            {
                result.SetPixel(contour[i].X, contour[i].Y, Color.FromArgb(255, 0, 0));
            }
            _pictureBoxResult.Image = result;
        }
    }
}
