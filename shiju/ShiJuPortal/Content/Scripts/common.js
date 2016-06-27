var accessToken = '';
var corporationIdString = '';
var imageHubServiceBaseUrl = '';
var emptyGuid = '00000000-0000-0000-0000-000000000000';
var Global = Global || {};
//Global.FormMethod = '';
//Global.EditingEntityId = '00000000-0000-0000-0000-000000000000';

function getImageUrl(imageId, scaleType, width, height, format) {
    if (!format) {
        format = 'jpg';
    }
    return imageHubServiceBaseUrl + "/" + imageId + "_" + scaleType + "_" + width + "_" + height + "." + format;
}

function getRawUrl(relativeUrl) {
    return baseServerUrl + relativeUrl;
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

function stringIsNullOrEmpty(str) {
    return str.replace(/' '/g, '').length == 0;
}

function showSuccessToast(message, title) {
    if (title) {
        message = "<h4>" + title + "</h4>&nbsp;" + message;
    }
    $.toast(message, { duration: 3000, sticky: false, type: "success" });
}

function showInfoToast(message, title) {
    if (title) {
        message = "<h4>" + title + "</h4>&nbsp;" + message;
    }
    $.toast(message, { duration: 3000, sticky: false, type: "info" });
}

function showErrorToast(message, title) {
    if (title) {
        message = "<h4>" + title + "</h4>&nbsp;" + message;
    }
    $.toast(message, { duration: 4000, sticky: false, type: "danger" });
}

Date.prototype.Format = function (fmt) {
    var o = {
        "y+": this.getFullYear(),
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "H+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function refreshGrid(gridId)
{
    $("#" + gridId).data("uiDataMatrix").reloadGrid();
}

//Initialize silverlight related elements;
function InitializeSiverlight(slElementId, htmlValueElementId, isMultiple) {
    var silverLightObjectId = slElementId + htmlValueElementId + isMultiple;
    var now = new Date();
    var paramId = now.getFullYear().toString() + now.getMonth().toString() + now.getDate().toString() + now.getHours().toString() + now.getMinutes().toString() + now.getSeconds().toString() + now.getMilliseconds().toString();
    var silverlightHtml = "<object id='" + silverLightObjectId + "' data='data:application/x-silverlight-2,' type='application/x-silverlight-2' width='300' height='100'><param id='silverlightsource' name='source' value='" + imageUploadKit + "'><param name='onError' value='onSilverlightError'><param name='background' value='white'><param name='minRuntimeVersion' value='5.0.61118.0'><param name='windowless'  value='true'><param id='" + paramId + "' name='initparams' value='ImageServiceUrl=" + imageHubUrl + "/, AppIdOrName=Timing,HtmlValueElementId=" + htmlValueElementId + ",IsMultiple=" + isMultiple + "'><param name='autoUpgrade'  value='true'><a href='http://go.microsoft.com/fwlink/?LinkID=149156&amp;v=5.0.61118.0' style='text-decoration:none'><img src='http://go.microsoft.com/fwlink/?LinkId=161376' alt='Get Microsoft Silverlight' style='border-style:none'></a></object>";
    $("#" + slElementId).html(silverlightHtml);
}