using Microsoft.AspNetCore.Mvc.Rendering;
using VAVS_Client.Data;
using VAVS_Client.Models;

namespace VAVS_Client.Services.Impl
{
    public class TownshipServiceImpl : AbstractServiceImpl<Township>, TownshipService
    {
        private readonly ILogger<TownshipServiceImpl> _logger;

        public TownshipServiceImpl(VAVSClientDBContext context, ILogger<TownshipServiceImpl> logger) : base(context, logger)
        {
            _logger = logger;
        }

        public List<SelectListItem> GetSelectListTownships()
        {
            _logger.LogInformation(">>>>>>>>>> [TownshipServiceImpl][GetSelectListTownships]  Get SelectList Townships. <<<<<<<<<<");
            try
            {
                _logger.LogInformation($">>>>>>>>>> Success. Get SelectList Townships. <<<<<<<<<<");
                return GetItemsFromList(GetTownships(), "TownshipPkid", "TownshipName");
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when getting selectList townships. <<<<<<<<<<" + e);
                throw;
            }
        }

        public List<Township> GetTownships()
        {
            _logger.LogInformation(">>>>>>>>>> [TownshipServiceImpl][GetTownships]  Get all Townships. <<<<<<<<<<");
            try
            {
                _logger.LogInformation($">>>>>>>>>> Success. Get all Townships. <<<<<<<<<<");
                return GetAll();
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when getting all townships. <<<<<<<<<<" + e);
                throw;
            }
        }
    }
}
