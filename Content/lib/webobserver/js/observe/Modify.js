
var block_ele = $('#ObserveCard');
var block_ele2 = $('#createStepModal');
var block_ele3 = $('#editStepModal');
var block_ele4 = $('#addRecipientStepModal');
var block_ele5 = $('#configureCatchModal');
var table;
var table2;

$(document).ready(function () {
    document.getElementById('IntervalOption').value = intervalPreselect;

    table = $('.dataex-rowre-event').DataTable({
        rowReorder: {
            selector: 'td:nth-child(2)'
        },
        "columns": [
            { "width": "10%" },
            { "width": "70%" },
            { "width": "5%" },
            { "width": "5%" },
            { "width": "10%" }
        ],
        responsive: true
    });

    table2 = $('.Recipient-table').DataTable({
    });

    table.on('row-reorder', function (e, diff, edit) {
        onLoadingModal();
        setTimeout(function () {
            var changeRow = edit.triggerRow.data()[0];
            var minRow = 0;
            var maxRow = 0;
            for (var i = 0, ien = diff.length; i < ien; i++) {
                if (i == 0) {
                    minRow = diff[i].newData;
                }
                if (i == diff.length - 1) {
                    maxRow = diff[i].newData;
                }
            }
            if (minRow != 0 && maxRow != 0) {
                $.ajax({
                    url: '/Api/Step/ReorderStep',
                    type: 'POST',
                    dataType: 'json',
                    async: false,
                    data: { 'ObserveId': observeId, 'initOrder': changeRow, 'minOrder': minRow, 'maxOrder': maxRow },
                    success: function (response) {
                        if (response == 2) {
                            swal("Rejected", "Unauthorized access detected.", "error");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        swal("Error", errorThrown, "error");
                    }
                });
            }
            offLoadingModal();
        }, 500);
    });

    reinitializeCreationModal();

    setInterval(refreshList(), 1000);
});

function initICheck() {
    $('.Recipient-Checkbox').iCheck({
        checkboxClass: 'icheckbox_square-red',
        radioClass: 'iradio_square-red',
        increaseArea: '20%' // optional
    });
}

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

function onLoadingCreateModal() {
    $(block_ele2).block({
        message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}

function offLoadingCreateModal() {
    $(block_ele2).unblock({
    });
}

function offLoadingEditModal() {
    $(block_ele3).unblock({
    });
}

function onLoadingEditModal() {
    $(block_ele3).block({
        message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}

function offLoadingRecipientModal() {
    $(block_ele4).unblock({
    });
}

function onLoadingRecipientModal() {
    $(block_ele4).block({
        message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}

function offLoadingCatchModal() {
    $(block_ele5).unblock({
    });
}

function onLoadingCatchModal() {
    $(block_ele5).block({
        message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}


function refreshList() {
    $.ajax({
        url: '/Api/Step/GetList',
        type: 'POST',
        async: false,
        dataType: 'json',
        data: { 'ObserveId': observeId },
        success: function (responds) {
            table.clear().destroy();
            document.getElementById("Step-List").innerHTML = "";
            responds.forEach(e => {
                var StepId = e.Id;
                var Priority = e.Priority;
                var Url = e.Url;
                if (e.Method == 'GET') {
                    var Method = "<div class='badge badge-info'>GET</div>";
                } else {
                    var Method = "<div class='badge badge-primary'>POST</div>";
                }

                if (e.SetHeader == 'Yes') {
                    var SetHeader = "<div class='badge badge-success'>Yes</div>";
                } else if (e.SetHeader == 'No') {
                    var SetHeader = "<div class='badge badge-danger'>No</div>";
                } else if (e.SetHeader == 'Pre') {
                    var SetHeader = "<div class='badge badge-warning'>Pre</div>";
                }
                document.getElementById("Step-List").insertAdjacentHTML("beforeend", "<tr><td style='text-align:center;'>" + Priority + "</td><td>" + Url + "</td><td style='width:20px; text-align: center;'>" + Method + "</td><td style='width:20px; text-align: center;'>" + SetHeader + "</td><td style='width:20px;'><div class='btn-group'><button type='button' class='btn btn-danger btn-block dropdown-toggle' data-toggle='dropdown'aria-haspopup='true' aria-expanded='false'>Action</button><div class='dropdown-menu open-left arrow'><button class='dropdown-item' type='button' onclick=runStep('" + StepId + "')>Run</button><button class='dropdown-item' type='button' onclick=editStep('" + StepId + "')>Edit</button><button class='dropdown-item' type='button' onclick=setCatch('"+StepId+"')>Set Catch</button><button class='dropdown-item' type='button' onclick=deleteStep('" + StepId + "')>Delete</button></div></div></td></tr>");
            })
            table = $('.dataex-rowre-event').DataTable({
                rowReorder: {
                    selector: 'td:nth-child(2)'
                },
                "columns": [
                    { "width": "10%" },
                    { "width": "70%" },
                    { "width": "5%" },
                    { "width": "5%" },
                    { "width": "10%" }
                ],
                responsive: true
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            swal("Error", errorThrown, "error");
        }
    });
}

function refreshRecipientList() {
    $.ajax({
        url: '/Api/Observe/GetRecipientList',
        type: 'POST',
        dataType: 'json',
        async: false,
        data: { 'ObserveId': observeId },
        success: function (data) {
            if (data.Status == "Unauthorized") {
                swal("Rejected", "Unauthorized access detected.", "error");
            } else {
                document.getElementById("Recipient-List").innerHTML = "";
                table2.clear().destroy();
                data.forEach(e => {
                    var RecipientId = e.RecipientId;
                    var Email = e.Email;
                    document.getElementById("Recipient-List").insertAdjacentHTML("beforeend", "<tr><td style='width:5%;'><input class='Recipient-Checkbox' type='checkbox' id='" + RecipientId + "'></td><td>" + Email + "</td><td style='width:5%;'><div class='btn-group'><button type='button' class='btn btn-danger btn-block dropdown-toggle' data-toggle='dropdown'aria-haspopup='true' aria-expanded='false'>Action</button><div class='dropdown-menu open-left arrow'><button class='dropdown-item' type='button' onclick=confirmDeleteRecipient('" + RecipientId + "')>Delete</button></div></div></td></tr>");
                });
                table2 = $('.Recipient-table').DataTable({
                });
                initICheck();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            swal("Error", errorThrown, "error");
        }
    });
}

function switchEnable() {
    onLoadingModal();
    setTimeout(function () {
        if ($("#observeSwitch").is(":checked")) {
            $.ajax({
                url: '/Api/Observe/SetStatus',
                type: 'POST',
                dataType: 'json',
                async: false,
                data: { 'Id': observeId, 'Status': 1 },
                success: function (response) {
                    if (response == 2) {
                        swal("Rejected", "Unauthorized access detected.", "error");
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    swal("Error", errorThrown, "error");
                }
            });
        } else {
            $.ajax({
                url: '/Api/Observe/SetStatus',
                type: 'POST',
                dataType: 'json',
                async: false,
                data: { 'Id': observeId, 'Status': 0 },
                success: function () {
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    swal("Error", errorThrown, "error");
                }
            });
        }
        offLoadingModal();
    }, 500);
}

function changeName() {
    onLoadingModal();
    var Name = document.getElementById('ObserveNewName').value;
    setTimeout(function () {
        $.ajax({
            url: '/Api/Observe/SetName',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'Id': observeId, 'Name': Name },
            success: function (response) {
                if (response == 1) {
                    document.getElementById('NameHeader').innerHTML = Name;
                    document.getElementById('NameBreadCrumb').innerHTML = Name;
                    swal("Rename Successfully", "The selected observe(s) has been renamed.", "success");
                } else if (response == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function createNewStep() {
    onLoadingModal();

    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/GetStepCount',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'ObserveId': observeId },
            success: function (response) {
                if (response == 0) {
                    document.getElementById("PriorityOption").innerHTML = "<option value='0' selected='' disabled=''>- Select -</option>";
                    document.getElementById("PriorityOption").insertAdjacentHTML("beforeend", "<option value='1'>First</option>");
                } else {
                    document.getElementById("PriorityOption").innerHTML = "<option value='0' selected='' disabled=''>- Select -</option>";
                    document.getElementById("PriorityOption").insertAdjacentHTML("beforeend", "<option value='1'>First</option>");
                    document.getElementById("PriorityOption").insertAdjacentHTML("beforeend", "<option value='2'>Last</option>");
                }
                $('#createStepModal').modal('show');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function submitNewStep() {
    onLoadingCreateModal();
    var inputUrl = document.getElementById('UrlOption').value;
    var inputMethod = document.getElementById('MethodOption').value;
    var inputPriority = document.getElementById('PriorityOption').value;
    var inputMethodContent = document.getElementById('MethodContent').value;
    var inputPC1 = null;
    var inputPC2 = null;
    if (inputMethod == 2) {
        if (document.getElementById('numPC').value == "1") {
            inputPC1 = document.getElementById('PC1').value;
        } else if (document.getElementById('numPC').value == "2") {
            inputPC1 = document.getElementById('PC1').value;
            inputPC2 = document.getElementById('PC2').value;
        }
    }
    var inputCustomHeader = null;
    if ($("#UseHeaderOption").is(":checked")) {
        if ($("#PredefineOption").is(":checked")) {
            inputCustomHeader = document.getElementById('CustomHeaderOption').value;
            var inputHeader = 2;
        } else {
            var inputHeader = 1;
        }
    } else {
        var inputHeader = 0;
    }
    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/CreateNewStep',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'ObserveId': observeId, 'Url': inputUrl, 'Method': inputMethod, 'Header': inputHeader, 'Priority': inputPriority, 'PredefinedHeader': inputCustomHeader, 'ContentMethod': inputMethodContent, 'PC1': inputPC1, 'PC2': inputPC2 },
            success: function (response) {
                if (response == 1) {
                    refreshList();
                    offLoadingCreateModal();
                    swal("Error", "Url not defined, please try again.", "error");
                } else if (response == 2) {
                    refreshList();
                    offLoadingCreateModal();
                    swal("Error", "Method not set, please try again.", "error");
                } else if (response == 3) {
                    refreshList();
                    offLoadingCreateModal();
                    swal("Error", "Priority not set, please try again.", "error");
                } else if (response == 4) {
                    refreshList();
                    offLoadingCreateModal();
                    reinitializeCreationModal();
                    $('#createStepModal').modal('hide');
                    swal("Successful Creation", "The new step has been created successfully.", "success");
                } else if (response == 5) {
                    refreshList();
                    offLoadingCreateModal();
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
    }, 500);
}

function resubmitStep() {
    onLoadingEditModal();
    var originalStepId = document.getElementById('editStepInputId').value;
    var inputUrl = document.getElementById('editUrlOption').value;
    var inputMethod = document.getElementById('editMethodOption').value;
    var inputCustomHeader = null;
    var inputPCMethod = document.getElementById('editMethodContent').value;
    var inputPC1 = null;
    var inputPC2 = null;

    if (inputMethod == 2) {
        if (document.getElementById('numEditPC').value == "1") {
            inputPC1 = document.getElementById('editPC1').value;
        } else if (document.getElementById('numEditPC').value == "2") {
            inputPC1 = document.getElementById('editPC1').value;
            inputPC2 = document.getElementById('editPC2').value;
        }
    }

    if ($("#editUseHeaderOption").is(":checked")) {
        if ($("#editPredefineOption").is(":checked")) {
            inputCustomHeader = document.getElementById('editCustomHeaderOption').value;
            var inputHeader = 2;
        } else {
            var inputHeader = 1;
        }
    } else {
        var inputHeader = 0;
    }
    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/ResubmitStep',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'StepId': originalStepId, 'Url': inputUrl, 'Method': inputMethod, 'Header': inputHeader, 'PredefinedHeader': inputCustomHeader, 'PC_Method': inputPCMethod, 'PC1': inputPC1, 'PC2': inputPC2 },
            success: function (response) {
                if (response == 1) {
                    refreshList();
                    swal("Error", "Url not defined, please try again.", "error");
                } else if (response == 2) {
                    refreshList();
                    swal("Error", "Method not set, please try again.", "error");
                } else if (response == 3) {
                    refreshList();
                    swal("Error", "Priority not set, please try again.", "error");
                } else if (response == 4) {
                    refreshList();
                    $('#editStepModal').modal('hide');
                    swal("Successful Update", "The step has been updated.", "success");
                } else if (response == 5) {
                    refreshList();
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingEditModal();
    }, 500);
}

function editStep(stepId) {

    //Reinitialize
    onLoadingModal();
    document.getElementById('editUrlOption').value = null;
    document.getElementById('editStepInputId').value = null;
    document.getElementById('editMethodOption').value = 0;
    document.getElementById('editMethodContent').value = 0;
    $('#editUseHeaderOption').prop('checked', false);
    $('#editPredefineOption').prop('checked', false);
    document.getElementById('editHeaderUsage-Section').setAttribute('style', 'display:none;');
    document.getElementById('editCustomHeader-Section').setAttribute('style', 'display:none;');
    document.getElementById('editCustomHeaderOption').value = null;
    document.getElementById('editPostContent-Section').innerHTML = "";

    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/GetStepDetail',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'StepId': stepId },
            success: function (response) {
                if (response.Status == "Authorized") {
                    var editUrl = response.Url;
                    var editStepId = response.StepId;
                    var editMethod = response.Method;
                    var editCustomHeader = response.PredefinedHeader;
                    var editHeader = response.Header;
                    var editPCMethod = response.PC_Method;
                    var editPC1 = response.PC1;
                    var editPC2 = response.PC2;
                    document.getElementById('editUrlOption').value = editUrl;
                    document.getElementById('editStepInputId').value = editStepId;
                    document.getElementById('editMethodOption').value = editMethod;
                    document.getElementById('editPostContentMethod-Section').setAttribute('style', 'display:none;')
                    if (editHeader == 1) {
                        $('#editUseHeaderOption').prop('checked', true);
                        document.getElementById('editHeaderUsage-Section').setAttribute('style', 'display:block;');
                    } else if (editHeader == 2) {
                        $('#editUseHeaderOption').prop('checked', true);
                        $('#editPredefineOption').prop('checked', true);
                        document.getElementById('editHeaderUsage-Section').setAttribute('style', 'display:block;');
                        document.getElementById('editCustomHeader-Section').setAttribute('style', 'display:block;');
                        document.getElementById('editCustomHeaderOption').value = editCustomHeader;
                    }
                    if (editMethod == 2) {
                        document.getElementById('editPostContentMethod-Section').setAttribute('style', 'display:block;')
                        if (editPCMethod == 1) {
                            document.getElementById('numEditPC').value = 2;
                            document.getElementById('editMethodContent').value = editPCMethod;
                            document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='editPC1'>email</label><input type='text' id='editPC1' class='form-control' name='editPC1' value=" + editPC1 + "></div><div class='form-group'><label for='editPC2'>password</label><input type='password' id='editPC2' class='form-control' name='editPC2' value=" + editPC2 + "></div>");
                        } else if (editPCMethod >= 3) {
                            document.getElementById('editMethodContent').value = editPCMethod;
                            if (response.numPC == 1) {
                                document.getElementById('numEditPC').value = 1;
                                document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='editPC1'>" + response.PC1Label + "</label><input type='text' id='editPC1' class='form-control' name='editPC1' value=" + editPC1 + "></div>");
                            } else if (response.numPC == 2) {
                                document.getElementById('numEditPC').value = 2;
                                document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='editPC1'>" + response.PC1Label + "</label><input type='text' id='editPC1' class='form-control' name='editPC1' value=" + editPC1 + "></div><div class='form-group'><label for='editPC2'>" + response.PC2Label + "</label><input type='password' id='editPC2' class='form-control' name='editPC2' value=" + editPC2 + "></div>");
                            } else {
                                document.getElementById('numEditPC').value = 0;
                            }
                        }
                        document.getElementById('editPostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
                        if (editPC1 == "null") {
                            document.getElementById('editPC1').value = null;
                        }
                        if (editPC2 == "null") {
                            document.getElementById('editPC2').value = null;
                        }
                    }
                } else if (response.Status == "Unauthorized") {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
                $('#editStepModal').modal('show');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function deleteStep(stepId) {
    onLoadingModal();

    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/DeleteStep',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'StepId': stepId },
            success: function (response) {
                if (response == 1) {
                    refreshList();
                    swal("Successful Deletion", "The selected step has been deleted.", "success");
                } else if (response == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function confirmDeleteRecipient(id) {
    swal({
        title: "Delete Confirmation",
        text: "Are you sure you want to delete the recipient (including all the selection)?",
        icon: "warning",
        buttons: {
            cancel: {
                text: "Cancel",
                value: null,
                visible: true,
                className: "",
                closeModal: true,
            },
            confirm: {
                text: "Yes",
                value: true,
                visible: true,
                className: "",
                closeModal: true
            }
        }
    }).then(isConfirm => {
        if (isConfirm) {
            deleteRecipient(id);
        } else {
        }
    });
}

function deleteRecipient(id) {
    onLoadingRecipientModal();
    var DeleteStatus = 0;
    setTimeout(function () {
        $.ajax({
            url: '/Api/Observe/DeleteRecipient',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'RecipientId': id },
            success: function (response) {
                if (response == 1) {
                    DeleteStatus = 1;
                } else if (response == 2) {
                    DeleteStatus = 2;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                error = 1;
            }
        });
        $("input:checked").each(function () {
            if ($(this).attr('id') != id) {
                $.ajax({
                    url: '/Api/Observe/DeleteRecipient',
                    type: 'POST',
                    dataType: 'json',
                    async: false,
                    data: { 'RecipientId': $(this).attr('id') },
                    success: function (response) {
                        if (response == 1) {
                            DeleteStatus = 1;
                        } else if (response == 2) {
                            if (DeleteStatus != 1) {
                                DeleteStatus = 2;
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        error = 1;
                    }
                });
            }
        });
        refreshRecipientList();
        offLoadingRecipientModal();
        if (DeleteStatus == 1) {
            swal("Successful Deletion", "The selected recipient(s) have been deleted.", "success");
        } else {
            swal("Rejected", "Unauthorized access detected.", "error");
        }
    }, 500);
}

$('#ObserveNewName').on("keypress", function (e) {
    if (e.which == 13) {
        changeName();
    }
});

$('#NewRecipient').on("keypress", function (e) {
    if (e.which == 13) {
        submitNewRecipient();
    }
});

function runStep(StepId) {
    
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/ExecuteStep',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'StepId': StepId },
            success: function (data) {
                if (data.Status != "Authorized") {
                    swal("Rejected", "Unauthorized access detected.", "error");
                } else {
                    $('.CodeMirror').remove();
                    document.getElementById('ResponseBody').value = data.ResponseContent;
                    var editor = new CodeMirror.fromTextArea(document.getElementById('ResponseBody'), {
                        mode: 'text/html',
                        theme: "midnight"
                    });
                    
                    $('html, body').animate({
                        scrollTop: $("#Result-Section").offset().top
                    }, 2000);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function runBatchStep() {
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/ExecuteBatchStep',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'ObserveId': observeId },
            success: function (data) {
                if (data.Status != "Authorized") {
                    swal("Rejected", "Unauthorized access detected.", "error");
                } else {
                    $('.CodeMirror').remove();
                    document.getElementById('ResponseBody').value = data.ResponseContent;
                    var editor = new CodeMirror.fromTextArea(document.getElementById('ResponseBody'), {
                        mode: 'text/html',
                        theme: "midnight"
                    });

                    $('html, body').animate({
                        scrollTop: $("#Result-Section").offset().top
                    }, 2000);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function setCatch(id) {
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Catch/GetList',
            type: 'POST',
            async: false,
            dataType: 'json',
            data: { 'StepId': id },
            success: function (data) {
                document.getElementById('configureCatchId').value = id;
                document.getElementById("CatchMethod").innerHTML = "<option value='0' selected=''>None</option>";
                data.Catches.forEach(e => {
                    document.getElementById("CatchMethod").insertAdjacentHTML("beforeend", "<option value='"+e.Id+"'>"+e.Name+"</option>");
                })
                
                if (data.SelectedId != "0") {
                    document.getElementById('CatchMethod').value = data.SelectedId;
                }
                $('#configureCatchModal').modal("show");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function submitCatch() {
    onLoadingCatchModal();
    var stepId = document.getElementById('configureCatchId').value;
    var catchId = document.getElementById('CatchMethod').value;
    setTimeout(function () {
        $.ajax({
            url: '/Api/Step/SetCatch',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'StepId': stepId, 'CatchId': catchId },
            success: function (data) {
                if (data == 1) {
                    $('#configureCatchModal').modal("hide");
                    swal("Catch Set", "The new catch has been set successfully.", "success");
                } else if (data == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingCatchModal();
    }, 500);
}

function changeInterval() {
    var Interval = document.getElementById('IntervalOption').value;
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Observe/SetInterval',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'ObserveId': observeId, 'Interval': Interval },
            success: function (data) {
                if (data == 1) {
                    swal("Interval Set", "The new interval has been set successfully.", "success");
                } else if (data == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}

function addRecipient() {
    onLoadingModal();
    setTimeout(function () {
        refreshRecipientList();
        offLoadingModal();
        $('#addRecipientStepModal').modal('show');
    }, 500);
}

function submitNewRecipient() {
    onLoadingRecipientModal();
    setTimeout(function () {
        var recipientEmail = document.getElementById('NewRecipient').value;
        $.ajax({
            url: '/Api/Observe/AddRecipient',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'ObserveId': observeId, 'Email': recipientEmail },
            success: function (data) {
                if (data == 1) {
                    document.getElementById('NewRecipient').value = null;
                    refreshRecipientList();
                    swal("Recipient Added", "The new Recipient's email has been added into the list.", "success");
                } else if (data == 2) {
                    swal("Rejected", "Unauthorized access detected.", "error");
                } else if (data == 3) {
                    swal("Rejected", "The email has already been entered into the list.", "error");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingRecipientModal();
    }, 500);
}

function triggerPredefine() {
    if ($("#PredefineOption").is(":checked")) {
        $("#PredefineOption").setAttribute
        document.getElementById('CustomHeader-Section').setAttribute('style', 'display:block;');
    } else {
        document.getElementById('CustomHeader-Section').setAttribute('style', 'display:none;');
    }
}

function triggerHeader() {
    if ($("#UseHeaderOption").is(":checked")) {
        document.getElementById('HeaderUsage-Section').setAttribute('style', 'display:block;');
    } else {
        document.getElementById('HeaderUsage-Section').setAttribute('style', 'display:none;');
    }
}

function triggerEditPredefine() {
    if ($("#editPredefineOption").is(":checked")) {
        $("#editPredefineOption").setAttribute
        document.getElementById('editCustomHeader-Section').setAttribute('style', 'display:block;');
    } else {
        document.getElementById('editCustomHeader-Section').setAttribute('style', 'display:none;');
    }
}

function triggerEditHeader() {
    if ($("#editUseHeaderOption").is(":checked")) {
        document.getElementById('editHeaderUsage-Section').setAttribute('style', 'display:block;');
    } else {
        document.getElementById('editHeaderUsage-Section').setAttribute('style', 'display:none;');
    }
}

function reinitializeCreationModal() {
    if (document.getElementById('MethodContent').value != 0) {
        if (document.getElementById('numPC').value == "1") {
            document.getElementById('PC1').value = null;
        } else if (document.getElementById('numPC').value == "2") {
            document.getElementById('PC1').value = null;
            document.getElementById('PC2').value = null;
        }
    }
    document.getElementById('UrlOption').value = null;
    document.getElementById('MethodOption').value = 0;
    document.getElementById('PriorityOption').value = 0;
    document.getElementById('MethodContent').value = 0;
    document.getElementById('CustomHeaderOption').value = null;
    document.getElementById('HeaderUsage-Section').setAttribute('style', 'display:none;');
    document.getElementById('CustomHeader-Section').setAttribute('style', 'display:none;');
    $('#UseHeaderOption').prop('checked', false);
    $('#PredefineOption').prop('checked', false);
}

function changeMethodOption() {
    if (document.getElementById('MethodOption').value == 2) {
        document.getElementById('PostContentMethod-Section').setAttribute('style', 'display: block;');
    } else {
        document.getElementById('PostContentMethod-Section').setAttribute('style', 'display: none;');
    }
}

function changeMethodContent() {
    document.getElementById('PostContent-Section').innerHTML = "";
    if (document.getElementById('MethodContent').value == 1) {
        document.getElementById('numPC').value = "2";
        document.getElementById('PostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
        document.getElementById("PostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='PC1'>email</label><input type='text' id='PC1' class='form-control' name='PC1'></div><div class='form-group'><label for='PC2'>password</label><input type='password' id='PC2' class='form-control' name='PC2'></div>");
    } else if (document.getElementById('MethodContent').value >= 3) {
        onLoadingCreateModal();
        setTimeout(function () {
            $.ajax({
                url: '/Api/Step/GetPCMethodType',
                type: 'POST',
                async: false,
                success: function (data) {
                    data.forEach(e => {
                        if (e.Id == document.getElementById('MethodContent').value) {
                            if (e.Type == 1) {
                                document.getElementById('numPC').value = "1";
                                document.getElementById('PostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
                                document.getElementById("PostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='PC1'>" + e.PC1 + "</label><input type='text' id='PC1' class='form-control' name='PC1'></div>");
                            } else if (e.Type == 2) {
                                document.getElementById('numPC').value = "2";
                                document.getElementById('PostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
                                document.getElementById("PostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='PC1'>" + e.PC1 + "</label><input type='text' id='PC1' class='form-control' name='PC1'></div><div class='form-group'><label for='PC2'>" + e.PC2 + "</label><input type='password' id='PC2' class='form-control' name='PC2'></div>");
                            } else {
                                document.getElementById('numPC').value = "0";
                            }
                        }
                    });
                    document.getElementById('PostContentMethod-Section').setAttribute('style', 'display: block;');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    swal("Error", errorThrown, "error");
                }
            });
            offLoadingCreateModal();
        }, 500);
    } else {
        document.getElementById('PostContent-Section').setAttribute('style', 'display: none;');
    }
}

function changeEditMethodOption() {
    if (document.getElementById('editMethodOption').value == 2) {
        document.getElementById('editPostContentMethod-Section').setAttribute('style', 'display: block;');
    } else {
        document.getElementById('editPostContentMethod-Section').setAttribute('style', 'display: none;');
    }
}

function changeEditMethodContent() {
    document.getElementById('editPostContent-Section').innerHTML = "";
    if (document.getElementById('editMethodContent').value == 1) {
        document.getElementById('numEditPC').value = "2";
        document.getElementById('editPostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
        document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='EditPC1'>email</label><input type='text' id='EditPC1' class='form-control' name='EditPC1'></div><div class='form-group'><label for='EditPC2'>password</label><input type='password' id='EditPC2' class='form-control' name='EditPC2'></div>");
    } else if (document.getElementById('editMethodContent').value >= 3) {
        onLoadingEditModal();
        setTimeout(function () {
            $.ajax({
                url: '/Api/Step/GetPCMethodType',
                type: 'POST',
                async: false,
                success: function (data) {
                    data.forEach(e => {
                        if (e.Id == document.getElementById('editMethodContent').value) {
                            if (e.Type == 1) {
                                document.getElementById('numEditPC').value = "1";
                                document.getElementById('editPostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
                                document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='PC1'>" + e.PC1 + "</label><input type='text' id='editPC1' class='form-control' name='PC1'></div>");
                            } else if (e.Type == 2) {
                                document.getElementById('numEditPC').value = "2";
                                document.getElementById('editPostContent-Section').setAttribute('style', 'display: block;padding: 0px 50px;');
                                document.getElementById("editPostContent-Section").insertAdjacentHTML("beforeend", "<div class='form-group mt-1'><label for='PC1'>" + e.PC1 + "</label><input type='text' id='editPC1' class='form-control' name='PC1'></div><div class='form-group'><label for='PC2'>" + e.PC2 + "</label><input type='password' id='editPC2' class='form-control' name='PC2'></div>");
                            } else {
                                document.getElementById('numEditPC').value = "0";
                            }
                        }
                    });
                    document.getElementById('editPostContentMethod-Section').setAttribute('style', 'display: block;');
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    swal("Error", errorThrown, "error");
                }
            });
            offLoadingEditModal();
        }, 500);
    } else {
        document.getElementById('editPostContent-Section').setAttribute('style', 'display: none;');
    }
}
