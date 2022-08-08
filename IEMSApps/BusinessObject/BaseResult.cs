namespace IEMSApps.BusinessObject
{
    public class Result<T> : BaseResult
    {
        public Result()
        {
            Datas = default(T);
        }
        public T Datas { get; set; }
    }

    public class BaseResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}