var counter = 0;
var regex = '';

function GetByRoot(id, thisDiv) {
    var filter = $('#searchMKB').val();
    if (filter == "") {
        if ($("#" + id).hasClass('close')) {
            $.get("Dijagnoza/DijagnozaPV", { root: id }, function (result) {
                $('#' + id).html(result);
                $('#' + id).addClass('open');
                $('#' + id).removeClass('close');
                $(thisDiv).find('h5').css('font-weight', 'bold');
            }).fail(function (result) {
                AjaxOnFailure(result);
            });
        }
        else {
            $('#' + id).addClass('close');
            $('#' + id).removeClass('open');
            $(thisDiv).find('h5').css('font-weight', 'normal');
        }
    }
    else {
        $('#searchMKB').val("");
        var parent = thisDiv.getAttribute('name');
        var node = thisDiv.innerText;
        $.get("Dijagnoza/GetByRootAfterSearch", { root: id }, function (result) {
            $('#MKB-10').html(result);
            document.querySelectorAll('div[name]').forEach(x => {
                if (x.getAttribute('name') != "") {
                    $('#' + x.getAttribute('name')).append(x);
                    $('#' + x.getAttribute('name')).removeClass('close');
                    $('#' + x.getAttribute('name')).addClass('open');
                }
            });
            $('html, body').animate({
                scrollTop: $("#" + id).offset().top - 180
            }, 200);
            if (parent != "") {
                $('h5:contains(' + parent + ')').css('font-weight', 'bold');
            }
            $('h5:contains(' + node + ')').css('font-weight', 'bold');

        }).fail(function (result) {
            AjaxOnFailure(result)
        });
    }
}

var timeout = null;
function doSearch(event) {
    var filter = $('#searchMKB').val().trim().replaceAll("'", "''");
    if (filter != "" && filter.length > 2) {
        clearTimeout(timeout)
        timeout = setTimeout(function () {
            ShowLoader();
            $.get("Dijagnoza/GetBySearch", { filter: filter }, function (result) {
                $('#MKB-10').html(result);
                document.querySelectorAll('div[name]').forEach(x => {
                    if (x.getAttribute('name') != "") {
                        $('#' + x.getAttribute('name')).append(x);
                        $('#' + x.getAttribute('name')).removeClass('close');
                        $('#' + x.getAttribute('name')).addClass('open');
                        $('.level_0').find('h5').css('font-weight', 'bold');
                        $('.level_1').find('h5').css('font-weight', 'bold');
                    }
                });
                $('html, body').animate({
                    scrollTop: $("#MKB-10").offset().top - 150
                }, 200);
                HideLoader();
            }).fail(function (result) {
                AjaxOnFailure(result)
            });
        }, 300)
    }
    else {
        $.get("Dijagnoza/GetByLevel", { level: 0 }, function (result) {
            $('#MKB-10').html(result);
        }).fail(function (result) {
            AjaxOnFailure(result)
        });
    }
}

window.onscroll = function () {
    scrollFunction();
};

function scrollFunction() {
    if (
        document.body.scrollTop > 20 ||
        document.documentElement.scrollTop > 20
    ) {
        $("#btn-back-to-top").css('display', 'block');
    } else {
        $("#btn-back-to-top").css('display', 'none');
    }
}

function backToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

function addTable(idDijagnoza, btn) {
    ShowLoader();
    counter++;
    $.get("Dijagnoza/GetDrugsByDiagnosis", { idDijagnoza: idDijagnoza, counter: counter }, function (result) {
        if (!$("#wrapper").hasClass("menuDisplayed")){
            $("#wrapper").toggleClass("menuDisplayed");
        }
        if (!$("#overlayAll").hasClass('active')) {
            $("#overlayAll").addClass('active');
            $("#overlayAll").css('display', 'block');
            $('body').css('overflow-y', 'hidden');
        }
        else {
            $("#overlayAll").removeClass('active');
            $("#overlayAll").css('display', 'none');
            $('body').css('overflow-y', 'scroll');
        }
        $('#sidebar-wrapper').append(result);
        $(btn).addClass('hidden');
        $(".dataTables_scrollHeadInner").css("width", "100%");
        $(".datatables").css("width", "100%");
        $('#overlayLoader').hide();
        $('.lds-ring').hide();
        //scroll do novog
        setTimeout(function () {
            $('#sidebar-wrapper').animate({
                scrollTop: $("#" + idDijagnoza).offset().top - $('#sidebar-wrapper .sidebar-dijagnoza').first().offset().top
            }, 100);
        }, 350)
    }).fail(function (result) {
        AjaxOnFailure(result);
    });
}

