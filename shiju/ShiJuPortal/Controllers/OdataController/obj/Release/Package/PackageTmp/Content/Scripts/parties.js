var Global = Global || {};
Global.FormMethod = "post";
Global.EditingEntityId = emptyGuid;
Global.FoodTypeID = emptyGuid;
Global.EditingPartyCommentEntityId = emptyGuid;
var MessageType = {
    DeleteConfirm: 0,
    EnableConfirm: 1
}
var isCreated = true;
Global.PartyCommentListGridId = '';
var commentListGridId = '';
var messageType = MessageType.DeleteConfirm;
Global.DeleteingEntityId = emptyGuid;
var defaltDate = new Date();
var defaltStrDate = defaltDate.getFullYear() + "/" + (defaltDate.getMonth() + 1) + "/" + defaltDate.getDate();
$(function () {
    var form = $("#typeform");
    var validator = form.validate({
        errorElement: "span",
        rules: {
            "Title": {
                required: true,
                maxlength: 15
            },
            "LikeCount": {
                required: true,
                number: true,
                maxlength: 10
            },
            "CommentCount": {
                required: true,
                number: true,
                maxlength: 10
            },
            "MaxUserCount": {
                number: true,
                required: true,
                maxlength: 10
            },
            "Description": {
                required: true,
                maxlength: 30
            },
            "Address": {
                required: true,
                maxlength: 30
            },
        },
        messages: {
            "Title": {
                required: "请输入主题",
                maxlength: $.validator.format("最多不能超过 {0} 字.")
            },
            "Description": {
                required: "请输入简介",
                maxlength: $.validator.format("最多不能超过 {0} 字.")
            },
            "LikeCount": {
                required: "请输入数量",
                maxlength: $.validator.format("最多不能超过 {0} 字."),
                number: "只能包含数字",
            },
            "CommentCount": {
                required: "请输入数量",
                maxlength: $.validator.format("最多不能超过 {0} 字."),
                number: "只能包含数字",
            },
            "MaxUserCount": {
                required: "请输入数量",
                maxlength: $.validator.format("最多不能超过 {0} 字."),
                number: "只能包含数字",
            },
            "Address": {
                required: "请输入地址",
                maxlength: $.validator.format("最多不能超过 {0} 字.")
            }
        },
        errorClass: "error"
    });

    var partiesDialog = $("#partiesDialog").dialog({
        autoOpen: false,
        width: 1200,
        height: 650,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'foodType-ok',
                text: '确定',
                click: function () {
                    submitFoodType();
                    //$(this).dialog("close");
                }
            },
            {
                id: 'foodType-cancel',
                text: '取消',
                click: function () {
                    //$(this).dialog("refresh");
                    $(this).dialog("close");
                }

            }
        ],
        open: function () {
            $("#foodType-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#foodType-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#foodType-ok").blur();
            $("#foodType-cancel").blur();
            validator.resetForm();
            InitializeSiverlight('multipleSilverLight', 'Images', true);
            InitializeSiverlight('voteSilverLight1', 'voteImageId1', false);
        }

    });

    $("#btn_create").on("click", function () {
        var date = new Date();
        Global.FormMethod = "post";
        $(".voteTroggle").show();
        //$(".Sponsor").show();
        $(".participantCount").hide();
        $(".expireCount").show();
        // initFoodTypePositionSelection(true);  
        $("#Images").val("");
        $("#Title").val("");
        $("#Sponsor").val("");
        $("#addBeginTime").val(date.format("yyyy-MM-dd HH:mm"));
        $("#addEndTime").val(date.format("yyyy-MM-dd HH:mm"));
        $("#LikeCount").val("");
        $("#CommentCount").val("");
        $("#ParticipantCount").val("");
        $("#MaxUserCount").val("");
        $("#VoteChoicesJson").val("");
        $("input[name='IsDisabled']").val("");
        $("#Address").val("");
        $("#Description").val("");
        $("#createdtime").val("");
        $("#isHot1").prop("checked", "checked");
        $("#isEnableType1").prop("checked", "checked");
        document.getElementById("imageValidate").style.display = "none";
        $("#partiesDialog").dialog("open").dialog({ "title": "创建活动" });
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
                            disableFoodType(Global.DeleteingEntityId);
                            break;
                        case MessageType.EnableConfirm:
                            enableFoodType(Global.EditingEntityId);
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

function submitFoodType() {
    var form = $("#typeform");
    form.submit(function (event) { event.preventDefault(); });
    form.submit();
    var validator = form.validate();
    if (validator.numberOfInvalids() <= 0) {
        var url = getRawUrl("odata/PartiesOData");
        if (Global.FormMethod == 'put') {
            url += "(" + Global.EditingEntityId + ")";
        } else {
            var jsonArray = new Array();
            var count = parseInt($("#VoteCount").val());
            for (var i = 0; i < count; i++) {
                voteJsonObject = {
                    Text: $("#vote" + (i + 1)).val(),
                    ImageId: $("#voteImageId" + (i + 1)).val(),
                    Position: (i + 1)
                }
                jsonArray[i] = voteJsonObject;
            }
            var voteChoiseJson = JSON.stringify(jsonArray);
            $("#VoteChoicesJson").val(voteChoiseJson);
        }
        var myDate = new Date();
        var nowTime = myDate.toLocaleString();
        // alert($("input[name='IsEnable']").prop("checked"));
        if ($("#Createdtime").val() == "") {
            $("#Createdtime").val(nowTime);
        }
        //var foodType = {
        //    Id: Global.EditingEntityId,
        //    Name: $("#name").val(),
        //    Description: $("#description").val(),
        //    //IsEnabled:true,
        //    Enabled: $("input[name='Enable']").prop("checked"),
        //    CreatedTime: $("#createdtime").val()
        //};
        var data = $("#typeform").serialize();

        //说明通过了 
        $.ajax({
            url: url,
            data: data,
            type: Global.FormMethod,
            async: false,
            success: function (data) {
                $("#partiesDialog").dialog("close");
                refreshFoodTypeGrid();
            }
        });
    }

}
function editParty(row) {
    $(".voteTroggle").hide();
    $(".expireCount").hide();
   // $(".Sponsor").hide();
    $(".participantCount").show();

    $("#Id").val(row.Id);
    $("#Images").val(row.Images);
    $("#Title").val(row.Title);
    $("#Sponsor").val(row.Sponsor);
    //var beginDate = Date.parse(row.BeginTime.replace('T', ' ').replace(/-/g, "/"));
    $("#addBeginTime").val(row.BeginTime.replace('T', ' ').substring(0, row.EndTime.length - 3));

    $("#addEndTime").val(row.EndTime.replace('T', ' ').substring(0, row.EndTime.length-3));

    $("#LikeCount").val(row.LikeCount);
    $("#CommentCount").val(row.CommentCount);

    $("#ParticipantCount").val(row.ParticipantCount);

    $("#MaxUserCount").val(row.MaxUserCount);

    $("#VoteChoicesJson").val(row.VoteChoisesJson);

    //$("input[name='IsDisabled']").val(row.IsDisabled);
    //$("input[name='IsHot']").val(row.IsHot);
    $("#Address").val(row.Address);
    $("#Description").val(row.Description);

    $("#Createdtime").val(row.CreatedTime);
    if (!row.IsHot) {
        // $("input[name='IsEnable']").attr("checked", 'false');
        $("#isHot2").prop("checked", "checked");
        //$("input[name='Enable']").prop("checked", 'false');
    } else {
        $("#isHot1").prop("checked", "checked");
    }
    if (!row.IsDisabled) {
        // $("input[name='IsEnable']").attr("checked", 'false');
        $("#isEnableType2").prop("checked", "checked");
        //$("input[name='Enable']").prop("checked", 'false');
    } else {
        $("#isEnableType1").prop("checked", "checked");
    }

    var url = getRawUrl("GetParticipantCount");
    $.ajax({
        url: url + "?partyId=" + row.Id,
        async: false,
        success: function (data) {
            $("#ParticipantCount").val(data.Description);
        }
    });


    $("#partiesDialog").dialog("open").dialog({ "title": "编辑活动" });
}

function disableFoodType(id) {
    var url = getRawUrl("odata/FoodTypes(" + id + ")");
    $.ajax({
        url: url,
        type: 'delete',
        async: false,
        success: function (data) {
            refreshFoodTypeGrid();
        }
    });
}

function enableFoodType(id) {
    var url = getRawUrl("odata/FoodTypes(" + id + ")");
    $.ajax({
        url: url,
        type: 'patch',
        async: false,
        success: function (data) {
            refreshFoodTypeGrid();
        }
    });
}



function showMessage(msgType) {
    var message = "";
    messageType = msgType;
    switch (msgType) {
        case MessageType.DeleteConfirm:
            message = "禁用分类？"; break;
        case MessageType.EnableConfirm:
            message = "启用分类？"; break;
        default:
            message = "无消息提醒";
    }
    $("#message").html(message);
    $("#MessageDialog").dialog("open");
}

function showDeleteMessage(id) {
    if (confirm("确认删除该评论吗？")) {
        var url = getRawUrl("odata/PartyCommentsOData(" + id + ")");
        $.ajax({
            url: url,
            type: 'delete',
            async: false,
            success: function (data) {
                refreshgridCommentId(commentListGridId);
            }
        });
    }
}

function initPartyCommentPositionSelection(isCreate, foodtypeid) {
    var url = getRawUrl("PartyComments/Count?foodtypeid=" + Global.FoodTypeID);
    $.ajax({
        url: url,
        async: false,
        cache: false,
        success: function (response) {
            var count = response.Count;
            if (isCreate) {
                count += 1;
            }
            var html = "";
            for (var i = 1; i <= count; i++) {
                html += "<option value='" + i + "'>" + i + "</option>";
            }

            $("#position").html(html);
        }
    });
}

function refreshFoodTypeGrid() {
    $.ajaxSetup({ async: false });
    refreshGrid('paties');
    $.ajaxSetup({ async: true });
    for (var i = 0; i < $("#paties-grid tr").length; i++) {
        var td = $("td:nth(2)", $("#paties-grid tr")[i]);
        td.prop("title", td.text());
    }
}


$(function () {
    var commentsDialog = $("#commentsDialog").dialog({
        autoOpen: false,
        width: 800,
        height: 600,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'comment-distribution-cancel',
                text: '关闭',
                click: function () {
                    $(this).dialog("close");
                }

            }
        ],
        open: function () {
            $("#comment-distribution-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#comment-distribution-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#comment-distribution-ok").blur();
            $("#comment-distribution-cancel").blur();
            document.getElementById("imageValidate").style.display = "none";
        }
    });

    //$("#btn_create").on("click", function () {
    //    Global.FormMethod = "post";
    //    //initPartyCommentPositionSelection(true);  
    //    $("#name").val("");
    //    $("#description").val("");
    //    $("#createdtime").val("");
    //    $("#partiesDialog").dialog("open");

    //});
})

