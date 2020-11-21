
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler.Models
{
	[Table("agent_heartbeat")]
	public class AgentHeartbeat:IEntityRoot
	{
		/// <summary>
		/// 节点标识
		/// </summary>
		public int Id { get;  set; }

		/// <summary>
		/// 标识
		/// </summary>
		[StringLength(36)]
		[Column("agent_id")]
		public string AgentId { get;  set; }

		/// <summary>
		/// 名称
		/// </summary>
		[StringLength(255)]
		[Column("agent_name")]
		public string AgentName { get;  set; }

		/// <summary>
		/// 空闲内存
		/// </summary>
		[Column("free_memory")]
		public int FreeMemory { get;  set; }

		[Column("cpu_load")]
		public int CpuLoad { get;  set; }

		/// <summary>
		/// 上报时间
		/// </summary>
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get;  set; }

		public AgentHeartbeat(string agentId, string agentName, int freeMemory, int cpuLoad)
		{
			//agentId.NotNullOrWhiteSpace(nameof(agentId));

			AgentId = agentId;
			AgentName = agentName;
			FreeMemory = freeMemory;
			CpuLoad = cpuLoad;
			CreationTime = DateTimeOffset.Now;
		}
	}
}
