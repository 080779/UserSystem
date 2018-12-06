var noresizeFrame = true;
$(document).ready(function(e) {
	$(document).on("click",".VerifyImage",function(e){
		e.preventDefault();
		e.stopPropagation();
		var nSrc = $.trim($(this).attr("src"));
		var tPos = nSrc.indexOf("?");
		var rndStr = "randstr="+(new Date()).getTime();
		if(tPos>=0)
		{
			tPos = nSrc.indexOf("randstr=");
			if(tPos>=0)
				nSrc = nSrc.substr(0,tPos)+rndStr;
			else
			{
				if(nSrc.indexOf("&") >= 0)
					nSrc += "&"+rndStr;
				else
					nSrc += rndStr;
			}
		}
		else
			nSrc += "?"+rndStr;
		$(this).attr("src",nSrc);
	});
	$("#Mask > #EmbedPageBoard > .plate_003 > .status > .CloseButton").click(function(ex){
		ex.stopPropagation();
		ex.preventDefault();
		$(this).parents("body").css("overflow","visible");
		$(this).parents(".plate_003").slideUp(300,function(){
			$(this).parents("#EmbedPageBoard").siblings("div").each(function(index, element) {
                $(this).hide();
            });
			$(this).find(".title").text("web dialog");
			$(this).parents("#Mask").hide();
			var jqoFrame = $(this).find("iframe");
			if(jqoFrame.length > 0)
				$(jqoFrame.get(0).contentWindow.document).find("body").empty();
			reflushMainFrame();
		});
	});
	$(".OrderTableAllCol th").click(function(ex){
		ex.stopPropagation();
		ex.preventDefault();
		ReorderTable(this,"vo");
	});
	try
	{
		pageReady(e);
	}catch(er){}
	if(!noresizeFrame)
		resizeFrame();
});
function showEmbedPage(url,size)
{
	if(typeof(size)=="undefined")
		size = "small";
	else if(typeof(size) == "string" && size=="big")
		size = "big";
	else
		size = "small";
	if(typeof(url) == "string" && $.trim(url).length>0)
	{
		var curWindow = window.self;
		do
		{
			var tObj = curWindow.window.parent;
			if(tObj === curWindow)
				break;
			else
				curWindow = curWindow.parent;
		}while(true);
		var plateEle = $(curWindow.document).find("#Mask > #EmbedPageBoard > .plate_003");
		if(plateEle.length > 0)
		{
			var jqoFrame = plateEle.find("iframe");
			if(jqoFrame.length > 0)
			{
				plateEle.parents("body").css("overflow","hidden");
				jqoFrame.load(function(){
					var jqoTitle = $(this.contentWindow.document).find("title");
					if(jqoTitle.length > 0)
						plateEle.find(".title").text(jqoTitle.text());
					$(this).unbind("load");
				});
				jqoFrame.attr("src",url);
				plateEle.siblings("div").each(function(index, element) {
                    $(this).hide();
                });
				plateEle.parent("#EmbedPageBoard").show();
				if(size == "big")
					plateEle.parent("#EmbedPageBoard").addClass("big");
				else
					plateEle.parent("#EmbedPageBoard").removeClass("big");
				plateEle.parents("#Mask").show();
				plateEle.slideDown(350);
			}
		}
	}
}
function reflushMainFrame()
{
	var jqoFrame = findFrame();
	if(jqoFrame)
	{
		jqoFrame.get(0).contentWindow.document.location.reload();
	}
}

