using QStack.Framework.Basic.Model.ViewModel;
using QStack.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.ViewModel.Shop
{
    public class ProductDownloadDto:BaseRootDto
    {
        public virtual int ProductId { get; set; }
        public virtual string DownloadUrl { get; set; }

        public virtual string ValidCode { get; set; }

        public virtual string ExtDesc { get; set; }

        public virtual int DownloadNum { get; set; }
        public virtual string Gid { get; set; } = Guid.NewGuid().ToString("N");

    }
}
