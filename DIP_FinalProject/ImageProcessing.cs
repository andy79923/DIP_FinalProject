using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageProcessing
{
    delegate void SmoothingMethod(ref Bitmap image, out Bitmap result, int filterSize);
    class ImageProcessing
    {
        static public void RGBExtraction(ref Bitmap image, out List<Bitmap> results)
        {
            results = new List<Bitmap>();
            for (int i = 0; i < 3; i++)
            {
                results.Add(new Bitmap(image.Width, image.Height));
            }

            Bitmap RImage = results[0];
            Bitmap GImage = results[1];
            Bitmap BImage = results[2];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    RImage.SetPixel(x, y, Color.FromArgb(RGB.R, RGB.R, RGB.R));
                    GImage.SetPixel(x, y, Color.FromArgb(RGB.G, RGB.G, RGB.G));
                    BImage.SetPixel(x, y, Color.FromArgb(RGB.B, RGB.B, RGB.B));
                }
            }
        }

        static public void TranslateGrayLevel(ref Bitmap image, out Bitmap result)
        {
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    int grayLevel = (RGB.R + RGB.G + RGB.B) / 3;
                    result.SetPixel(x, y, Color.FromArgb(grayLevel, grayLevel, grayLevel));
                }
            }
        }

        static public void Thresholding(ref Bitmap image, out Bitmap result, int thresholdValue)//the image should be a gray level image
        {
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    byte intensity = image.GetPixel(x, y).R;
                    result.SetPixel(x, y, (intensity > thresholdValue) ? Color.FromArgb(255, 255, 255) : Color.FromArgb(0, 0, 0));
                }
            }
        }

        static public void MeanSmoothing(ref Bitmap image, out Bitmap result, int filterSize)//the image should be a gray level image
        {
            if (filterSize % 2 != 1)
            {
                result = new Bitmap(image);
                return;
            }
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = 0;
                    for (int j = 0; j < filterSize; j++)//Use replicate to interpolate the pixel when the filter position is out of the boundary of the source image
                    {
                        int wY = y - filterSize / 2 + j;
                        wY = (wY < 0) ? 0 : wY;
                        wY = (wY >= image.Height) ? image.Height - 1 : wY;
                        for (int i = 0; i < filterSize; i++)
                        {
                            int wX = x - filterSize / 2 + i;
                            wX = (wX < 0) ? 0 : wX;
                            wX = (wX >= image.Width) ? image.Width - 1 : wX;
                            intensity += image.GetPixel(wX, wY).R;
                        }
                    }
                    intensity /= (filterSize * filterSize);
                    result.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public void MedianSmoothing(ref Bitmap image, out Bitmap result, int filterSize)//the image should be a gray level image
        {
            if (filterSize % 2 != 1)
            {
                result = new Bitmap(image);
                return;
            }
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    List<int> intensity = new List<int>();
                    for (int j = 0; j < filterSize; j++)//Use replicate to interpolate the pixel when the filter position is out of the boundary of the source image
                    {
                        int wY = y - filterSize / 2 + j;
                        wY = (wY < 0) ? 0 : wY;
                        wY = (wY >= image.Height) ? image.Height - 1 : wY;
                        for (int i = 0; i < filterSize; i++)
                        {
                            int wX = x - filterSize / 2 + i;
                            wX = (wX < 0) ? 0 : wX;
                            wX = (wX >= image.Width) ? image.Width - 1 : wX;
                            intensity.Add((image.GetPixel(wX, wY).R));
                        }
                    }
                    intensity.Sort();
                    int median = intensity.Count / 2;
                    result.SetPixel(x, y, Color.FromArgb(intensity[median], intensity[median], intensity[median]));
                }
            }
        }

        static public void HistogramEqualization(ref Bitmap image, out Bitmap result)//the image should be a gray level image
        {
            result = new Bitmap(image.Width, image.Height);
            double[] histogram = new double[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = image.GetPixel(x, y).R;
                    histogram[intensity] += 1;
                }
            }
            for (int i = 1; i < 256; i++)
            {
                histogram[i] += histogram[i - 1];
            }
            double ratio = 255 / histogram[255];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = Convert.ToInt32(Math.Round(histogram[image.GetPixel(x, y).R] * ratio));
                    result.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public void Sobel(ref Bitmap image, out Bitmap result, bool x_order)//the image should be a gray level image
        {
            result = new Bitmap(image.Width, image.Height);
            int[,] filter = (x_order == true) ? new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } } : new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = 0;
                    for (int j = 0; j < 3; j++)//Use replicate to interpolate the pixel when the filter position is out of the boundary of the source image
                    {
                        int wY = y - 3 / 2 + j;
                        wY = (wY < 0) ? 0 : wY;
                        wY = (wY >= image.Height) ? image.Height - 1 : wY;
                        for (int i = 0; i < 3; i++)
                        {
                            int wX = x - 3 / 2 + i;
                            wX = (wX < 0) ? 0 : wX;
                            wX = (wX >= image.Width) ? image.Width - 1 : wX;
                            intensity += (image.GetPixel(wX, wY).R * filter[i, j]);
                        }
                    }
                    intensity = Math.Abs(intensity);
                    intensity = (intensity > 255) ? 255 : intensity;
                    result.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public void Stretching(ref Bitmap image, out Bitmap result, double xScale, double yScale)//the image should be a gray level image
        {
            result = new Bitmap((int)((double)image.Width * xScale), (int)((double)image.Height * yScale));
            double[,] invertibleTransformation = new double[2, 2] { { yScale / (xScale * yScale), 0 }, { 0, xScale / (xScale * yScale) } };

            for (int rY = 0; rY < result.Height; rY++)
            {
                for (int rX = 0; rX < result.Width; rX++)
                {
                    double x = invertibleTransformation[0, 0] * rX + invertibleTransformation[0, 1] * rY;
                    double y = invertibleTransformation[1, 0] * rX + invertibleTransformation[1, 1] * rY;
                    int leftX = (int)x;
                    int rightX = (leftX + 1 >= image.Width) ? leftX : leftX + 1;
                    int topY = (int)y;
                    int bottomY = (topY + 1 >= image.Height) ? topY : topY + 1;
                    double alpha = x - (double)leftX;
                    double beta = y - (double)topY;
                    int intensity = (int)(Math.Round((1 - alpha) * (1 - beta) * (double)image.GetPixel(leftX, topY).R + alpha * (1 - beta) * (double)image.GetPixel(rightX, topY).R
                                    + (1 - alpha) * beta * (double)image.GetPixel(leftX, bottomY).R + alpha * beta * (double)image.GetPixel(rightX, bottomY).R));
                    result.SetPixel(rX, rY, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public void Rotation(ref Bitmap image, out Bitmap result, double angle, PointF center)//the image should be a gray level image
        {
            double[,] transform = new double[2, 2] { { Math.Cos(angle), -Math.Sin(angle) }, { Math.Sin(angle), Math.Cos(angle) } };
            PointF p1 = new PointF(0, 0);
            PointF p2 = new PointF(0, image.Height);
            PointF p3 = new PointF(image.Width, 0);
            PointF p4 = new PointF(image.Width, image.Height);
            PointF np1 = new PointF((float)Math.Round(transform[0, 0] * p1.X + transform[0, 1] * p1.Y), (float)Math.Round(transform[1, 0] * p1.X + transform[1, 1] * p1.Y));
            PointF np2 = new PointF((float)Math.Round(transform[0, 0] * p2.X + transform[0, 1] * p2.Y), (float)Math.Round(transform[1, 0] * p2.X + transform[1, 1] * p2.Y));
            PointF np3 = new PointF((float)Math.Round(transform[0, 0] * p3.X + transform[0, 1] * p3.Y), (float)Math.Round(transform[1, 0] * p3.X + transform[1, 1] * p3.Y));
            PointF np4 = new PointF((float)Math.Round(transform[0, 0] * p4.X + transform[0, 1] * p4.Y), (float)Math.Round(transform[1, 0] * p4.X + transform[1, 1] * p4.Y));

            double minX = np1.X, maxX = np1.X, minY = np1.Y, maxY = np1.Y;
            minX = (np2.X < minX) ? np2.X : minX;
            minX = (np3.X < minX) ? np3.X : minX;
            minX = (np4.X < minX) ? np4.X : minX;
            maxX = (np2.X > maxX) ? np2.X : maxX;
            maxX = (np3.X > maxX) ? np3.X : maxX;
            maxX = (np4.X > maxX) ? np4.X : maxX;
            minY = (np2.Y < minY) ? np2.Y : minY;
            minY = (np3.Y < minY) ? np3.Y : minY;
            minY = (np4.Y < minY) ? np4.Y : minY;
            maxY = (np2.Y > maxY) ? np2.Y : maxY;
            maxY = (np3.Y > maxY) ? np3.Y : maxY;
            maxY = (np4.Y > maxY) ? np4.Y : maxY;


            result = new Bitmap((int)(maxX - minX), (int)(maxY - minY));
            PointF resultTranslate = new PointF((float)(result.Width - 1) / 2, (float)(result.Height - 1) / 2);
            double transformDivision = transform[0, 0] * transform[1, 1] - transform[0, 1] * transform[1, 0];
            double[,] invertableTransform = new double[2, 2] { { transform[1, 1] / transformDivision, -transform[0, 1] / transformDivision }, { -transform[1, 0] / transformDivision, transform[0, 0] / transformDivision } };

            for (int rY = 0; rY < result.Height; rY++)
            {
                for (int rX = 0; rX < result.Width; rX++)
                {
                    double x = invertableTransform[0, 0] * ((double)rX - resultTranslate.X) + invertableTransform[0, 1] * ((double)rY - resultTranslate.Y) + center.X;
                    double y = invertableTransform[1, 0] * ((double)rX - resultTranslate.X) + invertableTransform[1, 1] * ((double)rY - resultTranslate.Y) + center.Y;

                    int leftX = (int)x;
                    int rightX = (leftX + 1 >= image.Width) ? leftX : leftX + 1;
                    int topY = (int)y;
                    int bottomY = (topY + 1 >= image.Height) ? topY : topY + 1;
                    double alpha = x - (double)leftX;
                    double beta = y - (double)topY;
                    leftX = (leftX == -1) ? leftX + 1 : leftX;
                    rightX = (rightX == image.Width) ? rightX - 1 : rightX;
                    topY = (topY == -1) ? topY + 1 : topY;
                    bottomY = (bottomY == image.Height) ? bottomY - 1 : bottomY;
                    double leftTopPixel = (leftX < 0 || leftX >= image.Width || topY < 0 || topY >= image.Height) ? 0 : (double)image.GetPixel(leftX, topY).R;
                    double rightTopPixel = (rightX < 0 || rightX >= image.Width || topY < 0 || topY >= image.Height) ? 0 : (double)image.GetPixel(rightX, topY).R;
                    double leftBottomPixel = (leftX < 0 || leftX >= image.Width || bottomY < 0 || bottomY >= image.Height) ? 0 : (double)image.GetPixel(leftX, bottomY).R;
                    double rightBottomPixel = (rightX < 0 || rightX >= image.Width || bottomY < 0 || bottomY >= image.Height) ? 0 : (double)image.GetPixel(rightX, bottomY).R;

                    int intensity = (int)(Math.Round((1 - alpha) * (1 - beta) * leftTopPixel + alpha * (1 - beta) * rightTopPixel + (1 - alpha) * beta * leftBottomPixel + alpha * beta * rightBottomPixel));
                    intensity = (intensity < 0) ? 0 : intensity;
                    intensity = (intensity > 255) ? 255 : intensity;
                    result.SetPixel(rX, rY, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public void UnsharpMasking(ref Bitmap image, out Bitmap result, SmoothingMethod smoothing, int smoothingSize, double weight)
        {
            Bitmap smoothingResult;
            result = new Bitmap(image.Width, image.Height);
            smoothing(ref image, out smoothingResult, smoothingSize);
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    double intensity = ((1.0 + weight) * (double)image.GetPixel(x, y).R - weight * (double)smoothingResult.GetPixel(x, y).R);
                    intensity = (intensity < 0) ? 0 : intensity;
                    intensity = (intensity > 255) ? 255 : intensity;
                    result.SetPixel(x, y, Color.FromArgb((int)intensity, (int)intensity, (int)intensity));
                }
            }
        }

    }
}
