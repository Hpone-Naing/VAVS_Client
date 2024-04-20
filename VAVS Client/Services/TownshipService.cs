using Microsoft.AspNetCore.Mvc.Rendering;

namespace VAVS_Client.Services
{
    public interface TownshipService
    {
        List<Township> GetTownships();
        List<SelectListItem> GetSelectListTownships();
    }
}
