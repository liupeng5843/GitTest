var Global = Global || {};
Global.FormMethod = "post";
Global.EditingEntityId = emptyGuid;
Global.Text = "";
Global.CreatedTime = "";
var MessageType = {
    DeleteConfirm: 0,
    EnableConfirm: 1
}

var messageType = MessageType.DeleteConfirm;
//MessageBox
$(function () {
    var MessageDialog = $("#MessageDialog").dialog({
        autoOpen: false,
        title:'管理后台',
        width: 400,
        height: 250,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'Message-ok',
                text: '确定',
                click: function () {
                    switch (messageType) {
                        case MessageType.DeleteConfirm:
                            disablemessage(Global.DeleteingEntityId);
                            break;
                        case MessageType.EnableConfirm:
                            enablemessage(Global.EditingEntityId);
                            break;
                        default:
                            break;
                    }
                    $(this).dialog("close");
                }
            },
            {
                id: 'Message-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }

            }
        ],
        open: function () {
            $("#Message-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#Message-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#Message-ok").blur();
            $("#Message-cancel").blur();
        }
    });
});
//messagesDialog
$(function () {
    var messagesDialog = $("#messagesDialog").dialog({
        autoOpen: false,
        title: '新建消息',
        width: 400,
        height: 300,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'messagesDialog-ok',
                text: '发送',
                click: function () {
                    submitMessage();
                }
            },
            {
                id: 'messagesDialog-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }

            }
        ],
        open: function () {
            $("#messagesDialog-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#messagesDialog-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#messagesDialog-ok").blur();
            $("#messagesDialog-cancel").blur();
        }
    });

    $("#btn_create").on("click", function () {
        Global.FormMethod = "post";
        // initFoodTypePositionSelection(true);  
        $("#text_Message_Create").val("");
        $("#messagesDialog").dialog("open");
    });
});

//messagesDetailDialog
$(function () {
    var messagesDetailDialog = $("#messagesDetailDialog").dialog({
        autoOpen: false,
        title: '新建消息',
        width: 500,
        height: 400,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'messagesDetailDialog-ok',
                text: '确定',
                click: function () {
                    $(this).dialog("close");
                }
            }          
        ],
        open: function () {
            $("#messagesDetailDialog-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");
            $("#messagesDetailDialog-ok").blur();
           // validator.resetForm();
        }
    });
});

function submitMessage()
{
    var url = getRawUrl("odata/Messages");
    
    var message = {
        Text: $("#text_Message_Create").val(),
    };
    if ($("#text_Message_Create").val() == "") {
        alert("内容不能为空");
        return false;
    }
    if ($("#text_Message_Create").val().length >200) {
        alert("内容不能超过200字");
        return false;
    }
    $.ajax({
        url: url,
        data: message,
        type: Global.FormMethod,
        async:false,
        success: function (data) {
            $("#messagesDialog").dialog("close");
            refreshMessageGrid();
        }
    });
}

function showDetailMessage(){
    $("#createTime_Message").val(Global.CreatedTime);
    $("#text_Message").val(Global.Text);
    $("#ui-id-3").html("详情");
    $("#messagesDetailDialog").dialog("open");
}

function showMessage(msgType)
{
    var message = "";
    messageType = msgType;
    switch (msgType) {
        case MessageType.DeleteConfirm:
            message = "禁用该用户？"; break;
        case MessageType.EnableConfirm:
            message = "启用该用户？"; break;
        default:
            message = "无消息提醒";
    }
    $("#message").html(message);
    $("#MessageDialog").dialog("open");
}

function refreshMessageGrid()
{
    refreshGrid('messages');
}