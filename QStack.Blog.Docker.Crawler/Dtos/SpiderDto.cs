using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Blog.Docker.Crawler.Dtos
{
	
	public class SpiderDto : BaseDto
	{


		/// <summary>
		/// 是否启用
		/// </summary>
		
		public bool Enabled { get; set; }

		/// <summary>
		/// 爬虫名称
		/// </summary>
		[Required]
		[StringLength(255)]
		
		public string Name { get; set; }

		/// <summary>
		/// 爬虫名称
		/// </summary>
		[Required]
		[StringLength(255)]
	
		public string Image { get; set; }

		/// <summary>
		/// 定时表达式
		/// </summary>
		[StringLength(255)]
		[Required]
		
		public string Cron { get; set; }

		/// <summary>
		/// docker 运行的环境变量
		/// </summary>
		[StringLength(2000)]
		
		public string Environment { get; set; }

		/// <summary>
		/// docker 运行挂载的盘
		/// </summary>
		[StringLength(2000)]
		
		public string Volume { get; set; }

		/// <summary>
		/// Creation time of this entity.
		/// </summary>
		[Required]
		
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// 上一次更新时间
		/// </summary>
		
		public DateTimeOffset LastModificationTime { get; set; }
	}
}
