// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Lấy id của submenu đang hoạt động từ sessionStorage
    var activeSubmenuId = sessionStorage.getItem('activeSubmenu');

    if (activeSubmenuId) {
        // Tìm và thêm class 'show' để mở submenu tương ứng
        $(activeSubmenuId).addClass('show');
    }

    // Gán sự kiện click cho tất cả các link kích hoạt submenu trong sidebar
    $('#sidebarMenu .nav-link[data-bs-toggle="collapse"]').on('click', function () {
        // Lấy id của submenu mà link này điều khiển (giá trị của thuộc tính href)
        var submenuId = $(this).attr('href');

        // Kiểm tra xem submenu sắp được mở hay đóng
        var isOpening = !$(submenuId).hasClass('show');

        if (isOpening) {
            // Nếu đang mở, lưu id của nó vào sessionStorage
            sessionStorage.setItem('activeSubmenu', submenuId);
        } else {
            // Nếu đang đóng (nhấp vào menu đang mở), xóa key khỏi sessionStorage
            // Điều này cho phép người dùng chủ động đóng menu
            if (sessionStorage.getItem('activeSubmenu') === submenuId) {
                sessionStorage.removeItem('activeSubmenu');
            }
        }
    });
});