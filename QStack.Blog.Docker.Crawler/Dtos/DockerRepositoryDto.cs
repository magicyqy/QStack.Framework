using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QStack.Blog.Docker.Crawler.Dtos
{
	
	public class DockerRepositoryDto : BaseDto
	{

		/// <summary>
		///
		/// </summary>
		[StringLength(255)]
		[Required]
	
		public string Name { get; set; }

		/// <summary>
		/// http:// or https://
		/// </summary>

		[StringLength(10)]
		public string Schema { get; set; }

		/// <summary>
		/// registry.cn-shanghai.aliyuncs.com/ 允许为空，表示本地镜像
		/// </summary>
		[StringLength(255)]

		public string Registry { get; set; }

		/// <summary>
		/// zlzforever/ids4admin
		/// </summary>
		[StringLength(255)]
		[Required]

		public string Repository { get; set; }


		[StringLength(255)]
		public string UserName { get; set; }


		[StringLength(255)]
		public string Password { get; set; }

		/// <summary>
		/// Creation time of this entity.
		/// </summary>
		[Required]

		public DateTimeOffset CreationTime { get; set; }
	}
}

