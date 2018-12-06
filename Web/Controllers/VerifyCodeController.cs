using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class VerifyCodeController : ApiController
    {
        [HttpPost]
        public ApiResult Get()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            string imgBase64 = "";
            //var userDTO = await userService.GetModelAsync(user.Id);

            //if (userDTO == null)
            //{
            //    return new ApiResult { status = 0, msg = "会员不存在" };
            //}
            string code = RandomCode(4).ToUpper();
            HttpContext.Current.Session["VerifyCode"+ user.Id] = code;
            imgBase64 = CreateCheckCodeImage(code);
           
            return new ApiResult { status = 1, data = imgBase64 };
        }
        //public ApiResult GetSession()
        //{
        //    User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
        //    var code = HttpContext.Current.Session["VerifyCode" + user.Id];
        //    return new ApiResult { status = 1, data = code };
        //}
        private string CreateCheckCodeImage(string checkCode)
        {
            if ((checkCode != null) && !(checkCode.Trim() == string.Empty))
            {
                Bitmap image = new Bitmap((int)Math.Ceiling(checkCode.Length * 18.5), 0x20);
                Graphics g = Graphics.FromImage(image);
                try
                {
                    int i;
                    Random random = new Random();
                    g.Clear(Color.White);
                    for (i = 0; i < 0x2; i++)
                    {
                        int x1 = random.Next(image.Width);
                        int x2 = random.Next(image.Width);
                        int y1 = random.Next(image.Height);
                        int y2 = random.Next(image.Height);
                        g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                    }
                    Font font = new Font("Arial", 16f, FontStyle.Italic | FontStyle.Bold);
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Red, Color.BlueViolet, 1.2f, true);
                    g.DrawString(checkCode, font, brush, (float)2f, (float)2f);
                    for (i = 0; i < 50; i++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);
                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }
                    //g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Png);

                    //byte[] arr = new byte[ms.Length];
                    //ms.Position = 0;
                    //ms.Read(arr, 0, (int)ms.Length);
                    //ms.Close();
                    return "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());

                }
                finally
                {
                    g.Dispose();
                    image.Dispose();
                }
            }
            return "";
        }

        private string GetValidateCode()
        {
            byte[] data = null;
            string code = RandomCode(4);
            HttpContext.Current.Session["VerifyCode"].ToString(); 
            //定义一个画板
            MemoryStream ms = new MemoryStream();
            using (Bitmap map = new Bitmap(100, 40))
            {
                //画笔,在指定画板画板上画图
                //g.Dispose();
                using (Graphics g = Graphics.FromImage(map))
                {
                    g.Clear(Color.White);
                    g.DrawString(code.ToUpper(), new Font("黑体", 18.0F), Brushes.Blue, new Point(10, 8));
                    //绘制干扰线
                    PaintInterLine(g, 3, map.Width, map.Height);
                }
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            data = ms.GetBuffer();

            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return "data:image/jpeg;base64," + Convert.ToBase64String(arr);

        }
        private void PaintInterLine(Graphics g, int num, int width, int height)
        {
            Random r = new Random();
            int startX, startY, endX, endY;
            for (int i = 0; i < num; i++)
            {
                startX = r.Next(0, width);
                startY = r.Next(0, height);
                endX = r.Next(0, width);
                endY = r.Next(0, height);
                g.DrawLine(new Pen(Brushes.Red), startX, startY, endX, endY);
            }
        }

        // 随机生成指定长度的验证码字符串
        private string RandomCode(int length)
        {
            string s = "0123456789zxcvbnmasdfghjklqwertyuiop";
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            int index;
            for (int i = 0; i < length; i++)
            {
                index = rand.Next(0, s.Length);
                sb.Append(s[index]);
            }
            return sb.ToString();
        }
    }


}