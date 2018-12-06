function checkCommit() {
    $("input").filter(".verify").each(function () {
        $(this).blur();
    });

    var booValue = true;
    $("span").filter(".regLable").each(function () {
        if ($(this).css("display") != "none") {
            booValue = false;
        }
    });
    if (booValue == false) {
        return false;
    }
    else {
    return confirm("确定提交吗？")
}

}

function checkUserCode(txtid) {
    var userCode = $.trim($(txtid).val());
    if (userCode == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入会员编号！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var reg = /^[0-9]{6}$/;
//    var reg = /^[a-zA-Z|0-9]{6,10}$/;
    if (!reg.test(userCode)) {
        $(txtid.id.replace("txt", "#lbl")).text("会员编号必须由6到10位数字或字母组成！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var isValiable = Web.UserReg.chenkUserCode(userCode).value.trim();
    if (isValiable == "false") {
        $(txtid.id.replace("txt", "#lbl")).text("该会员编号已存在！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });

}

function checkRecommendCode(txtid) {
    var recommendCode = $.trim($(txtid).val());
    if (recommendCode == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入推荐会员编号！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var isValiable = Web.UserReg.checkRecommendCode(recommendCode).value.trim();
    if (isValiable == "false") {
        $(txtid.id.replace("txt", "#lbl")).text("推荐会员编号不可用！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkParentCode(txtid) {
    var parentCode = $.trim($(txtid).val());
    if (parentCode == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入接点会员编号！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var isValiable = Web.UserReg.checkParentCode(parentCode).value.trim();
    if (isValiable == "false") {
        $(txtid.id.replace("txt", "#lbl")).text("接点会员编号不可用！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    if (isValiable == "full") {
        $(txtid.id.replace("txt", "#lbl")).text("接点会员脚下无空位！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    if (isValiable == "onlyleft") {
        $(txtid.id.replace("txt", "#lbl")).text("接点会员脚下无空位！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkAgentCode(txtid) {
    var agentCode = $.trim($(txtid).val());
    if (agentCode == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入服务中心编号！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var isValiable = Web.UserReg.checkAgentCode(agentCode).value.trim();
    if (isValiable == "false") {
        $(txtid.id.replace("txt", "#lbl")).text("服务中心编号不可用！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkPassword(txtid, level) {
    var reg = /^[a-zA-Z0-9]{3,8}$/;
    var text = $.trim($(txtid).val());
    if (text == "") {
        if (level == 1) {
            $(txtid.id.replace("txt", "#lbl")).text("请输入登录密码！");
        }
        if (level == 2) {
            $(txtid.id.replace("txt", "#lbl")).text("请输入二级密码！");
        }
        if (level == 3) {
            $(txtid.id.replace("txt", "#lbl")).text("请输入三级密码！");
        }
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    if (!reg.test(text)) {
        $(txtid.id.replace("txt", "#lbl")).text("密码必须由3位到8位数字或字母组成！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkConfirmPassword(txtid, contxtid) {
    var text = $.trim($(txtid).val());
    var context = $.trim($(contxtid).val());
    if (text == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入确认密码！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    if (text != context) {
        $(txtid.id.replace("txt", "#lbl")).text("两次输入的密码不一致！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkIdenCode(txtid) {
    var reg = /(^\d{15}$)|(^\d{17}([0-9]|X)$)/;
    var text = $.trim($(txtid).val());
    if (text == "") {
        $(txtid.id.replace("txt", "#lbl")).text("请输入身份证号！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    if (!reg.test(text)) {
        $(txtid.id.replace("txt", "#lbl")).text("身份证号格式不正确！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    var isValiable = Web.UserReg.checkIdenCode(text).value.trim();
    if (isValiable == "false") {
        $(txtid.id.replace("txt", "#lbl")).text("您的身份证号已经被占用！");
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function checkBank() {
    var seleText = $.trim($('#ddlsele_province option:selected').text());
    if (seleText == "请选择") {
        $("#lbl_bankBranch").css({ display: "block" });
        $("#img_bankBranch").css({ display: "none" });
        return false;
    }
    var text = $.trim($("#txt_bankBranch").val());
    if (text == "") {
        $("#lbl_bankBranch").css({ display: "block" });
        $("#img_bankBranch").css({ display: "none" });
        return false;
    }
    $("#lbl_bankBranch").css({ display: "none" });
    $("#img_bankBranch").css({ display: "" });
}

function checkEmpty(txtid) {
    var text = $.trim($(txtid).val());
    if (text == "") {
        $(txtid.id.replace("txt", "#lbl")).css({ display: "block" });
        $(txtid.id.replace("txt", "#img")).css({ display: "none" });
        return false;
    }
    $(txtid.id.replace("txt", "#lbl")).css({ display: "none" });
    $(txtid.id.replace("txt", "#img")).css({ display: "" });
}

function CheckContent() {
    var aer = $('#city').find("select").eq(2).val();
    if (aer == "请选择") {
        document.getElementById("img_bankBranch").style.display = "none";
        document.getElementById("lbl_bankBranch").style.display = "block";
        return false;
    }
    document.getElementById("img_bankBranch").style.display = "none";
    document.getElementById("lbl_bankBranch").style.display = "";
}