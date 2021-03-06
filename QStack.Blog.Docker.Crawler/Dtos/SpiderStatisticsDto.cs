﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Dtos
{
	
	public class SpiderStatisticsDto
	{
		/// <summary>
		/// 爬虫标识
		/// </summary>
		[StringLength(36)]
		[Column("id")]
		public virtual string Id { get;  set; }

		/// <summary>
		/// 爬虫名称
		/// </summary>
		[StringLength(255)]
		
		public virtual string Name { get;  set; }

		/// <summary>
		/// 爬虫开始时间
		/// </summary>
		
		public virtual DateTimeOffset? Start { get;  set; }

		/// <summary>
		/// 爬虫退出时间
		/// </summary>
		
		public virtual DateTimeOffset? Exit { get;  set; }

		/// <summary>
		/// 链接总数
		/// </summary>
	
		public virtual long Total { get;  set; }

		/// <summary>
		/// 已经完成
		/// </summary>
		
		public virtual long Success { get;  set; }

		/// <summary>
		/// 失败链接数
		/// </summary>
		
		public virtual long Failure { get;  set; }

		/// <summary>
		///
		/// </summary>
	
		public DateTimeOffset LastModificationTime { get;  set; }

		/// <summary>
		///
		/// </summary>
		
		public DateTimeOffset CreationTime { get;  set; }

		public SpiderStatisticsDto(string id)
		{
		

			Id = id;
		}

		public void SetName(string name)
		{
			
			Name = name;
		}

		public void OnStarted()
		{
			Start = DateTimeOffset.Now;
		}

		public void OnExited()
		{
			Exit = DateTimeOffset.Now;
		}

		public void IncrementSuccess()
		{
			Success += 1;
		}

		public void IncrementFailure()
		{
			Failure += 1;
		}

		public void IncrementTotal(long count)
		{
			Total += count;
		}
	}
}
