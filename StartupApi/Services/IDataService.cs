using System;
using System.Threading.Tasks;
using StartupApi.Model;

namespace StartupApi.Services
{
    /// <summary>
    /// Data service.
    /// This interface is only for template use, you must create a data service
    /// interface for all the data that you will use/expose
    /// The name of the services / interface should match the data your exposing
    /// </summary>
    public interface IDataService
    {
        Task<PagedResults<AppData>> GetDataAsync(
            PagingOptions pagingOptions,
            SortOptions<AppData, AppDataEntity> sortOptions,
            SearchOptions<AppData, AppDataEntity> searchOptions);

        Task<AppData> GetDataAsync(Guid id);

        Task<PagedResults<string>> GetAvailableCategoriesAsync(
            PagingOptions pagingOptions,
            SearchOptions<AppData, AppDataEntity> searchOptions);
    }


}
