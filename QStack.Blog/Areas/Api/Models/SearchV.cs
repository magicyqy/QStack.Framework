using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Areas.Api.Models
{
    public class SearchV
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 20;

        public DateTimeOffset?[] DateRange { get; set; }

        public int? CatagoryId { get; set; }

        public int? ArticleId { get; set; }
    }
}
