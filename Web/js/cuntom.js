$(function(){
$('#_menu').find('li[menu]').each(function(){
var _menu = $(this).attr('menu');
$(this).children(':eq(0)').unbind().click(function(){
		var show_menu = $(this).parent().parent().find('ul').filter(':visible');
		if(show_menu.length > 0 ) show_menu.slideUp(200);
		var _left = $(this).offset().left + 'px',
			_top = $(this).offset().top + parseInt($(this).outerHeight()) + 'px';
		$('#'+_menu).css({left:_left,top:_top});
		if($('#'+_menu).css('display')=='none')
			$('#'+_menu).slideDown(400); 
	});
});
})

var speed,animateTime,pObj,
	moveHeight = MyMar = 0;
function JsMove(dom,num,_speed,_aniTime){
	pObj = dom;
	speed = _speed == null ? 2500 : _speed;
	animateTime = _aniTime == null ? 800 : _aniTime;
	var childObj = $('#'+pObj).children(':first');
	$('#'+pObj).children(':not(:first)').remove();
	$('#'+pObj).scrollTop(0);
	if(childObj.children().length > 0 && speed > animateTime){
		var __childName = childObj.children(':first')[0].tagName.toLowerCase();
		moveHeight = num*(childObj.height() / childObj.find(__childName).length);
		childObj.clone().insertAfter(childObj).end().html(childObj.html());
		MyMar = setInterval(Marquee,speed);
		$('#'+pObj).hover(function(){clearInterval(MyMar)},
		function(){MyMar = setInterval(Marquee,speed)});
	}
}
function Marquee(){
	var parentObj = $('#'+pObj),
		f_childObj = parentObj.children(':first'),
		s_childObj = f_childObj.next();
	if(s_childObj.height()-parentObj.scrollTop() <= 0){
		parentObj.scrollTop(parentObj.scrollTop() - f_childObj.height());
		parentObj.animate({scrollTop:"+=" + moveHeight},animateTime);
	}else{
		parentObj.animate({scrollTop:"+=" + moveHeight},animateTime);
	}
}