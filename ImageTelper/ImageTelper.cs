using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;

namespace ImageTelper {
    public class ImageTelper {
        /// <summary>
        /// 이미지 해상도 지정
        /// </summary>
        public enum ImageResolutionOption {
            HighQuality,
            High,
            Nomal,
            Row,
            PreferCapacity
        }
        public enum AutoBackground {
            AutoFill,
            TransparencyFill,
            WhiteFill,
            BlackFill,
            None
        }
        /// <summary>
        /// 이미지 가져오기
        /// </summary>
        /// <param name="path">경로</param>
        /// <returns></returns>
        public static Image GetImage(string path) {
            if (path.Contains("http"))
                return getImageFromURL(path);
            else
                return new Bitmap(path);
        }
        /// <summary>
        /// 이미지 사이즈 변경
        /// </summary>
        /// <param name="newWidth">가로</param>
        /// <param name="newHeight">세로</param>
        /// <param name="image">이미지</param>
        /// <returns></returns>
        public static Image ResizeImage(int newWidth, int newHeight, Image image, ImageResolutionOption option = ImageResolutionOption.Nomal, AutoBackground backgroundOption = AutoBackground.None) {
            var ratioX = (double)newWidth / image.Width;
            var ratioY = (double)newHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var maxWidth = (int)(image.Width * ratio);
            var maxHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newWidth);
            using (var graphics = Graphics.FromImage(newImage))
            {
                switch (option)
                {
                    case ImageResolutionOption.HighQuality:
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        break;
                    case ImageResolutionOption.High:
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        break;
                    case ImageResolutionOption.Nomal:
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        break;
                    case ImageResolutionOption.Row:
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        break;
                    case ImageResolutionOption.PreferCapacity:
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        break;

                }
                switch (backgroundOption)
                {
                    case AutoBackground.None:
                        break;
                    case AutoBackground.BlackFill:
                        graphics.Clear(ColorTranslator.FromHtml("#000000"));
                        break;
                    case AutoBackground.WhiteFill:
                        graphics.Clear(ColorTranslator.FromHtml("#FFFFFF"));
                        break;
                    case AutoBackground.TransparencyFill:
                        graphics.Clear(Color.Transparent);
                        break;
                    case AutoBackground.AutoFill:
                        string backgroundColor = PixelColorFrequency(image).Keys.First();
                        graphics.Clear(ColorTranslator.FromHtml(backgroundColor.Equals('#') ? backgroundColor : "#" + backgroundColor));
                        break;
                }
                // Calculate x and y which center the image
                int y = (newHeight / 2) - maxHeight / 2;
                int x = (newWidth / 2) - maxWidth / 2;

                // Draw image on x and y with newWidth and newHeight
                graphics.DrawImage(image, x, y, maxWidth, maxHeight);
            }

            return newImage;
        }
        /// <summary>
        /// 이미지 사이즈 변경
        /// </summary>
        /// <param name="newWidth">가로</param>
        /// <param name="newHeight">세로</param>
        /// <param name="path">이미지 경로</param>
        /// <returns></returns>
        public static Image ResizeImage(int newWidth, int newHeight, string path, ImageResolutionOption option = ImageResolutionOption.Nomal, AutoBackground backgroundOption = AutoBackground.None) {
            if (path.Contains("http"))
            {
                return ResizeImage(newWidth, newHeight, getImageFromURL(path), option, backgroundOption);
            } else
            {
                return ResizeImage(newWidth, newHeight, new Bitmap(path), option, backgroundOption);
            }
        }

        private static Dictionary<string, int> PixelColorFrequency(Image image) {
            Bitmap bitmap = new Bitmap(image);
            Dictionary<string, int> colorFrequency = new Dictionary<string, int>();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    string hex = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + (color.A.ToString("X2").Equals("FF") ? string.Empty : color.A.ToString("X2"));
                    if (colorFrequency.Keys.Contains(hex))
                        colorFrequency[hex]++;
                    else
                        colorFrequency.Add(hex, 1);
                }
            }

            colorFrequency.ToList().Sort((x1, x2) => x1.Value.CompareTo(x2.Value));

            return colorFrequency;
        }
        public static Stream DownloadData(string Url) {
            try
            {
                WebRequest req = WebRequest.Create(Url);
                WebResponse response = req.GetResponse();
                return response.GetResponseStream();
            } catch (NotSupportedException)
            {
                return null;
            }
        }

        public static Image getImageFromURL(string url) {
            try
            {
                Image imgPhoto = Image.FromStream(DownloadData(url));
                return imgPhoto;
            } catch (NotSupportedException)
            {
                return null;
            }
        }

        public static Image GrayScale(Image ImageValue) {
            Bitmap bm = (Bitmap)ImageValue.Clone();
            System.Drawing.Imaging.BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bm.Width * 3;

                byte red, green, blue;

                for (int y = 0; y < bm.Height; ++y)
                {
                    for (int x = 0; x < bm.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green
                            + .114 * blue);

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bm.UnlockBits(bmData);

            return bm as Image;
        }

        public static Image BinaryScale(Image ImageValue) {
            Bitmap bm = (Bitmap)ImageValue.Clone();
            System.Drawing.Imaging.BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int stride = bmData.Stride; //the length of the line
            System.IntPtr Scan0 = bmData.Scan0;
            int Threshold = 220;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bm.Width * 3;

                byte red, green, blue;
                byte binary;

                for (int y = 0; y < bm.Height; ++y)
                {
                    for (int x = 0; x < bm.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        binary = (byte)(.299 * red
                            + .587 * green
                            + .114 * blue);

                        if (binary < Threshold)
                            p[0] = p[1] = p[2] = 0;
                        else
                            if (binary >= Threshold)
                            p[0] = p[1] = p[2] = 255;
                        else
                            if (binary < Threshold)
                            p[0] = p[1] = p[2] = 255;
                        else
                            p[0] = p[1] = p[2] = 0;
                        p += 3;
                    }
                    p += nOffset;
                }

            }

            bm.UnlockBits(bmData);

            return bm as Image;
        }

        public static Image NoiseRemove(Image ImageValue) {
            Bitmap b1 = (Bitmap)ImageValue.Clone();
            Bitmap b2 = (Bitmap)ImageValue.Clone();
            
            byte val;
            
            System.Drawing.Imaging.BitmapData bmData = b1.LockBits(new Rectangle(0, 0, ImageValue.Width, ImageValue.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData bmData2 = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr Scan02 = bmData2.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* p2 = (byte*)(void*)Scan02;

                int nOffset = stride - ImageValue.Width * 3;
                int nWidth = ImageValue.Width * 3;

                p += stride;
                p2 += stride;
                //int val;
                for (int y = 1; y < ImageValue.Height - 1; ++y)
                {
                    p += 3;
                    p2 += 3;

                    for (int x = 3; x < nWidth - 3; ++x)
                    {
                        val = p2[0];
                        if (val == 0)
                            if ((p2 + 3)[0] == 0 || (p2 - 3)[0] == 0 || (p2 + stride)[0] == 0 || (p2 - stride)[0] == 0 || (p2 + stride + 3)[0] == val || (p2 + stride - 3)[0] == 0 || (p2 - stride - 3)[0] == 0 || (p2 + stride + 3)[0] == 0)
                                p[0] = val;
                            else
                                p[0] = 255;

                        ++p;
                        ++p2;
                    }

                    p += nOffset + 3;
                    p2 += nOffset + 3;
                }
            }

            b1.UnlockBits(bmData);
            b2.UnlockBits(bmData2);

            return b1 as Image;
        }
    }
}