function showComments(row) {
    $.ajaxSetup({ async: false });
    Global.FoodTypeID = row.Id;
    $("#btn_create_comment").attr("href_foodTypeId", row.Id);
    var now = new Date();
    var gridPartyCommentId = "batchOrderGrid" + now.getHours() + now.getMinutes() + now.getSeconds() + now.getMilliseconds();
    $("#comments").html("<div id='" + gridPartyCommentId + "' />");
    commentListGridId = gridPartyCommentId;
    $("#" + gridPartyCommentId).dataMatrix({
        oDataUrl: baseServerUrl + "odata/PartyCommentsOData",
        extraRequestData: { partyId: row.Id },
        implementationType: "bootGrid",
        isRowNumbersVisible: true,
        properties: [
            { type: "datetime", name: "CreatedTime", displayName: "创建时间", isColumnHidden: true, isSortable: false, isSearchable: false },
            { type: "string", name: "Id", displayName: "Id", isEditable: false, isKey: true, isColumnHidden: true, isSearchable: false, isSortable: false },
            { type: "string", name: "UserName", displayName: "姓名", columnWidth: 120, isSearchable: true, isSortable: true },
            { type: "string", name: "Text", displayName: "评论", columnWidth: 240, isSearchable: true, isSortable: true },
            { type: "string", name: "AudioId", displayName: "语音", columnWidth: 240, isSearchable: false, isSortable: true },
            { type: "action", displayName: "操作", columnWidth: 240, isSortable: false, isSearchable: false },
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
        defaultSortPropertyName: "Position",
        actionButtons: [
              {
                  displayName: "删除",
                  buttonType: DataMatrixRowActionButtonType.Link,
                  onClick: function (button, rowId) {
                      var row = $("#" + gridPartyCommentId).dataMatrix("getRow", rowId);
                      Global.DeleteingEntityId = rowId;
                      showDeleteMessage(rowId);
                  }
              },
        ],
    });


    $("#commentsDialog").dialog("open").dialog({ "title": row.Name });
    $.ajaxSetup({ async: true, cache: true });


    $.ajaxSetup({ async: true })
    for (var i = 0; i < $("#" + gridPartyCommentId + "-grid tr").length; i++) {
        var td = $("td:nth(3)", $("#" + gridPartyCommentId + "-grid tr")[i]);
        td.prop("title", td.text());
    }
}

$(function () {

    var form = $("#itemform");
    var validator = form.validate({
        errorElement: "span",
        rules: {
            "itemname": {
                required: true,
                maxlength: 15
            },
            "itemdescription": {
                required: true,
                maxlength: 100

            }
        },
        messages: {
            "itemname": {
                required: "请输入名称",
                maxlength: $.validator.format("最多不能超过 {0} 字.")

            },
            "itemdescription": {
                required: "请输入简介",
                maxlength: $.validator.format("最多不能超过 {0} 字.")

            }
        }
    });

    var commentsDialog = $("#commentsDialog").dialog({
        autoOpen: false,
        width: 1000,
        height: 700,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'comment-ok',
                text: '确定',
                click: function () {
                    submitPartyComment();
                    //$(this).dialog("close");
                    //refreshPartyCommentGrid();
                }
            },
            {
                id: 'comment-cancel',
                text: '取消',
                click: function () {
                    $(this).dialog("close");
                    //validator.resetForm();
                }

            }
        ],
        open: function () {
            $("#comment-ok").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#comment-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#comment-ok").blur();
            $("#comment-cancel").blur();
            validator.resetForm();
            InitializeSiverlight('mainImageSilverLight', 'commentMainImageId', false);
            document.getElementById("imageValidate").style.display = "none";
        },
    });

    $("#btn_create_comment").on("click", function () {
        Global.FormMethod = "post";
        initPartyCommentPositionSelection(true, Global.FoodTypeID);
        //alert($("#btn_create_comment").attr("href_foodTypeId"));
        $("#foodTypeId").val($("#btn_create_comment").attr("href_foodTypeId"));
        $("#commentMainImageId").val("");
        $("#name_comment").val("");
        $("#description_comment").val("");
        $("#createdtime_comment").val("");
        $("#ui-id-4").html("添加菜品");
        $("#commentsDialog").dialog("open");
    });
})

