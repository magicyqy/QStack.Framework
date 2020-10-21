using QStack.Framework.Core.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Blog.Docker.Crawler.Models
{
	[Table("SPIDER_HISTORIES")]
	public class SpiderHistory:EntityBase
	{
		

		/// <summary>
		///
		/// </summary>
		[Column("spider_id")]
		[Required]
		public int SpiderId { get; set; }
		[Column("SPIDER_NAME")]
		[StringLength(255)]
		[Required]
		public string SpiderName { get; set; }
		/// <summary>
		/// 容器标识
		/// </summary>
		[Column("container_id")]
		[StringLength(100)]
		public string ContainerId { get; set; }

		/// <summary>
		/// 容器标识
		/// </summary>
		[Column("batch")]
		[StringLength(100)]
		public string Batch { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("creation_time")]
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		[Column("status")]
		[StringLength(20)]
		[Required]
		public string Status { get; set; }
	}
}
