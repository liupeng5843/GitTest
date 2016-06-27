var Batch = {} || Batch;
var BatchOrder = {} || BatchOrder;
var BatchDispatch = {} || BatchDispatch;
Batch.BatchGridId = "batchGrid";
$(function () {
    var batchorderDialog = $("#batchorderdialog").dialog({
        title: "订单",
        autoOpen: false,
        width: 1200,
        height: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-distribution-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-distribution-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#batch-distribution-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-distribution-ok").blur();
            $("#batch-distribution-cancel").blur();
        }
    });

    var batchdispatchdialog = $("#batchdispatchdialog").dialog({
        title: "司机搜索",
        autoOpen: false,
        width: 400,
        height: 400,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-driver-ok',
                text: '确认',
                click: function () {
                    Batch.Dispatch();
                }
            },
            {
                id: 'batch-driver-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-driver-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#batch-driver-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-driver-ok").blur();
            $("#batch-driver-cancel").blur();

            Batch.GetDrivers("");
        }
    });

    //change batch status dialog
    var batchstatusdialog = $("#batchstatusdialog").dialog({
        title: "状态修改",
        autoOpen: false,
        width: 400,
        height: 300,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-status-ok',
                text: '确认',
                click: function () {
                    var status = $("input[name='batchStatus']:checked").val();
                    Batch.ChangeStatus(BatchDispatch.BatchId, status);
                    $(this).dialog("close");
                }
            },
            {
                id: 'batch-status-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-status-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#batch-status-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-status-ok").blur();
            $("#batch-status-cancel").blur();

            $("#batchStatus" + BatchDispatch.Status).prop("checked", true);
            $("input[name='batchStatus']").blur();
        }
    });

    //change batch order status dialog
    var batchorderstatusdialog = $("#batchorderstatusdialog").dialog({
        title: "状态修改",
        autoOpen: false,
        width: 400,
        height: 300,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-order-status-ok',
                text: '确认',
                click: function () {
                    var status = $("input[name='batchOrderStatus']:checked").val();
                    BatchOrder.ChangeStatus(BatchOrder.OrderId, status);
                    $(this).dialog("close");
                }
            },
            {
                id: 'batch-order-status-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-order-status-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#batch-order-status-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-order-status-ok").blur();
            $("#batch-order-status-cancel").blur();

            $("#batchOrderStatus" + BatchOrder.OrderStatus).prop("checked", true);
            $("input[name='batchOrderStatus']").blur();
        }
    });

    //batch moment dialog
    var batchMomentDialog = $("#batchMomentDialog").dialog({
        title: "订单包消息",
        autoOpen: false,
        width: 1200,
        height: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'batch-moment-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#batch-moment-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#batch-moment-cancel").blur();
        }
    });


    $("#searchDriverText").on("keyup", function () {
        var driverName = $(this).val();
        Batch.GetDrivers(driverName);
    });

    $(".datetimeSearchInput").datetimepicker({
        dateFormat: 'yy-mm-dd', timeFormat: 'HH:mm', minDateTime: new Date(1900, 0, 1), maxDateTime: new Date(2099, 12, 31), showOn: 'focus', showButtonPanel: true, changeMonth: true, changeYear: true
    });

    //workaround for raising z-index of datepicker;
    $(".datetimeSearchInput").on("mouseup", function () {
        $("#ui-datepicker-div").css("z-index", 99);
    });

});

Batch.SearchByTime = function (str) {
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
    //var baseUrl = location.origin + location.pathname + "?menu=" + menu;
    var baseUrl = location.pathname + "?menu=" + menu;
    if (orderSendStartTimeSearch.trim() == '' && orderSendEndTimeSearch.trim() == '') {
        url = baseUrl;
    }
    else {
        url = baseUrl + "&orderSendStartTime=" + orderSendStartTimeSearch + "&orderSendEndTime=" + orderSendEndTimeSearch + "&statusCombination=" + str;
    }
    window.location.href = url;
}

