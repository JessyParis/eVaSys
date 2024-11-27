/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création :24/10/2019
/// ----------------------------------------------------------------------------------------------------- 

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace eVaSys.Utils
{
    /// <summary>
    /// Utilities for images
    /// </summary>
    public static class ImageUtils
    {
        #region Image resizing utilities
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to. 0 if ratio resizing according to new height</param>
        /// <param name="height">The height to resize to. 0 if ratio resizing according to new width</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            //Calculate destination height and width
            if (width == 0)
            {
                //Resizing according to height
                width =(int)(height * (Convert.ToDecimal(image.Width) / image.Height));
            }
            if (height == 0)
            {
                //Resizing according to width
                height = (int)(width * (Convert.ToDecimal(image.Height) / image.Width));
            }
            //Process
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.Clamp);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }
            }
            return destImage;
        }
        /// <summary>
        /// Check if a memory stream is an image.
        /// </summary>
        /// <param name="mS">MemoryStream to check</param>
        /// <returns>True if the stream is an image, false if not</returns>
        public static bool IsValidImage(MemoryStream mS)
        {
            try
            {
                using (Image newImage = Image.FromStream(mS))
                { }
            }
            catch (Exception ex)
            {
                //The file does not have a valid image format.
                //-or- GDI+ does not support the pixel format of the file

                return false;
            }
            return true;
        }
        #endregion
    }
}

