using NUnit.Framework;
using Leaf.Blog.LuceneWrap.Imps;
using System;
using System.Collections.Generic;
using System.Text;
using jieba.NET;
using QStack.Framework.SearchEngine.Imps;
using QStack.Framework.SearchEngine.Interfaces;
using Microsoft.Extensions.Hosting;
using Leaf.BlogTests;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using QStack.Framework.Core.Cache;

namespace Leaf.Blog.LuceneWrap.Imps.Tests
{
    [TestFixture()]
    public class LuceneHighLighterTests:BaseTests
    {
        LuceneHighLighter highLighter;
        [SetUp]
        public new void Init()
        {
            base.Init();
            var analyzer = new JieBaAnalyzer(JiebaNet.Segmenter.TokenizerMode.Search);
            var searcher = new LuceneIndexSearcher(host.Services.GetService<Directory>(), analyzer, host.Services.GetService<ICache>());
            highLighter =new LuceneHighLighter(analyzer, searcher, 150,200, ILuceneHighLighter.DEFAULT_PRETAG, ILuceneHighLighter.DEFAULT_POSTTAG);

        }
        [Test()]
        public void HighLightTest()
        {
           var str= highLighter.HighLight("苍老师", @"苍老师鱼儿离不开水啊对方水电费，水果蛋糕，依妹儿 吃 依地酸二钠，项目经理必须负责项目的顺利推进");
            Console.WriteLine(str);
            Assert.IsNotNull(str);
        }
    }
}