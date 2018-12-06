var v = 600000;
$(document).ready(function () {
    setInterval("CountDown()", 1000);
   
})

$("body").click(function () {
    v = v + 2000;
    //alert("还剩：" + v + "毫秒")
    if (v < 0) {
       // $.session.clear();
        window.location = '/loginout.aspx';
    }
})
function CountDown() {
    v = v - 1000;
}