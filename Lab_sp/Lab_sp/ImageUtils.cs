﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;

namespace Lab_sp
{
    class ImageUtils
    {
        private ImageUtils() { }

        /// <summary>
        /// Гауссовское размытие
        /// </summary>
        /// <param name="grayImage">Изображение для размытия</param>
        /// <param name="sigmaX">Стандартное отклонение</param>
        public static void GaussianBlur(Image<Gray, Byte> grayImage, double sigmaX = 2)
        {
            Image<Gray, Byte> blurImage = new Image<Gray, Byte>(grayImage.Width, grayImage.Height);
            CvInvoke.GaussianBlur(grayImage, blurImage, new Size(13, 13), sigmaX);
            grayImage.Bitmap = blurImage.Bitmap;
        }

        /// <summary>
        /// Пороговая обработка
        /// </summary>
        /// <param name="grayImage">Изображения для обработки</param>
        /// <param name="threshold">Порог</param>
        /// <param name="maxValue">Максимальное значение</param>
        public static void Threshold(Image<Gray, Byte> grayImage, double threshold = 127, double maxValue = 255)
        {
            Image<Gray, Byte> thrImage = new Image<Gray, Byte>(grayImage.Width, grayImage.Height);
            CvInvoke.Threshold(grayImage, thrImage, threshold, maxValue, ThresholdType.Binary);
            grayImage.Bitmap = thrImage.Bitmap;
        }

        /// <summary>
        /// Алгоритм Кенни
        /// </summary>
        /// <param name="binaryImage">Изображение для обработки</param>
        /// <param name="threshold1">Первый порог</param>
        /// <param name="threshold2">Второй порог</param>
        public static void Canny(Image<Gray, Byte> binaryImage, double threshold1 = 127, double threshold2 = 255)
        {
            Image<Gray, Byte> cannyImage = new Image<Gray, Byte>(binaryImage.Width, binaryImage.Height);
            CvInvoke.Canny(binaryImage, cannyImage, threshold1, threshold2);
            binaryImage.Bitmap = cannyImage.Bitmap;
        }

        /// <summary>
        /// Нахождение контуров изображение
        /// </summary>
        /// <param name="image">Изображения, у которого требуется найти контуры</param>
        /// <returns>Список контуров</returns>
        public static VectorOfVectorOfPoint FindContours(Mat image)
        {
            Image<Gray, Byte> grayImage = image.ToImage<Gray, Byte>();
            GaussianBlur(grayImage);
            Threshold(grayImage);
            Canny(grayImage);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(grayImage, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            return contours;
        }

        //========================================================================================
        // Отдел по экспериментальным функциями
        //========================================================================================
        /// <summary>
        /// Преобразование цветного изображения в ч/б, которая получается сложением
        /// монохромных изображений, в которых белый цвет соответствует R,G или B на цветной картинке
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image<Gray, Byte> GetMonohromImage(Image<Bgr, Byte> img)
        {
            Image<Hsv, Byte> hsvImage = img.Convert<Hsv, Byte>();
            //Определение красного
            Image<Gray, Byte> red = hsvImage.InRange(new Hsv(0, 70, 0), new Hsv(10, 255, 255))//0-255
                .Or(hsvImage.InRange(new Hsv(170, 70, 0), new Hsv(180, 255, 255)));//0-255
            Image<Gray, Byte> yellow = hsvImage.InRange(new Hsv(20, 150, 170), new Hsv(30, 255, 255));//170-255
            Image<Gray, Byte> blue = hsvImage.InRange(new Hsv(100, 120, 100), new Hsv(135, 255, 255));//100-255
            Image<Gray, Byte> white = hsvImage.InRange(new Hsv(0, 0, 230), new Hsv(180, 30, 255));//
            var resimg = yellow.Or(red).Or(blue);
            //var resimg = white;
            //var resimg = blue;
            return resimg;
        }

        /// <summary>
        /// Подсчитать отношение кол-ва белых пикселей к суммарному кол-ву пикселей изображения
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static double GetPropBGR(Image<Gray, Byte> img)
        {
            var chan = img.CountNonzero();
            return (double)chan[0] / (img.Width * img.Height);
        }
    }
}
