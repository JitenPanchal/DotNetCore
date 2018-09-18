using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Database
{
    public interface IAuditable
    {
        DateTime CreatedDate { get; set; }
        int CreatedBy { get; set; }
        DateTime? ModifiedDate { get; set; }
        int? ModifiedBy { get; set; }
    }
}
