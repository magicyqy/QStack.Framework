namespace QStack.Framework.Basic
{
    public class ResponseResult
    {
       
        public string Message { get; set; }

       
        public int Code { get; set; }

        public ResponseResult()
        {
            Code = 200;
            Message = "success";
        }
    }



    public class ResponseResult<T> : ResponseResult
    {
        public ResponseResult() : base() { }
        public ResponseResult(T data):base()
        {
            Data = data;
         }  
        /// <summary>
        /// 回传的结果
        /// </summary>
        public T Data { get; set; }
    }
}
