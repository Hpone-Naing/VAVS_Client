function FilterStateDivisionByTownship(lstStateDivisionCtrl, lstTownshipId) {

    var lstTownships = $("#" + lstTownshipId);
    lstTownships.empty();

    var selectedStateDivision = lstStateDivisionCtrl.options[lstStateDivisionCtrl.selectedIndex].value;

    if (selectedStateDivision != null && selectedStateDivision != '') {
        $.getJSON("/Login/GetTownshipByStateDivision", { stateDivisionPkId: selectedStateDivision }, function (townships) {
            if (townships != null && !jQuery.isEmptyObject(townships)) {
                $.each(townships, function (index, township) {
                    lstTownships.append($('<option/>',
                        {
                            value: township.value,
                            text: township.text
                        }));
                });
            };
        });
    }

    return;
}