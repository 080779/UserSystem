var code; //在全局 定义验证码
var Zh = {
    username: "用户名",
    password: "密   码",
    Code:"验证码",
    msg: "出错了！",
    shuRu: "请输入验证码！",
    Yanzhen: "验证码输入错误！",
    TianUserCode:"请填写用户名！",
    SelectQuestion:"请选择密保问题！",
    ShuAnswer: "请填写密保答案！",
    QuestionError: "密保问题错误！",
    AnswerError:"密保答案错误！",
    CunZai: "用户不存在！",
    LianXi: "密保问题和答案已输入错误3次，请联系管理员！",
    MiMa: "您的密码已修改为\"111111\",请登录后修改！"
}
var En = {
    username: "userCode",
    password: "Password",
    Code: "identifying code",
    msg: "wrong!",
    shuRu: "Please enter the verification code!",
    Yanzhen: "Code input error!",
    TianUserCode: "Please fill in your user name!",
    SelectQuestion: "Please select a secret question!",
    ShuAnswer: "Please fill out the secret security answer!",
    QuestionError: "Secret security issues errors!",
    AnswerError: "Secret security answer errors!",
    CunZai: "User does not exist!",
    LianXi: "Secret security question and answer is incorrectly entered 3 times, please contact the administrator!",
    MiMa: "Your password has been modified to '111111' Please login to modify!"
}
var lang = Zh;

function createCode() {
    var code = "";
    var codeLength = 6; //验证码的长度
    var checkCode = document.getElementById("checkCode");
    checkCode.src = "ValidatedCode.aspx";
    //var selectChar = new Array(2, 3, 4, 5, 6, 7, 8, 9, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');

    //for (var i = 0; i < codeLength; i++) {
    //    var charIndex = Math.floor(Math.random() * 32);
    //    code += selectChar[charIndex];
    //}
    //if (code.length != codeLength) {
    //    createCode();
    //}
    //alert(code);
    //document.getElementById("checkCode").src = "ValidatedCode.aspx";
}

function validate() {
    var inputCode = document.getElementById("txtVa").value.toUpperCase();

    if (inputCode.length <= 0) {
        alert(lang.shuRu);
        return false;
    }
    else if (inputCode != code) {
        alert(lang.Yanzhen);
        createCode();
        return false;
    }
    else {
        return true;
    }

}

//找回密码
$(function () {
    $(".thickbox").ready(function () {
        var windowWidth = document.documentElement.clientWidth;
        var windowHeight = document.documentElement.clientHeight;
        var popupHeight = $(".backdiv").height();
        var popupWidth = $(".backdiv").width();
        var _top = windowHeight / 2 - popupHeight / 2;
        var _left = windowWidth / 2 - popupWidth / 2;
        $(".backdiv").css({ left: _left, top: _top });
    });
    $(".thickbox").click(function () {
        $(this).hide();
        $(".backdiv").slideDown(200);
    });

    $("#cle").click(function () {
        $(".backdiv").fadeOut('slow', function () {
            $(".thickbox").show();
        });
    });
})
$(document).ready(function () {
    createCode();
    $("#txtPwd").focus(function () {
        $("#pwd_tip").html("");
    }).blur(function () {
        var tip = $("#pwd_tip").html();
        var txtpwd = $("#txtPwd").val();
        if (txtpwd == "") {
            $("#pwd_tip").html(lang.password);
        } else {
            $("#pwd_tip").html("");
        }
    });
    $("#txtUserName").focus(function () {
        $("#user_tip").html("");
    }).blur(function () {
        var tip = $("#user_tip").html();
        var txtpwd = $("#txtUserName").val();
        if (txtpwd == "") {
            $("#user_tip").html(lang.username);
        } else {
            $("#user_tip").html("");
        }
    });
    $("#txtVa").focus(function () {
        $("#Va_tip").html("");
    }).blur(function () {
        var tip = $("#Va_tip").html();
        var txtpwd = $("#txtVa").val();
        if (txtpwd == "") {
            $("#Va_tip").html(lang.Code);
        } else {
            $("#Va_tip").html("");
        }
    });
    var item = $("input[name='rdo']:checked").val();
    if (item == 2) {
        rdoEn_onclick();
    } else {
        rdoZH_onclick();
    }
});
function ssss(t, v) {
    $("#" + t).html("");
    $("#" + v).focus();
}
//验证找回密码输入文字
function checkinput() {
    if ($("#txtUserCode").val() == "") {
        alert(lang.TianUserCode);
        return;
    }
    if ($("#ddlQuestion").val() == "0") {
        alert(lang.SelectQuestion);
        return;
    }
    if ($("#txtAnswer").val() == "") {
        alert(lang.ShuAnswer);
        return;
    }
    $.get("getemailcode.aspx", { action: "selectpwd", ucode: $("#txtUserCode").val(), question: $("#ddlQuestion option:selected").text(), answer: encodeURI($("#txtAnswer").val()) }, function (data) {
        switch (data) {
            case "2":
                alert(lang.QuestionError);
                break;
            case "3":
                alert(lang.AnswerError);
                break;
            case "1":
                alert(lang.CunZai);
                break;
            case "4":
                alert(lang.LianXi);
                break;
            case "5":
                alert(lang.MiMa); window.top.location = window.location.href;
                break;
            case "6":
                alert(lang.msg); window.top.location = window.location.href;
                break;
        }
    });
}
 
function rdoEn_onclick() {
    lang = En;
    $("#pwd_tip").text("Password");
    $("#user_tip").text("UserName");
    $("#Va_tip").text("identifying code");
    $("#Wang").text("Forgot password？");
    $(".thickbox").text("Retrieve password");
    $("#UserName").text("UserName");
    $("#question").text("Secret security issues");
    $("#anwers").text("Secret security answer");
    $("#dropQuestion").css("display", "none");
    $("#dropQuestionEn").css("display", "block");
    $("#cle").text("Close");
    $("#Button6").val("Confirmation");
    $("#Button1").css("background", "url(../images/btn_en.jpg) no-repeat");
    $("#img").attr("src", "images/xi_en.jpg");
}

function rdoZH_onclick() {
    lang = Zh;
    $("#pwd_tip").text("密码");
    $("#user_tip").text("用户名");
    $("#Va_tip").text("验证码");
    $("#Wang").text("忘记密码？");
    $(".thickbox").text("找回密码");
    $("#UserName").text("用户名");
    $("#question").text("密保问题");
    $("#anwers").text("密保答案");
    $("#dropQuestion").css("display", "block");
    $("#dropQuestionEn").css("display", "none");
    $("#cle").text("关闭");
    $("#Button6").val("确认");
    $("#img").attr("src", "images/xi.jpg");
}
