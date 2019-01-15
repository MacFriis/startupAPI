using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using StartupApi.Model;

namespace StartupApi.Services
{
    public class DefaultDataService : IDataService
    {
        private readonly AppDbContext context;

        private readonly IConfigurationProvider mappingConfiguration;

        public DefaultDataService(
            AppDbContext context,
            IConfigurationProvider mappingConfiguration)
        {
            this.context = context;
            this.mappingConfiguration = mappingConfiguration;
        }



        public async Task<PagedResults<string>> GetAvailableCategoriesAsync(PagingOptions pagingOptions, SearchOptions<AppData, AppDataEntity> searchOptions)
        {
            IQueryable<AppDataEntity> query = context.AppDatas.Where(x => x.Category != null);

            query = searchOptions.Apply(query);

            var items = await query
                .Select(x => x.Category)
                .Distinct()
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ToArrayAsync();

            var size = items.Count();

            return new PagedResults<string>
            {
                Items = items,
                TotalSize = size
            };

        }

        public async Task<PagedResults<AppData>> GetDataAsync(PagingOptions pagingOptions, SortOptions<AppData, AppDataEntity> sortOptions, SearchOptions<AppData, AppDataEntity> searchOptions)
        {
            IQueryable<AppDataEntity> query = context.AppDatas;
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            var size = await query.CountAsync();

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<AppData>(mappingConfiguration)
                .ToArrayAsync();

            return new PagedResults<AppData>
            {
                Items = items,
                TotalSize = size
            };
        }

        public async Task<AppData> GetDataAsync(Guid id)
        {
            var entity = await context.AppDatas
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            var mapper = mappingConfiguration.CreateMapper();
            return mapper.Map<AppData>(entity);
        }
    }
}
