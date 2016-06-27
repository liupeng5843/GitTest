
function BookVote(url) {
    $.ajax({
        type: 'post',
        url: url,
        async: false,
        success: function (data) {
            if (data.code == 0) {
                alert('投票成功');
            }
            else if (data.code == -2) {
                alert('您还没有登录，不能进行投票哦');
            } else {
                alert('投票失败，请刷新重试');
            }
        }
    });
}
