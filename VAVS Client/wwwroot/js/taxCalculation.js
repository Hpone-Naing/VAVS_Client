function changeUnit(element) {
    var inputValue = $(element).val().trim();
    var selectUnit = $('#select-unit');

    if (inputValue.length === 1) {
        selectUnit.val('1').trigger('change');
    } else if (inputValue.length === 2) {
        selectUnit.val('10').trigger('change');
    } else if (inputValue.length === 3) {
        selectUnit.val('100').trigger('change');
    } else if (inputValue.length === 4) {
        selectUnit.val('1000').trigger('change');
    } else if (inputValue.length === 5) {
        selectUnit.val('10000').trigger('change');
    } else {
        selectUnit.val('').trigger('change');
    }
}