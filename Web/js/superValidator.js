//***********************************************************
//根据原有验证框架进行改进
//使用时候需要给要增加验证的标签增加check属性
//当的时候,允许录入为空,如果输入数据就按reg属性绑定的正则表达进行验证.
//当的时候,就直接按照reg绑定的正则表达式进行验证.
//有问题希望跟我交流下,谢谢支持 QQ6997467
//***********************************************************
//获得所有需要验证的标签
(function($){
	$(document).ready(function(){
		$('select[tip],select[check],input[tip],input[check],textarea[tip],textarea[check]').tooltip();
	});
})(jQuery);

(function($) {
    $.fn.tooltip = function(options){
		var getthis = this;
        var opts = $.extend({}, $.fn.tooltip.defaults, options);
		//创建提示框
        $('body').append('<table id="tipTable" class="tableTip"><tr><td  class="leftImage"></td> <td class="contenImage" align="left"></td> <td class="rightImage"></td></tr></table>');
		//移动鼠标隐藏刚创建的提示框
        $(document).mouseout(function(){$('#tipTable').hide()});
		
        this.each(function(){
            if($(this).attr('tip') != '')
            {
                $(this).mouseover(function(){
                    $('#tipTable').css({left:$.getLeft(this)+'px',top:$.getTop(this)+'px'});
                    $('.contenImage').html($(this).attr('tip'));
                    $('#tipTable').fadeIn("fast");
                },
                function(){
                    $('#tipTable').hide();
                });
            }
            if($(this).attr('check') != '')
            {
				
                $(this).focus(function()
				{
                    $(this).removeClass('tooltipinputerr');
                }).blur(function(){
                    if($(this).attr('toupper') == 'true')
                    {
                        this.value = this.value.toUpperCase();
                    }
					if($(this).attr('check') != '')
					{
						
						if($(this).attr('check')=="1")
						{
							
							
							if($(this).attr('value')==null)
							{
								
								$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
							}else
							{
								
								var thisReg = new RegExp($(this).attr('reg'));
								if(thisReg.test(this.value))
								{
									$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
								}
								else
								{
									$(this).removeClass('tooltipinputok').addClass('tooltipinputerr');
								}
								
							}
						}
						if($(this).attr('check')=="2")
						{
							var thisReg = new RegExp($(this).attr('reg'));
								if(thisReg.test(this.value))
								{
									$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
								}
								else
								{
									$(this).removeClass('tooltipinputok').addClass('tooltipinputerr');
								}
						}			
					}
                    
                });
            }
        });
        if(opts.onsubmit)
        {
            $('form').submit( function () {
                var isSubmit = true;
                getthis.each(function(){
					if($(this).attr('check')=="1")
						{
							
							
							if($(this).attr('value')==null)
							{
								
								$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
							}else
							{
								
								var thisReg = new RegExp($(this).attr('reg'));
								if(thisReg.test(this.value))
								{
									$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
								}
								else
								{
									$(this).removeClass('tooltipinputok').addClass('tooltipinputerr');
									isSubmit = false;
								}
								
							}
						}
                    if($(this).attr('check')=="2")
						{
							var thisReg = new RegExp($(this).attr('reg'));
								if(thisReg.test(this.value))
								{
									$(this).removeClass('tooltipinputerr').addClass('tooltipinputok');
								}
								else
								{
									$(this).removeClass('tooltipinputok').addClass('tooltipinputerr');
									isSubmit = false;
								}
						}			
                });
                return isSubmit;
            } );
        }
    };

    $.extend({
        getWidth : function(object) {
            return object.offsetWidth;
        },

        getLeft : function(object) {
            var go = object;
            var oParent,oLeft = go.offsetLeft;
            while(go.offsetParent!=null) {
                oParent = go.offsetParent;
                oLeft += oParent.offsetLeft;
                go = oParent;
            }
            return oLeft;
        },

        getTop : function(object) {
            var go = object;
            var oParent,oTop = go.offsetTop;
            while(go.offsetParent!=null) {
                oParent = go.offsetParent;
                oTop += oParent.offsetTop;
                go = oParent;
            }
            return oTop + $(object).height()+ 5;
        },

        onsubmit : true
    });
    $.fn.tooltip.defaults = { onsubmit: true };
})(jQuery);

//***************************************************************************************************************************************************
//利用JQuery功能对标签属性设置表达式
//传入的标签ID组必须为"name1:name2:name3"中间用':'分隔.