function removeDrugsFromSidebar(idDijagnoza) {
    $('#' + idDijagnoza).remove();
    $('#dodaj_' + idDijagnoza).removeClass('hidden');
}

function initializeLekoviDijagnozeDataTable(data, counter) {
    var idT = '#lekForChosenDataTable' + counter;
    $(idT).DataTable({
        "stateSave": false,
        "destroy": true,
        "processing": true,
        "responsive": true,
        "autoWidth": true,
        "data": data,
        "pagingType": "input",
        "lengthChange": true,
        "lengthMenu": [[5, 10, 20], [5, 10, 20]],
        "order": [[8, "asc"]],
        "pageLength": 5,
        "select": true,
        "columnDefs": [
            { "orderable": true, "searchable": true, "targets": [0, 1, 2, 3] },
            { "orderable": true, "searchable": false, "type": "custom-decimal", "targets": [4, 5, 6] },
            { "orderable": false, "searchable": false, "targets": 7 },
        ],
        "columns": [
            { "data": "Naziv", "sClass": "Naziv" },
            { "data": "NazivProizvodjaca", "sClass": "NazivProizvodjaca" },
            {
                "data": "NaRecept", "sClass": "NaRecept text-center", "render": function (data, type, full) {
                    if (data == true) {
                        return "Da";
                    }
                    else {
                        return "Ne";
                    }
                }
            },
            {
                "data": "ListaRFZO", "sClass": "ListaRFZO text-center", "render": function (data, type, full) {
                    if (data != null) {
                        return data;
                    }
                    else {
                        return "/";
                    }
                }
            },
            {
                "data": "DDD", "sClass": "DDD", "render": function (data, type, full) {
                    if (data != null) {
                        return data;
                    }
                    else {
                        return "/";
                    }
                }
            },
            {
                "data": "UkupnaCena", "sClass": "UkupnaCena text-center", "render": function (data, type, full) {
                    if (data != null) {
                        return data.toFixed(2);
                    }
                    else {
                        return "/";
                    }
                }
            },
            {
                "data": "Doplata", "sClass": "Doplata text-center", "render": function (data, type, full) {
                    if (full['UkupnaCena'] != null && full['ProcenatUcesca'] != null) {
                        full['Doplata'] = ((full['UkupnaCena'] * full['ProcenatUcesca']) / 100).toFixed(2);
                        return full['Doplata'];
                    }
                    else {
                        return "/";
                    }
                }
            },
            {
                "data": "SelectButton", "sClass": "SelectButton text-center", "render": function (val, _, obj) {

                    return RenderPlusMinusButton(obj);
                },
            },
            { "data": "Kolicina", "sClass": "Id", "visible": false },
            { "data": "Id", "sClass": "Id", "visible": false }
        ],
        "drawCallback": function (settings) {
            var inputs = $("input[type = 'number']");
            inputs.each(function () {
                if (!$(this).hasClass('input-spinner')) {
                    $(this).inputSpinner();
                    $(this).addClass('input-spinner');
                }
            });
        },
        "language": { url: '/Content/dataTableResourceSerbian.json' }
    });
};

function RenderPlusMinusButton(obj) {
    return '<input type="number" value="0" min="0" max="100" step="1"/>';
}

$("#menu-toggle, #menu-open").click(function (e) {
    e.preventDefault();
    $("#wrapper").toggleClass("menuDisplayed");
    if (!$("#overlayAll").hasClass('active')) {
        $("#overlayAll").addClass('active');
        $("#overlayAll").css('display', 'block');
        $('body').css('overflow-y', 'hidden');
    }
    else {
        $("#overlayAll").removeClass('active');
        $("#overlayAll").css('display', 'none');
        $('body').css('overflow-y', 'scroll')
    }
});

