$(document).ready(function () {
    let quantity = 1;

    // Increase quantity
    $("#increase").click(function () {
        quantity++;
        $("#quantity_value").text(quantity);
    });

    // Decrease quantity
    $("#decrease").click(function () {
        if (quantity > 1) {
            quantity--;
            $("#quantity_value").text(quantity);
        }
    });

    // Add to cart button click
    $(".btnAddToCart").click(function (e) {
        e.preventDefault();
        const productId = $(this).data("id");
        const selectedQuantity = parseInt($("#quantity_value").text());

        console.log("Adding product to cart", { productId, selectedQuantity });
        // Add AJAX request here to add product to cart
    });
});