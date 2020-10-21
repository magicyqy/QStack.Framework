using QStack.Framework.SearchEngine.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search.Highlight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QStack.Framework.Util;

using jieba.NET;
using JiebaNet.Segmenter;
using Lucene.Net.Search;
using System.Text.RegularExpressions;

namespace QStack.Framework.SearchEngine.Imps
{
    public class LuceneHighLighter : ILuceneHighLighter
    {
        
        Analyzer _analyzer;

        IFormatter formatter;
        Highlighter highlighter;
        MultiFieldQueryParser queryParser;
        int fragmentSize = 100;
        int maxNumFragments = 10;
        string filedName = $"content";
        ILuceneIndexSearcher _luceneIndexSearcher;
        public LuceneHighLighter(Analyzer analyzer,ILuceneIndexSearcher luceneIndexSearcher, int fragmentSize, int maxNumFragments, string preTag, string postTag)
        {
           
            _analyzer = analyzer;
            _luceneIndexSearcher = luceneIndexSearcher;
            formatter = new SimpleHTMLFormatter(ILuceneHighLighter.DEFAULT_PRETAG, ILuceneHighLighter.DEFAULT_POSTTAG);
            this.fragmentSize = fragmentSize <= 0 ? ILuceneHighLighter.FRAGMENTSIZE : fragmentSize;
            this.maxNumFragments = maxNumFragments <= 0 ? ILuceneHighLighter.MAXNUMFRAGMENTS : maxNumFragments;
            queryParser = new MultiFieldQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, new string[] { filedName }, _analyzer);

        }

        public string HighLight(string keyword, string sourceText)
        {
            if (keyword.IsNullOrWhiteSpace() || sourceText.IsNullOrWhiteSpace())
                return string.Empty;
    
         
            //queryParser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, filedName, _analyzer);

            //var query = queryParser.Parse(keyword);
          
            var query = _luceneIndexSearcher.GetFuzzyquery(queryParser, keyword);
            var scorer = new QueryScorer(query);
            Highlighter highlighter = new Highlighter(formatter, scorer);
            highlighter.TextFragmenter = new SimpleFragmenter(fragmentSize);
            //highlighter.MaxDocCharsToAnalyze = 200;
            TokenStream tokenStream = _analyzer.GetTokenStream(filedName, new StringReader(sourceText));
            var frags = highlighter.GetBestFragments(tokenStream, sourceText, maxNumFragments);
            return frags.Length > 0 ? frags[0] : sourceText.Substring(0, Math.Min(this.fragmentSize,sourceText.Length));
        }

      
    }
}
