using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prelims.Helpers
{
    public class FilterService
    {
        public FntEntities DbContext { get; set; }
        public string FilterRegionCode { get; set; }
        public int UserId { get; set; }

        public FilterService(FntEntities _dbContext, string filterRegionCode, int userId)
        {
            this.DbContext = _dbContext;
            this.FilterRegionCode = filterRegionCode;
            this.UserId = userId;
        }

        public UserFilter GetFilter(string filterName)
        {
            string filterCode = FilterRegionCode + "-" + filterName;
            return DbContext.UserFilters.FirstOrDefault(x => x.UserId == UserId && x.FilerCode == filterCode);
        }

        public void SaveFilter(string filterName, string filterJson)
        {
            string filterCode = FilterRegionCode + "-" + filterName;

            var isAnyFilter = DbContext.UserFilters.Any(x => x.UserId == UserId && x.FilerCode == filterCode);

            if (isAnyFilter)
            {
                var existingFilter = DbContext.UserFilters.FirstOrDefault(x => x.UserId == UserId && x.FilerCode == filterCode);
                existingFilter.FilterJson = filterJson;
            }
            else
            {
                var newFilter = new UserFilter();
                newFilter.FilerCode = filterCode;
                newFilter.UserId = UserId;
                newFilter.FilterJson = filterJson;

                DbContext.UserFilters.Add(newFilter);
            }

            DbContext.SaveChanges();
        }
    }
}