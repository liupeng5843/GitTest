var User = User || {};
var FormSubmitType = {
    Create: 0,
    Update: 1,
    Delete: 2
};
User.FormType = FormSubmitType.Create;

$(function () {

    var inputList = $("#userForm input");
    inputList.change(function () {
        var inputText = $(this).val();
        var nextSpan = "#" + $(this).attr("id") + "Tip";
        if (inputText == "") {
            $(nextSpan).html("").append("<font color='red'>*不能为空</font>");
        }
        else {
            $(nextSpan).html("").append("<font color='red'>*</font>");
        }

    });

    var userFormDialog = $("#userForm").dialog({
        title: "用户信息",
        autoOpen: false,
        width: 480,
        height: 470,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'create-user-edit',
                text: '编辑',
                click: function () {
                    User.EnableEdit();
                }
            },
            {
                id: 'create-user-ok',
                text: '确认',
                click: function () {
                    if (User.Valid()) {
                        User.SubmitUserForm();
                    }
                }
            },
            {
                id: 'create-user-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#create-user-edit").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#create-user-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#create-user-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");
        },
        close: User.ClearCreateUserForm
    });
});

User.Create = function () {
    $("#userForm").dialog("open");
    $("#userForm").find("input").removeAttr("disabled");
    $("#userForm").find("select").removeAttr("disabled");
    $("#userForm").find("span").html("").append("<font color='red'>*</font>");
    $("#Comment").val("-1");
    $("#create-user-edit").hide();
    $("#create-user-ok").show();
};

User.SubmitUserForm = function () {
    var url = baseServerUrl + "odata/CustomerOData";
    var type = "POST";

    var data;
    if (User.FormType == FormSubmitType.Create) {
        type = "POST";
        url = url;

    }
    else {
        type = "PUT";
        url = url + '(' + $("#UserId").val() + ')';
    }

    data = {
        UserId: $("#UserId").val(),
        AccountName: $("#AccountName").val(),
        UserName: $("#UserName").val(),
        Password: $("#Password").val(),
        PhoneNumber: $("#PhoneNumber").val(),
        Address: $("#Address").val(),
        Description: $("#Description").val(),
        Comment: $("#Comment").val(),
        Item1: $("#item1").val(),
        Item2: $("#item2").val(),
        Item3: $("#item3").val(),
        Item4: $("#item4").val(),
        Item5: $("#item5").val(),
    };
    $.ajax({
        url: url,
        type: type,
        data: data,
        success: function (result) {
            var code = result["Code"];
            if (code == -1) {
                //$("#AccountNameTip").text(result["Description"]);
                alert(result["Description"]);
            }
            else if (code == -2) {
                alert(result["Description"]);
            }
            else if (code == 1) {
                alert(result["Description"]);
                $("#userForm").dialog("close");
                User.Refresh();
            }
        },
        error: function () {
            alert("操作失败！");
        }
    });
};

User.Edit = function (user) {
    User.FillUserForm(user);
    $("#userForm").dialog("open");
    $("#userForm").find("input").attr("disabled", "disabled");
    $("#userForm").find("select").attr("disabled", "disabled");
    $("#userForm").find("span").html("").append("<font color='red'>*</font>");
    $("#create-user-edit").show();
    $("#create-user-ok").hide();
}

User.Delete = function () {
    var type = 'DELETE';
    var url = baseServerUrl + "odata/CustomerOData(" + $("#UserId").val() + ")";

    var okay = window.confirm("确认删除？");
    if (!okay) {
        return false;
    }

    $.ajax({
        url: url,
        type: type,
        success: function (data) {
            User.Refresh();
        },
        error: function () {
            alert("操作失败！");
        }
    });
}

User.FillUserForm = function (user) {
    $("#UserId").val(user.UserId);
    $("#AccountName").val(user.AccountName);
    $("#UserName").val(user.UserName);
    $("#Password").val(user.Password);
    $("#PhoneNumber").val(user.PhoneNumber);
    $("#Address").val(user.Address);
    $("#Description").val(user.Description);
    $("#Comment").val(user.Comment);
    $("#item1").val(user.Item1);
    $("#item2").val(user.Item2);
    $("#item3").val(user.Item3);
    $("#item4").val(user.Item4);
    $("#item5").val(user.Item5);
}

User.ClearCreateUserForm = function () {
    $("#userForm input").val('');
};

User.Refresh = function () {
    $("#usersGrid").data("uiDataMatrix").reloadGrid();
}

User.Valid = function () {
    var inputList = $("#userForm input").not("#UserId").not("#Address").not("#Description");
    var isValid = true;
    for (var i = 0; i < inputList.size() ; i++) {
        var inputText = $(inputList.get(i)).val();
        var nextSpan = $("#" + $(inputList.get(i)).attr("id") + "Tip");
        if (inputText == "") {
            isValid = false;
            nextSpan.html("").append("<font color='red'>*不能为空</font>");
        }
        else {
            nextSpan.html("").append("<font color='red'>*</font>");
        }
    }
    return isValid;
}

User.EnableEdit = function () {
    $("#userForm").find("input").removeAttr("disabled");
    $("#userForm").find("select").removeAttr("disabled");
    $("#create-user-edit").hide();
    $("#create-user-ok").show();
}