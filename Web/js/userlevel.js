// JavaScript Document
window.onload = function ShowPic() {
    for (var i = 0; i < getTextValue(); i++) {
        document.getElementById('star').appendChild(document.createTextNode(''));
        var img = document.createElement('img');
        img.src = '../images/start.png';
        document.getElementById('star').appendChild(img);
    }
}
function getTextValue() {
    var starNum = document.getElementById("userlevel").innerHTML;
    return starNum;
}