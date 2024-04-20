using Microsoft.AspNetCore.Mvc.Rendering;

namespace VAVS_Client.Services
{
    public interface StateDivisionService
    {
        List<StateDivision> GetStateDivisions();
        List<SelectListItem> GetSelectListStateDivisions();
    }
}