function surnam_keyup(text) {
    //控件值
    var textvalue = text.value;
    //非法字符集
    var codes = '<>/@#%';
    //非法字符数组
    var codearray = codes.split('');
    //循环替换非法字符
    for (i = 0; i < codearray.length; i++) {
        while (textvalue.indexOf(codearray[i]) != -1) {
            textvalue = textvalue.replace(codearray[i], '');
        }
    }
    //重新给控件赋值
    text.value = textvalue;

}


//对所有需要整数验证的标签进行设置正则表达式
function setIntegeCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^[0-9]\\d*$");
		}
	}
}

//对所有需要金额验证的标签进行设置正则表达式
function setMoneyCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^(-)?(([1-9]{1}\\d*)|([0]{1}))(\\.(\\d){1,2})?$");
		}
	}
}

//对所有需要正浮点验证的标签进行设置正则表达式
function setFloatCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^[0-9]+\\.[0-9]+$");
		}
	}
}

//对所有需要电子邮件验证的标签进行设置正则表达式
function setMailCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$");
		}
	}
}

//对所有需要邮编验证的标签进行设置正则表达式
function setZipcodeCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^\\d{6}$");
		}
	}
}

//对所有需要手机验证的标签进行设置正则表达式
function setMobileCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^(13|15|18)[0-9]{9}$");
		}
	}
}

//////对所有需要手机验证的标签进行设置正则表达式
//function islike(validatorString)
//{
//	var validatorStrings="";
//	if(validatorString!="")
//	{
//		validatorStrings=validatorString.split(":");
//		$("#"+validatorStrings[1]).attr("reg",validatorStrings[0]).attr("reg"));
//	}
//}

//对所有需要电话验证的标签进行设置正则表达式
function setstaticCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^(0)[0-9]{3}(-)[0-9]{7}$");
		}
	}
}

//对所有需要身份证验证的标签进行设置正则表达式
function setIDCheck(validatorString) {
    var validatorStrings = "";
    if (validatorString != "") {
        validatorStrings = validatorString.split(":");
        for (i = 0; i < validatorStrings.length; i++) {
            $("#" + validatorStrings[i]).attr("reg", "^[1-9]([0-9]{14}|[0-9]{17})|[1-9]([0-9]{13}|[0-9]{16})[x]$");
        }
    }
}
//对所有需要身份证验证的标签进行设置正则表达式
function setbankcodeCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^[0-9]{19}$");
		}
	}
}

//对所有需要登录帐号验证的标签进行设置正则表达式
function setUserIDCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^\\w+$");
		}
	}
}
//对所有日期的标签进行设置正则表达式
function setDateCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
		    $("#"+validatorStrings[i]).attr("reg","^[1|2]{1}\\d{3}-[1|0]{1}\\d{1}-[1|2|3]{1}\\d{1}$");
//			$("#"+validatorStrings[i]).attr("reg","^\\d{4}-[1|0]\\d{1}-[1|2|3]\\d{1}$");
		}
	}
}

//对所有需要登录密码验证的标签进行设置正则表达式
function setpasswordCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^[\@A-Za-z0-9\!\#\$\%\^\&\*\.\~]{6,18}$");
		}
	}
}

//对所有需要非空验证的标签进行设置正则表达式
function setEmptyCheck(validatorString)
{
	
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg",'.*\\S.*');
		}
	}
}

//对所有需要中文验证的标签进行设置正则表达式
function setChineseCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^[\\u4E00-\\u9FA5\\uF900-\\uFA2D]+$");
		}
	}
}
// 验证（中文，字母，数字，下划线）的正则表达式
function setAllCheck(validatorString) {
    var validatorStrings = "";
    if (validatorString != "") {
        validatorStrings = validatorString.split(":");
        for (i = 0; i < validatorStrings.length; i++) {
            $("#" + validatorStrings[i]).attr("reg", "^[0-9a-zA-Z\u4e00-\u9fa5]+$");
        }
    }
}
//对所有需要URL验证的标签进行设置正则表达式
function setURLCheck(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","^http[s]?:\\/\\/([\\w-]+\\.)+[\\w-]+([\\w-./?%&=]*)?$");
		}
	}
}
//匹配国内电话号码(0511-4405222 或 021-87888822) 
function setTell(validatorString)
{
	var validatorStrings="";
	if(validatorString!="")
	{
		validatorStrings=validatorString.split(":");
		for(i=0;i<validatorStrings.length;i++)
		{
			$("#"+validatorStrings[i]).attr("reg","\\d{3}-\\d{8}|\\d{4}-\\d{7}");
		}
	}
}  
//***************************************************************************************************************************************************