///////////////////////////////////////////////////////////
//  IDao.cs

using System;
using System.Collections;
using System.Collections.Generic;

namespace QStack.Framework.Core.Entity
{
    public class PageModel
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount=>TotalCount%PageSize==0? TotalCount /PageSize: TotalCount / PageSize+1;
        /// <summary>
        /// 数据总数
        /// </summary>
        public int TotalCount { get; set; } = 0;
        /// <summary>
        /// 实际总数，用于含子孙节点的统计
        /// </summary>
        public int ActualTotalCount { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
       

        public bool HasPreviousPage { get { return (Page >1); } }
        public bool HasNextPage { get { return (Page * PageSize) < TotalCount - PageSize; } }
    }
    public class PageModel<T> :PageModel where T : class
    {

        public List<T> Data { get; set; } = new List<T>();


    }
}