var culture = $("#HtmlEditorScript").attr("data-culture");
tinymce.init({
    selector: '.HtmlEditor',
    relative_urls: false,
    paste_as_text: true,
    setup: function (ed) {

        ed.addButton('ExtraHTMLElements', {
            type: 'menubutton',
            text: 'Dodatni HTML elementi',
            tooltip: 'Dodatni HTML elementi - Napomena ovisno o web pregledniku neki elementi neće biti pravilno prikazani',
            icon: false,
            menu: [
                {
                    text: 'Canvas',
                    onclick: function () {
                        var canvasId = prompt("Unesite identifikator za svoj canvas element");
                        if (canvasId != null && canvasId != undefined && canvasId != "") {
                            ed.insertContent('&nbsp; Canvas-> <canvas id="' + canvasId + '" style"width="100" height="50"">Your browser does not support the HTML5 canvas tag.</canvas> &nbsp;');
                            $(".CanvasFile").show();
                        }                       
                    }
                }
            ]
        });
    },
    height: "10em",
    font_formats: tm_fonts,
    fontsize_formats: '8pt 10pt 12pt 14pt 18pt 24pt 36pt',
    language: culture,
    extended_valid_elements: "canvas[id]",
    custom_elements: "canvas",
    menubar: '',
    plugins: ['link image textcolor colorpicker lists paste'],
    toolbar1: 'undo redo | insert paste | styleselect | bold italic |  fontselect |  fontsizeselect | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | forecolor backcolor | ExtraHTMLElements'
});