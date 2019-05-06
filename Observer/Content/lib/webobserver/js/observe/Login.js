var loginInterval;
var block_ele = $('#LoginCard');

$(document).ready(function () {
    loginInterval = setInterval(checkStatus, 500);
});

$("#loginbutton").on("click", function () {
    submitEmailLogin();
});

$('#Email').on("keypress", function (e) {
    if (e.which == 13) {
        submitEmailLogin();
    }
});

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

function checkStatus() {
    $.ajax({
        url: '/Api/Login/CheckStatus',
        type: 'POST',
        dataType: 'json',
        data: { 'SessionId': loginSessionId, 'SessionKey': loginKey },
        success: function (responds) {
            if (responds.Status == "LoginReady") {
                clearInterval(loginInterval);
                window.location.href = homeLink + responds.Path + "?status=2";
            } else if (responds.Status == "Expired") {
                clearInterval(loginInterval);
                expireRefresh();
            } else if (responds.Status == "QRPending") {
                document.getElementById('SessionCode').innerHTML = responds.SessionCode;
                document.getElementById('InitialMenu').setAttribute('style', 'display:none;');
                document.getElementById('PostMenu').setAttribute('style', 'display:block;');
                toastr.warning('QR has been captured!', 'QR Captured', { "showMethod": "fadeIn", "hideMethod": "fadeOut", timeOut: 30000, "positionClass": "toast-bottom-right", "progressBar": true });
                clearInterval(loginInterval);
                setInterval(qrCheckStatus, 500);
                setTimeout(expireRefresh, 30000);
            }
        }
    });
}

function qrCheckStatus() {
    $.ajax({
        url: '/Api/Login/CheckStatus',
        type: 'POST',
        dataType: 'json',
        data: { 'SessionId': loginSessionId, 'SessionKey': loginKey },
        success: function (responds) {
            if (responds.Status == "LoginReady") {
                window.location.href = homeLink;
            } else if (responds.Status == "Expired") {
                expireRefresh();
            }
        }
    });
}

function expireRefresh() {
    window.location.href = loginLink;
}

function submitEmailLogin() {
    onLoadingModal();
    setTimeout(function () {
        $.ajax({
            url: '/Api/Login/EmailLogin',
            type: 'POST',
            dataType: 'json',
            async: false,
            data: { 'SessionId': loginSessionId, 'Email': document.getElementById('Email').value, 'SessionKey': loginKey },
            success: function (response) {
                if (response == 0) {
                    window.location.href = loginLink;
                } else if (response == 1) {
                    swal("Rejected", "Session Type Error.", "error");
                } else if (response == 2) {
                    swal("Rejected", "Email has not been registered.", "error");
                } else if (response == 3) {
                    swal("Rejected", "Unauthorized access.", "error");
                } else if (response == 4) {
                    document.getElementById('Email').setAttribute('disabled', 'disabled');
                    document.getElementById('loginbutton').setAttribute('style', 'display: none;');
                    document.getElementById('base-pillOpt2').setAttribute('class', 'nav-link disabled');
                    swal("Email Sent", "An email has been sent to you for verification.", "success");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("Error", errorThrown, "error");
            }
        });
        offLoadingModal();
    }, 500);
}