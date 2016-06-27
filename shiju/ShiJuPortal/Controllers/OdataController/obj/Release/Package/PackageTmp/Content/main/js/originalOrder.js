$(function () {
    var isuploading = false;

    $("#batchInsert").dialog({
        closeOnEscape: false,
        autoOpen: false,
        width: 400,
        height: 340,
        modal: true,
        resizable: false,
        buttons: [
            {
                id: 'order-upload-okay',
                text: '确定',
                click: function () {
                    $(this).dialog("close");
                }
            },
            {
                id: 'order-upload-cancel',
                text: '取消 ',
                click: function () {
                    if (isuploading) {
                        if (confirm("文件上传尚未结束，您确定要退出么?")) {
                            $(this).dialog("close");
                        }
                    } else {
                        $(this).dialog("close");
                    }
                }
            }],
        open: function (event, ui) {
            resetUploader();

            $(".ui-dialog-titlebar-close", $(this).parent()).hide();

            var $list = $('#thelist');
            var $btn = $('#ctlBtn');
            var state = 'pending';
            var uploader;

            uploader = WebUploader.create({
                auto: true,
                resize: false,
                swf: Global.baseUrl + 'Content/webuploader/Uploader.swf',
                //server: Global.baseUrl + 'UI/ImportNews?accessToken=' + Global.accessToken,
                server: Global.baseUrl + 'OriginalOrder/Import',
                pick: { id: '#picker', multiple: false },
                accept: {
                    title: 'xls,xlsx',
                    extensions: 'xls,xlsx',
                    mimeTypes: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel'
                }
            });

            uploader.on('fileQueued', function (file) {
                $list.append('<div id="' + file.id + '" class="item">' +
                    '<h4 class="info">' + file.name + '</h4>' +
                    '<p class="state">等待上传...</p>' +
                '</div>');
            });

            uploader.on('uploadProgress', function (file, percentage) {
                isuploading = true;

                var $li = $('#' + file.id),
                    $percent = $li.find('.progress .progress-bar');

                if (!$percent.length) {
                    $percent = $('<div class="progress progress-striped active">' +
                      '<div class="progress-bar" role="progressbar" style="width: 0%">' +
                      '</div>' +
                    '</div>').appendTo($li).find('.progress-bar');
                }

                $li.find('p.state').text('上传中');

                $percent.css('width', percentage * 100 + '%');
            });

            uploader.on('uploadSuccess', function (file, response) {
                var code = response.Code;
                if (code == -1) {
                    $('#' + file.id).find('p.state').text(response.Description);
                    //alert(response.Description);
                }
                else {
                    $('#' + file.id).find('p.state').text('导入成功。');
                }
            });

            uploader.on('uploadError', function (file, response) {
                $('#' + file.id).find('p.state').text('导入失败。');

            });

            uploader.on('uploadComplete', function (file) {
                $('#' + file.id).find('.progress').fadeOut();
                isuploading = false;
            });

            uploader.on('all', function (type) {
                if (type === 'startUpload') {
                    state = 'uploading';
                } else if (type === 'stopUpload') {
                    state = 'paused';
                } else if (type === 'uploadFinished') {
                    state = 'done';
                }

                if (state === 'uploading') {
                    $btn.text('暂停上传');
                } else {
                    $btn.text('开始上传');
                }
            });

            $btn.on('click', function () {
                if (state === 'uploading') {
                    uploader.stop();
                } else {
                    uploader.upload();
                }
            });

            Global.uploader = uploader;

            $("#order-upload-okay").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-primary");

            $("#order-upload-cancel").removeClass("ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only")
            .addClass("btn btn-default");

            $("#order-upload-okay").blur();
            $("#order-upload-cancel").blur();
        }
    });

    function resetUploader() {
        $('#thelist').html('');
        $('#picker').html('选择文件');
    }

    $('#importBtn').on('click', function () {
        $('#batchInsert').dialog('open');
    });

    $('#templateBtn').on('click', function () {
        window.open(Global.baseUrl + 'Content/template/template.xlsx');
    });
});