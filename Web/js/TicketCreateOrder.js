var num = 1;
var htmlval = '';
$(document).ready(function () {

    loadbx($(".selectinsuranceproduct").eq(0)[0]);
    //$("#span_priceinfo").html(parseFloat($("#span_priceinfo").html()))
    $(".Jsselect").change(function () {
        $(this).prev().html($(this).find("option:selected").html());
    });
    //处理出生日期
    $(".idcardnumber").change(function () {
        var str = $(this).val().substring(0, 14)
        var b = str.substr(6, 8);
        var y = b.substr(0, 4)
        var m = b.substr(4, 2)
        var d = b.substr(6, 2)
        var bra = y + "-" + m + "-" + d
        if (IdentityCodeValid($(this).val())) {
            $(this).next().val(bra);
        }
        //alert($(this).next().val());
    });
    $("#passenger").on("change", ".selectinsuranceproduct", function (e) {
        //$(".selectinsuranceproduct").change(function () {
        console.log(e.target.value)
        loadbx(e.target);
    })
})
function loadbx(obj) {
    var action = "getinsuranceproducts";
    $.ajax({
        url: "/APPService/Ticket.ashx?act=getinsuranceproducts&action=" + action + "&type=" + obj.value,
        type: 'post',
        dataType: 'json',
        beforeSend: function () {
            layer.load();
        },
        complete: function () {
            layer.closeAll('loading');
        },
        success: function (msg) {

            if (msg.state == "error") {
                alert("提交失败")
            } else if (msg.state == "success") {
                var datajson = eval('(' + msg.data + ')');
                if (datajson.successcode == "T") {
                    var total = 0;
                    console.log(datajson.result[0].name);
                    $(".bx").eq($(".selectinsuranceproduct").index(obj)).html(datajson.result[0].name + "[" + datajson.result[0].price + "元]")

                    $(".bxje").eq($(".selectinsuranceproduct").index(obj)).html(datajson.result[0].price)
                    $(".bxms").eq($(".selectinsuranceproduct").index(obj)).html(datajson.result[0].remark)
                    htmlval = htmlval || "<ul class='passengerinfo'>" + $(".passengerinfo").html() + "</ul>";


                    $(".bxje").each(function () {
                        total += this.innerHTML * 1;
                    });

                    $("#span_priceinfo").html(parseFloat($("#pricesum").val()) * num + total);
                } else {
                    alert(datajson.info);
                }
            }
        },
        error: function () {

            alert("提交异常！");
        }
    })


}
function bxms(obj) {
    //console.log($(".bx").index($(obj).prev().prev()));
    layer.open({
        type: 1,
        title: false,
        closeBtn: 0,
        shadeClose: true,
        area: ['300px', '350px'],
        skin: 'alerts',
        content: $(".bxms").eq($(".bx").index($(obj).prev().prev())).html()
    });
}



