using Microsoft.AspNetCore.Mvc.Rendering;
using VAVS_Client.Data;

namespace VAVS_Client.Services.Impl
{
    public class StateDivisionServiceImpl : AbstractServiceImpl<StateDivision>, StateDivisionService
    {
        private readonly ILogger<StateDivisionServiceImpl> _logger;

        public StateDivisionServiceImpl(VAVSClientDBContext context, ILogger<StateDivisionServiceImpl> logger) : base(context, logger)
        {
            _logger = logger;
        }

        public List<SelectListItem> GetSelectListStateDivisions()
        {
            _logger.LogInformation(">>>>>>>>>> [StateDivisionServiceImpl][GetSelectListStateDivisions]  Get SelectList StateDivisions. <<<<<<<<<<");
            try
            {
                _logger.LogInformation($">>>>>>>>>> Success. Get SelectList StateDivisions. <<<<<<<<<<");
                return GetItemsFromList(GetStateDivisions(), "StateDivisionPkid", "StateDivisionName");
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when getting selectList StateDivisions. <<<<<<<<<<" + e);
                throw;
            }
        }

        public List<StateDivision> GetStateDivisions()
        {
            _logger.LogInformation(">>>>>>>>>> [TownshipServiceImpl][GetTownships]  Get all StateDivisions. <<<<<<<<<<");
            try
            {
                _logger.LogInformation($">>>>>>>>>> Success. Get all StateDivisions. <<<<<<<<<<");
                return GetAll();
            }
            catch (Exception e)
            {
                _logger.LogError(">>>>>>>>>> Error occur when getting all StateDivisions. <<<<<<<<<<" + e);
                throw;
            }
        }
    }
}
