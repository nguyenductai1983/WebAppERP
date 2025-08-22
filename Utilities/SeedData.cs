using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebAppERP.Data
{
    public static class SeedData
    {
        public static async Task Initialize(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // --- 1. TẠO CÁC ROLE ---
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- 2. TẠO TÀI KHOẢN ADMIN MẶC ĐỊNH ---
            var adminEmail = "admin@erp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new IdentityUser()
                {
                    UserName = "admin", // Username để đăng nhập
                    Email = adminEmail,
                    EmailConfirmed = true // Tự động xác thực email
                };

                // Đặt mật khẩu cho tài khoản admin
                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    // Gán quyền "Admin" cho tài khoản vừa tạo
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}