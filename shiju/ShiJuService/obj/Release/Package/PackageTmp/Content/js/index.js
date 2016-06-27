(function (doc, win) {
    var imageBaseUrl = $("#ImageBaseUrl").val();
    var getPartyUrl = $("#GetPartyUrl").val();
    var isImages = function (data) {
        var Images = " ";
        if (data.Images !== "[]") {
            var Images = data.Images.slice(),
				l = Images.length,
				str = " ";
            for (var i = 0; i < l; i++) {
                if (i > 0 && i < l - 1) {
                    str += Images[i];
                }
            }
            Images = "<div class='slide_box' id='slide_box'>" +
				"<ul class='slide'><li class='active'>" +
				"<img src='"
                + imageBaseUrl
                + str + "_5_0_0.jpg' />" +
				"</li></ul>" +
				"<ul class='circle'></ul>" +
				"</div>";

        }
        return Images;
    }
    var Participants = function (data) {
        var str = "",
			Participants = data.Participants.length;
        for (var i = 0; i < Participants; i++) {
            str += "<li><img src='"
                +imageBaseUrl
                +data.Participants[i].User.Portrait + "_2_75_75.jpg' /></li>";
        }
        return {
            length: Participants,
            str: str
        };
    }
    var url = getPartyUrl;
    // "http://10.1.1.4:9001/ShiJuService/Parties/105B0C88-EB2A-441E-BA80-5C445C0703C7?";
    var getData = function () {
        $.ajax({
            type: "GET",
            url: url,
            data: "userId=00000000000000000000000000000000",
            success: function (d) {
                var data = d;
                data = data.Party;
                var htmlTemplate = [isImages(data),
				"<div class='article'>",
					"<div class='share_p'>",
						"<span class='photo'><img src='"
                        +imageBaseUrl
                        +data.CreatorUser.Portrait + "_2_75_75.jpg' /></span>",
						"<p>发起人</p>",
						"<span class='name'>" + data.CreatorUser.NickName + "</span>",
					"</div>",
					"<article>" + data.Description + "</article>",
				"</div>",
				"<div class='info'>",
					"<div class='date'><i class='icon'></i><span>" + data.BeginTime + "</span> — <span>" + data.EndTime + "</span></div>",
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
                var VoteChoicesJson = $.parseJSON(data.VoteChoicesJson),
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
                // 投票
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
                        var year = parseInt(data.PartyComments[i].CreatedTime.substr(0, 4)),
							month = parseInt(data.PartyComments[i].CreatedTime.substr(5, 2)) - 1,
							day = parseInt(data.PartyComments[i].CreatedTime.substr(8, 2)),
							hours = parseInt(data.PartyComments[i].CreatedTime.substr(11, 2)),
							minutes = parseInt(data.PartyComments[i].CreatedTime.substr(14, 2)),
							seconds = parseInt(data.PartyComments[i].CreatedTime.substr(17, 2)),
							ts = (new Date()) - (new Date(year, month, day, hours, minutes, seconds)),
							dd = parseInt(ts / 1000 / 60 / 60 / 24, 10),
							hh = parseInt(ts / 1000 / 60 / 60 % 24, 10),
							mm = parseInt(ts / 1000 / 60 % 60, 10);
                        if (dd > 0) {
                            commentStr += "<dl>" +
										"<dt><span><img src='"
                                        +imageBaseUrl
                                        + data.PartyComments[i].User.Portrait + "_2_75_75.jpg'></span></dt>" +
										"<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + dd + "</b>天前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
										"</dl>";
                        }
                        else if (hh > 0) {
                            commentStr += "<dl>" +
										"<dt><span><img src='"
                                        + imageBaseUrl 
                                        + data.PartyComments[i].User.Portrait + "_2_75_75.jpg'></span></dt>" +
										"<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + hh + "</b>小时前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
										"</dl>";
                        }
                        else {
                            commentStr += "<dl>" +
										"<dt><span><img src='"
                                        + imageBaseUrl
                                        + data.PartyComments[i].User.Portrait + "_2_75_75.jpg'></span></dt>" +
										"<dd><span class='name'>" + data.PartyComments[i].User.NickName + "</span><span class='time'><b>" + mm + "</b>分钟前</span><p>" + data.PartyComments[i].Text + "</p></dd>" +
										"</dl>";
                        }
                    }
                    $(".comm_list").html(commentStr);
                }
                // 单击事件
                $(".sign_up,.comm_btn,.toolbar,.option img").click(function () {
                    noScroll();
                    $(".prompt").addClass("proShow");
                });
                $(".prompt .close").click(function () {
                    autoScroll();
                    $(".prompt").removeClass("proShow");
                });
            }
        });
        setTimeout(function () {
            getData();
        }, 60000);
    }
    getData();
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
    var noScroll = function () {
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

    var docEl = doc.documentElement,
	resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
	recalc = function () {
	    var clientWidth = docEl.clientWidth;
	    if (!clientWidth) return;
	    var fontSize = 20 * (clientWidth / 640);
	    docEl.style.fontSize = fontSize + "px"; // (fontSize>20) ? 20 + 'px' : (fontSize + 'px');
	};
    if (!doc.addEventListener) return;
    win.addEventListener(resizeEvt, recalc, false);
    doc.addEventListener('DOMContentLoaded', recalc, false);
    doc.body.addEventListener('touchstart', function () { }, false);

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
                $(this).prop({
                    "href": "http://m.ioffice100.com"
                });
                /*var ifr = document.createElement('iframe');
                ifr.src = 'ioffice100://';
                ifr.style.display = 'none';
                document.body.appendChild(ifr);
                window.setTimeout(function () {
                    document.body.removeChild(ifr);
                }, 3000);*/
            }
            else if (browser.versions.android) {
                $(this).prop({
                    "href": "http://m.ioffice100.com"
                });
            }
        }
    });
    $(".dl_prompt").click(function () {
        $(this).hide();
    });
    /* 
	if(is_weixin() && android){
		noScroll();
		$(".ad_error").show();
	}
	var index = 0;
	var num = $slideBox.find(".slide").find("li").length;
	var addLi = function(num){
		if(num > 1){
			var str = "";
			for(var i=0; i<num; i++){
				if(i == 0){
					str += "<li class='cur'></li>";
				}
				else{
					str += "<li></li>";
				}
			}
			$(".slide_box .circle").html(str);
		}
	}*/
    /*var slider = function(i){
		var $curSlide = $slideBox.find(".slide");
		if(i > index){
			index = i;
			$curSlide.find("li").eq(index).addClass("active");
			$curSlide.finish().animate({
				"left":-w + "px"
			},"normal",function(){
				$curSlide.find("li").eq(index).siblings().removeClass("active");
				$curSlide.css("left",0);
			});
		}
		else{
			index = i;
			$curSlide.find("li").eq(index).addClass("active");
			$curSlide.css("left",-w + "px").finish().animate({
				"left":0
			},"normal",function(){
				$curSlide.find("li").eq(index).siblings().removeClass("active");
				$curSlide.css("left",0);
			});
		}
		$(".slide_box .circle").find("li").eq(index).addClass("cur").siblings().removeClass("cur");
	}*/

    // 为slider原点绑定事件
    /*$(".slide_box .circle").on("click","li",function(){
		slider($(this).index());
	});*/

    // 活动图滑动
    /* var isRight = function(i){
		i--;
		if(i < 0){
			i = num-1;
		}
		slider(i);
		
	}
	var isLeft = function(i){
		i++;
		if(i > num-1){
			i = 0;
		}
		slider(i);
	}
	var slideBox = document.getElementById("slide_box");
	var startX, startY, direction = null; 
	var touchStart = function(e){
		var touch = e.touches[0];
		var x = Number(touch.pageX);
		var y = Number(touch.pageY);
		startY = y;
		startX = x;
		index = $slideBox.find(".circle .cur").index();
		slideBox.addEventListener("touchmove", touchMove, false);
	    slideBox.addEventListener("touchend", touchEnd, false);
	}
	var touchMove = function(e){
		var touch = e.touches[0];
		var x = Number(touch.pageX);
		var y = Number(touch.pageY);
		var moveSize = x-startX;
		if(Math.abs(x-startX) > Math.abs(y-startY)){
			if(x - startX < 0){
				e.preventDefault();
				direction = 1;
			}
			else{
				e.preventDefault();
				direction = 0;
			}
		}
	}
	var touchEnd = function(){
		if(direction == 1){
			isLeft(index);
		}
		else if(direction == 0){
			isRight(index);
		}
		else{
			return;
		}
		direction = null;
		slideBox.removeEventListener("touchmove", touchMove, false);
		slideBox.removeEventListener("touchend", touchEnd, false);
	}
	slideBox.addEventListener("touchstart", touchStart, false);*/
})(document, window);
