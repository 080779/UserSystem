var game = function (energyArr, UserID) {

    // 能量球
    var energyArr = energyArr ? energyArr : []
    var energyHtml = '', energy = energyArr.length;
    for (var i = 0; i < energyArr.length; i++) {
        energyHtml += '<span Class="NLQ" id=' + energyArr[i].ID + ' style=" bottom: ' + randomNum(0, 60) + '%; left: ' + randomNum(15, 85) + '%;">' + energyArr[i].Money + '</span>'
    }
    $("#gameBall").html(energyHtml)
    var collectMusic = document.getElementById("collectMusic");
    noEnergy()
    $("#gameBall span").click(function () {
        if (energy == 0) {
            return false;
        }
        var ID = this.id;
        var that = $(this);
        //alert(ID);
        //alert(UserID); 
        $.ajax({
            url: "/APPService/YunTu_IDSub.ashx?ID=" + ID + "&&UserID=" + UserID,
            type: "POST",
            success: function (data1) {

                var t = 0;
              
                if (!$("#gameCart").hasClass("in")) {
                    $("#gameCart").addClass("in").removeClass("out");
                    t = 1000
                }
                setTimeout(function () {
                    collectMusic.play();
                    energy--;
                    that.animate({
                        "bottom": "-20%",
                        "left": "55%",
                        "opacity": "0",
                    }, 1000)
                    setTimeout(function () {
                        $("#gameCart").addClass("full");
                        noEnergy()
                    }, 1000)
                }, t)
            }
        })

        
    })
    function noEnergy() {
        if (energy == 0) {
            $("#gameBall").html('<span class="no" style=" bottom: 0; left: 50%; opacity: 0;">正在生长中</span>')
            $(".no").animate({ "opacity": "1" });
            $("#gameCart").addClass("out").removeClass("in");
        }
        $("#gameCart").css("display", "block");
    }
}

//生成从minNum到maxNum的随机数
function randomNum(minNum, maxNum) {
    switch (arguments.length) {
        case 1:
            return parseInt(Math.random() * minNum + 1, 10);
            break;
        case 2:
            return parseInt(Math.random() * (maxNum - minNum + 1) + minNum, 10);
            break;
        default:
            return 0;
            break;
    }
}