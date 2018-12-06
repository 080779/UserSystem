using CodeCarvings.Piczard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Common
{
    public static class ImageHelper
    {
        public static bool SaveBase64(string file,out string res)
        {
            if(!file.Contains(";base64"))
            {
                res = "不是base64图片文件";
                return false;
            }
            string[] strs = file.Split(',');
            string[] formats = strs[0].Replace(";base64", "").Split(':');
            string img = strs[1];
            string format = formats[1];
            string[] imgFormats = { "image/png", "image/jpg", "image/jpeg", "image/bmp", "IMAGE/PNG", "IMAGE/JPG", "IMAGE/JPEG", "IMAGE/BMP" };

            if (!imgFormats.Contains(format))
            {
                res = "请选择正确的图片格式，支持png、jpg、jpeg、png格式";
                return false;
            }
            string ext = "." + format.Split('/')[1];
            byte[] imgBytes = null;
            try
            {
                imgBytes = Convert.FromBase64String(img);
            }
            catch (Exception ex)
            {
                res = "base64图片文件解码错误";
                return false;
            }

            string md5 = CommonHelper.GetMD5(imgBytes);
            string path = "/upload/" + DateTime.Now.ToString("yyyy") + "/" + md5 + ext;
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();

            //Install-Package CodeCarvings.Piczard
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.SaveProcessedImageToFileSystem(imgBytes, fullPath);
            res = path;
            return true;
        }

        public static async Task<KeyValuePair<bool, string>> Base64SaveAsync(string base64File)
        {
            bool flag = true;
            string res = "";
            if (string.IsNullOrEmpty(base64File))
            {
                res = "请选择图片文件";
                return new KeyValuePair<bool, string>(flag, res);
            }

            if (!base64File.Contains(";base64"))
            {
                res = base64File;
                return new KeyValuePair<bool, string>(flag, res);
            }
            string[] strs = base64File.Split(',');
            string[] formats = strs[0].Replace(";base64", "").Split(':');
            string img = strs[1];
            string format = formats[1];
            string[] imgFormats = { "image/png", "image/jpg", "image/jpeg", "image/bmp", "IMAGE/PNG", "IMAGE/JPG", "IMAGE/JPEG", "IMAGE/BMP" };

            if (!imgFormats.Contains(format))
            {
                res = "请选择正确的图片格式，支持png、jpg、jpeg、png格式";
                flag = false;
                return new KeyValuePair<bool, string>(flag, res);
            }
            string ext = "." + format.Split('/')[1];
            byte[] imgBytes = null;
            try
            {
                imgBytes = Convert.FromBase64String(img);
            }
            catch (Exception ex)
            {
                res = "base64图片文件解码错误";
                flag = false;
                return new KeyValuePair<bool, string>(flag, res);
            }

            string md5 = CommonHelper.GetMD5(imgBytes);
            string path = "/upload/" + DateTime.Now.ToString("yyyy") + "/" + md5 + ext;
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();

            using (FileStream stream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                await stream.WriteAsync(imgBytes, 0, imgBytes.Length);
                res = path;
                return new KeyValuePair<bool, string>(flag, res);
            }
        }

        public static string SaveByte(Stream stream)
        {
            byte[] files = StreamToBytes(stream);
            string md5 = CommonHelper.GetMD5(files);
            string path = "/upload/" + DateTime.Now.ToString("yyyy") + "/" + md5 + ".jpeg";
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();
            //file.SaveAs(fullPath);
            //缩略图
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.SaveProcessedImageToFileSystem(files, fullPath);
            return path;
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static string SaveImage(HttpPostedFileBase file)
        {
            string md5 = CommonHelper.GetMD5(file.InputStream);
            string ext = Path.GetExtension(file.FileName);
            string path = "/upload/" + DateTime.Now.ToString("yyyy") + "/" + md5 + ext;
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();
            //file.SaveAs(fullPath);
            //缩略图
            file.InputStream.Position = 0;
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.SaveProcessedImageToFileSystem(file.InputStream, fullPath);
            return path;
        }

        public static string[] Save(HttpPostedFileBase file)
        {
            string md5 = CommonHelper.GetMD5(file.InputStream);
            string ext = Path.GetExtension(file.FileName);
            string path = "/upload/" + DateTime.Now.ToString("yyyy") + "/" + md5 + ext;
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();
            //file.SaveAs(fullPath);
            //缩略图
            file.InputStream.Position = 0;
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.SaveProcessedImageToFileSystem(file.InputStream, fullPath);
            string[] paths = { path };
            return paths;
        }
    }
}
