using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpTools.Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// 图片转为base64编码的文本
        /// </summary>
        /// <param name="Imagefilename"></param>
        /// <returns></returns>
        public static string ImgToBase64String(string Imagefilename)
        {
            var result = string.Empty;
            try
            {
                System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(Imagefilename);
                using (MemoryStream ms1 = new MemoryStream())
                {
                    bmp1.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr1 = new byte[ms1.Length];
                    ms1.Position = 0;
                    ms1.Read(arr1, 0, (int)ms1.Length);
                    ms1.Close();
                    result = Convert.ToBase64String(arr1);
                }

            }
            catch (Exception ex)
            {
                result = string.Format("转换失败：{0}", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 图片转为base64编码的文本
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static string ImgToBase64String(Bitmap bmp)
        {
            var result = string.Empty;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr1 = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr1, 0, (int)ms.Length);
                    ms.Close();
                    result = Convert.ToBase64String(arr1);
                }
            }
            catch (Exception ex)
            {
                result = string.Format("转换失败：{0}", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// base64转为图片
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="filePath">存储路径</param>
        /// <param name="fileName">文件名（带后缀）</param>
        public static void Base64ToImg(string base64, string filePath, string fileName)
        {
            byte[] arr2 = Convert.FromBase64String(base64);
            using (MemoryStream ms2 = new MemoryStream(arr2))
            {
                var filedic = string.Format("{0}/{1}", filePath.Trim('/'), fileName);
                Bitmap bmp2 = new Bitmap(ms2);
                //bmp2.Save(filedic + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp2.Save(filePath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bmp2.Save(filePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                bmp2.Save(filedic, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// base64转为图片
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static Bitmap Base64ToImg(string base64)
        {
            Bitmap bmp = null;
            byte[] arr2 = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(arr2))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }
    }
}
