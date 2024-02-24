using Talabat.APIs.DTOs;

namespace Talabat.APIs.Helpers
{
    public class Pagination<T>
    {
        private IReadOnlyList<ProductToReturnDTO> mappedProducts;

        public Pagination(int pageSize, int pageIndex, IReadOnlyList<T> data, int count)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Data = data;
            Count = count;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
