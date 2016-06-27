//the following code is to modify the moment layout from list to timeline
function loadMoments(batchId, momentListId, currentPageBatchObjectName) {
    $.ajax({
        url: baseServerUrl + "odata/BatchMomentOData?batchId=" + batchId,
        type: 'get',
        dataType: 'json',
        success: function (data, textStatus, jqXhr) {
            var moments = data.value;
            var html = '<table id="momentListTable"><tr><th width=200>消息</th><th width=200>图片</th><th width=200>地点</th><th width=200>时间</th><th width=100>隐藏/显示</th><th>操作</th></tr>';
            for (var i = 0; i < moments.length; i++) {
                var moment = moments[i];
                html += "<tr>";
                html += "<td>" + moment.Text + "</td>";

                //add image
                html += "<td>";
                if (moment.AttachmentsJson.trim() != '') {
                    var attachmentJson = JSON.parse(moment.AttachmentsJson);
                    if (attachmentJson.Mime == '.jpg') {
                        var attachmentUrl = getAttachmentUrl(batchId, attachmentJson.Id);
                        html += "<img src='" + attachmentUrl + "' with=50 height=100 style='cursor:pointer;' onclick='javascript:showLargeImage(\"" + attachmentUrl + "\");' />";
                    }
                    else {
                        html += "无图";
                    }
                    html += "</td>";
                }
                else {
                    html += "无图";
                    html += "</td>";
                }

                html += "<td>" + moment.Place + "</td>";
                html += "<td>" + moment.CreatedTimeText + "</td>";
                var isHiddenMoment = (moment.Status == -1);//-1 as forbidden to user, 0 as normal, -2 as deleted
                //add status
                html += "<td>"
                if (isHiddenMoment) {
                    html += "隐藏";
                }
                else {
                    html += "显示";
                }
                html += "</td>";

                //add button
                var buttonHtml = "";
                html += "<td>"
                if (isHiddenMoment) {
                    buttonHtml += "<a href='javascript:" + currentPageBatchObjectName + ".NormalMoment(\"" + moment.Id + "\");'>显示消息</a>";
                }
                else {
                    buttonHtml += "<a href='javascript:" + currentPageBatchObjectName + ".HideMoment(\"" + moment.Id + "\")'>隐藏消息</a>";
                }
                html += buttonHtml;
                html += "</td>";

                html += "</tr>";
            }
            html += "</table>";

            $("#" + momentListId).html(html);
        },
        error: function (jqXhr, textStatus, errorThrow) {
            console.log(errorThrow);
        }
    });
}