function submitPartyComment() {
    var url = getRawUrl("odata/PartyComments");
    if (Global.FormMethod == 'put') {
        url += "(" + Global.EditingPartyCommentEntityId + ")";
    }

    if ($("#commentMainImageId").val() == "" || $("#commentMainImageId").val() == "00000000000000000000000000000000") {
        document.getElementById("imageValidate").style.display = "block";
        // $("#imageValidate").style.display = "block";
        return false;
    }
    var myDate = new Date();
    var nowTime = myDate.toLocaleString();
    // alert($("input[name='IsEnable']").prop("checked"));
    if ($("#createdtime_comment").val() == "") {
        $("#createdtime_comment").val(nowTime);
    }
    var comment = {
        Id: Global.EditingPartyCommentEntityId,
        Name: $("#name_comment").val(),
        Description: $("#description_comment").val(),
        MainImageId: $("#commentMainImageId").val(),
        FoodTypeId: $("#foodTypeId").val(),
        Position: $("#position option:selected").val(),
        //IsEnabled:true,
        Enabled: $("input[name='EnablePartyComment']").prop("checked"),
        CreatedTime: $("#createdtime_comment").val()
    };


    var form = $("#itemform");
    form.submit(function (event) { event.preventDefault(); });
    form.submit();
    var validator = form.validate();
    if (validator.numberOfInvalids() <= 0) { //说明通过了

        $.ajax({
            url: url,
            data: comment,
            type: Global.FormMethod,
            async: false,
            success: function (data) {
                $("#commentsDialog").dialog("close");
                $.ajaxSetup({ async: false })
                refreshgridCommentId(commentListGridId);
                $.ajaxSetup({ async: true });
                for (var i = 0; i < $("#" + gridPartyCommentId + "-grid tr").length; i++) {
                    var td = $("td:nth(2)", $("#" + gridPartyCommentId + "-grid tr")[i]);
                    td.prop("title", td.text());
                }
            }
        });
    }
}

function editPartyComment(row_comments) {
    initPartyCommentPositionSelection(false, Global.FoodTypeID);
    $("#commentMainImageId").val(row_comments.MainImageId);
    $("#name_comment").val(row_comments.Name);
    $("#foodTypeId").val(row_comments.FoodTypeId);
    $("#description_comment").val(row_comments.Description);
    $("input[name='Enable']").val(row_comments.Enabled);
    $("#createdtime_comment").val(row_comments.CreatedTime);
    if (!row_comments.Enabled) {
        $("#isEnableItem_2").prop("checked", "checked");
    } else {
        $("#isEnableItem_1").prop("checked", "checked");
    }
    $("#position").val(row_comments.Position);
    $("#ui-id-4").html("编辑菜品");
    $("#commentsDialog").dialog("open");
}

function refreshgridCommentId(commentListGridId) {
    refreshGrid(commentListGridId);
}
