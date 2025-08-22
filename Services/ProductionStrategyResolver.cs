// File: Services/ProductionStrategyResolver.cs
using System.Collections.Generic;
using System.Linq;
using WebAppERP.Services.ProductionStrategies;

namespace WebAppERP.Services
{
    /// <summary>
    /// Đóng vai trò là một "Factory" hoặc "Resolver", có nhiệm vụ tìm và cung cấp
    /// chiến lược xử lý sản xuất (IProductionStrategy) phù hợp dựa trên tên công đoạn.
    /// </summary>
    public class ProductionStrategyResolver
    {
        private readonly IEnumerable<IProductionStrategy> _strategies;

        /// <summary>
        /// Constructor này sử dụng sức mạnh của Dependency Injection (DI).
        /// ASP.NET Core sẽ tự động tìm tất cả các class đã được đăng ký với interface IProductionStrategy
        /// và "tiêm" chúng vào đây dưới dạng một danh sách (IEnumerable).
        /// </summary>
        /// <param name="strategies">Một danh sách tất cả các chiến lược có sẵn trong hệ thống.</param>
        public ProductionStrategyResolver(IEnumerable<IProductionStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Tìm chiến lược phù hợp dựa trên tên của công đoạn sản xuất.
        /// </summary>
        /// <param name="stageName">Tên công đoạn (ví dụ: "Đùn Sợi", "Dệt").</param>
        /// <returns>Đối tượng chiến lược phù hợp hoặc null nếu không tìm thấy.</returns>
        public IProductionStrategy GetStrategy(string stageName)
        {
            // Dùng LINQ để tìm trong danh sách chiến lược đã được inject
            // chiến lược nào có thuộc tính StageName khớp với tên được cung cấp.
            return _strategies.FirstOrDefault(s => s.StageName == stageName);
        }
    }
}