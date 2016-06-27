var Driver = Driver || {};
var FormSubmitType = {
    Create: 0,
    Update: 1,
    Delete: 2
};
Driver.FormType = FormSubmitType.Create;

$(function () {
    var inputList = $("#driverForm input");
    inputList.change(function () {
        var inputText = $(this).val();
        var nextSpan = "#"+$(this).attr("id")+"Tip";
        if (inputText == "") {
            $(nextSpan).html("").append("<font color='red'>*不能为空</font>");
        }
        else {
            $(nextSpan).html("").append("<font color='red'>*</font>");
        }

    });

    var driverFormDialog = $("#driverForm").dialog({
        title: "司机信息",
        autoOpen: false,
        width: 480,
        height: 635,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'create-driver-edit',
                text: '编辑',
                click: function () {
                    Driver.EnableEdit();
                }
            },
            {
                id: 'create-driver-ok',
                text: '确认',
                click: function () {
                    if (Driver.Valid()) {
                        Driver.SubmitDriverForm();
                    }
                }
            },
            {
                id: 'create-driver-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {

            $("#create-driver-edit").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
           .addClass("btn btn-primary");

            $("#create-driver-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#create-driver-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");
        },
        close: Driver.ClearCreateDriverForm
    });
});

Driver.Create = function () {
    $("#driverForm").dialog("open");
    $("#driverForm").find("input").removeAttr("disabled");
    $("#driverForm").find("span").html("").append("<font color='red'>*</font>");
    $("#EditBadSend").hide();
    $("#create-driver-edit").hide();
    $("#create-driver-ok").show();
};

Driver.SubmitDriverForm = function () {
    var url = baseServerUrl + "odata/DriverOData";
    var type = "POST";
    var data;

    if (Driver.FormType == FormSubmitType.Create) {
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
        VehicleNumber: $("#VehicleNumber").val(),
        DriverLicence: $("#DriverLicence").val(),
        VehicleLicence: $("#VehicleLicence").val(),
        Description: $("#Description").val(),
        Hobby: $("#Hobby").val(),
        BadSend: $("#BadSend").val(),
        Fleet: $("#Fleet").val(),
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
                $("#driverForm").dialog("close");
                Driver.Refresh();
            }
        },
        error: function () {
            alert("操作失败！");
        }
    });
};

Driver.Edit = function (driver) {
    Driver.FillDriverForm(driver);
    $("#EditBadSend").show();
    $("#driverForm").dialog("open");
    $("#driverForm").find("input").attr("disabled", "disabled");
    $("#driverForm").find("span").html("").append("<font color='red'>*</font>");
    $("#create-driver-edit").show();
    $("#create-driver-ok").hide();
}

Driver.Delete = function () {
    var type = 'DELETE';
    var url = baseServerUrl + "odata/DriverOData(" + $("#UserId").val() + ")";

    var okay = window.confirm("确认删除？");
    if (!okay) {
        return false;
    }

    $.ajax({
        url: url,
        type: type,
        success: function (data) {
            Driver.Refresh();
        },
        error: function () {
            alert("删除失败！");
        }
    });
}

Driver.FillDriverForm = function (driver) {
    $("#UserId").val(driver.UserId);
    $("#AccountName").val(driver.AccountName);
    $("#UserName").val(driver.UserName);
    $("#Password").val(driver.Password);
    $("#PhoneNumber").val(driver.PhoneNumber);
    $("#VehicleNumber").val(driver.VehicleNumber);
    $("#DriverLicence").val(driver.DriverLicence);
    $("#VehicleLicence").val(driver.VehicleLicence);
    $("#Description").val(driver.Description);
    $("#Hobby").val(driver.Hobby);
    $("#BadSend").val(driver.BadSend);
    $("#Fleet").val(driver.Fleet);
}

Driver.ClearCreateDriverForm = function () {
    $("#driverForm input").val('');
};

Driver.Refresh = function () {
    $("#driverGrid").data("uiDataMatrix").reloadGrid();
}

Driver.Valid = function () {

    var inputList = $("#driverForm input").not("#UserId").not("#Description").not("#Hobby").not("#BadSend");
    var isValid = true;
    for (var i = 0; i < inputList.size(); i++)
    {
        var inputText = $(inputList.get(i)).val();
        var nextSpan = $("#" + $(inputList.get(i)).attr("id") + "Tip");
        if (inputText == "") {
            nextSpan.html("").append("<font color='red'>*不能为空</font>");
            isValid = false;
        }
        else {
            nextSpan.html("").append("<font color='red'>*</font>");
        }
    }
    return isValid;
}

Driver.EnableEdit = function () {
    $("#driverForm").find("input").removeAttr("disabled");
    $("#create-driver-edit").hide();
    $("#create-driver-ok").show();
}