function onClickSubmitForm(e) {
    if (!$('#myForm').valid()) {
        e.preventDefault();
    }
    else {
        e.preventDefault();
        ShowLoader();
        submitRecept();
    }
}

function submitRecept() {
    var model = {};
    var dict = [];
    var dijagnoze = [];
    $('#sidebar-wrapper').find('div.sidebar-dijagnoza').each(function () {
        dijagnoze.push(this.id);
    });

    dijagnoze.forEach((element, index) => {
        var values = [];
        var id = $('#sidebar-wrapper').find('#' + element).find('table[id^="lekForChosenDataTable"]')[0].id;
        var table = $('#' + id).DataTable();

        table.rows(function (idx, data, node) {
            return data.Kolicina > 0 ? values.push(data) : null;
        });

        if (values.length == 0) {
            values = "";
        }

        dict.push({
            "key": element,
            "value": values
        });
    });

    model.receptDictionary = dict;
    model.ImePrezime = $('#recipient-name').val();
    model.JMBG = $('#jmbg').val();
    model.BrojKnjizice = $('#broj-knjizice').val();
    model.LBO = $('#lbo').val();
    model.Opis = $('#message-text').val();
    model.BrojKartona = $('#broj-kartona').val();
    model.DatumRodjenja = $('#datum-rodjenja').val();

    $.post("Home/SubmitRecept", { receptModel: model }, function (result) {
        if (result.Message == "200") {
            HideLoader();
            Swal.fire({
                title: 'Uspešno ste sačuvali dokument!',
                confirmButtonText: 'Pregledaj',
                icon: 'success'
            }).then((resultSwal) => {
                if (resultSwal.isConfirmed) {
                    window.open('/SavedDocuments/'+ result.IdDocument, '_blank');
                    location.reload();
                }
            })
        }
    }).fail(function (result) {
        AjaxOnFailure(result);
    });
}

function inputSpinnerEvent(element) {
    var valueOfCounter = $(element).parent().parent().find('input').val();
    var tableId = $(element).closest('table')[0].id;
    $('#' + tableId).DataTable().rows($(element).parent().parent().parent().parent()).data()[0].Kolicina = valueOfCounter;
}

function changeValue(element) {
    var valueOfCounter = $(element).val();
    var tableId = $(element).closest('table')[0].id;
    if (valueOfCounter > 0) {
        $('#' + tableId).DataTable().rows($(element).parent().parent().parent().parent()).data()[0].Kolicina = valueOfCounter;
    }
    else {
        $('#' + tableId).DataTable().rows($(element).parent().parent().parent().parent()).data()[0].Kolicina = 0;
    }
}

function openCreateModal() {
    $.get("Dijagnoza/CreateDijagnoza", function (result) {
        $('#createDijagnozaModel').modal("show");
        $('#createDijagnozaModel').find('.modal-body').html(result);
    }).fail(function (result) {
        $('#createDijagnozaModel').modal("hide");
        AjaxOnFailure(result);
    });
}

$(document).on('change', '#SelectedCategory', function (e) {
    var category = $('#SelectedCategory').val();
    if (category != '') {
        $.get("Dijagnoza/GetByRoot", { root: category }, function (result) {
            $('#SelectedSubCategory').html('');
            result.forEach(x => {
                $('#SelectedSubCategory').append($('<option>', {
                    value: x.Id_dijagnoza,
                    text: x.Id_dijagnoza + " " + x.NazivSrpski
                }));
            });
            $('#SelectedSubCategory').prop('disabled', false);
            $('#SelectedIdCategory').prop('disabled', false);
            var subCategory = $('#SelectedSubCategory').val().charAt(0);
            $('.prefixCategoryId').val(subCategory);
            $('.prefixCategoryId').text(subCategory);
            $('#SelectedSubCategory').trigger('input');
        }).fail(function (result) {
            $('#createDijagnozaModel').modal("hide");
            AjaxOnFailure(result);
        });
    }
    else {
        $('#SelectedIdCategory').prop('disabled', true);
        $('#SelectedIdCategory').val('');
        $('#SelectedSubCategory').prop('disabled', true);
        $('#SelectedSubCategory').html('');
        $('.prefixCategoryId').val('');
        $('.prefixCategoryId').text('');
        $('#SelectedSubCategory').append($('<option>', {
            text: 'Izaberite podkategoriju dijagnoze'
        }));
        regex = '';
    }
});

