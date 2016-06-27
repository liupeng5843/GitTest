var Global = Global || {};
Global.FormMethod = "post";
Global.EditingEntityId = emptyGuid;
var MessageType = {
    DeleteConfirm: 0,
    EnableConfirm: 1
}

var messageType = MessageType.DeleteConfirm;
Global.DeleteingEntityId = emptyGuid;

$(function () {
    var orderDialog = $("#userDialog").dialog({
        autoOpen: false,
        width: 500,
        height: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'user-ok',
                text: '确定',
                click: function () {
                    submitUser();
                }
            },
            {
                id: 'user-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }

            }
        ],
        open: function () {
            $("#user-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#user-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#user-ok").blur();
            $("#user-cancel").blur();
            InitializeSiverlight('PortraitSilverLight', 'PortraitId', false);
            InitializeSiverlight('BackgroundImageSilverLight', 'BackgroundImageId', false);
        }
    });

    var form = $("#userform");
    var validator = form.validate({
        errorElement: "span",
        rules: {
            "NickName": {
                required: true,
                maxlength: 15
            },
            "PhoneNumber": {
                required: true,
                number: true,
                maxlength: 15
            },
            "Signature": {
                required: true,
                maxlength: 100
            },
            "District": {
                required: true,
                maxlength: 10
            },

        },
        messages: {
            "NickName": {
                required: "请输入昵称",
                maxlength: $.validator.format("最多不能超过 {0} 字.")
            },
            "PhoneNumber": {
                required: "请输入手机号",
                number: "只能包含数字",
                maxlength: $.validator.format("最多不能超过 {0} 字.")
            },
            "Signature": {
                required: "请输入签名",
                maxlength: $.validator.format("最多不能超过 {0} 字."),
            },
            "District": {
                required: "请输入城市名",
                maxlength: $.validator.format("最多不能超过 {0} 字."),
            },
        },
        errorClass: "error"
    });


})

//MessageBox
$(function () {
    var MessageDialog = $("#MessageDialog").dialog({
        autoOpen: false,
        title: '管理后台',
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
                            disableUser(Global.DeleteingEntityId);
                            break;
                        case MessageType.EnableConfirm:
                            enableUser(Global.EditingEntityId);
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


function editUser(row) {
    $("#Id").val(row.Id);
    $("#PhoneNumber").val(row.PhoneNumber);
    $("#NickName").val(row.NickName);
    $("#Signature").val(row.Signature);
    $("#PortraitId").val(row.Portrait);
    $("#BackgroundImageId").val(row.BackgroundImage);
    $("#District").val(row.District);
    $("#Gender").val(row.Gender);
    $("#CreatedTime").val(row.CreatedTime);
    $("#SignUpTime").val(row.SignUpTime);
    //$("input[name='OrderStatus']").val(row.Status);
    $("UserStatus").val(row.Status);
    //if (row.Status == "Active") {
    //    $("#radio_1").prop("checked", "checked");
    //}
    //else if (row.Status == "Disabled") {
    //    $("#radio_2").prop("checked", "checked");
    //}
    $("#userDialog").dialog("open");
}

function submitUser() {
    var form = $("#userform");
    form.submit(function (event) { event.preventDefault(); });
    form.submit();
    var validator = form.validate();
    if (validator.numberOfInvalids() <= 0) {
        var url = getRawUrl("odata/UsersOData");
        if (Global.FormMethod == 'put') {
            url += "(" + Global.EditingEntityId + ")";
        }
        var User = {
            Id: Global.EditingEntityId,
            PhoneNumber: $("#PhoneNumber").val(),
            NickName: $("#NickName").val(),
            Signature: $("#Signature").val(),
            Portrait: $("#PortraitId").val(),
            BackgroundImage: $("#BackgroundImageId").val(),
            District: $("#District").val(),
            Gender: $("#Gender").val(),
            //Status: $("input[name='UserStatus']:checked").val(),
            Status: $("UserStatus").val(),
            CreatedTime: $("#CreatedTime").val(),
            SignUpTime: $("#SignUpTime").val()
        };
        console.log(User.IsEnabled);
        $.ajax({
            url: url,
            data: User,
            type: Global.FormMethod,
            async: false,
            success: function (data) {
                $("#userDialog").dialog("close");
                refreshUserGrid();
            }
        });
    }
}

function disableUser(id) {
    var url = getRawUrl("odata/UsersOData(" + id + ")")
    $.ajax({
        url: url,
        type: 'delete',
        async: false,
        success: function (data) {
            refreshUserGrid();
        }
    });
}

function enableUser(id) {
    var url = getRawUrl("odata/UsersOData(" + id + ")")
    $.ajax({
        url: url,
        type: 'patch',
        async: false,
        success: function (data) {
            refreshUserGrid();
        }
    });
}

function showMessage(msgType) {
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

function refreshUserGrid() {
    refreshGrid('users');
}