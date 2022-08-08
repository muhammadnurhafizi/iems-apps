using IEMSApps.Utils;

namespace IEMSApps.BusinessObject.Inputs
{
    public class SearchDataInput
    {
        public Enums.SearchCarianType CarianType { get; set; }
        public string SearchValue { get; set; }
        public Enums.SearchTindakanType TindakanType { get; set; }
    }
}