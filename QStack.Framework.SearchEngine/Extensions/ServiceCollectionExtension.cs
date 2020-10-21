using jieba.NET;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using QStack.Framework.SearchEngine.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QStack.Framework.SearchEngine.Imps;

namespace QStack.Framework.SearchEngine.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static IServiceCollection AddSearchEngine(this IServiceCollection services, LuceneIndexerOptions option)
        {
            services.AddSingleton(s => option);
            //services.AddMemoryCache();
            services.TryAddSingleton<Directory>(s => FSDirectory.Open(option.Path));
            services.TryAddSingleton<Analyzer>(s => new JieBaAnalyzer(TokenizerMode.Search));
            services.TryAddTransient<ILuceneIndexer, LuceneIndexer>();
            services.TryAddTransient<ILuceneIndexSearcher, LuceneIndexSearcher>();
            services.TryAddTransient(typeof(ISearchEngine), typeof(SearchEngineImp));
       
            return services;
        }
    }
}