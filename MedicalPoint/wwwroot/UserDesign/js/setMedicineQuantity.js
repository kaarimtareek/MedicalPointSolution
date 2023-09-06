function setData({ Id }) {

    var e = document.getElementById(Id);
    var optionId = e.options[e.selectedIndex].id;
    var formattedId = optionId.replace('medicine-quantity-', '');
    var quantity = parseInt(formattedId);
    document.getElementById('add-visit-medicine-quantity-label').innerHTML = quantity;
}