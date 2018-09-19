using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DotNetCore.Models
{
    public class PagingRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid page number.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Invalid page size. Page size must be between 1 and 50 inclusive.")]
        public int PageSize { get; set; } = 10;
    }
}
