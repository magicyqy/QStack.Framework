namespace QStack.Framework.SearchEngine.Interfaces
{
    public interface ILuceneHighLighter
    {
        public const string DEFAULT_PRETAG = "<span style='color:red;'>";
        public const string DEFAULT_POSTTAG = "</span>";
        public const int FRAGMENTSIZE = 150;
        public const int MAXNUMFRAGMENTS = 200;
        string HighLight(string keyword, string sourceText);
    }
}