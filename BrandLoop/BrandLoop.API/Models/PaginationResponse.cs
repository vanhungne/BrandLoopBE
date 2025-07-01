namespace BrandLoop.API.Models
{
    public class PaginationResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PaginationResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
    public class PaginationResponseV2<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int TotalInvitation { get; set; } = 0;
        public int TotalWaitingInvitation { get; set; } = 0;
        public int TotalAcceptedInvitation { get; set; } = 0;
        public int TotalRejectedInvitation { get; set; } = 0;
        public int TotalExpiredInvitation { get; set; } = 0;
        public PaginationResponseV2(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords, int totalInvitation, int totalWaiting, int totalAcceptedInvitation, int totalRejectedInvitation, int totalExpiredInvitation)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            TotalInvitation = totalInvitation;
            TotalWaitingInvitation = totalWaiting;
            TotalAcceptedInvitation = totalAcceptedInvitation;
            TotalRejectedInvitation = totalRejectedInvitation;
            TotalExpiredInvitation = totalExpiredInvitation;
        }
    }
}   