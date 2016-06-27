var BatchOrder = {} || BatchOrder;
var Order = {} || Order;
$(function () {
    var batchorderDialog = $("#batchorder").dialog({
        title: "订单打包",
        autoOpen: false,
        width: 480,
        height: 470,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-order-ok',
                text: '确认',
                click: function () {
                    if (BatchOrder.CheckTime()) {
                        BatchOrder.Batch();
                    }
                }
            },
            {
                id: 'batch-order-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-order-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#batch-order-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-order-ok").blur();
        },
        close: function () {
            $("#batchorderform table input").val('');
        }
    });

    $("#batchOrder").on("click", function () {
        var batchOderIds = $("#originalOrderGrid").data("uiDataMatrix").getSelectedRowIds();
        if (batchOderIds.length <= 0) {
            alert("请选择需要打包的订单！");
            return false;
        }

        BatchOrder.GetCustomers("");
        $("#batchorder").dialog("open");
        BatchOrder.FillBatchOrderSummary();
    });

    $("#PlanArriveTime, #PlanSendTime").datetimepicker({
        dateFormat: 'yy-mm-dd', timeFormat: 'HH:mm', minDateTime: new Date(1900, 0, 1), maxDateTime: new Date(2099, 12, 31), showOn: 'focus', showButtonPanel: true, changeMonth: true, changeYear: true
    });

    function checkDatetime() {
        var orderSendStartTimeSearch = $("#orderSendStartTimeSearch").val();
        var orderSendEndTimeSearch = $("#orderSendEndTimeSearch").val();
        if (orderSendStartTimeSearch >= orderSendEndTimeSearch) {
            if (!(orderSendStartTimeSearch.trim() == '' && orderSendEndTimeSearch.trim() == '')) {
                alert("请选择正确的时间");
                return false;
            }
        }

        //postback with datetime search condition
        var url = "";
        var currentHref = window.location.href;
        var containSearchDatetime = currentHref.indexOf("orderSendStartTime") > 0;

        var menu = Request.QueryString("menu");
        var baseUrl = location.pathname + "?menu=" + menu;
        if (orderSendStartTimeSearch.trim() == '' && orderSendEndTimeSearch.trim() == '') {
            url = baseUrl;
        }
        else {
            url = baseUrl + "&orderSendStartTime=" + orderSendStartTimeSearch + "&orderSendEndTime=" + orderSendEndTimeSearch;
        }

        window.location.href = url;
    }

    //order search based on datetime;
    $(".datetimeSearchInput").datetimepicker({
        dateFormat: 'yy-mm-dd', timeFormat: 'HH:mm', minDateTime: new Date(1900, 0, 1), maxDateTime: new Date(2099, 12, 31), showOn: 'focus', showButtonPanel: true, changeMonth: true, changeYear: true
    });

    //workaround for raising z-index of datepicker;
    $(".datetimeSearchInput").on("mouseup", function () {
        $("#ui-datepicker-div").css("z-index", 99);
    });

    $(".datetimeSearchButton").on("click", checkDatetime);
});

BatchOrder.CheckTime = function () {
    var planSendTime = $("#PlanSendTime").val();
    var planArriveTime = $("#PlanArriveTime").val();
    if (planSendTime.trim() == '' || planArriveTime.trim() == '') {
        alert("请选择正确的时间");
        return false;
    }
    else if (planSendTime >= planArriveTime) {
        alert("请选择正确的时间");
        return false;
    }
    else {
        return true;
    }
}

