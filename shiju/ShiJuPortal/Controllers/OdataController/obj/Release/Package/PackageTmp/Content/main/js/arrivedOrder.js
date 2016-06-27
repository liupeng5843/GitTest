var ArrivedOrder = {} || ArrivedOrder;
$(function () {
    var arrivedOrderDialog = $("#arrivedOrderDialog").dialog({
        title: "订单",
        autoOpen: false,
        width: 1200,
        height: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'arrivedOrder-distribution-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
        open: function () {
            $("#arrivedOrder-distribution-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#arrivedOrder-distribution-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#arrivedOrder-distribution-ok").blur();
            $("#arrivedOrder-distribution-cancel").blur();
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
});

ArrivedOrder.Detail = function (batchId, batch) {
    $.ajaxSetup({ async: false });
    $("#arrivedOrderDialog").dialog("open").dialog({ "title": batch.BatchNo });

    var now = new Date();
    var batchOrderGridId = "batchOrderGrid" + now.getHours() + now.getMinutes() + now.getSeconds() + now.getMilliseconds();
    $("#arrivedOrderDialog").html("<div id='" + batchOrderGridId + "' />");
    ArrivedOrder.BatchOrderGridId = batchOrderGridId;

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
            { type: "string", name: "ToUser", displayName: "客户姓名", isSortable: true },
            //{ type: "string", name: "ToUser", displayName: "派送司机", isSortable: false },
            { type: "string", name: "PlanSendTimeText", displayName: "计划发货时间", isSortable: false, isSearchable: false },
            //{ type: "enum", typeOptions: { "enumValues": { 0: "直送", 1: "零担" } }, name: "SendType", displayName: "配送方式", isEditable: false, isColumnHidden: true },
            { type: "string", name: "PlanReceiveTimeText", displayName: "计划到达时间", isSortable: false, isSearchable: false },
            { type: "string", name: "CreatedTimeText", displayName: "创建时间", isSortable: false, isSearchable: false },
            { type: "string", name: "ArriveTimeText", displayName: "实际到达时间", isSortable: false, isSearchable: false },
            { type: "string", name: "Comment", displayName: "评价内容" },
            { type: "string", name: "Grade", displayName: "评分", isSearchable: false },
            { type: "imagehub", typeOptions: { imageUploadKitUrl: imageUploadKit, baseUrl: imageHubUrl, isMultiple: false, appIdOrName: 'mg' }, name: "ImageId", displayName: "回单", columnWidth: 100, isSortable: false, isSearchable: false }
            
            //{ type: "enum", typeOptions: { "enumValues": { 0: "待配送", 1: "已打包", 2: "配送中", 4: "已中转", 16: "已完成" } }, name: "Status", displayName: "订单状态", isEditable: false },
            //{ type: "action", displayName: "操作", columnWidth: 240, isSortable: false },
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
            {
                displayName: "删除",
                buttonType: DataMatrixRowActionButtonType.Link,
                onClick: function (button, rowId) {
                    var order = $("#" + batchOrderGridId).dataMatrix("getRow", rowId);
                    BatchOrder.Delete(rowId, order);
                }
            },
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

    var selector = "#" + batchOrderGridId + " td img";
    $(selector).on("click", function () {
        var lilImageUrl = $(this).prop("src");
        var bigImageUrl = lilImageUrl.substr(0, lilImageUrl.length - 14) + "_5_1_1.jpg";
        showLargeImage(bigImageUrl);
    });

    $.ajaxSetup({ async: true });
}

ArrivedOrder.PresentMoments = function (batchId, batch) {
    $("#batchMomentDialog").dialog("open").dialog({ "title": batch.BatchNo + "-消息" });
    var now = new Date();
    var batchMomentDialog = "batchMomentDialog" + now.getHours() + now.getMinutes() + now.getSeconds() + now.getMilliseconds();
    ArrivedOrder.MomentGridId = batchMomentDialog;
    $("#batchMomentDialog").html("<div id='momentList' />");

    $("#" + batchMomentDialog).dataMatrix({
        oDataUrl: baseServerUrl + "odata/BatchMomentOData",
        implementationType: "bootGrid",
        extraRequestData: { batchId: batchId },
        properties: [
            { type: "string", name: "Id", displayName: "ID", isEditable: false, isKey: true, isColumnHidden: true, isSearchable: false, isSortable: false },
            { type: "string", name: "Text", displayName: "消息", isSortable: false },
            { type: "imagehub", typeOptions: { imageUploadKitUrl: imageUploadKit, baseUrl: imageHubUrl, isMultiple: true, appIdOrName: 'mg' }, name: "ImageList", displayName: "回单", columnWidth: 100, isSortable: false },
            { type: "enum", typeOptions: { "enumValues": { "0": "显示", "-1": "隐藏", "-2": "删除" } }, name: "Status", displayName: "消息状态", isEditable: false },
            { type: "datetime", name: "CreatedTime", displayName: "创建时间", isSortable: false },
            { type: "action", displayName: "操作", columnWidth: 240, isSortable: false },
        ],
        isHeaderBarShow: true,
        rowCountPerPage: 40,
        rowCountsPerPage: [40, 50, 100, -1],
        isRowNumbersVisible: true,
        isCreateButtonVisible: false,
        isEditButtonVisible: false,
        isDeleteButtonVisible: false,
        defaultSortAscending: true,
        isMultipleSelection: false,
        isColumnChooserEnabled: true,
        actionButtons: [
            {
                displayName: "隐藏",
                buttonType: DataMatrixRowActionButtonType.Link,
                onClick: function (button, rowId) {
                    ArrivedOrder.HideMoment(rowId);
                }
            },
            {
                displayName: "显示",
                buttonType: DataMatrixRowActionButtonType.Link,
                onClick: function (button, rowId) {
                    ArrivedOrder.NormalMoment(rowId);
                }
            }
        ],
    });

    //the following code is to modify the moment layout from list to timeline
    loadMoments(batchId, 'momentList', "ArrivedOrder");
}

ArrivedOrder.FillMoments = function (batchId) {
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

ArrivedOrder.HideMoment = function (momentId) {
    var confirmed = window.confirm("确定隐藏此条消息吗？");
    if (!confirmed) {
        return false;
    }

    ArrivedOrder.ChangeMoment(momentId, -1);
}

ArrivedOrder.NormalMoment = function (momentId) {
    var confirmed = window.confirm("确定显示此条消息吗？");
    if (!confirmed) {
        return false;
    }

    ArrivedOrder.ChangeMoment(momentId, 0);
}

ArrivedOrder.ChangeMoment = function (momentId, status)
{
    var hideMomentUrl = baseServerUrl + "odata/BatchMomentOData(" + momentId + ")?status=" + status;
    $.ajax({
        url: hideMomentUrl,
        type: 'PUT',
        success: function (data) {
            ArrivedOrder.RefreshMoment();
        }
    });
}

ArrivedOrder.RefreshMoment = function ()
{
    //$("#" + ArrivedOrder.MomentGridId).data("uiDataMatrix").reloadGrid();
    loadMoments(ArrivedOrder.batchId, "momentList", "ArrivedOrder");
}