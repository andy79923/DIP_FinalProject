﻿using System;
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
            byte[] intensities = new byte[filterSize * filterSize];
            int median = (filterSize * filterSize) / 2;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
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
                            intensities[j * filterSize + i] = image.GetPixel(wX, wY).R;
                        }
                    }
                    Array.Sort(intensities);
                    result.SetPixel(x, y, Color.FromArgb(intensities[median], intensities[median], intensities[median]));
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

        static public void Laplacian(ref Bitmap image, out int[,] result)
        {
            result = new int[image.Height, image.Width];
            int[,] filter = new int[,] { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } };

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
                    result[y, x] = intensity;
                }
            }
        }

        static public void LocalBinaryPattern(ref Bitmap image, out Bitmap result)// only use 3X3 filter to detect the texture
        {
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = 0;
                    int weight = 1;
                    for (int j = 0; j < 3; j++)//Use replicate to interpolate the pixel when the filter position is out of the boundary of the source image
                    {
                        int wY = y - 3 / 2 + j;
                        wY = (wY < 0) ? 0 : wY;
                        wY = (wY >= image.Height) ? image.Height - 1 : wY;
                        for (int i = 0; i < 3; i++)
                        {
                            if (j == 1 && i == 1) continue;
                            int wX = x - 3 / 2 + i;
                            wX = (wX < 0) ? 0 : wX;
                            wX = (wX >= image.Width) ? image.Width - 1 : wX;
                            intensity += ((image.GetPixel(x, y).R - image.GetPixel(wX, wY).R >= 0) ? 1 : 0) * weight;
                            weight *= 2;
                        }
                    }
                    result.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                }
            }
        }

        static public int[] OtsuMethod(ref Bitmap image, int levelNum)
        {
            int[] threshold = new int[levelNum - 1];
            int[] T = new int[levelNum];
            double globalMean = 0;
            double[] histogram = new double[256];
            double maxVariancee = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    histogram[image.GetPixel(x, y).R]++;
                }
            }
            int totalNmu = image.Width * image.Height;
            for (int i = 0; i < 256; i++)
            {
                histogram[i] /= totalNmu;
                globalMean += i * histogram[i];
            }

            T[levelNum - 1] = 255;
            int maxLevel = levelNum - 2;
            int nowLevel = 0;
            for (int i = 0; T[0] < 256 - levelNum + 1; i++)
            {
                T[nowLevel] = i;
                if (nowLevel < maxLevel)
                {

                    nowLevel++;
                    continue;
                }
                int k = 0;
                double variance = 0;
                double[] probability = new double[levelNum];
                double[] mean = new double[levelNum];
                for (int j = 0; j < 256; j++)
                {
                    if (j <= T[k])
                    {
                        probability[k] += histogram[j];
                        mean[k] += j * histogram[j];
                        continue;
                    }
                    else
                    {
                        mean[k] /= probability[k];
                        variance += probability[k] * Math.Pow(mean[k] - globalMean, 2);
                        k++;
                        j--;
                    }

                }
                mean[k] /= probability[k];
                variance += probability[k] * Math.Pow(mean[k] - globalMean, 2);
                if (variance > maxVariancee)
                {
                    maxVariancee = variance;
                    for (int j = 0; j < levelNum - 1; j++)
                    {
                        threshold[j] = T[j];
                    }
                }
                while (T[nowLevel] >= 255 && nowLevel > 0)
                {
                    nowLevel--;
                    i = T[nowLevel];
                }
            }
            return threshold;
        }

        static public void Thresholding(ref Bitmap image, out Bitmap result, int[] thresholdValue, int levelNum)//the image should be a gray level image
        {
            result = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int intensity = image.GetPixel(x, y).R;
                    if (intensity <= thresholdValue[0])
                    {
                        result.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else if (intensity > thresholdValue[levelNum - 2])
                    {
                        result.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        for (int k = 1; k < levelNum - 1; k++)
                        {
                            if (intensity <= thresholdValue[k])
                            {
                                int value = (thresholdValue[k - 1] + thresholdValue[k]) / 2;
                                result.SetPixel(x, y, Color.FromArgb(value, value, value));
                            }
                        }
                    }
                }
            }
        }

        static public void RegionGrowing(ref Bitmap image, out List<Point> region, out List<Point> contour,ref Point originalSeed, Point thresholdRegion, bool countArea)
        {
            List<Point> contourPoint = new List<Point>();
            List<List<Point>> contours = new List<List<Point>>();
            int threshold = image.GetPixel(originalSeed.X, originalSeed.Y).R;
            bool[,] check = new bool[image.Height, image.Width];
            check[originalSeed.Y, originalSeed.X] = true;

            Queue<Point> seeds = new Queue<Point>();
            seeds.Enqueue(originalSeed);

            //Find all points of contour.
            while (seeds.Count != 0)
            {
                Point seed = seeds.Dequeue();
                int seekCount = 0;
                if (seed.X + 1 < image.Width)
                {
                    int intensity = image.GetPixel(seed.X + 1, seed.Y).R;
                    if (intensity > thresholdRegion.X && intensity <= thresholdRegion.Y)
                    {
                        seekCount++;
                        if (check[seed.Y, seed.X + 1] == false)
                        {
                            seeds.Enqueue(new Point(seed.X + 1, seed.Y));
                            check[seed.Y, seed.X + 1] = true;
                        }
                    }
                }

                if (seed.X - 1 >= 0)
                {
                    int intensity = image.GetPixel(seed.X - 1, seed.Y).R;
                    if (intensity > thresholdRegion.X && intensity <= thresholdRegion.Y)
                    {
                        seekCount++;
                        if (check[seed.Y, seed.X - 1] == false)
                        {
                            seeds.Enqueue(new Point(seed.X - 1, seed.Y));
                            check[seed.Y, seed.X - 1] = true;
                        }
                    }
                }

                if (seed.Y + 1 < image.Height)
                {
                    int intensity = image.GetPixel(seed.X, seed.Y + 1).R;
                    if (intensity > thresholdRegion.X && intensity <= thresholdRegion.Y)
                    {
                        seekCount++;
                        if (check[seed.Y + 1, seed.X] == false)
                        {
                            seeds.Enqueue(new Point(seed.X, seed.Y + 1));
                            check[seed.Y + 1, seed.X] = true;
                        }
                    }
                }

                if (seed.Y - 1 >= 0)
                {
                    int intensity = image.GetPixel(seed.X, seed.Y - 1).R;
                    if (intensity > thresholdRegion.X && intensity <= thresholdRegion.Y)
                    {
                        seekCount++;
                        if (check[seed.Y - 1, seed.X] == false)
                        {
                            seeds.Enqueue(new Point(seed.X, seed.Y - 1));
                            check[seed.Y - 1, seed.X] = true;
                        }
                    }
                }

                if (seekCount < 4)
                {
                    contourPoint.Add(new Point(seed.X, seed.Y));
                }
            }

            //Find the outer contour of all contours in the region. 
            //The inner contours probably attaches to the outer contour, if it is nearby the outer contour.
            int outerContour = 0;
            contours.Add(new List<Point>());
            contours[0].Add(contourPoint[0]);
            contourPoint.RemoveAt(0);
            bool[,] contourCheck = new bool[image.Height, image.Width];
            for (int i = 0; i < contours.Count; i++)
            {
                for (int j = 0; j < contours[i].Count && contourPoint.Count != 0; j++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (y == 1 && x == 1) continue;
                            int position = contourPoint.IndexOf(new Point(contours[i][j].X + x - 1, contours[i][j].Y + y - 1));
                            if (position != -1)
                            {
                                contours[i].Add(contourPoint[position]);
                                contourPoint.RemoveAt(position);
                            }
                        }
                    }
                }
                if (contourPoint.Count != 0)
                {
                    contours.Add(new List<Point>());
                    contours[contours.Count - 1].Add(contourPoint[0]);
                    contourPoint.RemoveAt(0);
                }

                if (contours.Count < 2)
                {
                    for (int j = 0; j < contours[outerContour].Count; j++)
                    {
                        contourCheck[contours[outerContour][j].Y, contours[outerContour][j].X] = true;
                    }
                }
                else
                {
                    int outerCount = 0;
                    for (int j = contours[i][0].X + 1; j < image.Width; j++)
                    {
                        if (contourCheck[contours[i][0].Y, j] == true)
                        {
                            outerCount++;
                            break;
                        }
                    }
                    for (int j = contours[i][0].X - 1; j >= 0; j--)
                    {
                        if (contourCheck[contours[i][0].Y, j] == true)
                        {
                            outerCount++;
                            break;
                        }
                    }
                    for (int j = contours[i][0].Y + 1; j < image.Height; j++)
                    {
                        if (contourCheck[j, contours[i][0].X] == true)
                        {
                            outerCount++;
                            break;
                        }
                    }
                    for (int j = contours[i][0].Y - 1; j >= 0; j--)
                    {
                        if (contourCheck[j, contours[i][0].X] == true)
                        {
                            outerCount++;
                            break;
                        }
                    }
                    if (outerCount != 4)
                    {
                        outerContour = i;
                        contourCheck = new bool[image.Height, image.Width];
                        for (int j = 0; j < contours[outerContour].Count; j++)
                        {
                            contourCheck[contours[outerContour][j].Y, contours[outerContour][j].X] = true;
                        }
                    }
                }
            }
            contour = new List<Point>(contours[outerContour]);

            if (contourCheck[originalSeed.Y, originalSeed.X] == true)
            {
                int rightCount = 0;
                for (int i = originalSeed.X + 1; i < image.Width && contourCheck[originalSeed.Y, i] != true && check[originalSeed.Y, i] == true; i++)
                {
                    rightCount++;
                }
                int leftCount = 0;
                for (int i = originalSeed.X - 1; i >= 0 && contourCheck[originalSeed.Y, i] != true && check[originalSeed.Y, i] == true; i--)
                {
                    leftCount++;
                }
                if (rightCount == 0 && leftCount != 0)
                {
                    originalSeed.X -= leftCount / 2;
                }
                else if (rightCount != 0 && leftCount == 0)
                {
                    originalSeed.X += rightCount;
                }

                int downCount = 0;
                for (int i = originalSeed.Y + 1; i < image.Height && contourCheck[i, originalSeed.X] != true && check[i, originalSeed.X] == true; i++)
                {
                    downCount++;
                }
                int upCount = 0;
                for (int i = originalSeed.Y - 1; i >=0 && contourCheck[i, originalSeed.X] != true && check[i, originalSeed.X] == true; i--)
                {
                    upCount++;
                }
                if (downCount == 0 && upCount != 0)
                {
                    originalSeed.Y -= upCount / 2;
                }
                else if (downCount != 0 && upCount == 0)
                {
                    originalSeed.Y += downCount / 2;
                }
            }

            region = new List<Point>(contour);
            if (countArea == false)
            {
                return;
            }
            check = new bool[image.Height, image.Width];
            seeds.Enqueue(new Point(originalSeed.X, originalSeed.Y));
            check[originalSeed.Y, originalSeed.X] = true;
            while (seeds.Count != 0)
            {
                Point seed = seeds.Dequeue();
                region.Add(new Point(seed.X, seed.Y));
                if (seed.X + 1 < image.Width && contourCheck[seed.Y, seed.X + 1] == false && check[seed.Y, seed.X + 1] == false)
                {
                    seeds.Enqueue(new Point(seed.X + 1, seed.Y));
                    check[seed.Y, seed.X + 1] = true;
                }

                if (seed.X - 1 >= 0 && contourCheck[seed.Y, seed.X - 1] == false && check[seed.Y, seed.X - 1] == false)
                {
                    seeds.Enqueue(new Point(seed.X - 1, seed.Y));
                    check[seed.Y, seed.X - 1] = true;
                }

                if (seed.Y + 1 < image.Height && contourCheck[seed.Y + 1, seed.X] == false && check[seed.Y + 1, seed.X] == false)
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
        }

        static public double MeanOfAbsoluteDifference(ref List<Point> contourA, ref List<Point> contourB, double maxDistance)
        {
            double minDistanceA = 0, minDistanceB = 0;
            for (int i = 0; i < contourA.Count; i++)
            {
                double minDistance = maxDistance;
                for (int j = 0; j < contourB.Count; j++)
                {
                    double distance = Math.Sqrt(Math.Pow(contourA[i].X - contourB[j].X, 2) + Math.Pow(contourA[i].Y - contourB[j].Y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
                minDistanceA += minDistance;
            }
            minDistanceA /= contourA.Count;
            for (int i = 0; i < contourB.Count; i++)
            {
                double minDistance = maxDistance;
                for (int j = 0; j < contourA.Count; j++)
                {
                    double distance = Math.Sqrt(Math.Pow(contourB[i].X - contourA[j].X, 2) + Math.Pow(contourB[i].Y - contourA[j].Y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
                minDistanceB += minDistance;
            }
            minDistanceB /= contourB.Count;
            return (minDistanceA + minDistanceB) / 2;
        }

        static public double DiceSimilarityCoefficient(ref List<Point> regionA, ref List<Point> regionB)
        {
            int intersection = 0;
            for (int i = 0; i < regionA.Count; i++)
            {
                for (int j = 0; j < regionB.Count; j++)
                {
                    if (regionA[i] == regionB[j])
                    {
                        intersection++;
                    }
                }
            }
            return 2 * intersection / (double)(regionA.Count + regionB.Count);
        }
    }
}
