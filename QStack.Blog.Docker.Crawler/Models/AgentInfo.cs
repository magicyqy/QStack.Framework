using QStack.Framework.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Models
{
	[Table("agent")]
	public class AgentInfo:IEntityRoot
	{
		/// <summary>
		/// 标识
		/// </summary>
		[StringLength(36)]
		public virtual string Id { get;  set; }

		/// <summary>
		/// 名称
		/// </summary>
		[StringLength(255)]
		public string Name { get;  set; }

		/// <summary>
		/// CPU 核心数
		/// </summary>
		[Column("processor_count")]
		public int ProcessorCount { get;  set; }

		/// <summary>
		/// 总内存
		/// </summary>
		[Column("total_memory")]
		public int TotalMemory { get;  set; }

		/// <summary>
		/// 上一次更新时间
		/// </summary>
		[Column("last_modification_time")]
		public DateTimeOffset LastModificationTime { get;  set; }

		/// <summary>
		/// 是否已经标记删除
		/// </summary>
		[Column("is_deleted")]
		public bool IsDeleted { get;  set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Required]
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get;  set; }

		/// <summary>
		/// 刷新上一次更新时间
		/// </summary>
		public void Refresh()
		{
			LastModificationTime = DateTimeOffset.Now;
		}

		public AgentInfo(string id, string name, int processorCount, int totalMemory)
		{
			//id.NotNullOrWhiteSpace(nameof(id));
			//name.NotNullOrWhiteSpace(nameof(name));

			Id = id;
			Name = name;
			ProcessorCount = processorCount;
			TotalMemory = totalMemory;
			CreationTime = DateTimeOffset.Now;
			LastModificationTime = CreationTime;
		}

		public bool Online => (DateTimeOffset.Now - LastModificationTime).TotalSeconds <= 30;
	}
}
