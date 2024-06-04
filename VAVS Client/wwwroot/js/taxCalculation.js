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

document.addEventListener("DOMContentLoaded", function () {
    console.log("here address class..................")
    var vehicleNumberCells = document.querySelectorAll(".address");
    var shortCells = document.querySelectorAll(".shortCell");
    vehicleNumberCells.forEach(function (cell) {
        var vehicleNumber = cell.textContent.trim();
        if (vehicleNumber.length > 15) {
            var splitLines = [];
            while (vehicleNumber.length > 15) {
                splitLines.push(vehicleNumber.substring(0, 15));
                vehicleNumber = vehicleNumber.substring(15);
            }
            splitLines.push(vehicleNumber);
            cell.innerHTML = splitLines.join("<br>");
        }
    });

    shortCells.forEach(function (cell) {
        var shortCell = cell.textContent.trim();
        if (shortCell.length > 9) {
            var splitLines = [];
            while (shortCell.length > 9) {
                splitLines.push(shortCell.substring(0, 9));
                shortCell = shortCell.substring(9);
            }
            splitLines.push(shortCell);
            cell.innerHTML = splitLines.join("<br>");
        }
    });
});