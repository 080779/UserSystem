//去空格
function Trim(strValue) {
    return (Ltrim(Rtrim(strValue)));
}
//去右空格
function Rtrim(strValue) {
    while (strValue.charCodeAt(strValue.length - 1) == 32) {
        strValue = strValue.substring(0, strValue.length - 1);
    }
    return strValue;
}
//去左空格
function Ltrim(strValue) {
    while (strValue.charCodeAt(0) == 32) {
        strValue = strValue.substring(1, strValue.length);
    }
    return strValue;
}