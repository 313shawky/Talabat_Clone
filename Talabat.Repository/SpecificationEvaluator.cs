using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        // Function to build dynamic query

        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var Query = inputQuery; // _dbContext.Set<T>();
            if(spec.Criteria is not null) // P => P.Id == Id
            {
                Query = Query.Where(spec.Criteria); // _dbContext.Set<T>().Where(P => P.Id == Id)
            }
            // P => P.ProductBrand , P => P.ProductType

            if (spec.OrderBy is not null)
            {
                Query = Query.OrderBy(spec.OrderBy);
            }

            // Pagination
            if (spec.IsPaginationEnabled)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }

            if (spec.OrderByDescending  is not null)
            {
                Query = Query.OrderByDescending(spec.OrderByDescending);
            }

            Query = spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            return Query;
        }
    }
}
