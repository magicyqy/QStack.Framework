using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Comon
{
    /// <summary>
    /// CreateThumbnail 的摘要说明
    /// 缩略图生成类
    /// </summary>
    public sealed class CreateThumbnail
    {
        /// <summary>
        /// 生成指定图片的缩略图，默认与原图片保存在同一目录
        /// </summary>
        /// <param name="sfilename">图片文件名</param>
        /// <param name="sSavePath">图片文件相对路径</param>
        /// <param name="ThumbWidth">缩略图宽</param>
        /// <param name="ThumbHeight">高</param>
        /// <returns>返回缩略图绝对路径</returns>
        public static string createSmallImage(string sfilename, string sSavePath, int ThumbWidth, int ThumbHeight, out int width, out int height)
        {
            //缩略图宽和高
            int intThumbWidth = ThumbWidth;
            int intThumbHeight = ThumbHeight;
            string sThumbFile = "";
            string sfilepath = sSavePath.TrimEnd('/') + "/";
            width = height = 0;
            if (!string.IsNullOrWhiteSpace(sfilename))
            {
                if (sfilename.ToString().ToLower().EndsWith("pdf"))
                {
                    return "images/filetype_pdf.png";
                }
                //缩略图保存名
                sThumbFile = sfilename + "_thumb" + ".Jpeg";
                //判断缩略图在当前文件夹下是否同名称文件存在
                string smallImagePath = Path.Combine(sfilepath, sThumbFile);
                if (File.Exists(smallImagePath))
                {
                    return smallImagePath;
                }
                try
                {

                    //原图加载
                    using (System.Drawing.Image sourceImage = Image.FromFile(Path.Combine(sfilepath, sfilename)))
                    {
                        //原图宽度和高度
                        width = sourceImage.Width;
                        height = sourceImage.Height;
                        int smallWidth;
                        int smallHeight;
                        //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)
                        if (((decimal)width) / height <= ((decimal)intThumbWidth) / intThumbHeight)
                        {
                            smallWidth = intThumbWidth;
                            smallHeight = intThumbWidth * height / width;
                        }
                        else
                        {
                            smallWidth = intThumbHeight * width / height;
                            smallHeight = intThumbHeight;
                        }


                       

                     
                        //新建一个图板,以最小等比例压缩大小绘制原图
                        using (System.Drawing.Image bitmap = new Bitmap(smallWidth, smallHeight))
                        {
                            //绘制中间图
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                //高清,平滑
                                g.InterpolationMode = InterpolationMode.High;
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.Clear(Color.Black);
                                g.DrawImage(
                                sourceImage,
                                new Rectangle(0, 0, smallWidth, smallHeight),
                                new Rectangle(0, 0, width, height),
                                GraphicsUnit.Pixel
                                );
                            }
                            //新建一个图板,以缩略图大小绘制中间图
                            using (System.Drawing.Image bitmap1 = new Bitmap(intThumbWidth, intThumbHeight))
                            {
                                //绘制缩略图
                                using (Graphics g = Graphics.FromImage(bitmap1))
                                {
                                    //高清,平滑
                                    g.InterpolationMode = InterpolationMode.High;
                                    g.SmoothingMode = SmoothingMode.HighQuality;
                                    g.Clear(Color.Black);
                                    int lwidth = (smallWidth - intThumbWidth) / 2;
                                    int bheight = (smallHeight - intThumbHeight) / 2;
                                    g.DrawImage(bitmap, new Rectangle(0, 0, intThumbWidth, intThumbHeight), lwidth, bheight, intThumbWidth, intThumbHeight, GraphicsUnit.Pixel);
                                    g.Dispose();
                                    bitmap1.Save(smallImagePath, ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //出错则删除
                    //System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(sfilepath + sfilename));
                    Bitmap validateimage = new Bitmap(intThumbWidth, intThumbHeight, PixelFormat.Format24bppRgb);


                    Graphics g = Graphics.FromImage(validateimage);
                    g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(110, 20), Color.FromArgb(240, 255, 255, 255), Color.FromArgb(240, 255, 255, 255)), 0, 0, 200, 200); //矩形框
                    g.DrawString("图片加载\r\n失败", new Font("arial", 18), new SolidBrush(Color.Red), new PointF(5, 20));//字体/颜色
                    g.Save();
                    var errorGifPath = Path.Combine(sfilepath, "temp_error.gif");
                    validateimage.Save(errorGifPath, ImageFormat.Gif);
                    return errorGifPath;
                }


            }
            return Path.Combine(sfilepath, sThumbFile);
        }
    }
}
