namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int pageRecords = 10;
        private readonly int maxPageRecords = 50;

        public int PageRecordsTotal
        {
            get { return pageRecords; }
            set { pageRecords = (value > maxPageRecords) ? maxPageRecords : value; }
        }
    }
}
