// Services/IMRPService.cs
using System.Collections.Generic; // <== THÊM DÒNG NÀY
using System.Threading.Tasks;
using WebAppERP.ViewModels;

namespace WebAppERP.Services
{
    public interface IMRPService
    {
        Task<List<MaterialRequirementViewModel>> CalculateRequirementsAsync();        
    }
}