
$(document).ready(function () {

    var userTable = $("#userTable").DataTable({
        "destroy": true,
        "processing": false, 
        "ajax": {
            "url": "/Home/GetUser",
            "dataSrc": "",
            "contentType": "application/json; charset=utf-8",
            "dataType": "json",
            "type": "POST",
        },
        "columnDefs": [
            {
                "targets": 0,
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).attr('id', rowData.UserID);
                },
            },
            {
                "targets": 1,
                "createdCell": function (td, cellData, rowData, row, col)
                {
                $(td).attr('id', rowData.UserID);
                    $(td).attr('class', 'editable text');
                },
            },
            {
                "targets": 2,
                "createdCell": function (td, cellData, rowData, row, col)
                {
                    $(td).attr('id', rowData.UserID);
                    $(td).attr('class', 'editable text');
                },
            },
            {
                "targets": 3,
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).attr('id', rowData.UserID);
                },
            },
        ]
        ,
        "columns": [
            { "data": "UserID", "name": "UserID" },
            { "data": "UserName", "name": "UserName" },
            { "data": "UserNo", "name": "UserNo" },
            { "data": "CreateDate", "name": "CreateDate" },
            {
                //edit button creation    
                render: function (data, type, row) {
                    return createButton('edit', row.id);
                }
            }, 
            {
                //delete button creation    
                render: function (data, type, row) {
                    return createButton('delete', row.id);
                }
            } 
        ],
        "initComplete": function (settings, json) {
            //test();
        },
        "drawCallback": function (settings) {
            //test();
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
            $("td:first", nRow).html(iDisplayIndex + 1);
            return nRow;
        },
        "fnCreatedRow": function (nRow, aData, iDataIndex) {
            $(nRow).attr('id', aData.UserID);
        }


    });




});

function CreateUser() {
   

    const UserName = document.getElementById("UserName").value;
    const UserNo = document.getElementById("UserNo").value;
 

    var scode = $.ajax({
        url: "/Home/CreateUser",
        dataType: "json",
        type: "POST",
        data: {
            "UserName": UserName,
            "UserNo": UserNo,
        },
        success: function (data) {

            if (!$.trim(data)) {
                toastr.error("Error");
            } else {
                if (data.code == '200') {

                    alert("sucess");

                    $('#userTable').DataTable().ajax.reload();
                } else if (data.code == '300') {
                    alert("failed");
                }

            }


        },
        error: function (exception) {
            alert('Exception:', exception);
        }
    });

}

function createButton(buttonType, rowID) {
    var buttonText = buttonType == "edit" ? "Edit" : "Delete";
    return '<button class="' + buttonType + '" type="button">' + buttonText + '</button>';
}    

$('#userTable').on('click', 'tbody td .edit', function (e) {
    fnResetControls();
    var dataTable = $('#userTable').DataTable();
    var clickedRow = $($(this).closest('td')).closest('tr');
    $(clickedRow).find('td').each(function () {
        // do your cool stuff    
        if ($(this).hasClass('editable')) {
            if ($(this).hasClass('text')) {
                var html = fnCreateTextBox($(this).html(), 'name');
                $(this).html($(html))
            }
        }
    });


    $('#userTable tbody tr td .update').removeClass('update').addClass('edit').html('Edit');
    $('#userTable tbody tr td .cancel').removeClass('cancel').addClass('delete').html('Delete');
    $(clickedRow).find('td .edit').removeClass('edit').addClass('update').html('Update');
    $(clickedRow).find('td .delete').removeClass('delete').addClass('cancel').html('Cancel');

});

function fnCreateTextBox(value, fieldprop) {
    return '<input data-field="' + fieldprop + '" type="text" value="' + value + '" ></input>';
}    

$('#userTable').on('click', 'tbody td .cancel', function (e) {
    fnResetControls();
    $('#userTable tbody tr td .update').removeClass('update').addClass('edit').html('Edit');
    $('#userTable tbody tr td .cancel').removeClass('cancel').addClass('delete').html('Delete');
});


function fnResetControls() {
    var openedTextBox = $('#userTable').find('input');
    $.each(openedTextBox, function (k, $cell) {
        $(openedTextBox[k]).closest('td').html($cell.value);
    })
}    

$('#userTable').on('click', 'tbody td .update', function (e) {
    var trid = $(this).closest('tr').attr('id'); // table row ID 

    var inputobject = [];
    var openedTextBox = $('#userTable').find('input');
    $.each(openedTextBox, function (k, $cell) {
        fnUpdateDataTableValue($cell, $cell.value);
        $(openedTextBox[k]).closest('td').html($cell.value);
        inputobject.push($cell.value);
    })

    console.log("asfas:" + trid);
    
    var detailsobj = new Object();
    detailsobj.UserID = trid;
    detailsobj.UserName = inputobject[0];
    detailsobj.UserNo = inputobject[1];


    $('#userTable tbody tr td .update').removeClass('update').addClass('edit').html('Edit');
    $('#userTable tbody tr td .cancel').removeClass('cancel').addClass('delete').html('Delete');
    

    var scode = $.ajax({
        url: "/Home/UpdateUser",
        dataType: "json",
        type: "POST",
        data: {
            "UserID": detailsobj.UserID,
            "UserName": detailsobj.UserName,
            "UserNo": detailsobj.UserNo
        },
        success: function (data) {

            if (!$.trim(data)) {
                toastr.error("Error");
            } else {
                if (data.code == '200') {

                    alert("sucess");

                    $('#userTable').DataTable().ajax.reload();
                } else if (data.code == '300') {
                    alert("failed");
                }

            }


        },
        error: function (exception) {
            alert('Exception:', exception);
        }
    });
});


$('#userTable').on('click', 'tbody td .delete', function (e) {
    var trid = $(this).closest('tr').attr('id'); // table row ID 

    var inputobject = [];
    var openedTextBox = $('#userTable').find('input');
    $.each(openedTextBox, function (k, $cell) {
        fnUpdateDataTableValue($cell, $cell.value);
        $(openedTextBox[k]).closest('td').html($cell.value);
        inputobject.push($cell.value);
    })

    var detailsobj = new Object();
    detailsobj.UserID = trid;


    var scode = $.ajax({
        url: "/Home/DeleteUser",
        dataType: "json",
        type: "POST",
        data: {
            "UserID": detailsobj.UserID,
        },
        success: function (data) {

            if (!$.trim(data)) {
                toastr.error("Error");
            } else {
                if (data.code == '200') {

                    alert("sucess");

                    $('#userTable').DataTable().ajax.reload();
                } else if (data.code == '300') {
                    alert("failed");
                }

            }


        },
        error: function (exception) {
            alert('Exception:', exception);
        }
    });
});

function fnUpdateDataTableValue($inputCell, value) {
    var dataTable = $('#userTable').DataTable();
    var rowIndex = dataTable.row($($inputCell).closest('tr')).index();
    var fieldName = $($inputCell).attr('data-field');
    dataTable.rows().data()[rowIndex][fieldName] = value;
}   