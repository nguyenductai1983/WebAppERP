// File: Controllers/UtilityController.cs
using Microsoft.AspNetCore.Mvc;
using WebAppERP.ViewModels;

public class UtilityController : Controller
{
    // Action này sẽ được gọi bằng AJAX
    public IActionResult AddProductionOutputRow(int index, string stageName)
    {
        // Truyền tên công đoạn sang Partial View
        ViewData["StageName"] = stageName;
        ViewData["index"] = index;

        // Trả về một Partial View với một model rỗng
        return PartialView("~/Views/ProductionLog/_ProductionOutputRow.cshtml", new ProductionOutputViewModel());
    }
}