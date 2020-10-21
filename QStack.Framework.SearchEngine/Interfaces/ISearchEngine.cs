using QStack.Framework.SearchEngine.Imps;
using System;
using System.Linq.Expressions;

namespace QStack.Framework.SearchEngine.Interfaces
{
    public interface ISearchEngine
    {
       
        /// <summary>
        /// 索引器
        /// </summary>
        ILuceneIndexer LuceneIndexer { get; }

        /// <summary>
        /// 索引搜索器
        /// </summary>
        ILuceneIndexSearcher LuceneIndexSearcher { get; }

        /// <summary>
        /// 索引总数
        /// </summary>
        int IndexCount { get; }

        /// <summary>
        /// 创建索引
        /// </summary>
        void CreateIndex(Action<ILuceneIndexer> luceneIndexer);
        void CreateIndex<T, TKey>(T[] indexObjects, Expression<Func<T, TKey>> keySelector, LuceneIndexBehavior<T> behavior = null);


        /// <summary>
        /// 删除索引
        /// </summary>
        void DeleteIndex();

      

       

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        IScoredSearchResultCollection<ILuceneIndexable> ScoredSearch(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        IScoredSearchResultCollection<T> ScoredSearch<T>(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        ISearchResultCollection<ILuceneIndexable> Search(SearchOptions options);

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        ISearchResultCollection<T> Search<T>(SearchOptions options);

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        object SearchOne(SearchOptions options);

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        T SearchOne<T>(SearchOptions options) where T : class;
        LuceneHighLighter GetHighLighter(string preTag = ILuceneHighLighter.DEFAULT_PRETAG, string postTag = ILuceneHighLighter.DEFAULT_POSTTAG, int fragmentSize = ILuceneHighLighter.FRAGMENTSIZE, int maxNumFragments = ILuceneHighLighter.MAXNUMFRAGMENTS);

    }
}