BatchOrder.GetCustomers = function (keyword) {
    var filter = "?$filter=substringof('" + keyword
        + "',UserName) eq true or substringof('"
        + keyword + "',PhoneNumber) eq true";

    var url = baseServerUrl + "odata/CustomerOData";
    if (keyword.trim() != '') {
        url += filter;
    }

    $.ajax({
        url: url,
        type: 'GET',
        async: false,
        success: function (data) {
            var drivers = data.value;
            var customersDropdownListHtml = "";
            for (var i = 0; i < drivers.length; i++) {
                var driver = drivers[i];
                customersDropdownListHtml += "<option value='" + driver.UserId + "'>" + driver.UserName + " " + driver.PhoneNumber + "</option>";
            }
            if (drivers.length == 0) {
                customersDropdownListHtml += "<option value='0'>未查询到任何结果！</option>";
            }
            $("#ToUserId").html(customersDropdownListHtml);
        }
    });
}
BatchOrder.Batch = function () {
    var batchOrdersIds = $("#originalOrderGrid").data("uiDataMatrix").getSelectedRowIds();
    $("#batchorderids").val(batchOrdersIds);
    var type = "POST";
    var url = baseServerUrl + "odata/BatchOData?orderIdString=" + $("#batchorderids").val();
    var data = {
        StartPosition: $("#StartPosition").val(),
        EndPosition: $("#EndPosition").val(),
        PlanArriveTime: $("#PlanArriveTime").val(),
        PlanSendTime: $("#PlanSendTime").val(),
        ToUserId: $("#ToUserId").val(),
        Description: $("#Description").val()
    };

    $.ajax({
        url: url,
        type: type,
        data: data,
        async: false,
        success: function (responseData, textStatus, jqXHR) {
            if (textStatus == 'success' && jqXHR.status == 200) {
                var isContinuePack = confirm(responseData.value + " 继续打包吗？");
                if (isContinuePack) {
                    $.ajax({
                        url: url + "&continuePack=true",
                        type: type,
                        data: data,
                        async: false,
                        success: function (r, ts, j) {
                            if (j.status == 200) {
                                var responseMessage = j.responseJSON.value;
                                alert("打包失败：" + responseMessage);
                                $("#batchorder").dialog("close");
                                Order.Refresh();
                                return false;
                            }

                            $("#batchorder").dialog("close");
                            Order.Refresh();
                            alert("打包成功！");
                        }
                    });
                }
                else {
                    return false;
                }
            }

            if (jqXHR.status == 202) {
                $("#batchorder").dialog("close");
                Order.Refresh();
                alert("打包成功！")
            }
        },
        error: function (jqXhr, textStatus, errorThrown) {
            if (jqXhr.status == 505) {
                var responseMessage = jqXhr.responseJSON.value;
                alert("打包失败：" + responseMessage);
                return false;
            }
            else if (jqXhr.status == 504) {
                var responseMessage = jqXhr.responseJSON.value;
                alert("打包失败：" + responseMessage);
                $("#batchorder").dialog("close");
                Order.Refresh();
                return false;
            }
            else {
                alert("打包失败！");
            }
        }
    });
}

BatchOrder.FillBatchOrderSummary = function () {
    var batchOrderIds = $("#originalOrderGrid").data("uiDataMatrix").getSelectedRowIds();
    var grid = $("#originalOrderGrid");

    var batchOrderNosString = "";
    for (var i = 0; i < batchOrderIds.length; i++) {
        var row = grid.dataMatrix("getRow", batchOrderIds[i]);
        batchOrderNosString += row.OrderNo + "，";
    }

    $("#batchordernos").text(batchOrderNosString);

    var defaultSenderAddress = "";
    var defaultCustomerAddress = "";
    var fromUser = "";
    var toUser = "";
    for (var i = 0; i < batchOrderIds.length; i++) {
        var row = grid.dataMatrix("getRow", batchOrderIds[i]);
        var saddress = row.StartPosition.trim();
        var caddress = row.EndPosition.trim();
        var fromU = row.FromUser;
        var toU = row.ToUser;
        if (saddress != '') {
            defaultSenderAddress = row.StartPosition;
        }

        if (caddress != '') {
            defaultCustomerAddress = row.EndPosition;
        }

        if (defaultSenderAddress != '' && defaultCustomerAddress != '') {
            break;
        }
    }

    $("#StartPosition").val(defaultSenderAddress);
    $("#EndPosition").val(defaultCustomerAddress);
}

Order.Refresh = function () {
    $("#originalOrderGrid").data("uiDataMatrix").reloadGrid();
}