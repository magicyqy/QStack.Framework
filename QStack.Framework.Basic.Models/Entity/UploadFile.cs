using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Model
{
    /// <summary>
    /// 全局的资源管理类，尽量将整站的资源（图片，视频，文档）等信息保存在此，方便统一管理
    /// </summary>
    public class UploadFile : EntityBase
    {

        /// <summary>
        /// 文件的md5码
        /// </summary>
        public virtual string MD5Code { get; set; }
      
        /// <summary>
        /// 资源类型，图片，视频，文档等
        /// </summary>
        public virtual ResouceTypeEnum ResouceType
        {
            get;
            set;
        }
        /// <summary>
        /// 文件后缀，如.doc,.png,.jpg
        /// </summary>
        public virtual string Extention
        {
            get;
            set;
        }

        /// <summary>
        /// 资源相对路径
        /// </summary>
        public virtual string RUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 资源名称
        /// </summary>
        public virtual string Filename
        {
            get;
            set;
        }
        /// <summary>
        /// 图片的缩略图
        /// </summary>
        public virtual string ThumbnailUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 源图片宽
        /// </summary>
        public virtual int SWidth { get; set; }

        /// <summary>
        /// 源图片高
        /// </summary>
        public virtual int SHeight { get; set; }
        /// <summary>
        /// 缩略图宽
        /// </summary>
        public virtual int TWidth
        {
            get;
            set;
        }
        /// <summary>
        /// 缩略图高
        /// </summary>
        public virtual int THeight
        {
            get;
            set;
        }
        /// <summary>
        /// 视频或其他种类文件的截图
        /// </summary>
        public virtual string FileCaptureUrl
        {
            get;
            set;
        }
             
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 资源绑定先关的功能名称描述，如首页图片
        /// </summary>
        public virtual string RelativeFucDes
        {
            get;
            set;

        }

        /// <summary>
        /// 关联的ID,存在多个资源的，需将相关的Id绑定到此字段,通过此字段查询返回多个资源的列表
        /// </summary>
        public virtual string RelativeId
        {
            get;
            set;

        }

       

    }
}
