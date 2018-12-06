var code; //在全局 定义验证码
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
var lang = En;

function createCode() {
    var code = "";
    var codeLength = 6; //验证码的长度
    var checkCode = document.getElementById("checkCode");
    checkCode.src = "ValidatedCode.aspx";
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
    if ($("#dropQuestionEn").val() == "0") {
        alert(lang.SelectQuestion);
        return;
    }
    if ($("#txtAnswer").val() == "") {
        alert(lang.ShuAnswer);
        return;
    }
    $.get("getemailcode.aspx", { action: "selectpwd", ucode: $("#txtUserCode").val(), question: $("#dropQuestionEn option:selected").text(), answer: encodeURI($("#txtAnswer").val()) }, function (data) {
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
