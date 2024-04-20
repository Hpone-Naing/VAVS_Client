$(document).ready(function () {

    var alertBox = $(".alert-msg");
    if (alertBox.length > 0) {
        setTimeout(function () {
            alertBox.fadeOut(500);
        }, 3000);
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var inputs = document.querySelectorAll(".phone");
    console.log(inputs);
    inputs.forEach(function (input) {
        if (input != null) {
            var iti = window.intlTelInput(input, {
                initialCountry: "mm",
                separateDialCode: true,
                utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.15/js/utils.js",
            });
        }
    });
});


var xmlHttp;
function srvTime() {
    try {
        //FF, Opera, Safari, Chrome
        xmlHttp = new XMLHttpRequest();
    }
    catch (err1) {
        //IE
        try {
            xmlHttp = new ActiveXObject('Msxml2.XMLHTTP');
        }
        catch (err2) {
            try {
                xmlHttp = new ActiveXObject('Microsoft.XMLHTTP');
            }
            catch (eerr3) {
                console.log("AJAX not supported");
            }
        }
    }
    xmlHttp.open('HEAD', window.location.href.toString(), false);
    xmlHttp.setRequestHeader("Content-Type", "text/html");
    xmlHttp.send('');
    return xmlHttp.getResponseHeader("Date");
}