(function($) { $.extend($.browser, { client: function() { return { width: document.documentElement.clientWidth, height: document.documentElement.clientHeight, bodyWidth: document.body.clientWidth, bodyHeight: document.body.clientHeight }; }, scroll: function() { return { width: document.documentElement.scrollWidth, height: document.documentElement.scrollHeight, bodyWidth: document.body.scrollWidth, bodyHeight: document.body.scrollHeight, left: document.documentElement.scrollLeft, top: document.documentElement.scrollTop + document.body.scrollTop }; }, screen: function() { return { width: window.screen.width, height: window.screen.height }; }, isIE6: $.browser.msie && $.browser.version == 6, isMinW: function(val) { return Math.min($.browser.client().bodyWidth, $.browser.client().width) <= val; }, isMinH: function(val) { return $.browser.client().height <= val; } }) })(jQuery); (function($) { $.widthForIE6 = function(option) { var s = $.extend({ max: null, min: null, padding: 0 }, option || {}); var init = function() { var w = $(document.body); if ($.browser.client().width >= s.max + s.padding) { w.width(s.max + "px"); } else if ($.browser.client().width <= s.min + s.padding) { w.width(s.min + "px"); } else { w.width("auto"); } }; init(); $(window).resize(init); } })(jQuery);
(function($) { $.fn.hoverForIE6 = function(option) { var s = $.extend({ current: "hover", delay: 1000 }, option || {}); $.each(this, function() { var timer1 = null, timer2 = null, flag = false; $(this).bind("mouseover", function() { if (flag) { clearTimeout(timer2); } else { var _this = $(this); timer1 = setTimeout(function() { _this.addClass(s.current); flag = true; }, s.delay); } }).bind("mouseout", function() { if (flag) { var _this = $(this); timer2 = setTimeout(function() { _this.removeClass(s.current); flag = false; }, s.delay); } else { clearTimeout(timer1); } }) }) } })(jQuery);
(function($) {
    $.extend({ _jsonp: { scripts: {}, counter: 1, head: document.getElementsByTagName("head")[0], name: function(callback) {
        var name = '_jsonp_' + (new Date).getTime() + '_' + this.counter; this.counter++;
        var cb = function(json) { eval('delete ' + name); callback(json); $._jsonp.head.removeChild($._jsonp.scripts[name]); delete $._jsonp.scripts[name]; }; eval(name + ' = cb'); return name;
    }, load: function(url, name) { var script = document.createElement('script'); script.type = 'text/javascript'; script.charset = this.charset; script.src = url; this.head.appendChild(script); this.scripts[name] = script; }
    }, getJSONP: function(url, callback) {
        var name = $._jsonp.name(callback);
        var url = url.replace(/{callback};/, name); $._jsonp.load(url, name); return this;
    }
    });
})(jQuery);
(function($) {
    $.fn.jdTab = function(option, callback) {
        if (typeof option == "function") { callback = option; option = {}; };
        var s = $.extend({ type: "static", auto: false, source: "data", event: "mouseover", currClass: "curr", tab: ".tab", content: ".tabcon", itemTag: "li", stay: 5000, delay: 100, mainTimer: null, subTimer: null, index: 0 }, option || {});
        var tabItem = $(this).find(s.tab).eq(0).find(s.itemTag), contentItem = $(this).find(s.content); if (tabItem.length != contentItem.length) return false;
        var reg = s.source.toLowerCase().match(/http:\/\/|\d|\.aspx|\.ascx|\.asp|\.php|\.html\.htm|.shtml|.js|\W/g);
        var init = function(n, tag) { s.subTimer = setTimeout(function() { hide(); if (tag) { s.index++; if (s.index == tabItem.length) s.index = 0; } else { s.index = n; }; s.type = (tabItem.eq(s.index).attr(s.source) != null) ? "dynamic" : "static"; show(); }, s.delay); };
        var autoSwitch = function() { s.mainTimer = setInterval(function() { init(s.index, true); }, s.stay); };
        var show = function() {
            tabItem.eq(s.index).addClass(s.currClass); switch (s.type) {
                default: case "static":
                    var source = ""; break; case "dynamic":
                    var source = (reg == null) ? tabItem.eq(s.index).attr(s.source) : s.source; tabItem.eq(s.index).removeAttr(s.source); break;
            }; if (callback) { callback(source, contentItem.eq(s.index), s.index); }; contentItem.eq(s.index).show();
        };
        var hide = function() { tabItem.eq(s.index).removeClass(s.currClass); contentItem.eq(s.index).hide(); }; tabItem.each(function(n) { $(this).bind(s.event, function() { clearTimeout(s.subTimer); clearInterval(s.mainTimer); init(n, false); return false; }).bind("mouseleave", function() { if (s.auto) { autoSwitch(); } else { return; } }) }); if (s.type == "dynamic") { init(s.index, false); }; if (s.auto) { autoSwitch(); }
    }
})(jQuery);
(function($) {
    $.fn.jdSlide = function(option) {
        var settings = $.extend({ width: null, height: null, pics: [], index: 0, type: "num", current: "curr", delay1: 200, delay2: 8000 }, option || {}); var element = this; var timer1, timer2, timer3, flag = true; var total = settings.pics.length; var init = function() { var img = "<ul style='position:absolute;top:0;left:0;'><li><a href='" + settings.pics[0].href + "' target='_blank'><img src='" + settings.pics[0].src + "' width='" + settings.width + "' height='" + settings.height + "' /></a></li></ul>"; element.css({ "position": "relative" }).html(img); $(function() { delayLoad(); }) }; init(); var initIndex = function() { var indexs = "<div>"; var current; var x; for (var i = 0; i < total; i++) { current = (i == settings.index) ? settings.current : ""; switch (settings.type) { case "num": x = i + 1; break; case "string": x = settings.pics[i].alt; break; case "image": x = "<img src='" + settings.pics[i].breviary + "' />"; default: break; }; indexs += "<span class='" + current + "'><a href='" + settings.pics[i].href + "' target='_blank'>" + x + "</a></span>"; }; indexs += "</div>"; element.append(indexs); element.find("span").bind("mouseover", function(e) { clearInterval(timer1); clearInterval(timer3); var index = element.find("span").index(this); if (index == settings.index) { return; } else { timer3 = setInterval(function() { if (flag) running(parseInt(index)); }, settings.delay1); } }).bind("mouseleave", function(e) { clearInterval(timer3); timer1 = setInterval(function() { running(settings.index + 1, true); }, settings.delay2); }) }; var running = function(index, tag) {
            if (index == total) { index = 0; }; element.find("span").eq(settings.index).removeClass(settings.current); element.find("span").eq(index).addClass(settings.current); timer2 = setInterval(function() {
                var pos_y = parseInt(element.find("ul").get(0).style.top);
                var pos_a = Math.abs(pos_y + settings.index * settings.height);
                var pos_b = Math.abs(index - settings.index) * settings.height;
                var pos_c = Math.ceil((pos_b - pos_a) / 4); if (pos_a == pos_b) { clearInterval(timer2); if (tag) { settings.index++; if (settings.index == total) { settings.index = 0; } } else { settings.index = index; }; flag = true; } else { if (settings.index < index) { element.find("ul").css({ top: pos_y - pos_c + "px" }); } else { element.find("ul").css({ top: pos_y + pos_c + "px" }); }; flag = false; }
            }, 10);
        };
        var delayLoad = function() { var img = ""; for (var i = 1; i < total; i++) { img += "<li><a href='" + settings.pics[i].href + "' target='_blank'><img src='" + settings.pics[i].src + "' width='" + settings.width + "' height='" + settings.height + "' /></a></li>"; }; element.find("ul").append(img); timer1 = setInterval(function() { running(settings.index + 1, true); }, settings.delay2); if (settings.type) initIndex(); };
    }
})(jQuery);