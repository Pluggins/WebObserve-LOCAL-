var statusBlock = $('#ServerStatusContainer');
var toggle = 0;
/*
$('#ServerStatusToggle').on('click', function () {
    if (toggle == 0) {
        toggle = 1;
        setInterval(retrieveStatus, 5000);
    }
});
*/

$(document).ready(function () {
    retrieveStatus();
});

function onLoadingStatusModal() {
    $(statusBlock).block({
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

function offLoadingStatusModal() {
    $(statusBlock).unblock({
    });
}

function retrieveStatus() {
    if (toggle == 0) {
        onLoadingStatusModal();
        toggle = 1;
        setTimeout(function () {
            $.ajax({
                url: '/Api/Server/Status',
                type: 'POST',
                success: function (response) {
                    if (response.Status == 1) {
                        document.getElementById('ServerOnPanel').setAttribute('style', 'display:block;');
                        document.getElementById('ServerOffPanel').setAttribute('style', 'display:none;');
                        document.getElementById('TimeOn').innerHTML = response.LastCheckedMin;
                        
                    } else {
                        document.getElementById('ServerOnPanel').setAttribute('style', 'display:none;');
                        document.getElementById('ServerOffPanel').setAttribute('style', 'display:block;');
                        document.getElementById('TimeOff').innerHTML = response.LastCheckedMin;
                        
                    }
                    document.getElementById('ServerSuccess').innerHTML = response.TotalIn;
                    document.getElementById('ServerFail').innerHTML = response.TotalOut;
                    document.getElementById('ServerSuccessRate').innerHTML = response.SuccessRate;
                    if (parseFloat(response.SuccessRate) > 80) {
                        document.getElementById('ServerSuccessRate').setAttribute('style', 'font-size:30px; color: #1c7a30;');
                    } else {
                        document.getElementById('ServerSuccessRate').setAttribute('style', 'font-size:30px; color: #b52020;');
                    }
                    setInterval(retrieveStatus, 10000);
                }
            });
            offLoadingStatusModal();
        }, 500);
    } else {
        $.ajax({
            url: '/Api/Server/Status',
            type: 'POST',
            success: function (response) {
                if (response.Status == 1) {
                    document.getElementById('ServerOnPanel').setAttribute('style', 'display:block;');
                    document.getElementById('ServerOffPanel').setAttribute('style', 'display:none;');
                    document.getElementById('TimeOn').innerHTML = response.LastCheckedMin;
                } else {
                    document.getElementById('ServerOnPanel').setAttribute('style', 'display:none;');
                    document.getElementById('ServerOffPanel').setAttribute('style', 'display:block;');
                    document.getElementById('TimeOff').innerHTML = response.LastCheckedMin;
                }
                document.getElementById('ServerSuccess').innerHTML = response.TotalIn;
                document.getElementById('ServerFail').innerHTML = response.TotalOut;
                document.getElementById('ServerSuccessRate').innerHTML = response.SuccessRate;
                if (parseFloat(response.SuccessRate) > 80) {
                    document.getElementById('ServerSuccessRate').setAttribute('style', 'font-size:30px; color: #1c7a30;');
                } else {
                    document.getElementById('ServerSuccessRate').setAttribute('style', 'font-size:30px; color: #b52020;');
                }
            }
        });
    }
}