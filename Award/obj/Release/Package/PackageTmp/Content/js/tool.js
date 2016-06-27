
var alert_diy = $('#mask,.alert-diy');
var alert_title = $('#alert-title');
var alert_text = $('#alert-text');
var alert_ok = $('#alert-ok');
var alert_cancel = $('#alert-cancel');

$('#mask,#alert-ok,#alert-cancel').click(function () {
    $('#mask,.alert-diy').hide();
});

function alert_custom(title, text, ok) {
    //弹层函数
    $('#alert-title').text(title);
    $('#alert-text').text(text);
    $('#alert-ok').text(ok);
    $('#mask,.alert-diy').show();
}

function BookVote(url) {
    var result = 0;
    $.ajax({
        type: 'post',
        url: url,
        async: false,
        success: function (data) {
            if (data.code == 0) {
                //alert('投票成功');
                alert_custom("恭喜您", "投票成功", "确定");
            }
            else if (data.code == 1) {
                var des="此漫画不在投票期，去看看其他漫画吧！";
                if (data.month > 0){
                    des = "此漫画不在投票期," + data.month + "月22日再来投票吧!";
                }
                alert_custom("投票成功", "非投票期的票数不计入榜单，仅用于人气漫画奖评选参考", "确定");
            }
            else if (data.code == -2) {
                //alert('您还没有登录，不能进行投票哦');
                //alert_custom("很遗憾", "您还没有登录，不能进行投票哦", "确定");
                alert("您还没有登录，不能进行投票哦");
                window.open("http://www.manhuadao.cn/Account/Index");
            } else {
                //alert('投票失败，一天只能投5票哦');
                alert_custom("很遗憾", "投票失败，一天只能投5票哦", "确定");
            }
            if (data.code == 1) {
                data.code = 0;
            }
            result = data.code;
        }
    });
    return result;
}

function browserRedirect() {
    var sUserAgent = navigator.userAgent.toLowerCase();
    var bIsIpad = sUserAgent.match(/ipad/i) == "ipad";
    var bIsIphoneOs = sUserAgent.match(/iphone os/i) == "iphone os";
    var bIsMidp = sUserAgent.match(/midp/i) == "midp";
    var bIsUc7 = sUserAgent.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
    var bIsUc = sUserAgent.match(/ucweb/i) == "ucweb";
    var bIsAndroid = sUserAgent.match(/android/i) == "android";
    var bIsCE = sUserAgent.match(/windows ce/i) == "windows ce";
    var bIsWM = sUserAgent.match(/windows mobile/i) == "windows mobile";
    if (bIsIpad || bIsIphoneOs || bIsMidp || bIsUc7 || bIsUc || bIsAndroid || bIsCE || bIsWM) {
        return "phone";
    } else {
        return "pc";
    }
}


function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=")
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return unescape(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}

function setCookie(c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + escape(value) +
    ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
}

function setVoteCookie() {
    if (getCookie('CBIDS') == "") {
        //document.cookie = "CBIDS=[]";
        setCookie(CBIDS, "", 1);
    }
    return true;
}


//function InitPagination(pageCount, currentPage) {
//    $("div[id^=tabs-]").each(function (i, e) { $(this).tabs(); });
//    //stab.tabs();

//    $('#pagination').twbsPagination({
//        totalPages: pageCount,
//        hrefVariable: currentPage,
//        visiblePages: 5,
//        first: "首页",
//        prev: "前一页",
//        next: "下一页",
//        last: "最后一页",
//        initiateStartPageClick: false,
//        href: '',
//        onPageClick: function (event, page) {
//            $.ajax({
//                type: 'get',
//                url: '/Comments/GetCommentList?page={{number}}&pageNumber=' + page,
//                async: false,
//                success: function (data) {
//                    if (page > 1) {
//                        $('#comment-box').html(data);
//                        InitPagination(pageCount, page)
//                        //$('.page').removeClass('active');
//                        //$($('.page')[page - 1]).addClass('active');
//                    }
//                }
//            });
//            this.hrefVariable = page;
//            // must add ajax attribute at here
//            $('#pagination').find("a").each(function (i, e) {
//                $(e).attr({ "data-ajax": "true", "data-ajax-method": "Get", "data-ajax-mode": "replace", "data-ajax-update": "#tabledata" })
//            });
//        }
//    });
//}