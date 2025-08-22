// File: Models/ProductionProcessingMethod.cs
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public enum ProductionProcessingMethod
    {
        [Display(Name = "")] // Rỗng cho None
        None = 0,

        [Display(Name = "YarnProduction")] // Tên Controller
        DefaultYarn = 1,

        [Display(Name = "FabricProduction")] // Tên Controller
        DefaultFabric = 2,

        [Display(Name = "CoatingProduction")] // Tên Controller
        DefaultCoating = 3
    }
}