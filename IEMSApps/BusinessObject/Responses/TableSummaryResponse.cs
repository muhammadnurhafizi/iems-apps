namespace IEMSApps.BusinessObject.Responses
{
    public class TableSummaryResponse
    {
        public string RecordDesc { get; set; }
        public string TableName { get; set; }
        public int TotalApp { get; set; } = 0;
        public int TotalRec { get; set; } = 0;
        public bool IsSelected { get; set; }
        public int IsModified { get; set; }
    }
}