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
        var selectedText1 = $('.dropdown-menu .active .text').text();
        console.log("Selected Option Text1: " + selectedText1);
        var selectedText3 = $('.filter-option-inner-inner').text();
        console.log("Selected Option Text3: " + selectedText3);

        var selectedText4 = $('.selectpicker').find("option:selected").text();
        console.log("Selected Option Text4: " + selectedText4);

    });
   
   
});
