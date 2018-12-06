$(function () {
    //$("#red").treeview({
    //    collapsed: false,
    //    url: "/Handle/DataHandler.ashx?type=tree&uid=" + $(".userid").val() + "&tage=" + $(".tage").val(),
    //    unique: false,
    //    persist: 'cookie',
    //    animated: "medium"

    //});


})
function Serach() {
    $('#red').data('jstree', false).empty();
    $('#red').jstree({
        'core': {
            'data': {
                "url": "/Handle/DataHandler.ashx?type=tree&uid=" + $("#uid").val() + "&tage=" + $(".tage").val() + "&UserCode=" + $("#txtUserCode").val(),
                "dataType": "json",
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        }

    });
    $('#red').jstree().hide_icons();
    //$("#red").html("");
    //$("#red").treeview({
    //    collapsed: false,
    //    url: "/Handle/DataHandler.ashx?type=tree&uid=" + $(".userid").val() + "&tage=" + $(".tage").val() + "&UserCode=" + $("#txtUserCode").val(),
    //    unique: false,
    //    persist: 'cookie',
    //    toggle: function (e) {
    //        console.log(this.text);
    //        e.preventDefault();//阻止事件冒泡
    //    }
    //    //animated: "medium"
    //});
}
