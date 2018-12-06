$(function () {
    $('#red').data('jstree', false).empty();
    $('#red').jstree({
        'core': {
            'data': {
                "url": "/Handle/DataHandler.ashx?type=tree&uid=" + $(".userid").val() + "&tage=" + $(".tage").val(),
                "dataType": "json",
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        }

    });
    $('#red').jstree().hide_icons();

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
}