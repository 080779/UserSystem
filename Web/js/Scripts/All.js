function request(paras) {
    var url = location.href;
    var paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
    var paraObj = {}
    for (i = 0; j = paraString[i]; i++) {
        paraObj[j.substring(0, j.indexOf("=")).toLowerCase()] = j.substring(j.indexOf("=") + 1, j.length);
    }
    var returnValue = paraObj[paras.toLowerCase()];
    if (typeof (returnValue) == "undefined") {
        return "";
    } else {
        return returnValue;
    }
}
var lan;
var lang;
$(document).ready(function () {
    lan = request("L").toLowerCase(); 
    if (lan == "en") { 
        lang = En;
    } else {
        lan="zh"
        lang = Zh;
    }
    for (var p in lang) {
        $(".span-" + p).html(lang[p]); 
        $(".btn-" + p).val( lang[p]);
    }
    $("body").addClass(lan);
    lang.sss = 111;  
});