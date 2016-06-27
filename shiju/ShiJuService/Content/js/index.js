(function () {
    var imageBaseUrl = $("#ImageBaseUrl").val();
    var getPartyUrl = $("#GetPartyUrl").val();
    var isImages = function (data) {
        var Images = " ";
        if (data.Images !== "[]" && data.Images !=="") {
            var Images = data.Images.slice(),
				l = Images.length,
				str = " ",
                imageId = "";
            for (var i = 0; i < l; i++) {
                if (i > 0 && i < l - 1) {
                    str += Images[i];
                }
            }
            var obj = jQuery.parseJSON(str);
            imageId = obj.ImageId;
            Images = "<div class='slide_box' id='slide_box'>" +
				"<ul class='slide'><li class='active'>" +
				"<img src='" + imageBaseUrl + imageId + "_5_10_10.jpg' /></li></ul><ul class='circle'></ul></div>";
        }
        return Images;
    }
    // 参与者头像
    var Participants = function (data) {
        var str = "",
			Participants = data.Participants.length;
        for (var i = 0; i < Participants; i++) {
            if (parseInt(data.Participants[i].User.Portrait) == 0 || data.Participants[i].User.Portrait == "") {
                str += "<li><img src='../../Content/images/d_photo.jpg' /></li>";
            }
            else {
                str += "<li><img src='" + imageBaseUrl + data.Participants[i].User.Portrait + "_2_75_75.jpg' /></li>";
            }
        }
        return {
            length: Participants,
            str: str
        };
    }
    // 日期转换
    var shicha = -(new Date().getTimezoneOffset() * 60000);
    var dateConversion = function (dateStr) {
        var date = dateStr.replace(/-/g, "/");
        date = new Date(date).getTime() + shicha;
        date = new Date(date);
        var year = date.getFullYear(),
            month = (date.getMonth() + 1 >= 10) ? (date.getMonth() + 1) : ("0" + (date.getMonth() + 1)),
            day = (date.getDate() >= 10) ? date.getDate() : "0" + date.getDate(),
            hours = (date.getHours() >= 10) ? date.getHours() : "0" + date.getHours(),
            minutes = (date.getMinutes() >= 10) ? date.getMinutes() : "0" + date.getMinutes(),
            seconds = (date.getSeconds() >= 10) ? date.getSeconds() : "0" + date.getSeconds();
        date = year + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
        return date;
    }
    var url = getPartyUrl;
        partyId = null;
    var getData = function () {
        $.ajax({
            type: "GET",
            url: url,
            data: "userId=00000000000000000000000000000000",
            dataType:"json",
            success: function (d) {
                var data = d;
                data = data.Party;
                partyId = data.Id;
                
                if (parseInt(data.CreatorUser.Portrait) == 0 || data.CreatorUser.Portrait == "") {
                    var organiserPhoto = "../../Content/images/d_photo.jpg";
                }
                else {
                    var organiserPhoto = imageBaseUrl + data.CreatorUser.Portrait + "_2_75_75.jpg";
                }
                
                if (window.localStorage) {
                    var beginTime = data.BeginTime;
                    var endTime = data.EndTime;
                    localStorage.removeItem("beginTime");
                    localStorage.removeItem("endTime");
                    localStorage.setItem("beginTime", beginTime);
                    localStorage.setItem("endTime", endTime);
                }
                var htmlTemplate = [isImages(data),
				"<div class='article'>",
					"<div class='share_p'>",
						"<span class='photo'><img src='" + organiserPhoto + "' /></span>",
						"<p>发起人</p>",
						"<span class='name'>" + data.CreatorUser.NickName + "</span>",
					"</div>",
					"<article>" + data.Description + "</article>",
				"</div>",
				"<div class='info'>",
					"<div class='date'><i class='icon'></i><span>" + dateConversion(data.BeginTime) + "</span> — <span>" + dateConversion(data.EndTime) + "</span></div>",
					"<div class='address'><i class='icon'></i><span>" + data.Address + "</span></div>",
					"<div class='people'><i class='icon'></i><span>期望人数 x</span><span class='number'>" + data.MaxUserCount + "</span></div>",
				"</div>",
				"<div class='join_p'>",
					"<a class='more' href='javascript:;' ></a>",
					"<div class='join_num'>参与者 x<span>" + Participants(data).length + "</span></div>",
					"<div class='p_photo'><ul>" + Participants(data).str + "</ul></div>",
				"</div>"].join("");
                $(".theme_content .title").find("h2").text(data.Title);
                $(".theme_content .middle").html(htmlTemplate);
                $(".toolbar .number").text(data.LikeCount);
                if (data.Description == " ") {
                    $(".article").find("article").hide();
                }
                else {
                    $(".article").find("article").show();
                }
                if (data.Address == "") {
                    $(".address").hide();
                }
                else {
                    $(".address").show();
                }
                $(".theme_content .middle .more").click(function () {
                    $(this).toggleClass("moveRotate");
                    $(".theme_content .middle .join_p .p_photo").toggleClass("heightAuto");
                });
                $(".vote_content").find("h2").text(data.VoteTitle);
                // 投票
                var VoteChoicesJson = JSON.parse(data.VoteChoicesJson),
				voteLen = VoteChoicesJson.length,
				voteStr = " ",
				abc = ["A", "B", "C", "D", "E"];
                for (var k = 0; k < voteLen; k++) {
                    voteStr += "<li><b>" + abc[k] + "</b><span>" + VoteChoicesJson[k].Text + "</span></li>";
                }
                $(".vote_content .option_c").find("ul").html(voteStr);
                var $optionPicture = $(".vote_content .option").find("img");
                switch (voteLen) {
                    case 3:
                        $optionPicture.prop("src", "../../Content/images/o_three.png");
                        break;
                    case 4:
                        $optionPicture.prop("src", "../../Content/images/o_four.png");
                        break;
                    case 5:
                        $optionPicture.prop("src", "../../Content/images/o_five.png");
                        break;
                    default:
                        $(".vote").hide();
                        $(".vote_content").hide();
                }
                // 评论
                $(".comment .number").text(data.PartyComments.length);
                var PartyComments = data.PartyComments.length;
                if (PartyComments == 0) {
                    $(".no_comment").show();
                }
                else {
                    $(".no_comment").hide();
                    PartyComments = (PartyComments > 5) ? 5 : PartyComments;
                    var commentStr = " ";
                    for (var i = 0; i < PartyComments; i++) {
                        if (parseInt(data.PartyComments[i].User.Portrait) == 0 || data.PartyComments[i].User.Portrait == "") {
                            var commentPhoto = "../../Content/images/d_photo.jpg";
                        }
                        else {
                            var commentPhoto = imageBaseUrl + data.PartyComments[i].User.Portrait + "_2_75_75.jpg";
                        }
                        var year = parseInt(data.PartyComments[i].CreatedTime.substr(0, 4)),
							month = parseInt(data.PartyComments[i].CreatedTime.substr(5, 2)) - 1,
							day = parseInt(data.PartyComments[i].CreatedTime.substr(8, 2)),
							hours = parseInt(data.PartyComments[i].CreatedTime.substr(11, 2)),
							minutes = parseInt(data.PartyComments[i].CreatedTime.substr(14, 2)),
							seconds = parseInt(data.PartyComments[i].CreatedTime.substr(17, 2)),
							ts = (new Date()) - ((new Date(year, month, day, hours, minutes, seconds)).getTime() + shicha),
							dd = parseInt(ts / 1000 / 60 / 60 / 24, 10),
							hh = parseInt(ts / 1000 / 60 / 60 % 24, 10),
							mm = parseInt(ts / 1000 / 60 % 60, 10);
                        if (data.PartyComments[i].Text != "") {
                            if (dd > 0) {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + dd + "</b>天前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
                                            "</dl>";

                            }
                            else if (hh > 0) {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + hh + "</b>小时前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
                                            "</dl>";
                            }
                            else {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + mm + "</b>分钟前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
                                            "</dl>";
                            }
                        }
                        else {
                            if (dd > 0) {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + dd + "</b>天前</span><p><img src = '../../Content/images/audio_icon.png'></p></dd>" +
                                            "</dl>";

                            }
                            else if (hh > 0) {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + hh + "</b>小时前</span><p><img src = '../../Content/images/audio_icon.png'></p></dd>" +
                                            "</dl>";
                            }
                            else {
                                commentStr += "<dl><dt><span><img src='" + commentPhoto + "'></span></dt>" +
                                            "<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + mm + "</b>分钟前</span><p><img src = '../../Content/images/audio_icon.png'></p></dd>" +
                                            "</dl>";
                            }
                        }
                    }
                    $(".comm_list").html(commentStr);
                }
            }
        });
        setTimeout(function () {
            getData();
        }, 60000);
    }
    getData();
    var scrollTop;
    var noScroll = function () {
        scrollTop = $(window).scrollTop();
        $("body,html").css({
            "overflow": "hidden",
            "width": "100%",
            "height": "100%"
        });
    }
    var autoScroll = function () {
        $("body,html").css({
            "overflow": "auto",
            "width": "auto",
            "height": "auto"
        });
        $(window).scrollTop(scrollTop);
    }
    // 单击事件
    $(".comm_btn,.toolbar,.option img").click(function () {
        noScroll();
        $(".prompt").addClass("proShow");
    });
    $(".prompt .close").click(function () {
        autoScroll();
        $(".prompt").removeClass("proShow");
    });
    // 报名click
    $(".sign_up").click(function () {
        if (!is_weixin()) {
            noScroll();
            $(".point_out").addClass("proShow");
        }
        else {
            // 是微信
            var serviceBaseUrl = $("#ServiceBaseUrl").val();
            var u = encodeURI(serviceBaseUrl + "Wechat/Redirect/Parties/" + partyId + "/Participant");
            $(this).attr("href", "http://open.weixin.qq.com/connect/oauth2/authorize?appid=wxbf96085ee6d7cb5a&redirect_uri=" + u + "&response_type=code&scope=snsapi_userinfo#wechat_redirect");
        }
    });
    
    $(".point_out").click(function () {
        $(this).removeClass("proShow");
        autoScroll();
    });

    var w, $slideBox = $("#slide_box");
    var is_weixin = function () {
        var ua = navigator.userAgent.toLowerCase();
        if (ua.match(/MicroMessenger/i) == "micromessenger") {
            return true;
        }
        else {
            return false;
        }
    }
    
    var browser = {
        versions: function () {
            var u = navigator.userAgent, app = navigator.appVersion;
            return {// 移动终端浏览器版本信息
                trident: u.indexOf('Trident') > -1, // IE内核
                presto: u.indexOf('Presto') > -1, // opera内核
                webKit: u.indexOf('AppleWebKit') > -1, // 苹果、谷歌内核
                gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, // 火狐内核
                mac: u.indexOf('Mac OS X') > -1,// 是否为Mac OS X
                mobile: !!u.match(/AppleWebKit.*Mobile.*/) || !!u.match(/AppleWebKit/), // 是否为移动终端
                ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, // android终端或者uc浏览器
                iPhone: u.indexOf('iPhone') > -1 || u.indexOf('Mac') > -1, // 是否为iPhone或者QQHD浏览器
                iPad: u.indexOf('iPad') > -1, // 是否iPad
                webApp: u.indexOf('Safari') == -1, // 是否web应该程序，没有头部与底部
                wp: u.indexOf("Windows Phone") > -1// 是否为winPhone
            };
        }(),
        language: (navigator.browserLanguage || navigator.language).toLowerCase()
    }

    $(window).load(function () {
        w = $slideBox.width();
    });
    $(window).resize(function () {
        w = $slideBox.width();
    }).trigger("resize");

    var showHide = function (ele, elec) {
        if (!ele || !elec) {
            return;
        }
        $(elec).slideToggle();
        ele.toggleClass("arrow_rot");
    }
    $(".theme_arrow").click(function () {
        var ele = $(this).find(".arrow");
        var elec = $(".theme_content");
        showHide(ele, elec);
    });
    $(".vote_arrow").click(function () {
        var ele = $(this).find(".arrow");
        var elec = $(".vote_content");
        showHide(ele, elec);
    });
    $(".comm_arrow").click(function () {
        var ele = $(this).find(".arrow");
        var elec = $(".comm_content");
        showHide(ele, elec);
    });
    $("#dl_app").click(function () {
        if (is_weixin()) {
            $(".dl_prompt").show();
        }
        else {
            if (browser.versions.ios || browser.versions.iPhone || browser.versions.iPad) {
                location.href = ""; // 时聚 scheme
                setTimeout(function () {
                    location.href = "https://itunes.apple.com/us/app/newbe/id1071983155?l=zh&ls=1&mt=8"; // IOS下载地址
                }, 250);
                setTimeout(function () {
                    location.reload();
                }, 1000);
            }
            else if (browser.versions.android) {
                $(this).prop({
                    "href": "" // 安卓下载地址
                });
            }
        }
    });
    $(".dl_prompt").click(function () {
        $(this).hide();
    });
})();
