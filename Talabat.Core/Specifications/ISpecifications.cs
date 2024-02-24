using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        // Signature for property => Where Condition
        public Expression<Func<T, bool>> Criteria { get; set; }

        // Signature for property => List of Include(s)
        public List<Expression<Func<T, object>>> Includes { get; set; }

        // OrderBy Asc
        public Expression<Func<T, object>> OrderBy { get; set; }

        // OrderBy Desc
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        // Skip
        public int Skip { get; set; }

        // Take
        public int Take { get; set; }

        public bool IsPaginationEnabled { get; set; }
    }
}
