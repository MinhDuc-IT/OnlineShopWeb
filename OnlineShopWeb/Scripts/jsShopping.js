$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;
        var tQuantity = $('#quantity_value').text();
        if (tQuantity != '') {
            quantity = parseInt(tQuantity);
        }
        /*alert(id + " " + quantity);*/
        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                if (rs.success) {
                    $('#cart-item-count').html(rs.Count);
                    alert(rs.msg);
                }
            }
        });
    });
    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm('Are you sure you want to remove these products from your cart');
        if (conf == true) {
            DeleteAll();
        }
    });
    $('body').on('click', '.cart_quantity_delete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var conf = confirm('Are you sure you want to remove this product from your cart');
        if (conf == true) {
            $.ajax({
                url: '/Cart/Delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.success) {
                        $('#cart-item-count').html(rs.Count);
                        $('#trow_' + id).remove();
                        UpdateCartTotal();
                    }
                }
            });
        }
    });
    // Increase quantity
    $('body').on('click', '.cart_quantity_up', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: '/Cart/IncreaseQuantity',
            type: 'POST',
            data: { id: id },
            success: function (rs) {
                if (rs.success) {
                    $('#trow_' + id).find('.cart_quantity_input').val(rs.newQuantity);
                    $('#trow_' + id).find('.cart_total_price').text(rs.newTotalPrice);
                    $('#cart-item-count').text(rs.cartCount);
                    UpdateCartTotal();
                }
            }
        });
    });
    $('body').on('click', '.btnDeleteSelected', function (e) {
        e.preventDefault();
        var selectedItems = [];
        $('.item-select:checked').each(function () {
            selectedItems.push($(this).data('id'));
        });
        if (selectedItems.length === 0) {
            alert("Please select at least one product to delete.");
            return;
        }
        var conf = confirm('Are you sure you want to remove these products from your cart?');
        if (conf == true) {
            $.ajax({
                url: '/Cart/DeleteMany',
                type: 'POST',
                data: { ids: selectedItems },
                success: function (rs) {
                    if (rs.success) {
                        $('#cart-item-count').html(rs.count);
                        selectedItems.forEach(function (id) {
                            $('#trow_' + id).remove();
                        });
                    }
                }
            });
        }
    });
    $('body').on('click', '.cart_quantity_down', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: '/Cart/DecreaseQuantity',
            type: 'POST',
            data: { id: id },
            success: function (rs) {
                if (rs.success) {
                    if (rs.newQuantity > 0) {
                        $('#trow_' + id).find('.cart_quantity_input').val(rs.newQuantity);
                        $('#trow_' + id).find('.cart_total_price').text(rs.newTotalPrice);
                    }
                    else {
                        var conf = confirm('Are you sure you want to remove this product from your cart');
                        if (conf == true) {
                            $('#trow_' + id).remove();
                        }
                    }
                    $('#cart-item-count').text(rs.cartCount);
                    UpdateCartTotal();
                }
            }
        });
    });
    $('#selectAll').on('change', function () {
        var isChecked = $(this).is(':checked');
        $('.item-select').prop('checked', isChecked);
        UpdateCartTotal();
    });
    $('body').on('change', '.item-select', function () {
        UpdateCartTotal();
    });
});
function ShowCount() {
    $.ajax({
        url: '/Cart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#cart-item-count').html(rs.Count);
        }
    });
}
function DeleteAll() {
    $.ajax({
        url: '/Cart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.success) {
                LoadCart();
            }
        }
    });
}
function LoadCart() {
    $.ajax({
        url: '/Cart/Index',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}
//function UpdateCartTotal() {
//    let totalSum = 0;
//    $('.item-select:checked').each(function () {
//        totalSum += parseFloat($(this).data('price'));
//    });
//    $('th:contains("Sum:")').next().text(totalSum.toFixed(2));
//}
function UpdateCartTotal() {
    let totalSum = 0;
    $('.cart_total_price').each(function () {
        // Kiểm tra xem item có được chọn hay không
        const itemRow = $(this).closest('tr'); // Lấy hàng của item
        const isSelected = itemRow.find('.item-select').is(':checked'); // Kiểm tra ô checkbox
        if (isSelected) {
            totalSum += parseFloat($(this).text()); // Chỉ cộng nếu item được chọn
        }
    });
    //$('th:contains("Sum:")').next().text(totalSum.toFixed(2)); // Cập nhật giá trị tổng
    $('#sum').text(totalSum.toFixed(2)); // Cập nhật giá trị tổng vào phần tử có id="sum"
}