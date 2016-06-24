$(function() {
  
  // 图片延迟加载
  // $("img.lazy, div.lazy").lazyload({
  //   effect : "fadeIn"
  // });

  // 头部登录后鼠标经过提示框
  (function(){
    var timer = null;
    $('.login-after').mouseover(function(){
      clearTimeout(timer);
      $(this).find('.tip-pop').show();
    }).mouseout(function(){
      timer = setTimeout(function(){
        $('.tip-pop').hide();
      }, 200);
    });
  })();
  
  // tab切换
  // $('.tabs_jlj .tab_hd').find('li').click(function(){
  //   var index = $(this).index();
  //   $(this).addClass('active').siblings().removeClass('active');
  //   $('.tabs_jlj .tab_bd>div').hide().eq(index).show();
  // });
  
  // (function($) {

  //   $('.list_faq dl').find('dt').each(function() {
      
  //     var flag = true;

  //     $(this).click(function() {

  //       if(flag) {
  //         $(this).find('em').attr('class', 'collapse').html('- 收起');
  //         $(this).siblings('dd').slideDown();
  //         flag = false;
  //       }else{
  //         $(this).find('em').attr('class', 'expand').html('+ 展开');
  //         $(this).siblings('dd').slideUp();
  //         flag = true;
  //       }
        
  //     });

  //   });    

  // })(jQuery);

  $('.list_faq dl').find('dt').click(function() {
    $(this).find('.tit').addClass('on');
    $(this).find('em').attr('class', 'expand').html('<span>-</span> 收起');
    $(this).siblings('dd').slideDown();
    $(this).parent().siblings().children('dt').find('.tit').removeClass('on');
    $(this).parent().siblings().children('dt').find('em').attr('class', 'collapse').html('<span>+</span> 展开');
    $(this).parent().siblings().children('dd').slideUp();
  });
  
  (function(){

    var data = {
      "tip1" : {
        "cls" : "创作要求",
        "cont" : [
          "各种绘画形式的叙事性漫画，结构合理，具备完整情节，至少是完整情节中的一个独立篇章。",
          "结构合理，主题新颖，有故事贯穿的纯粹绘本作品，16幅以上。",
          "同一主题（或内容）的系列作品，至少6幅，有独立欣赏价值，视觉风格独特。",
          "有一定知名度的动漫角色，具有良好的社会影响力，包括但不限于动画、漫画、游戏、表情、玩具等载体中，以动漫形式塑造而成的动漫角色。一个角色或相同故事的同一组角色皆可。",
          "针对国内动漫企业或个人的优秀动漫作品，须有一定知名度，并产生良好社会影响与商业价值。",
          "海内外知名动漫品牌，具有较强社会影响力，并创造了良好商业价值的动漫作品品牌。特定情况下，该奖项可以授予国际或国内动漫企业或机构。",
          "针对国内的动漫与游戏运营平台，包括线上与实体运营平台，如动漫基地、动漫园区、版权运营中心、交易中心等公共服务型平台机构，所服务的企业众多，其中不乏明星型企业和企业家，产生良好的社会效益与经济效益。",
          "针对动漫或游戏企业的杰出操盘手，被推荐人须具有一定业内行业知名度和美誉度，成功打造动漫游戏类产品品牌，团队卓越，服务现任企业二年以上，并创造出较大的经济收益。",
          "情节完整，综合水平优秀，非中国大陆或港澳台地区的动漫作品。",
          "组委会根据作品质量综合评定。",
          "情节完整，综合水平优秀，单项成绩突出。",
          "情节完整，综合水平优秀，单项成绩突出。",
          "在金龙奖征稿期间，利用自身资源优势，积极发动动漫爱好者参赛，并累计提交60部以上专业作品的院校或相关单位。申报单位需统一组织填写《金龙奖组稿参赛登记表》并提交。",
          "在金龙奖征稿期间，积极鼓励和发动投稿，并指导10部及以上作品参赛，或所指导参赛作品获有提名奖及以上奖项的专家或教师。"
        ]
      },
      "tip2" : {
        "cls" : "参评材料",
        "cont" : [
          "① 提交10张动漫角色的人设图，包括但不限于正面、左侧面、右侧面、45°侧面、背面等JPG格式电子文档；<br />② 提交一份动漫角色的形象说明手册（电子文档PPT），包括但不限于该形象简介、市场效益、社会影响力分析等；<br />③ 提交一份纸质报名表，经参评单位盖章或签字确认。",
          "① 提交动漫IP创意说明，包括但不限于作品创作生产情况、版权运营与开发情况、获奖情况等（电子文档PPT）；<br />② 提交一份纸质报名表，经参评单位盖章或签字确认。",
          "① 提交10张动漫角色的人设图，包括但不限于正面、左侧面、右侧面、45°侧面、背面等JPG格式电子文档；<br />② 提交一份动漫角色的形象说明手册（电子文档PPT），包括但不限于该形象简介、市场效益、社会影响力分析等；<br />③ 提交一份纸质报名表，经参评单位盖章或签字确认。",
          "① 提交一份平台说明手册（电子文档PPT），包括但不限于平台综合介绍、平台创造价值情况、服务对象或合作伙伴成长性介绍、相关获奖情况等；<br />② 提交一份纸质报名表，经参评单位盖章或签字确认。",
          "① 提交一份个人履历（电子文档PPT），包括但不限于成功运营品牌的情况、成功案例、价值收益、所获奖项等，同时提交任职企业的简介（电子文档PPT）；<br />② 提交一份纸质报名表，经参评单位盖章或签字确认。"
          
        ]
      }
    }

    $(".btn_creation").each(function(i,val){

      $(this).html('<div class="tip-dialog"><div class="arr"></div><div class="wp"><h3><em class="pen"></em>'+data.tip1.cls+'</h3><div class="ct">'+data.tip1.cont[i]+'</div></div></div>');
      tip($(this), i)
      
    });

    $(".btn_material").each(function(i){
      $(this).html('<div class="tip-dialog"><div class="arr"></div><div class="wp"><h3><em class="pen"></em>'+data.tip2.cls+'</h3><div class="ct">'+data.tip2.cont[i]+'</div></div></div>');
      tip($(this), i)
    });

    function tip(obj, index) {
      var timer = null;
      obj.mouseover(function(){
        clearTimeout(timer);
        obj.find('.tip-dialog').fadeIn();
      }).mouseout(function(){
        timer = setTimeout(function(){
          obj.find('.tip-dialog').fadeOut();
        }, 200);
      });
    }
    
  })();

});