Batch.Detail = function (batchId, batch) {
    $.ajaxSetup({ async: false });
    $("#batchorderdialog").dialog("open").dialog({ "title": batch.BatchNo });

    var now = new Date();
    var batchOrderGridId = "newsListGrid" + now.getHours() + now.getMinutes() + now.getSeconds() + now.getMilliseconds();
    $("#batchorderdialog").html("<div id='" + batchOrderGridId + "' />");
    Batch.BatchOrderGridId = batchOrderGridId;

    $("#" + batchOrderGridId).dataMatrix({
        oDataUrl: baseServerUrl + "odata/BatchOrderOData",
        implementationType: "bootGrid",
        extraRequestData: { batchId: batchId },
        properties: [
            { type: "string", name: "Id", displayName: "ID", isEditable: false, isKey: true, isColumnHidden: true, isSearchable: false, isSortable: false },
            { type: "string", name: "OrderNo", displayName: "订单号" },
            { type: "string", name: "StartPosition", displayName: "发货地" },
            { type: "string", name: "EndPosition", displayName: "目的地" },

            //{ type: "string", name: "FromUser", displayName: "发送者姓名", isSortable: false },
            //{ type: "string", name: "ToUser", displayName: "客户姓名", isSortable: false },
            { type: "string", name: "PlanSendTimeText", displayName: "计划发货时间", isSortable: false, isSearchable: false },
            { type: "string", name: "PlanReceiveTimeText", displayName: "计划到达时间", isSortable: false, isSearchable: false },
            { type: "enum", typeOptions: { "enumValues": { 0: "直送", 1: "零担" } }, name: "SendType", displayName: "配送方式", isEditable: false, isSortable: false, isSearchable: false, isColumnHidden: false },
            //{ type: "string", name: "PlanReceiveDate", displayName: "需求到货日期", isSortable: false },

            { type: "string", name: "CreatedTimeText", displayName: "创建时间", isSortable: false, isSortable: false, isSearchable: false },
            { type: "enum", typeOptions: { "enumValues": { 0: "待配送", 1: "已打包", 2: "配送中", 4: "已中转", 8: "待确认", 16: "已完成" } }, name: "Status", displayName: "订单状态", isEditable: false, isSortable: false, isSearchable: false },
            //{ type: "string", name: "Comment", displayName: "评价内容" },
            //{ type: "string", name: "Grade", displayName: "评分" },
            { type: "imagehub", typeOptions: { imageUploadKitUrl: imageUploadKit, baseUrl: imageHubUrl, isMultiple: false, appIdOrName: 'mg' }, name: "ImageId", displayName: "回单", columnWidth: 100, isSortable: false, isColumnHidden: isColumnHidden, isSearchable: false },
            { type: "action", displayName: "操作", columnWidth: 240, isSortable: false, isColumnHidden: isColumnHidden, isSearchable: false },
        ],
        isHeaderBarShow: true,
        rowCountPerPage: 40,
        rowCountsPerPage: [40, 50, 100, -1],
        isRowNumbersVisible: true,
        isCreateButtonVisible: false,
        isEditButtonVisible: false,
        isDeleteButtonVisible: false,
        //defaultSortPropertyName: 'Position',
        defaultSortAscending: true,
        isMultipleSelection: false,
        isColumnChooserEnabled: true,
        actionButtons: [
            //{
            //    displayName: "删除",
            //    buttonType: DataMatrixRowActionButtonType.Link,
            //    onClick: function (button, rowId) {
            //        var order = $("#" + batchOrderGridId).dataMatrix("getRow", rowId);
            //        BatchOrder.Delete(rowId, order);
            //    }
            //},
            {
                displayName: "状态",
                buttonType: DataMatrixRowActionButtonType.Link,
                onClick: function (button, rowId) {
                    var order = $("#" + batchOrderGridId).dataMatrix("getRow", rowId);
                    BatchOrder.OrderId = rowId;
                    BatchOrder.OrderStatus = order.Status;
                    $("#batchorderstatusdialog").dialog("open");
                }
            }
        ],
    });
    BatchOrder.BindShowLarge();

    $.ajaxSetup({ async: true });
}

