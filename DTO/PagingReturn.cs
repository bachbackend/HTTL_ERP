namespace HTTL_ERP.DTO
{
    public class PagingReturn
    {
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
    }
}
