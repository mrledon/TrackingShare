var openFunc = {
    pushNotifySuccess: function (message) {
        $.notify({
            message: message
        }, {
            type: "success",
            placement: {
                from: "top",
                align: "right"
            },
            delay: 4000,
            timer: 1000,
            z_index: 9999
        });
    },
    pushNotifyError: function (message) {
        $.notify({
            message: message
        }, {
            type: "danger",
            placement: {
                from: "top",
                align: "right"
            },
            delay: 4000,
            timer: 1000,
            z_index: 9999
        });
    },
    pushNotifyWarning: function (message) {
        $.notify({
            message: message
        }, {
            type: "warning",
            placement: {
                from: "top",
                align: "right"
            },
            delay: 4000,
            timer: 1000,
            z_index: 9999
        });
    }
}