function resizeFrame()
{
	var jqoFrame = findFrame();
	if(jqoFrame)
	{
		jqoFrame.height(1200);
		setFrame(jqoFrame);
		
		var oTitle = jqoFrame.parents("body").find("#SubPageTitle");
		var subPageTitleEle = $(jqoFrame.get(0).contentWindow.document).find("title");
		if(oTitle.length > 0 && subPageTitleEle.length > 0)
			oTitle.text(subPageTitleEle.text());
	}
	else
		;//alert('frame not found!');
}
function wapresizeFrame()
{
	var jqoFrame = findFrame();
	if(jqoFrame)
	{
		jqoFrame.height(1200);
		setFrame(jqoFrame);
		
		var oTitle = jqoFrame.parents("body").find("#wapSubPageTitle");
		var subPageTitleEle = $(jqoFrame.get(0).contentWindow.document).find("title");
		if(oTitle.length > 0 && subPageTitleEle.length > 0)
			oTitle.text(subPageTitleEle.text());
	}
	else
		;//alert('frame not found!');
}
function findFrame()
{
	var found = false;
	var oFrame = false;
	var curWindow = window.self;
	do
	{
		var tEle = $(curWindow.document).find("body");
		if(tEle.length > 0)
		{
			tEle = tEle.find("#MainFrame");
			if(tEle.length > 0)
			{
				oFrame = tEle;
				break;
			}
		}
		var tObj = curWindow.window.parent;
		if(tObj === curWindow)
			break;
		else
			curWindow = curWindow.parent;
	}while(true);
	
	return oFrame;
}
function setFrame(jqoFrame)
{
	if(typeof(jqoFrame) != "undefined" && jqoFrame.length > 0)
	{
		var tEle = $(jqoFrame.get(0).contentWindow.document);
		if(tEle.length > 0)
		{
			tEle = tEle.find("body");
			if(tEle.length > 0)
			{
				var tVal = tEle.get(0).scrollHeight;
				if(tVal < 500)
					tVal=500;
				jqoFrame.height(tVal);
				alert(tEle.get(0).scrollHeight);
			}
		}
	}
}
function getServerInfo(url,showEle)
{
	url = $.trim(url);
	var tEle = $(showEle);
	if(url.length > 0 && tEle.length > 0)
	{
		tEle.empty();
		$("<span style=\"color:#00F\">...</span>").appendTo(tEle);
		$.get(url,{"vrndstr":((new Date()).getTime()).toString()},function(data){
			tEle.empty();
			if(data.status == 0)
				$("<span style=\"color:#F00\">"+data.info+"</span>").appendTo(tEle);
			else
				$("<span style=\"color:inherit\">"+data.info+"</span>").appendTo(tEle);
		},"json");
	}
}
function ajaxForm(oForm,callback)
{
	var jqForm = $(oForm);
	if(jqForm.length > 0)
	{
		var url = jqForm.attr("action");
		var data = jqForm.serialize();
		var type = "post";
		if((jqForm.attr("method")).toString().toLowerCase() == "get")
			type = "get";
		try
		{
			if(typeof(callback)=="function")
				;
			else
				callback=function(d){};
		}catch(er){ callback=function(d){}; }
		send(url,type,data,callback);
	}
}
function send(url,type,data,callBack)
{
	var oPar = {};
	url = $.trim(url);
	if(url.length != 0)
		oPar.url = url;
	oPar.data = {};
	oPar.type = "post";
	if(typeof(type) == "string" && type.toLowerCase() == "get")
		oPar.type = "get";
	if(typeof(data) == "string" || typeof(data) == "object" && data != null)
	{
		if(typeof(data) == "string")
		{
			if(data.replace(/^(\w|\d|(\%[0-9a-fA-F]{2}))+\={1}.*(\&{1}(\w|\d|(\%[0-9a-fA-F]{2}))+\={1}.*)*$/g,"") == "")
				oPar.data = data;
		}
		else if(typeof(data) == "object")
		{
			if(Object.prototype.toString.call(data).toLowerCase() == "[object object]" && typeof(data.length) == "undefined")
			{
				var ic = 0;
				for(var i in data)
				{
					ic++;
					break;
				}
				if(ic > 0)
					oPar.data = data;
			}
		}
	}
	oPar.async = true;
	oPar.cache = false;
	oPar.dataType = "text";
	oPar.success = function(data, s){
		var rData;
		//alert(data);
		try
		{
			 rData= $.parseJSON(data);
		}
		catch(e){ return; }
		var status = parseInt(rData.status);
		if(!isNaN(status))
		{
			if(rData.info == 'user_no_login' || rData.info == 'login_timeout')
			{
				var curWindow = window.self;
				do
				{
					var tObj = curWindow.window.parent;
					if(tObj === curWindow)
					{
						curWindow.location.reload();
						break;
					}
					else
						curWindow = curWindow.parent;
				}while(true);
			}
			else
			{
				try
				{
					if(typeof(callBack)=="function")
						callBack(rData);
				}
				catch(er)
				{ 
					//alert(er);
					return;
				}
			}
		}
		else
	    {		
            //alert("Response data format error");
        }
	};
	oPar.error = function(o, s, e){
		switch(s)
		{
		case "timeout":
			//alert("Request failed: Timeout");
			break;
		case "error":
			if(o.readyState == 4)
				//alert("Request failed: Error");
			//alert(o.responseText);
			//alert(o.readyState);
			break;
		case "notmodified":
			//alert("Request failed: Notmodified");
			break;
		case "parsererror":
			//alert("Request failed: Parsererror");
			break;
		default:
			;
		}
	};
	if(typeof(oPar.data) == "string")
	{
		if(oPar.data.length == 0)
			Par.data = "ajax=1";
		else
			oPar.data += "&ajax=1";
	}
	else
		oPar.data.ajax = 1;
	$.ajax(oPar);
}

//实现老模板框架中的全选功能，函数名不变
function CheckAll(oEleForm)
{
	var oForm = $(oEleForm);
	if(oForm.length > 0)
	{
		oForm.find(":checkbox").each(function(index, element) {
			if($(this).prop("checked"))
				$(this).prop("checked",false);
			else
				$(this).prop("checked",true);
		});
	}
}