using System.Collections.Generic;
using System.Linq;
using WebAppERP.Data;
using WebAppERP.Models;

namespace WebAppERP.Services
{
    public class BomCalculationService
    {
        private readonly ApplicationDbContext _context;

        public BomCalculationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<WorkOrderBOM> CalculateAndGenerateBom(SalesOrderDetail orderDetail, int workOrderId)
        {
            var workOrderBoms = new List<WorkOrderBOM>();

            // --- Logic tính toán nhựa ---
            var plasticProduct = _context.Products.FirstOrDefault(p => p.Sku == "RAW-PLASTIC-01"); // Giả sử SKU của nhựa
            if (plasticProduct != null)
            {
                // Áp dụng công thức tính
                decimal requiredPlastic = orderDetail.GsmOrGlm * orderDetail.FabricWidth * orderDetail.Meterage * orderDetail.QuantityPerBag;

                workOrderBoms.Add(new WorkOrderBOM
                {
                    WorkOrderId = workOrderId,
                    ComponentId = plasticProduct.Id,
                    RequiredQuantity = requiredPlastic
                });
            }

            // --- Logic lấy các NVL cố định khác (nếu có) ---
            var standardComponents = _context.BillOfMaterials
                .Where(b => b.FinishedProductId == orderDetail.ProductId)
                .ToList();

            foreach (var item in standardComponents)
            {
                workOrderBoms.Add(new WorkOrderBOM
                {
                    WorkOrderId = workOrderId,
                    ComponentId = item.ComponentId,
                    RequiredQuantity = item.Quantity * orderDetail.Quantity // Định mức chuẩn nhân số lượng
                });
            }

            return workOrderBoms;
        }
    }
}