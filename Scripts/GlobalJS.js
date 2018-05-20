$(function () {
    // Ideja da jednom kada se učitaju ako im je vrijednost true onda da se postavi da su checkboxovi označeni
    //$('input[type="checkbox"]').ready(function () {
    //    if ($(this).val().toLowerCase() == "true") {
    //        $(this).prop('checked', true);
    //    }      
    //});

    $('input[type="checkbox"]').change(function () {
        $(this).val($(this).is(':checked'));
    });

});

function AjaxError(ajaxContext, status) {
    alert('AjaxError');
}