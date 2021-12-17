function initializeDataTable(data) {
    var lekoviTable = $('#lekDataTable').DataTable({
        "stateSave": true,
        "destroy": true,
        "processing": true,
        "scrollY": "300px",
        "scrollCollapse": true,
        "scrollX": true,
        //"scrollY":true,
        "autoWidth": true,
        "responsive": true,
        "data": data,
        "pagingType": "input",
        "lengthChange": true,
        "lengthMenu": [[8, 15, 30, 50], [8, 15, 30, 50]],
        "order": [[0, "asc"]],
        "pageLength": 8,
        "columnDefs": [
            { "width": 100, "orderable": true, "searchable": false, "targets": 2 },
            { "width": 100, "orderable": true, "searchable": false, "type": "custom-decimal", "targets": 3 },
            { "width": 150, "orderable": true, "searchable": false, "targets": 4 },
        ],
        "columns": [
            { "data": "Naziv", "sClass": "Naziv" },
            { "data": "NazivProizvodjaca", "sClass": "NazivProizvodjaca" },
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
                "data": "NaRecept", "sClass": "NaRecept text-center", "render": function (data, type, full) {
                    if (data == true) {
                        return "Da";
                    }
                    else {
                        return "Ne";
                    }
                }
            },
            { "data": "Id", "sClass": "Id", "visible": false }
        ],
        "language": { url: '/Content/dataTableResourceSerbian.json' }
    });
}

function openCreateModal() {
    $.get("Lekovi/CreateLek", function (result) {
        $('#createLekModel').modal("show");
        $('#createLekModel').find('.modal-body').html(result);
        $('[data-toggle="tooltip"]').tooltip();
        $('#SelectedProizvodjac').select2({
            theme: "bootstrap"
        });
        $('#mySelect2').select2({
            placeholder: "Pretražite djagnoze ili unesite opseg vrednosti (npr: I2[0-9]% ili I2[0245]%)",
            minimumInputLength: 3,
            maximumInputLength: 20,
            theme: "bootstrap",
            tags: true,
            ajax: {
                url: 'Dijagnoza/GetBySearchJson',
                data: function (params) {
                    if (params.term != undefined && params.term.length > 2) {
                        var query = {
                            filter: params.term
                        }
                        return query;
                    }
                },
                processResults: function (data) {
                    return {
                        results: data
                    };
                }
            }
        })
    }).fail(function (result) {
        $('#createLekModel').modal("hide");
        AjaxOnFailure(result);
    });
}

function createLek(e) {
    if (!$('#form-create-lek').valid()) {
        e.preventDefault();
    }
    else {
        $('#form-create-lek').submit();
        ShowLoader();
    }
}

function AjaxOnSuccessLek(result) {
    if (result.Message == '200') {
        HideLoader();
        Swal.fire({
            title: 'Uspešno ste sačuvali lek!',
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
            title: result.Message,
            confirmButtonText: 'OK',
            icon: 'info'
        });
    }
}

function AjaxOnFailureLek(result) {
    HideLoader();
    Swal.fire({
        title: 'Došlo je do greške! \n' + result.Message,
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
        if (a != '/')
            return parseFloat(a);
        else
            return -1;
    },

    "custom-decimal-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "custom-decimal-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

function ShowLoader() {
    $('#overlayLoader').css('display', 'grid');
    $('.lds-ring').show();
    $('body').css('overflow-y', 'hidden')
}

function HideLoader() {
    $('#overlayLoader').hide();
    $('.lds-ring').hide();
    $('body').css('overflow-y', 'scroll')
}

$(document).on('change', '#NaRecept', function () {
    if ($('#NaRecept').is(':checked')) {
        $('#ProcenatUcesca').attr('disabled', false)
    }
    else {
        $('#ProcenatUcesca').attr('disabled', true)
    }
})