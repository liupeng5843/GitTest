var Order = Order || {};
var FormSubmitType = {
    Create: 0,
    Update: 1,
    Delete: 2
};
Order.FormType = FormSubmitType.Create;

$(function () {
    var orderFormDialog = $("#orderForm").dialog({
        title: "修改订单",
        autoOpen: false,
        width: 480,
        height: 450,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'update-order-ok',
                text: '确认',
                click: function () {
                    if (Order.Valid()) {
                        Order.SubmitUserForm();
                    }
                }
            },
            {
                id: 'update-order-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#update-order-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#update-order-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");
        },
        close: Order.ClearCreateUserForm
    });
});


Order.SubmitUserForm = function () {
    var url = baseServerUrl + "odata/OrderOData";
    var type = "POST";

    if (Order.FormType == FormSubmitType.Create) {
        type = "POST";
        url = url;
    }
    else {
        type = "PUT";
        url = url + '(' + $("#Id").val() + ')';
    }

    var data = {
        StartPosition: $("#StartPosition").val(),
        EndPosition: $("#EndPosition").val(),
        FromUser: $("#FromUser").val(),
        ToUser: $("#ToUser").val()
    };

    $.ajax({
        url: url,
        type: type,
        data: data,
        success: function (data) {
            $("#orderForm").dialog("close");
            alert("操作成功！");
            Order.Refresh();
        },
        error: function (jqXhr, textStatus, errorThrown) {
            alert("操作失败，请确认地址无误！");
        }
    });
};

Order.Edit = function (order) {
    Order.FillOrderForm(order);
    $("#orderForm").dialog("open");
}

Order.Delete = function () {
    var type = 'DELETE';
    var url = baseServerUrl + "odata/OrderOData(" + $("#Id").val() + ")";

    var okay = window.confirm("确认删除？");
    if (!okay) {
        return false;
    }

    var data = {
        Id: $("#Id").val()
    };

    $.ajax({
        url: url,
        type: type,
        success: function (data) {
            Order.Refresh();
        },
        error: function () {
            alert("删除失败！");
        }
    });
}

Order.FillOrderForm = function (order) {
    $("#StartPosition").val(order.StartPosition);
    $("#EndPosition").val(order.EndPosition);
    $("#FromUser").val(order.FromUser);
    $("#ToUser").val(order.ToUser);
}

Order.ClearCreateUserForm = function () {
    $("#orderForm input").val('');
};

Order.Refresh = function () {
    $("#originalOrderGrid").data("uiDataMatrix").reloadGrid();
}

Order.Valid = function () {
    var startPosition = $("#StartPosition").val();
    var endPosition = $("#EndPosition").val();
    var fromUser = $("#FromUser").val();
    var toUser = $("#ToUser").val();
    if (startPosition == "" || endPosition == "" || fromUser == "" || toUser == "") {
        alert("信息不能为空！");
        return false;
    }
    else {
        return true;
    }
}