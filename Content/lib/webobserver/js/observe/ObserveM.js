var block_ele = $('#ObserveCard');
var table;

function onLoadingModal() {
    $(block_ele).block({
        message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0.8,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}

function offLoadingModal() {
    $(block_ele).unblock({
    });
}

$(document).ready(function () {
    table = $('.base-style').DataTable();
    initICheck();
    /*
    $.ajax({
        url: 'Api/Observe/GetList',
        type: 'POST',
        success: function (responds) {
            responds.forEach(e => {
                console.log("s");
                var Id = e.Id;
                var Name = e.Name;
                var Status = e.Status;
                var DateCreated = e.DateCreated;
                document.getElementById("Observe-List").insertAdjacentHTML("beforeend", "<tr role='row' class='odd'><td class='sorting_1'><div class='icheckbox_square-red' style='position: relative;'><input type='checkbox' id='input-11' style='position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; background: rgb(255, 255, 255) none repeat scroll 0% 0%; border: 0px none; opacity: 0;'><ins class='iCheck-helper' style='position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; background: rgb(255, 255, 255) none repeat scroll 0% 0%; border: 0px none; opacity: 0;'></ins></div></td><td>System Architect</td><td>Edinburgh</td><td>61</td></tr>");
            })
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            document.getElementById("ObserveName").removeAttribute("disabled");
            document.getElementById("createButton").setAttribute("onclick", "createObserve()");
            document.getElementById("createButton").innerHTML = "Create";
            swal("Error", errorThrown, "error");
        }
    });
    */
});

function initICheck() {
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-red',
        radioClass: 'iradio_square-red',
        increaseArea: '20%' // optional
    });
}

function createObserve() {
    document.getElementById("ObserveName").setAttribute("disabled", "disabled");
    document.getElementById("createButton").removeAttribute("onclick");
    document.getElementById("createButton").innerHTML = "<i><i class='la la-rotate-right icon-spin'></i>";
    var name = document.getElementById("ObserveName").value;

    $.ajax({
        url: '/Api/Observe/Create',
        type: 'POST',
        dataType: 'json',
        data: { 'Name': name },
        success: function (responds) {
            if (responds == 1) {
                document.getElementById("ObserveName").removeAttribute("disabled");
                document.getElementById("createButton").setAttribute("onclick", "createObserve()");
                document.getElementById("createButton").innerHTML = "Create";
                document.getElementById("ObserveName").value = "";
                $('#newModal').modal('hide');
                refreshList();

                swal("Create Successfully", "The new observe has been created.", "success");
            } else {
                document.getElementById("ObserveName").removeAttribute("disabled");
                document.getElementById("createButton").setAttribute("onclick", "createObserve()");
                document.getElementById("createButton").innerHTML = "Create";
                swal("Error", "Request is denied, please try again.", "error");
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            document.getElementById("ObserveName").removeAttribute("disabled");
            document.getElementById("createButton").setAttribute("onclick", "createObserve()");
            document.getElementById("createButton").innerHTML = "Create";
            swal("Error", errorThrown, "error");
        }
    });
}

$('#newModal').on("keypress", function (e) {
    if (e.which == 13) {
        createObserve();
    }
});

function refreshList() {
    $.ajax({
        url: '/Api/Observe/GetList',
        type: 'POST',
        async: false,
        success: function (responds) {
            document.getElementById("Observe-List").innerHTML = "";
            table.clear().destroy();
            responds.forEach(e => {
                var Id = e.Id;
                var Name = e.Name;
                var Status = e.Status;
                if (Status == 1) {
                    Status = "<div class='badge badge-success'>Enabled</div>";
                } else if (Status == 0) {
                    Status = "<div class='badge badge-danger'>Disabled</div>";
                }
                var DateCreated = e.DateCreated;
                var editStringMobile = "editSingleSelectionMobile('" + Id + "')";
                var deleteString = "deleteSingleSelection('" + Id + "')";
                var nameString = "<a href='/Observe/Modify/" + Id + "'>" + Name + "</a>";
                document.getElementById("Observe-List").insertAdjacentHTML("beforeend", "<tr><td><input type='checkbox' id='" + Id + "'></td><td>" + nameString + "</td><td style='width: 10px; text-align:center;'>" + Status + "</td><td style='width: 10px; text-align:center;'>" + DateCreated + "</td><td style='width: 10px;'><div class='btn-group'><button type='button' class='btn btn-danger btn-block dropdown-toggle' data-toggle='dropdown'aria-haspopup='true' aria-expanded='false'>Action</button><div class='dropdown-menu open-left arrow'><button class='dropdown-item' type='button' onclick=" + editStringMobile + ">Edit</button><button class='dropdown-item' type='button' onclick=" + deleteString + ">Delete</button></div></div></td></tr>");
            })
            table = $('.base-style').DataTable();
            initICheck();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            swal("Error", errorThrown, "error");
        }
    });
}

var error = 0;
function deleteQuery() {
    $("input:checked").each(function () {
        $.ajax({
            url: '/Api/Observe/Delete',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'Id': $(this).attr('id') },
            success: function (response) {
                if (response == 1) {
                    swal("Delete Successfully", "The selected observe(s) have been deleted.", "success");
                } else if (response == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error = 1;
            }
        });
    });
}

function deleteSelection() {
    onLoadingModal();
    setTimeout(function () {
        deleteQuery();
        refreshList();
        offLoadingModal();
    }, 500);
}

function deleteSingleSelection(id) {
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Observe/Delete',
            type: 'POST',
            async: false,
            dataType: 'json',
            data: { 'Id': id },
            success: function (responds) {
                deleteQuery();
                refreshList();
                if (responds == 1) {
                    swal("Delete Successfully", "The selected observe(s) have been deleted.", "success");
                } else if (responds == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
                offLoadingModal();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
    }, 500);
}

