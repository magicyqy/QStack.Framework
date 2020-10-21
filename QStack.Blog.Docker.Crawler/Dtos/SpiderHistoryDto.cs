using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Blog.Docker.Crawler.Dtos
{
	
	public class SpiderHistoryDto : BaseDto
	{

		public int SpiderId { get; set; }
		/// <summary>
		///
		/// </summary>
		public string SpiderName { get; set; }

		/// <summary>
		/// 容器标识
		/// </summary>
		public string ContainerId { get; set; }

		/// <summary>
		/// 容器标识
		/// </summary>
		public string Batch { get; set; }
		
		public DateTimeOffset CreationTime { get; set; }

		public string Status { get; set; }

		public long Left { get; set; }
		public long Total { get; set; }
		public long Success { get; set; }
		public long Failure { get; set; }

		public string Start { get; set; }

		public string Exit { get; set; }
	}
}