$(document).on('input', '#SelectedSubCategory', function (e) {
    var subCategory = $('#SelectedSubCategory').val().charAt(0);
    $('.prefixCategoryId').val(subCategory);
    $('.prefixCategoryId').text(subCategory);
    $('#SelectedIdCategory').prop('disabled', false);

    var first = '';
    var second = '';
    //1.slucaj A00-A09
    //2.slucaj A15 - A31
    if ($('#SelectedSubCategory').val().charAt(1) == $('#SelectedSubCategory').val().charAt(5)) {
        first = $('#SelectedSubCategory').val().charAt(1);
        second = '[' + $('#SelectedSubCategory').val().charAt(2) + '-' + $('#SelectedSubCategory').val().charAt(6) + ']';
        regex = new RegExp('^' + first + second + '([0-9])?([0-9])?$');
    }
    else {
        var tempStart = $('#SelectedSubCategory').val().charAt(1);
        var tempEnd = $('#SelectedSubCategory').val().charAt(5);
        first = $('#SelectedSubCategory').val().charAt(2);
        regex = '^((' + tempStart + '[' + first + '-9])';

        for (var i = parseInt(tempStart) + 1; i < parseInt(tempEnd); i++) {
            regex += '|(' + i + '[0-9])';
        }
        second = $('#SelectedSubCategory').val().charAt(6);

        regex += '|(' + tempEnd + '[0-' + second + ']))([0-9])?([0-9])?$'
        regex = new RegExp(regex);
    }
});

function regexValidation() {
    var value = $('#SelectedIdCategory').val();
    if (regex.test(value)) {
        $('.idValidation').text('');
    }
    else {
        var start = $('#SelectedSubCategory').val().substring(3, 1) + '[00]';
        var end = $('#SelectedSubCategory').val().substring(7, 5) + '[99]';
        $('.idValidation').text('Ovo polje mora biti u opsegu od ' + start + ' do ' + end + '.');
    }
}

function createDijagnoza(e) {
    if (!$('#form-create-dijagnoza').valid() || $('.idValidation').text() != "") {
        e.preventDefault();
        $('#divIdCategory').append($('#SelectedIdCategory-error'));
    }
    else {
        $('#form-create-dijagnoza').submit();
        ShowLoader();
    }
}

function AjaxOnSuccess(result) {
    if (result.Message == '200') {
        HideLoader();
        Swal.fire({
            title: 'Uspešno ste sačuvali dijagnozu!',
            confirmButtonText: 'OK',
            icon: 'success'
        }).then((result) => {
            if (result.isConfirmed) {
                location.reload();
            }
        });
    }
    else {
        HideLoader();
        Swal.fire({
            title: result.Messa,
            confirmButtonText: 'OK',
            icon: 'info'
        });
    }
}

function AjaxOnFailure(result) {
    HideLoader();
    Swal.fire({
        title: 'Doslo je do greške! \n' + result.Message,
        confirmButtonText: 'OK',
        icon: 'error'
    }).then((result) => {
        if (result.isConfirmed) {
            location.reload();
        }
    });
}

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "custom-decimal-pre": function (a) {
        if (a != '/') {
            return parseFloat(a);
        }
        else {
            return 0;
        }
    },

    "custom-decimal-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "custom-decimal-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

function ShowLoader() {
    $('#overlayLoader').css('display','grid');
    $('.lds-ring').show();
    $('body').css('overflow-y', 'hidden')
}

function HideLoader() {
    $('#overlayLoader').hide();
    $('.lds-ring').hide();
    $('body').css('overflow-y', 'scroll')
}