Batch.Cancel = function (batchId) {
    var okay = window.confirm("确认解除此包吗？")
    if (!okay) {
        return false;
    }

    var url = baseServerUrl + "odata/BatchOData(" + batchId + ")";
    $.ajax({
        url: url,
        type: 'DELETE',
        async: false,
        success: function (data) {
            Batch.Refresh();
        },
        error: function (jqXhr, textStatus, errorThrow) {
            if (jqXhr.status == 505) {
                var responseMessage = jqXhr.responseJSON.value;
                alert(responseMessage);
                Batch.Refresh();
                return false;
            }
        }
    });
}

Batch.Dispatch = function () {
    var driverId = $("#driversDropdownList").val();
    if (driverId == '0') {
        alert("请选择派送司机");
        return false;
    }

    var batchId = BatchDispatch.BatchId;
    var url = baseServerUrl + "odata/BatchDispatchOData?batchId=" + batchId + "&driverId=" + driverId;
    $.ajax({
        url: url,
        type: 'POST',
        async: false,
        success: function (data) {
            $("#batchdispatchdialog").dialog("close");
            alert("派发成功！");
            Batch.Refresh();
        }
    });
}

Batch.GetDrivers = function (keyword) {
    var filter = "?$filter=substringof('" + keyword
        + "',UserName) eq true or substringof('"
        + keyword + "',PhoneNumber) eq true";

    var url = baseServerUrl + "odata/DriverOData";
    if (keyword.trim() != '') {
        url += filter;
    }

    $.ajax({
        url: url,
        type: 'GET',
        async: false,
        success: function (data) {
            var drivers = data.value;
            var driversDropdownListHtml = "";
            for (var i = 0; i < drivers.length; i++) {
                var driver = drivers[i];
                driversDropdownListHtml += "<option value='" + driver.UserId + "'>" + driver.UserName + " " + driver.PhoneNumber + "</option>";
            }
            if (drivers.length == 0) {
                driversDropdownListHtml += "<option value='0'>未查询到任何结果！</option>";
            }
            $("#driversDropdownList").html(driversDropdownListHtml);
        }
    });
}

Batch.ChangeStatus = function (batchId, status) {
    var url = baseServerUrl + "ChangeStatus/Batch?key=" + batchId + "&status=" + status;
    $.ajax({
        url: url,
        type: 'post',
        async: false,
        success: function () {
            Batch.Refresh();
        },
        error: function () {
            alert("修改状态失败");
        }
    });
}

Batch.Refresh = function () {
    $("#" + Batch.BatchGridId).data("uiDataMatrix").reloadGrid();
}

BatchOrder.Delete = function (rowId, order) {
    var okay = window.confirm("确认删除订单“" + order.OrderNo + "”？");
    if (!okay) {
        return false;
    }

    var url = baseServerUrl + "odata/BatchOrderOData(" + order.Id + ")";
    var method = 'DELETE';
    $.ajax({
        url: url,
        type: method,
        async: false,
        success: function (data) {
            BatchOrder.Refresh();
        },
        error: function () { }
    });
}

BatchOrder.Refresh = function () {
    $.ajaxSetup({ async: false });

    $("#" + Batch.BatchOrderGridId).data("uiDataMatrix").reloadGrid();
    BatchOrder.BindShowLarge();

    $.ajaxSetup({ async: true });
}

BatchOrder.ChangeStatus = function (orderId, status) {
    var url = baseServerUrl + "ChangeStatus/Order?key=" + orderId + "&status=" + status;
    $.ajax({
        url: url,
        type: 'post',
        async: false,
        success: function () {
            BatchOrder.Refresh();
        },
        error: function () {
            alert("修改状态失败");
        }
    });
}

BatchOrder.BindShowLarge = function () {
    var selector = "#" + Batch.BatchOrderGridId + " td img";
    $(selector).on("click", function () {
        var lilImageUrl = $(this).prop("src");
        var bigImageUrl = lilImageUrl.substr(0, lilImageUrl.length - 14) + "_5_1_1.jpg";
        showLargeImage(bigImageUrl);
    });
}

