using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Requests
{
    public class TowerFilterParams : PaginationParams
    {
        public string? Name { get; set; }
    }
}