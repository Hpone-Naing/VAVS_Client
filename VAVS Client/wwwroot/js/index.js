$(document).ready(function () {
    $('.selectpicker').selectpicker();
    $('.bs-searchbox input').on('input', function () {
        var searchValue = $(this).val();

        if (searchValue != null && searchValue != '' && searchValue.length >= 3) {
            $.getJSON("/IRD_VAVS_Client/VehicleStandardValue/GetMadeModel", { searchString: searchValue }, function (models) {
                if (models != null && !jQuery.isEmptyObject(models)) {
                    $('.selectpicker').empty(); 
                    $.each(models, function (index, model) {
                        //console.log("model ......" + model);
                        $('.selectpicker').append('<option>' + model + '</option>');
                    });
                    $('.selectpicker').selectpicker('refresh'); 
                }
            });
        }
        var selectedText = $('.dropdown-menu .active .text').text();
        console.log("Selected Option Text: " + selectedText);
    });
   
   
});
