
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using QStack.Framework.Core.Entity;

namespace QStack.Framework.Basic.Model.Shop
{

    public  class Product :EntityBase
    {
        public  Product()
        {
            ProductImages = new List<ProductImage>();
        }

        public virtual string Name { get; set; }

        public virtual int State { get; set; }

        public virtual int ViewCount { get; set; }
        /// 产品图
        /// </summary>
        public virtual  string ImageUrl { get; set; }
        /// <summary>
        /// 产品缩略图
        /// </summary>
        public virtual string ImageThumbUrl { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public virtual int? BrandCD { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public virtual int ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual string PartNumber { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public virtual string Color { get; set; }
        /// <summary>
        /// 销售价格
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public virtual decimal? Price { get; set; }
        /// <summary>
        /// 折扣价格
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public virtual decimal? RebatePrice { get; set; }
        /// <summary>
        /// 进价，成本价
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public virtual decimal? PurchasePrice { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public virtual string Norm { get; set; }
        /// <summary>
        /// 保质期
        /// </summary>
        public virtual string ShelfLife { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string ProductContent { get; set; }

        public virtual string SEOTitle { get; set; }
        public virtual string SeoKeyWord { get; set; }
        public virtual string SeoDescription { get; set; }
        public virtual int? OrderIndex { get; set; }
        public virtual string SourceFrom { get; set; }
        public virtual string Url { get; set; }
        public virtual bool IsPublish { get; set; }
        public virtual DateTime? PublishDate { get; set; }
        public virtual string TargetFrom { get; set; }
        public virtual string TargetUrl { get; set; }
        [NotMapped]
        public virtual IList<ProductCategoryTag> ProductTags { get; set; }
        
        public virtual IList<ProductImage> ProductImages { get; set; }
        public virtual IList<ProductDownload> ProductDownloads { get; set; }
    }
  

}