Batch.PresentMoments = function (batchId, batch) {
    $("#batchMomentDialog").dialog("open").dialog({ "title": batch.BatchNo + "-消息" });
    $("#batchMomentDialog").html("<div id='momentList' />");
    //var now = new Date();
    //var batchMomentDialog = "batchMomentDialog" + now.getHours() + now.getMinutes() + now.getSeconds() + now.getMilliseconds();
    //Batch.MomentGridId = batchMomentDialog;
    //$("#batchMomentDialog").html("<div id='" + batchMomentDialog + "' />");

    //$("#" + batchMomentDialog).dataMatrix({
    //    oDataUrl: baseServerUrl + "odata/BatchMomentOData",
    //    implementationType: "bootGrid",
    //    extraRequestData: { batchId: batchId },
    //    properties: [
    //        { type: "string", name: "Id", displayName: "ID", isEditable: false, isKey: true, isColumnHidden: true, isSearchable: false, isSortable: false },
    //        { type: "string", name: "Text", displayName: "消息", isSortable: false },
    //        { type: "imagehub", typeOptions: { imageUploadKitUrl: imageUploadKit, baseUrl: imageHubUrl, isMultiple: false }, name: "ImageList", displayName: "回单", columnWidth: 100, isSortable: false },
    //        { type: "enum", typeOptions: { "enumValues": { "0": "显示", "-1": "隐藏", "-2": "删除" } }, name: "Status", displayName: "消息状态", isEditable: false },
    //        { type: "datetime", name: "CreatedTime", displayName: "创建时间", isSortable: false },
    //        { type: "action", displayName: "操作", columnWidth: 240, isSortable: false },
    //    ],
    //    isHeaderBarShow: true,
    //    rowCountPerPage: 40,
    //    rowCountsPerPage: [40, 50, 100, -1],
    //    isRowNumbersVisible: true,
    //    isCreateButtonVisible: false,
    //    isEditButtonVisible: false,
    //    isDeleteButtonVisible: false,
    //    defaultSortAscending: true,
    //    isMultipleSelection: false,
    //    isColumnChooserEnabled: true,
    //    actionButtons: [
    //        {
    //            displayName: "隐藏",
    //            buttonType: DataMatrixRowActionButtonType.Link,
    //            onClick: function (button, rowId) {
    //                Batch.HideMoment(rowId);
    //            }
    //        },
    //        {
    //            displayName: "显示",
    //            buttonType: DataMatrixRowActionButtonType.Link,
    //            onClick: function (button, rowId) {
    //                Batch.NormalMoment(rowId);
    //            }
    //        }
    //    ],
    //});
    loadMoments(batchId, 'momentList', "Batch");
}

Batch.FillMoments = function (batchId) {
    var getMomentUrl = baseServerUrl + "odata/BatchMomentOData?batchId=" + batchId;
    $.ajax({
        url: getMomentUrl,
        type: 'GET',
        success: function (data) {
            var moments = data.value;
            var momentsHtml = "<ul>";
            if (moments.length > 0) {
                for (var i = 0; i < moments.length; i++) {

                }
            }

            momentsHtml += "</ul>";

        }
    });
}

Batch.HideMoment = function (momentId) {
    var confirmed = window.confirm("确定隐藏此条消息吗？");
    if (!confirmed) {
        return false;
    }

    Batch.ChangeMoment(momentId, -1);
}

Batch.NormalMoment = function (momentId) {
    var confirmed = window.confirm("确定显示此条消息吗？");
    if (!confirmed) {
        return false;
    }

    Batch.ChangeMoment(momentId, 0);
}

Batch.ChangeMoment = function (momentId, status) {
    var hideMomentUrl = baseServerUrl + "odata/BatchMomentOData(" + momentId + ")?status=" + status;
    $.ajax({
        url: hideMomentUrl,
        type: 'PUT',
        success: function (data) {
            Batch.RefreshMoment();
        }
    });
}

Batch.RefreshMoment = function () {
    //$("#" + Batch.MomentGridId).data("uiDataMatrix").reloadGrid();
    loadMoments(BatchDispatch.BatchId, "momentList", "Batch");
}
