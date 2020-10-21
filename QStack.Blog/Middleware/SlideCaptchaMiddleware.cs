using QStack.Blog.Areas.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Basic;
using QStack.Framework.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Middleware
{
    public class SlideCaptchaMiddleware
    {
        private readonly RequestDelegate _next;
      
        ICompositeViewEngine viewEngine;
        IWebHostEnvironment environment;
        ITempDataProvider tempDataProvider;
        IJsonHelper jsonHelper;
        public SlideCaptchaMiddleware(RequestDelegate next,IWebHostEnvironment environment, ICompositeViewEngine engine, ITempDataProvider tempDataProvider, IJsonHelper jsonHelper)
        {
            _next = next;
            viewEngine = engine;
            this.environment = environment;
            this.tempDataProvider = tempDataProvider;
            this.jsonHelper = jsonHelper;
        }

        public async Task Invoke(HttpContext context)
        {
            
            
            var path = context.Request.Path.ToString().ToLower();
            if (interceptorRoute.Any(r => path.Contains(r)))
            {
                //await GetCode(context);
                //context.Response.Redirect("/captcha/get");
                if (!CheckResult(context))
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.PreconditionRequired;
                    var response = new ResponseResult<string>();
                    response.Data = context.Request.GetHostUri().TrimEnd('/') + "/captcha/get";
                    response.Code = BusinessCode.Captcha_Required;
                    response.Message = nameof(BusinessCode.Captcha_Required);
                    await context.Response.WriteAsync(jsonHelper.Serialize(response).ToString());
                    return;
                }
            }
                  
           
            var action = actionList.FirstOrDefault(a => path.Contains(a));
            switch (action)
                {
                    case "captcha/get":
                        await GetCode(context);
                        break;
                    case "captcha/check":
                        await Check(context);
                        break;
                    //case "captcha/checkresult":
                    //    await CheckResult(context);
                    //    break;
                   
                    default: 
                        await _next.Invoke(context);
                        break;
             }
        }

        #region 参数
        //裁剪的小图大小
        private const int _shearSize = 40;
        //原始图片所在路径 300*300
        private string path = string.Format("captcha{0}slideimages{0}300_200", Path.DirectorySeparatorChar);
        //原始图片数量
        private const int _ImgNum = 60;
        //原始图片宽px
        private int _ImgWidth = 300;
        //原始图片高px
        private int _ImgHeight = 200;
        //裁剪位置X轴最小位置
        private int _MinRangeX = 30;
        //裁剪位置X轴最大位置
        private int _MaxRangeX = 240;
        //裁剪位置Y轴最小位置
        private int _MinRangeY = 30;
        //裁剪位置Y轴最大位置
        private int _MaxRangeY = 150;
        //裁剪X轴大小 裁剪成20张上10张下10张
        private int _CutX = 30;
        //裁剪Y轴大小 裁剪成20张上10张下10张
        private int _CutY = 100;
        //小图相对原图左上角的x坐标  x坐标保存到session 用于校验
        private int _PositionX;
        //小图相对原图左上角的y坐标  y坐标返回到前端
        private int _PositionY;

        private string[] interceptorRoute = { "api/comment/postcomment" };
        //action命令列表
        private string[] actionList = { "captcha/get", "captcha/check" };
        //图片规格列表 默认300*200
        private string[] imgspecList = { "300*300", "300*200", "200*100" };
        //允许误差 单位像素
        private const int _deviationPx = 2;
        //是否跨域访问 在将项目做成第三方使用时可用跨域解决方案 所有的session替换成可共用的变量(Redis)
        private Boolean _isCallback = false;
        //最大错误次数
        private const int _MaxErrorNum = 4;
        #endregion


        /// <summary>
        /// 校验前端是否通过验证
        /// </summary>
        private async Task Check(HttpContext context)
        {
            var result = new ResponseResult<int>();
            context.Response.ContentType = "text/plain";
            string ls_point = context.Request.Form["point"];//完成时x轴对于左上角位置位置
            string datelist = context.Request.Form["datelist"];//滑动过程特征
            string timespan = context.Request.Form["timespan"];//耗时
            if (context.Session.GetInt32("code") == null)
            {
                result.Message = nameof(BusinessCode.Captcha_Code_NotExist);
                result.Code = BusinessCode.Captcha_Code_NotExist;
            }
            if (ls_point.IsNullOrWhiteSpace())
            {
                result.Message = nameof(BusinessCode.Captcha_Pos_NotExist);
                result.Code = BusinessCode.Captcha_Pos_NotExist;
            }
            int li_old_point = 0, li_now_point = 0;

            li_old_point = context.Session.GetInt32("code")==null?0: context.Session.GetInt32("code").Value;
            
            li_now_point = Convert.ToInt32(ls_point);

            if (li_old_point <= 0 || li_now_point<= 0)
            {
                result.Message = nameof(BusinessCode.Captcha_Pos_NotValid);
                result.Code = BusinessCode.Captcha_Pos_NotValid;
            }
            //错误
            if (Math.Abs(li_old_point - li_now_point) > _deviationPx)
            {
                int li_count = 0;
                try
                {
                    li_count = context.Session.GetInt32("code_errornum") == null ? 0 : context.Session.GetInt32("code_errornum").Value; ;
                }
                catch
                {
                    li_count = 0;
                }
                li_count++;
                if (li_count > _MaxErrorNum)
                {
                    //超过最大错误次数后不再校验
                    context.Session.Remove("code");
                    result.Message = nameof(BusinessCode.Captcha_Retry_Exceed);
                    result.Code = BusinessCode.Captcha_Retry_Exceed;

                }
                else
                {
                    context.Session.SetInt32("code_errornum", li_count);
                    result.Message = nameof(BusinessCode.Captcha_Pos_Error);
                    result.Code = BusinessCode.Captcha_Pos_Error;
                    //返回错误次数
                    result.Data = li_count;



                }
            }
            else
                result.Data = li_old_point;
            if (SlideFeature(datelist))
            {
                //机器人??
            }
            //校验成功 返回正确坐标
            context.Session.SetString("isCheck","OK");
            context.Session.Remove("code_errornum");
            context.Session.Remove("code");
            await context.Response.WriteAsync(jsonHelper.Serialize(result).ToString());
           
        }
        /// <summary>
        /// 后台校验验证是否被伪造
        /// </summary>
        private bool CheckResult(HttpContext context)
        {
            var result = new ResponseResult();
            var isCheck = context.Session.GetString("isCheck");
            //校验成功 返回正确坐标
            if (isCheck.IsNullOrWhiteSpace())
            {
                return false;
            }
            if (!isCheck.Equals("OK"))
            {
                return false;
            }
            context.Session.Remove("isCheck");
            return true;
        }

        private async Task GetCode(HttpContext context)
        {
            Random rd = new Random();
            _PositionX = rd.Next(_MinRangeX, _MaxRangeX);
            _PositionY = rd.Next(_MinRangeY, _MaxRangeY);
            context.Session.SetInt32("code", _PositionX);
            context.Session.SetInt32("code_errornum",0);
            int[] a = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            int[] array = a.OrderBy(x => Guid.NewGuid()).ToArray();
            var filename = Path.Combine(environment.WebRootPath, path, (new Random()).Next(0, _ImgNum - 1) + ".jpg");
            Bitmap bmp = new Bitmap(filename);
            string ls_small = "data:image/jpg;base64," + ImgToBase64String(cutImage(bmp, _shearSize, _shearSize, _PositionX, _PositionY));
            Bitmap lb_normal = GetNewBitMap(bmp, _shearSize, _shearSize, _PositionX, _PositionY);
            string ls_confusion = "data:image/jpg;base64," + ImgToBase64String(ConfusionImage(array, lb_normal));
            var captchaInfo = new CaptchaInfo
            {
                Errcode=0,
                Y= _PositionY,
                Array= string.Join(",", array),
                ImgX= _ImgWidth,
                ImgY= _ImgHeight,
                Small= ls_small,
                Normal= ls_confusion
            };

            /* errcode: 状态值 成功为0
             * y:裁剪图片y轴位置
             * small：小图字符串
             * normal：剪切小图后的原图并按无序数组重新排列后的图
             * array：无序数组
             * imgx：原图宽
             * imgy：原图高
             */

            var viewResult = viewEngine.GetView("~/", "~/Views/Shared/SlideCaptcha.cshtml", true);
            if (viewResult.Success)
            {
                //创建临时的StringWriter实例，用来配置到视图上下文中
                using (var output = new StringWriter())
                {
                    //视图上下文对于视图渲染来说很重要，视图中的前后台交互都需要它
                    var viewContext = new ViewContext()
                    {
                        HttpContext = context,
                        Writer = output,
                        RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                        {
                            
                        },
                        ViewData = new ViewDataDictionary( new EmptyModelMetadataProvider(),new ModelStateDictionary())
                        {
                            Model = captchaInfo
                        },
                        TempData=  new TempDataDictionary(context, tempDataProvider), //ViewData
                        View = viewResult.View,
                        FormContext = new FormContext(),
                        ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                    };
                    //渲染
                    await viewResult.View.RenderAsync(viewContext);
                    var html = output.ToString();
                    context.Response.ContentType = "text/html";
                    //输出到响应体
                    await context.Response.WriteAsync(html);
                }
            }
            else    
                await context.Response.WriteAsync(jsonHelper.Serialize(captchaInfo).ToString());
        }


        #region tool method
        /// <summary>
        /// 获取裁剪的小图
        /// </summary>
        /// <param name="sFromBmp">原图</param>
        /// <param name="cutWidth">剪切宽度</param>
        /// <param name="cutHeight">剪切高度</param>
        /// <param name="x">X轴剪切位置</param>
        /// <param name="y">Y轴剪切位置</param>
        private Bitmap cutImage(Bitmap sFromBmp, int cutWidth, int cutHeight, int x, int y)
        {
            //载入底图   
            Image fromImage = sFromBmp;

            //先初始化一个位图对象，来存储截取后的图像
            Bitmap bmpDest = new Bitmap(cutWidth, cutHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            //这个矩形定义了，你将要在被截取的图像上要截取的图像区域的左顶点位置和截取的大小
            Rectangle rectSource = new Rectangle(x, y, cutWidth, cutHeight);

            //这个矩形定义了，你将要把 截取的图像区域 绘制到初始化的位图的位置和大小
            //我的定义，说明，我将把截取的区域，从位图左顶点开始绘制，绘制截取的区域原来大小
            Rectangle rectDest = new Rectangle(0, 0, cutWidth, cutHeight);

            //第一个参数就是加载你要截取的图像对象，第二个和第三个参数及如上所说定义截取和绘制图像过程中的相关属性，第四个属性定义了属性值所使用的度量单位
            Graphics g = Graphics.FromImage(bmpDest);
            g.DrawImage(fromImage, rectDest, rectSource, GraphicsUnit.Pixel);
            g.Dispose();
            return bmpDest;
        }
        /// <summary>
        /// 获取裁剪小图后的原图
        /// </summary>
        /// <param name="sFromBmp">原图</param>
        /// <param name="cutWidth">剪切宽度</param>
        /// <param name="cutHeight">剪切高度</param>
        /// <param name="spaceX">X轴剪切位置</param>
        /// <param name="spaceY">Y轴剪切位置</param>
        public Bitmap GetNewBitMap(Bitmap sFromBmp, int cutWidth, int cutHeight, int spaceX, int spaceY)
        {
            // 加载原图片 
            Bitmap oldBmp = sFromBmp;
            // 绑定画板 
            Graphics grap = Graphics.FromImage(oldBmp);
            // 加载水印图片 
            Bitmap bt = new Bitmap(cutWidth, cutHeight);
            Graphics g1 = Graphics.FromImage(bt);  //创建b1的Graphics
            g1.FillRectangle(Brushes.Black, new Rectangle(0, 0, cutWidth, cutHeight));   //把b1涂成红色
            bt = PTransparentAdjust(bt, 120);
            // 添加水印 
            grap.DrawImage(bt, spaceX, spaceY, cutWidth, cutHeight);
            grap.Dispose();
            g1.Dispose();
            return oldBmp;
        }
        /// <summary>
        /// 获取半透明图像
        /// </summary>
        /// <param name="bmp">Bitmap对象</param>
        /// <param name="alpha">alpha分量。有效值为从 0 到 255。</param>
        public Bitmap PTransparentAdjust(Bitmap bmp, int alpha)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color bmpcolor = bmp.GetPixel(i, j);
                    byte A = bmpcolor.A;
                    byte R = bmpcolor.R;
                    byte G = bmpcolor.G;
                    byte B = bmpcolor.B;
                    bmpcolor = Color.FromArgb(alpha, R, G, B);
                    bmp.SetPixel(i, j, bmpcolor);
                }
            }
            return bmp;
        }
        /// <summary>
        /// 获取混淆拼接的图片
        /// </summary>
        /// <param name="a">无序数组</param>
        /// <param name="bmp">剪切小图后的原图</param>
        public Bitmap ConfusionImage(int[] a, Bitmap cutbmp)
        {
            Bitmap[] bmp = new Bitmap[20];
            for (int i = 0; i < 20; i++)
            {
                int x, y;
                x = a[i] > 9 ? (a[i] - 10) * _CutX : a[i] * _CutX;
                y = a[i] > 9 ? _CutY : 0;
                bmp[i] = cutImage(cutbmp, _CutX, _CutY, x, y);
            }
            Bitmap Img = new Bitmap(_ImgWidth, _ImgHeight);      //创建一张空白图片
            Graphics g = Graphics.FromImage(Img);   //从空白图片创建一个Graphics
            for (int i = 0; i < 20; i++)
            {
                //把图片指定坐标位置并画到空白图片上面
                g.DrawImage(bmp[i], new Point(i > 9 ? (i - 10) * _CutX : i * _CutX, i > 9 ? _CutY : 0));
            }
            g.Dispose();
            return Img;
        }

        //Bitmap转为base64编码的文本
        private string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch
            {
                //ImgToBase64String 转换失败\nException:" + ex.Message);
                return null;
            }
        }
        //base64编码的文本转为Bitmap
        private Bitmap Base64StringToImage(string txtBase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(txtBase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch
            {
                //Base64StringToImage 转换失败\nException：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 滑动特性
        /// </summary>
        public bool SlideFeature(string as_data)
        {
            if (string.IsNullOrEmpty(as_data))
                return false;
            string[] _datalist = as_data.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (_datalist.Length < 10)
                return false;
            //__array二维数组共两列 第一列为x轴坐标值 第二列为时间
            long[,] __array = new long[_datalist.Length, 2];
            #region 获取__array
            int row = 0;
            foreach (string str in _datalist)
            {
                string[] strlist = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strlist.Length != 2)
                    return false;
                long __coor = 0, __date = 0;
                try { __coor = long.Parse(strlist[0]); __date = long.Parse(strlist[1]); }
                catch { return false; }
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        __array[row, i] = __coor;
                    if (i == 1)
                        __array[row, i] = __date;
                }
                row++;
            }
            #endregion
            #region 计算速度 加速度 以及他们的标准差
            //速度 像素/每秒
            double[] __v = new double[_datalist.Length - 1];
            //加速度 像素/每2次方秒
            double[] __a = new double[_datalist.Length - 1];
            //总时间
            int __totaldate = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                //时间差
                int __timeSpan = 0;
                if (__array[i + 1, 1] - __array[i, 1] == 0)
                    __timeSpan = 1;
                else
                    __timeSpan = (GetTime(__array[i + 1, 1].ToString()) - GetTime(__array[i, 1].ToString())).Milliseconds;
                __v[i] = (double)1000 * Math.Abs(__array[i + 1, 0] - __array[i, 0]) / __timeSpan;//有可能移过再一回来 这里只取正值
                __a[i] = (double)1000 * __v[i] / __timeSpan;
                __totaldate += __timeSpan;
            }
            //速度的计算公式：v=S/t
            //加速度计算公式：a=Δv/Δt
            //分析速度与加速度

            //平均速度
            double __mv = 0;
            double __sumv = 0;
            double __s2v = 0;//速度方差
            double __o2v = 0;//速度标准差
            foreach (double a in __v)
            {
                __sumv += a;
            }
            __mv = __sumv / __v.Length;
            __sumv = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                __sumv += Math.Pow(__v[i] - __mv, 2);
            }
            __s2v = __sumv / __v.Length;
            __o2v = Math.Sqrt(__s2v);

            //平均加速度
            double __ma = 0;
            double __suma = 0;
            double __s2a = 0;//加速度方差
            double __o2a = 0;//加速度标准差
            foreach (double a in __a)
            {
                __suma += a;
            }
            __ma = __suma / __a.Length;
            __suma = 0;
            for (int i = 0; i < __a.Length; i++)
            {
                __suma += Math.Pow(__a[i] - __ma, 2);
            }
            __s2a = __suma / __v.Length;
            __o2a = Math.Sqrt(__s2a);

            double threeEqual = __a.Length / 3;
            //将加速度数组分成三等分 求每一份的加速度
            double __ma1 = 0, __ma2 = 0, __ma3 = 0;
            for (int i = 0; i < __a.Length; i++)
            {
                if (i > threeEqual * 2)
                    __ma3 += __a[i];
                else if (i > threeEqual && i < threeEqual * 2)
                    __ma2 += __a[i];
                else
                    __ma1 += __a[i];
            }
            __ma1 = __ma1 / threeEqual;
            __ma2 = __ma2 / threeEqual;
            __ma3 = __ma3 / threeEqual;
            //将速度数组分成三等分 求每一份的速度
            threeEqual = __v.Length / 3;
            double __mv1 = 0, __mv2 = 0, __mv3 = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                if (i > threeEqual * 2)
                    __mv3 += __v[i];
                else if (i > threeEqual && i < threeEqual * 2)
                    __mv2 += __v[i];
                else
                    __mv1 += __v[i];
            }
            __mv1 = __mv1 / threeEqual;
            __mv2 = __mv2 / threeEqual;
            __mv3 = __mv3 / threeEqual;
            #endregion
            
            return true;
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime now = dtStart.Add(toNow);
            return now;
        }
        #endregion
    }
}
