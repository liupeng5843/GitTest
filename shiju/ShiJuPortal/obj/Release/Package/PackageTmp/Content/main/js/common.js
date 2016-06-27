var Global = Global || {};
Global.Error = Global.Error || {};
Global.Error.message = function showError(message) {
    alert(message);
};

function ajaxJ(config) {
    config = $.extend({
        isTraditional: true,
        reqData: {},
        httpType: 'POST',
        error: Global.Error.message
    }, config);

    $.ajax({
        url: config.url,
        traditional: config.isTraditional,
        data: config.reqData,
        dataType: 'json',
        type: config.httpType,
        success: function (data) {
            var variable = data ? data : {};
            config.successCall.call(this, variable);
        },
        error: function (message) {
            var text;

            if (message.status == "409") {
                text = message.responseJSON.value;
            } else {
                text = "出错了！";
            }

            config.error.call(this, text);
        },
        timeout: 30000
    });
}

$(function () {
    var userFormDialog = $("#messageBox").dialog({
        title: "后台消息",
        autoOpen: false,
        width: 100,
        height: 50,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'create-user-ok',
                text: '确认',
                click: function () {
                    Global.MessageBoxOkEvent();
                }
            },
            {
                id: 'create-user-cancel',
                text: '取消',
                click: function () {
                    Global.MessageBoxOkEvent();
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#create-user-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#create-user-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");
        }
    });
});

Global.MessageBoxOkEvent = function () { };
Global.MessageBoxCancelEvent = function () { };

function getAttachmentUrl(batchId, attachmentId) {
    var baseUrl = (fumaServiceBaseUrl.substr(fumaServiceBaseUrl.length-1) == '/')?fumaServiceBaseUrl:fumaServiceBaseUrl + "/";
    
    return baseUrl + "api/Batches/" + batchId + "/Moments/Attachments/" + attachmentId;
}