function copypassenger() {
    $("#passhead").before(htmlval);
    var total = 0;
    $(".bxje").each(function () {
        total += this.innerHTML * 1;
    });
    // $("#passenger").before(htmlval);
    num = $(".passengerinfo").size()
    $("#span_priceinfo").html($("#pricesum").val() * num + total)
    $(".Jsselect").change(function () {
        $(this).prev().html($(this).find("option:selected").html());
    });
    $(".idcardnumber").change(function () {
        var str = $(this).val().substring(0, 14)
        var b = str.substr(6, 8);
        var y = b.substr(0, 4)
        var m = b.substr(4, 2)
        var d = b.substr(6, 2)
        var bra = y + "-" + m + "-" + d
        if (IdentityCodeValid($(this).val())) {
            $(this).next().val(bra);
        }

        //alert($(this).next().val());
    });
    $(".deletepassenger").show();
    $(".sc-cjr").off('click').on('click', function () {
        $(this).parents(".passengerinfo").remove();
        num -= 1;
        var total = 0;
        $(".bxje").each(function () {
            total += this.innerHTML * 1;
        });

        $("#span_priceinfo").html(parseFloat($("#pricesum").val()) * num + total);


        if ($(".deletepassenger").length > 1) {
            $(".deletepassenger").show();
        } else {
            $(".deletepassenger").hide();
        }
    });
}
function SubmitOrder() {

    if ($("#linkman").val() == "") {
        alert("请填写联系人姓名")
        return;
    }
    if ($("#linkmobile").val() == "") {
        alert("请填写联系人手机号码")
        return;
    }
    if ($(".idcardnumber").val() == "") {
        alert("请填写身份证号码")
        return;
    }

    var action = "createorder";
    var count = $(".passengerinfo").size()
    var passengers = "[";
    for (var i = 0; i < count; i++) {
        if (IdentityCodeValid($(".idcardnumber").eq(i).val())) {
            passengers += "{\"name\":\"" + $(".ticketname").eq(i).val() + "\",\"cardno\":\"" + $(".idcardnumber").eq(i).val() + "\",\"cardtype\":\"" + $(".selectcard").eq(i).find("option:selected").val() + "\",\"mantype\":\"" + $(".selectpaio").eq(i).find("option:selected").val() + "\",\"birthday\":\"" + $(".birthday").eq(i).val() + "\",\"sex\":\"" + $(".selectsex").eq(i).find("option:selected").val() + "\",\"insuranceprice\":\"" + $(".bxje").eq(i).html() + "\",\"insurancenum\":\"1\"}";
            if (i < count - 1) {
                passengers += ",";
            }
        } else {
            return;
        }
    }
    passengers += "]";
    console.log("起飞时间：" + $("#depdate").val() + " " + $("#deptime").val() + "，到达时间：" + $("#depdate").val() + " " + $("#arrtime").val())
    var segments = "{\"aircode\":\"" + $("#aircode").val() + "\",\"depcity\":\"" + $("#depcity").val() + "\",\"arrcity\":\"" + $("#arrcity").val() + "\",\"flight\":\"" + $("#flight").val() + "\",\"flightmodel\":\"" + $("#flightmodel").val() + "\",\"cabin\":\"" + $("#cabin").val() + "\",\"depdate\":\"" + $("#depdate").val() + "\",\"deptime\":\"" +$("#deptime").val() + "\",\"arrtime\":\"" + $("#arrtime").val() + "\",\"yprice\":\"" + $("#yprice").val() + "\",\"discount\":\"" + $("#discount").val() + "\",\"refundrule\":\"\",\"modifyrule\":\"\",\"updaterule\":\"不能改签\",\"depterminal\":\"" + $("#depterminal").val() + "\",\"arrterminal\":\"" + $("#arrterminal").val() + "\",\"prices\":{\"mantype\":\"ADT\",\"fare\":\"" + $("#dprice").val() + "\",\"totalprice\":\"" + (parseInt($("#dprice").val()) + parseInt($("#airportfee").val()) + parseInt($("#fuelfee").val())) + "\",\"airportfee\":\"" + $("#airportfee").val() + "\",\"fuelfee\":\"" + $("#fuelfee").val() + "\",\"staynum\":\"" + $("#staynum").val() + "\"}}";;
    var contact = "{\"linkman\":\"" + $("#linkman").val() + "\",\"linkmobile\":\"" + $("#linkmobile").val() + "\"}";
    var travelsheet = "{\"posttype\":\"1\",\"postprice\":\"0\",\"linkman\":\"\",\"linkmobile\":\"\",\"address\":\"\",\"needsheet\":\"\"}";
    console.log(passengers + "\n" + segments + "\n" + contact + "\n" + travelsheet)

    $.ajax({
        url: "/APPService/Ticket.ashx?act=createorder&UserID=" + $("#uid").val() + "&allprice=" + $("#span_priceinfo").html() + "&action=" + action + "&passengers=" + passengers + "&segments=" + segments + "&contact=" + contact + "&travelsheet=" + travelsheet + "&airname=" + $("#ticketcodeTxt").val() + "&depcityname=" + $("#depacity").val() + "&arrcityname=" + $("#arrycity").val(),
        type: 'post',
        dataType: 'json',
        beforeSend: function () {
            layer.load();
        },
        complete: function () {
            layer.closeAll('loading');
        },
        success: function (msg) {

            if (msg.state == "error") {
                alert("提交失败")
            } else if (msg.state == "success") {
                var datajson = eval('(' + msg.data + ')');
                if (datajson.successcode == "T") {
                    console.log(datajson.result.orderno);

                    var pams = [];
                    pams.push($('<input>', { name: 'ordercode', value: datajson.result.orderno, type: "hidden" }));
                    $('<form> ', {
                        method: 'post',
                        action: 'TicketOrderDetail.aspx'
                    }).append(pams).appendTo("body").submit();
                } else {
                    alert(datajson.info);
                }
            }
        },
        error: function () {

            alert("提交异常！");
        }
    })

}
function onclacktype(value) {
    if (value == "CHD") {
        $("#span_priceinfo").html(parseFloat($("#yprice").val() * 50 / 100) * num);
    }
    if (value == "INF") {
        $("#span_priceinfo").html(parseFloat($("#yprice").val() * 10 / 100) * num);
    }
    if (value == "ADT") {
        $("#span_priceinfo").html(parseFloat($("#pricesum").val()) * num);
    }
}
function IdentityCodeValid(code) {
    var city = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江 ", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北 ", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏 ", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外 " };
    var tip = "";
    var pass = true;

    if (!code || !/^\d{6}(18|19|20)?\d{2}(0[1-9]|1[12])(0[1-9]|[12]\d|3[01])\d{3}(\d|X)$/i.test(code)) {
        tip = "身份证号格式错误";
        pass = false;
    }

    else if (!city[code.substr(0, 2)]) {
        tip = "地址编码错误";
        pass = false;
    }
    else {
        //18位身份证需要验证最后一位校验位
        if (code.length == 18) {
            code = code.split('');
            //∑(ai×Wi)(mod 11)
            //加权因子
            var factor = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
            //校验位
            var parity = [1, 0, 'X', 9, 8, 7, 6, 5, 4, 3, 2];
            var sum = 0;
            var ai = 0;
            var wi = 0;
            for (var i = 0; i < 17; i++) {
                ai = code[i];
                wi = factor[i];
                sum += ai * wi;
            }
            var last = parity[sum % 11];
            if (parity[sum % 11] != code[17]) {
                tip = "身份证号格式不正确";
                pass = false;
            }
        }
    }
    if (!pass) alert(tip);
    return pass;
}