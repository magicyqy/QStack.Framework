using QStack.Framework.SearchEngine.Imps;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using QStack.Framework.SearchEngine.Extensions;
using QStack.Framework.SearchEngine.Interfaces;
using QStack.Framework.Core.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace QStack.Framework.SearchEngine.Imps
{
    /// <summary>
    /// 搜索引擎
    /// </summary>
    public class SearchEngineImp : ISearchEngine
    {
        /// <summary>
        /// 索引器
        /// </summary>
        public ILuceneIndexer LuceneIndexer { get; }

        /// <summary>
        /// 索引搜索器
        /// </summary>
        public ILuceneIndexSearcher LuceneIndexSearcher { get; }

        /// <summary>
        /// 索引条数
        /// </summary>
        public int IndexCount => LuceneIndexer.Count();

        public LuceneHighLighter HighLighter { get; }

        private Analyzer analyzer;
        /// <summary>
        /// 搜索引擎
        /// </summary>   
        /// <param name="directory"></param>
        /// <param name="analyzer"></param>
        /// <param name="memoryCache"></param>
        public SearchEngineImp(Directory directory, Analyzer analyzer, ICache cache)
        {
            this.analyzer = analyzer;
            LuceneIndexer = new LuceneIndexer(directory, analyzer);
            LuceneIndexSearcher = new LuceneIndexSearcher(directory, analyzer, cache);
         
        }

       

        /// <summary>
        ///获取文档的具体版本
        /// </summary>
        /// <param name ="doc">要转换的文档</param>
        /// <returns></returns>
        private object GetConcreteFromDocument(Document doc)
        {
            var t = Type.GetType(doc.Get("Type"));
            var obj = t.Assembly.CreateInstance(t.FullName, true);
            foreach (var p in t.GetProperties())
            {
                p.SetValue(obj, doc.Get(p.Name, p.PropertyType));
            }
            return obj;
        }

       


        /// <summary>
        /// 创建索引
        /// </summary>
        public void CreateIndex<T,TKey>(T[] indexObjects,Expression<Func<T, TKey>> keySelector,LuceneIndexBehavior<T> behavior=null)
        {
            if (LuceneIndexer == null)
            {
                return;
            }

            var index = new List<ILuceneIndexable>();
            var properties = typeof(T).GetProperties();
            foreach (var obj in indexObjects)
            {
                index.Add(new LuceneIndexable<T, TKey>(obj, keySelector.Compile()(obj), behavior));
            }

            if (index.Any())
            {
                LuceneIndexer.CreateIndex(index);
            }
        }


        /// <summary>
        /// 删除索引
        /// </summary>
        public void DeleteIndex()
        {
            LuceneIndexer?.DeleteAll();
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public ISearchResultCollection<T> Search<T>(SearchOptions options)
        {
            options.Type = typeof(T);
            var indexResults = LuceneIndexSearcher.ScoredSearch(options);
            ISearchResultCollection<T> resultSet = new SearchResultCollection<T>
            {
                TotalHits = indexResults.TotalHits
            };

            var sw = Stopwatch.StartNew();
            foreach (var indexResult in indexResults.Results)
            {
                var entity = (T)GetConcreteFromDocument(indexResult.Document);
                resultSet.Results.Add(entity);
            }

            sw.Stop();
            resultSet.Elapsed = indexResults.Elapsed + sw.ElapsedMilliseconds;
            return resultSet;
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型，但不返回任何评分信息
        /// </summary>
        /// <typeparam name ="T">要搜索的实体类型 - 注意：必须实现ILuceneIndexable </typeparam>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public IScoredSearchResultCollection<T> ScoredSearch<T>(SearchOptions options)
        {
            // 确保类型匹配
            if (typeof(T) != typeof(ILuceneIndexable))
            {
                options.Type = typeof(T);
            }

            var indexResults = LuceneIndexSearcher.ScoredSearch(options);
            IScoredSearchResultCollection<T> results = new ScoredSearchResultCollection<T>();
            results.TotalHits = indexResults.TotalHits;
            var sw = Stopwatch.StartNew();
            foreach (var indexResult in indexResults.Results)
            {
                IScoredSearchResult<T> result = new ScoredSearchResult<T>();
                result.Score = indexResult.Score;
                result.Entity = (T)GetConcreteFromDocument(indexResult.Document);
                results.Results.Add(result);
            }

            sw.Stop();
            results.Elapsed = indexResults.Elapsed + sw.ElapsedMilliseconds;
            return results;
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public IScoredSearchResultCollection<ILuceneIndexable> ScoredSearch(SearchOptions options)
        {
            return ScoredSearch<ILuceneIndexable>(options);
        }

        /// <summary>
        /// 执行搜索并将结果限制为特定类型，在返回之前，搜索结果将转换为相关类型
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public ISearchResultCollection<ILuceneIndexable> Search(SearchOptions options)
        {
            return Search<ILuceneIndexable>(options);
        }

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public object SearchOne(SearchOptions options)
        {
            return GetConcreteFromDocument(LuceneIndexSearcher.ScoredSearchSingle(options));
        }

        /// <summary>
        /// 搜索一条匹配度最高的记录
        /// </summary>
        /// <param name ="options">搜索选项</param>
        /// <returns></returns>
        public T SearchOne<T>(SearchOptions options) where T : class
        {
            return GetConcreteFromDocument(LuceneIndexSearcher.ScoredSearchSingle(options)) as T;
        }

        public void CreateIndex(Action<ILuceneIndexer> luceneIndexer)
        {
            luceneIndexer.Invoke(this.LuceneIndexer);
        }

        public LuceneHighLighter GetHighLighter(string preTag=ILuceneHighLighter.DEFAULT_PRETAG,string postTag= ILuceneHighLighter.DEFAULT_POSTTAG,int fragmentSize= ILuceneHighLighter.FRAGMENTSIZE,int maxNumFragments= ILuceneHighLighter.MAXNUMFRAGMENTS)
        {
            return new LuceneHighLighter(analyzer, LuceneIndexSearcher, fragmentSize, maxNumFragments, preTag, postTag);
        }
    }
}