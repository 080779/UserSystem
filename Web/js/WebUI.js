if(typeof WebUI == "undefined") WebUI={};
if(typeof WebUI.MemberServer == "undefined") WebUI.MemberServer={};
if(typeof WebUI.MemberServer.Index == "undefined") WebUI.MemberServer.Index={};
WebUI.MemberServer.Index_class = function() {};
Object.extend(WebUI.MemberServer.Index_class.prototype, Object.extend(new AjaxPro.AjaxClass(), {
	getNextClass: function() {
		return this.invoke("getNextClass", {}, this.getNextClass.getArguments().slice(0));
	},
	url: '/ajaxpro/WebUI.MemberServer.Index,App_Web_index.aspx.cdcab7d2.ashx'
}));
WebUI.MemberServer.Index = new WebUI.MemberServer.Index_class();

