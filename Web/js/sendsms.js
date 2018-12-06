$(function () {
    
    if ($.cookie("total") != undefined && $.cookie("total") != 'NaN' && $.cookie("total") != 'null') {//cookie存在倒计时

        timekeeping();

    } else {//cookie 没有倒计时

        $('#btnSendsms').attr("disabled", false);
    }

    function timekeeping() {
        //把按钮设置为不可以点击
        $('#btnSendsms').attr("disabled", true);
        var interval = setInterval(function () {//每秒读取一次cookie
            //从cookie 中读取剩余倒计时
            total = $.cookie("total");
            //在发送按钮显示剩余倒计时
            $('#btnSendsms').text('请等待' + total + '秒');
            //把剩余总倒计时减掉1
            total--;

            if (total == 0) {//剩余倒计时为零，则显示 重新发送，可点击
                //清除定时器
                clearInterval(interval);
                //删除cookie
                total = $.cookie("total", total, { expires: -1 });

                //显示重新发送
                $('#btnSendsms').text('获取验证码');
                //把发送按钮设置为可点击
                $('#btnSendsms').attr("disabled", false);
            } else {//剩余倒计时不为零

                //重新写入总倒计时
                $.cookie("total", total);
            }

        }, 1000);

    }
    //绑定发送按钮
    $('#btnSendsms').click(function (event) {
        /* Act on the event */
        // alert($("#btnSendsms").val());
        var itype = 1;//$('#hdtype').val();
        
        //校验手机号码
        var phone = $('#txtPhoneNum').val();
        var pre = /^[1][3578][0-9]{9}$/;
        if (phone == '') {
            //layer.open({
            //    content: '手机号码不能为空',
            //    time: 2
            //});
            alert("手机号码不能为空");
            return this;
        } else {
            //var pre = /^[1][358][0-9]{9}$/;
            if (!pre.test(phone)) {
                //layer.open({
                //    content: '手机号码格式有误！',
                //    time: 2
                //});
                alert("手机号码格式有误");
                return this;
            }
        }
        $('#btnSendsms').attr("disabled", true);
        $.ajax({
            url: '/APPService/SendSMS.ashx',//服务器发送短信
            type: 'GET',
            dataType: 'json',
            data: { phone: phone, type: itype },
            success: (function (re) {
                var str = "发送短信验证码成功，请注意查看您的手机";
                //console.log(re);
                if (re[0].name == true) {
                    $.cookie("total", 60);
                    timekeeping();
                } else {
                    var aa = $.cookie("total");
                    if (aa > 0) {
                        timekeeping();
                    }
                    else {
                        $('#btnSendsms').attr("disabled", false);
                    }
                    alert(re[0].value);
                }
                //layer.open({
                //    content: str,
                //    time: 2
                //});
                //alert(str);
            }),
            error: function (errorMsg) {
                //alert("图表请求数据错误!" + errorMsg.errorMsg);
                $('#btnSendsms').attr("disabled", false);
                console.log("error");
            }
            //.fail(function () {
            //    console.log("又error");
            //})
            //.always(function () {
            //    console.log("只好complete");
            //});
        });
    })
})