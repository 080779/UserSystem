var arrDate=new Array();
$.fn.extend({
    hoverClass: function (a) {
        return $(this).bind("touchstart", function () {
            var b = $(this);
            b.addClass(a || "active");
            setTimeout(function () {
                b.removeClass(a || "active");
            }, 500);
        }).bind("touchend", function () {
            $(this).removeClass(a || "active");
        });
    },
    datepicker: function (a) {
        var b = function (d, c) {
            this.fn = {
                div: function (e, g) {
                    var f = $("<div>").addClass(e);
                    if (g) {
                        f.appendTo(g);
                    }
                    return f;
                },
                a: function (e, f) {
                    return $("<a>").addClass(e).attr("id", f);
                },
                parseDate: function (f, e) {
                    if (f) {
                        if (typeof f === "string") {
                            f = Date.fromString(f);
                        }
                        return f;
                    } else {
                        return e;
                    }
                }
            };
            this.lastDate = null;
            this.currentDate = null;
            this.$input = d; //控件对象
            this.$f = $.dp_frame; //扩展信息
            this.config = {
                lang: {
                    prevMonth: "",
                    nextMonth: "",
                    today: "今天",
                    cancel: "取消",
                    month: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"]
                },
                onPicked: null,
                onClose: null,
                onOpen: null,
                onChange: null,
                firstDay: 7,
                minDate: new Date(1900, 0, 1),
                maxDate: new Date(2099, 11, 31),
                validDateFn: null,
                valueFormat: "yyyy-MM-dd",
                titleFormat: "yyyy年MM月",
                apiURL: null,
                MaxMonth: 6 //显示的月份数
            };
            this.setMaxDate = function (e) {
                this.config.maxDate = this.fn.parseDate(e, this.config.maxDate);
            };
            this.setMinDate = function (e) {
                this.config.minDate = this.fn.parseDate(e, this.config.minDate);
            };
            this.val = function (h) {
                if (h) {
                    var f = this.fn.parseDate(h);
                    if (this.config.onPicked) {
                        try {
                            this.config.onPicked(f);
                        } catch (g) { }
                    }
                    if (!this.lastDate || (this.config.onChange && this.lastDate.getTime() != f.getTime())) {
                        try {
   
                            this.config.onChange(this.lastDate, f);
                        } catch (g) { }
                    }
                    this.lastDate = f;
                    this.$input.val(f.format(this.config.valueFormat));
                    return f;
                }else {
                    return Date.fromString(this.$input.val());
                }
            };
            this.close = function () {
                this.currentDate = null;
                this.$f.wrapper.hide();
                $("select").show();
                $(document).unbind("scroll");
                if (this.config.onClose) {
                    try {
                        this.config.onClose();
                    } catch (f) { }
                }
            };

            this.show = function () {
                this.$f.weeks.empty();
                /*week html*/
                for (var g = 0; g < 7; g++) {
                    this.fn.div("ui_calendar_week", this.$f.weeks).html($.weekText[(g + this.config.firstDay) % 7]);
                }
                if (this.$f.month_content != null && this.$f.month_content != undefined) {
                    $("div").filter(".ui_calendar_content").remove();
                }
                if (this.$f.header != null && this.$f.header != undefined) {
                    $("div").filter(".ui_calendar_header").remove();
                }
                this.currentDate = new Date(); /*把查询日期更改为当前日期*/
                var tMonth=this.currentDate.getMonth();
                var tYear=this.currentDate.getFullYear();
                var bigMonth=0;

                for (var mIndex = 0; mIndex < this.config.MaxMonth; mIndex++) {
                    this.currentDate = new Date();
                    this.currentDate.setDate(1);
                    this.currentDate.setMonth(this.currentDate.getMonth() + mIndex);
                    drawCld(tYear,tMonth);
                    this.toDate(this.currentDate);
                    tMonth++;
                    if(tMonth>11&&bigMonth==0){
                        tYear++;
                        tMonth=0;
                        bigMonth++;
                    }
                }
                $("select").hide();
                this.$f.wrapper.show();
                if (this.config.onOpen) {
                    try {
                        this.config.onOpen();
                    } catch (f) { }
                }
            };
            this.nextMonth = function () {
                this.currentDate = this.currentDate || new Date();
                this.currentDate.setDate(1);
                this.currentDate.setMonth(this.currentDate.getMonth() + 1);
                this.toDate(this.currentDate);
            };
            this.prevMonth = function () {
                this.currentDate = this.currentDate || new Date();
                this.currentDate.setDate(1);
                this.currentDate.setMonth(this.currentDate.getMonth() - 1);
                this.toDate(this.currentDate);
            };
            this.toDate = function (f) {
                /*建立日期*/
                this.buildMonth(f.copy());
                this.$f.caption.html(f.format(this.config.titleFormat));
                var i = this.fn.parseDate(this.config.minDate);
                var g = this.fn.parseDate(this.config.maxDate);
                var j = new Date();
                var e = new Date(f.getFullYear(), f.getMonth(), 1);
                i.setHours(0, 0, 0, 0);
                g.setHours(0, 0, 0, 0);
                j.setHours(0, 0, 0, 0);
                var h = this;
                if (this.$f.prevbtn != null && this.$f.prevbtn != undefined) {
                    this.$f.prevbtn.unbind().html(this.config.lang.prevMonth).hide();
                    if (e.getTime() > i.getTime()) {
                        this.$f.prevbtn.click(function () { h.prevMonth(); }).hoverClass().show();
                    }
                }
                if (this.$f.nextbtn != null && this.$f.nextbtn != undefined) {
                    this.$f.nextbtn.unbind().html(this.config.lang.nextMonth).hide();
                    e.setDate(f.daysInMonth());
                    if (e.getTime() < g.getTime()) {
                        this.$f.nextbtn.click(function () { h.nextMonth(); }).hoverClass().show();
                    }
                }
                //今天控件显示
                if (this.$f.todaybtn != null && this.$f.todaybtn != undefined) {
                    this.$f.todaybtn.unbind().html(this.config.lang.today).hide();
                    if (j.getTime() >= i.getTime() && j.getTime() <= g.getTime() && (!this.config.validDateFn || this.config.validDateFn(j))) {
                        this.$f.todaybtn.click(function () { h.val(new Date()); h.close(); }).show();
                    }
                }
                //取消控件显示
                if (this.$f.cancelbtn != null && this.$f.cancelbtn != undefined) {
                    this.$f.cancelbtn.unbind().click(function () { h.close(); }).html(this.config.lang.cancel);
                }
            };
            this.initialize = function () {
                var g = $("body");
                var i = this.$f;
                var j = this;
                if (!i.wrapper) {
                    i.wrapper = $("<div>").addClass("ui_calendar").attr("id", "dp_wrapper").appendTo(g); //最外围的DIV层
                    i.weeks = this.fn.div("ui_calendar_weeks", i.wrapper);
                    //$(document).bind("scroll", function () {
                    //    var scrollHeight = $(document).scrollTop();
                    //    if (scrollHeight >= 45) {
                    //        $("#dp_wrapper").find("div:first").css({ "top": "0px", "position": "fixed", "z-index": "9992" });
                    //    } else {
                    //        $("#dp_wrapper").find("div:first").css({ "top": "44px", "position": "initial", "z-index": "9992" });
                    //    }
                    //});
                    //i.prevbtn = this.fn.a("ui ui_arrow_l", null).hoverClass().html(this.config.lang.prevMonth).appendTo(i.header);
                    //i.nextbtn = this.fn.a("ui ui_arrow_r", null).hoverClass().html(this.config.lang.nextMonth).appendTo(i.header);
                    //i.footer = this.fn.div("ui_calendar_footer", i.wrapper);
                    //i.todaybtn = this.fn.a("", null).hoverClass().appendTo(i.footer);
                    //i.cancelbtn = this.fn.a("", null).css("float", "right").hoverClass().appendTo(i.footer);
                }
                //if (this.config.onPicked && this.val() != "Invalid Date") {
                //    try {
                //        this.config.onPicked(this.val());
                //    } catch (h) { }
                //}
            };


            this.getStartDate = function () {
                return this.fn.parseDate(this.$input.val(), new Date());
            };
            this.buildMonth = function (h) {
                this.$f.header = this.fn.div("ui_calendar_header", this.$f.wrapper);
                this.$f.caption = this.fn.div("ui_calendar_month", this.$f.header);
                this.$f.month_content = this.fn.div("ui_calendar_content", this.$f.wrapper);

                var n = this;
                h.setDate(1);
                h.setHours(0, 0, 0, 0);
                var q = (h.firstDay() - this.config.firstDay + 7) % 7; //上个月应该出现的天数
                var j = h.daysInMonth(); //本月天数
                var p = h.addDays(-1 * q); //上个月的应显示的起始日期
                var r = this.getStartDate(); //当前查询日期
                var s = new Date(); //当前日期
                s.setHours(0, 0, 0, 0);
                var o = this.fn.parseDate(this.config.minDate); //最小日期
                o.setHours(0, 0, 0, 0);
                var m = this.fn.parseDate(this.config.maxDate); //最大日期
                m.setHours(0, 0, 0, 0);
                var g = null, e = null, f = true;
                //g:div最外围div层
                var currentMonthWeek = h.firstDay();
                var WeekIndex = 0;

                var CurrentMonth = s.getMonth() + 1;
                var HMonth = h.getMonth() + 1;

                for (var k = 0; k < j; k++) {
                    g = k >= q && q < (k + j) ? "ui_calendar_day" : "ui_calendar_day";
                    if (p.getTime() == s.getTime()) {
                        //是否等于当前日期
                   
                        if (CurrentMonth == HMonth) {
                            g += " ui_calendar_today";
                        } else if (r.getMonth() == h.getMonth()) {
                            g += " ";
                        }
                    }
                    if (p.getTime() == r.getTime()) {
                        //是否等于当前选择日期
                        
                        if (CurrentMonth == HMonth) {
                            g += " ui_calendar_active";
                        } else if (r.getMonth() == h.getMonth()) {
                            g += " ui_calendar_active";
                        }
                    }
                    if (p.getDay() % 6 == 0) {
                        g += " weekendday ";
                    }

                    f = p < o || p > m || (this.config.validDateFn && !this.config.validDateFn(p));
                    if (f) {
                        g += " ui_calendar_disable";
                    }
                    

                    if (WeekIndex < currentMonthWeek) {
                        WeekIndex++;
                        k = -1;
                        this.fn.a(g, ["dp", p.getFullYear(), "-", p.getMonth() + 1, "-", p.getDate()].join("")).html("<span class='num'>&nbsp;</span>").appendTo(this.$f.month_content);
                    } else {
                        var bhtml="";
                        for(var datei= 0;datei<arrDate.length;datei++)
                        {
                            if(arrDate[datei].gongli=="dp"+p.getFullYear()+'-'+( p.getMonth()+1)+ "-"+p.getDate())
                            {
                                bhtml= arrDate[datei].bhtml;
                            }
                        }
                        e = this.fn.a(g, ["dp", p.getFullYear(), "-", (p.getMonth() +1), "-", p.getDate()].join("")).html("<span class='num'>" + p.getDate() + "</span>").appendTo(this.$f.month_content).append(bhtml);
                        if (!f) {
                            e.click(function () {
                                n.val(Date.fromString($(this).attr("id").substr(2)));
                                n.close();
                            }).hoverClass();
                        }
                    }
                    p.setDate(p.getDate() + 1);
                }
                
            };
            $.extend(this.config, c);
            this.initialize();
        };
        return new b($(this), a);
    }
});
Date.fromString = function(a) {
    return new Date(Date.parse(a.trim().replace(/\-/g, "/")));
};
Date.prototype.format = function(a) {
    a = a || "yyyy-MM-dd";
    var c = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        S: this.getMilliseconds()
    };
    if (/(y+)/.test(a)) {
        a = a.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var b in c) {
        if (new RegExp("(" + b + ")").test(a)) {
            a = a.replace(RegExp.$1, RegExp.$1.length == 1 ? c[b] : ("00" + c[b]).substr(("" + c[b]).length));
        }
    }
    return a;
};
//是否是闰年
Date.prototype.isLeapYear = function() {
    return (0 == this.getYear() % 4 && ((this.getYear() % 100 != 0) || (this.getYear() % 400 == 0)));
};
Date.prototype.daysInMonth = function() {
    switch (this.getMonth()) {
        default:
            return 0;
        case 0:
        case 2:
        case 4:
        case 6:
        case 7:
        case 9:
        case 11:
            return 31;
        case 3:
        case 5:
        case 8:
        case 10:
            return 30;
        case 1:
            return this.isLeapYear() ? 29 : 28;
    }
};
Date.prototype.firstDay = function() {
    return new Date(this.getFullYear(), this.getMonth(), 1).getDay();
};
Date.prototype.addDays = function(a) {
    return new Date(this.getTime() + 86400000 * a);
};
Date.prototype.trim = function() {
    return new Date(this.getFullYear(), this.getMonth(), this.getDate());
};
Date.prototype.copy = function() {
    return new Date(this.getTime());
};
Date.prototype.getWeek = function() {
    return $.weekText[this.getDay()];
};



$.extend({
    weekText: ["周日", "周一", "周二", "周三", "周四", "周五", "周六"],
    dp_frame: {
        wrapper: null,
        masker: null,
        header: null,
        title: null,
        caption: null,
        prevbtn: null,
        nextbtn: null,
        weeks: null,
        footer: null,
        todaybtn: null,
        cancelbtn: null,
        body: null,
        month: null,
        month_bgcanvas: null,
        month_content: null
    }
});






<!--
/*****************************************************************************
                                   日期资料
*****************************************************************************/

var lunarInfo=new Array(
0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,0x16554,0x056a0,0x09ad0,0x055d2,
0x04ae0,0x0a5b6,0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,0x095b0,0x14977,
0x04970,0x0a4b0,0x0b4b5,0x06a50,0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,
0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,0x186e3,0x092e0,0x1c8d7,0x0c950,
0x0d4a0,0x1d8a6,0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,0x0a950,0x0b557,
0x06ca0,0x0b550,0x15355,0x04da0,0x0a5d0,0x14573,0x052d0,0x0a9a8,0x0e950,0x06aa0,
0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,0x0f263,0x0d950,0x05b57,0x056a0,
0x096d0,0x04dd5,0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,0x0b5a0,0x195a6,
0x095b0,0x049b0,0x0a974,0x0a4b0,0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,
0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,0x055c0,0x0ab60,0x096d5,0x092e0,
0x0c960,0x0d954,0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,0x092d0,0x0cab5,
0x0a950,0x0b4a0,0x0baa4,0x0ad50,0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,
0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,0x0a4e0,0x0d260,0x0ea65,0x0d530,
0x05aa0,0x076a3,0x096d0,0x04bd7,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,0x0d520,0x0dd45,
0x0b5a0,0x056d0,0x055b2,0x049b0,0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0)

var solarMonth=new Array(31,28,31,30,31,30,31,31,30,31,30,31);
var Gan=new Array("甲","乙","丙","丁","戊","己","庚","辛","壬","癸");
var Zhi=new Array("子","丑","寅","卯","辰","巳","午","未","申","酉","戌","亥");
var Animals=new Array("鼠","牛","虎","兔","龙","蛇","马","羊","猴","鸡","狗","猪");
var solarTerm = new Array("小寒","大寒","立春","雨水","惊蛰","春分","清明","谷雨","立夏","小满","芒种","夏至","小暑","大暑","立秋","处暑","白露","秋分","寒露","霜降","立冬","小雪","大雪","冬至")
var sTermInfo = new Array(0,21208,42467,63836,85337,107014,128867,150921,173149,195551,218072,240693,263343,285989,308563,331033,353350,375494,397447,419210,440795,462224,483532,504758)
var nStr1 = new Array('日','一','二','三','四','五','六','七','八','九','十')
var nStr2 = new Array('初','十','廿','卅','　')
var monthName = new Array("JAN","FEB","MAR","APR","MAY","JUN","JUL","AUG","SEP","OCT","NOV","DEC");

//国历节日 *表示放假日
var sFtv = new Array(
"0101*元旦",
"0214 情人节",
"0303 我生日",
"0308 妇女节",
"0312 植树节",
"0401 愚人节",
"0501 劳动节",
"0504 青年节",
"0512 护士节",
"0601 儿童节",
"0801 建军节",
"0910 教师节",
"1001*国庆节",
"1006 老人节",
"1024 联合国日"
)

//农历节日 *表示放假日
var lFtv = new Array(
"0101*春节",
"0115 元宵节",
"0505 端午节",
"0815 中秋节",
"0909 重阳节",
"1208 腊八节",
"1224 小年",
"0100*除夕")

//某月的第几个星期几
var wFtv = new Array(
"1144 Thanksgiving")


/*****************************************************************************
                                      日期计算
*****************************************************************************/

//====================================== 传回农历 y年的总天数
function lYearDays(y) {
    var i, sum = 348
    for(i=0x8000; i>0x8; i>>=1) sum += (lunarInfo[y-1900] & i)? 1: 0
    return(sum+leapDays(y))
}

//====================================== 传回农历 y年闰月的天数
function leapDays(y) {
    if(leapMonth(y))  return((lunarInfo[y-1900] & 0x10000)? 30: 29)
    else return(0)
}

//====================================== 传回农历 y年闰哪个月 1-12 , 没闰传回 0
function leapMonth(y) {
    return(lunarInfo[y-1900] & 0xf)
}

//====================================== 传回农历 y年m月的总天数
function monthDays(y,m) {
    return( (lunarInfo[y-1900] & (0x10000>>m))? 30: 29 )
}

//====================================== 算出农历, 传入日期物件, 传回农历日期物件
//                                       该物件属性有 .year .month .day .isLeap .yearCyl .dayCyl .monCyl
function Lunar(objDate) {

    var i, leap=0, temp=0
    var baseDate = new Date(1900,0,31)
    var offset   = (objDate - baseDate)/86400000

    this.dayCyl = offset + 40
    this.monCyl = 14

    for(i=1900; i<2050 && offset>0; i++) {
        temp = lYearDays(i)
        offset -= temp
        this.monCyl += 12
    }

    if(offset<0) {
        offset += temp;
        i--;
        this.monCyl -= 12
    }

    this.year = i
    this.yearCyl = i-1864

    leap = leapMonth(i) //闰哪个月
    this.isLeap = false

    for(i=1; i<13 && offset>0; i++) {
        //闰月
        if(leap>0 && i==(leap+1) && this.isLeap==false)
        { --i; this.isLeap = true; temp = leapDays(this.year); }
        else
        { temp = monthDays(this.year, i); }

        //解除闰月
        if(this.isLeap==true && i==(leap+1)) this.isLeap = false

        offset -= temp
        if(this.isLeap == false) this.monCyl ++
    }

    if(offset==0 && leap>0 && i==leap+1)
        if(this.isLeap)
        { this.isLeap = false; }
        else
        { this.isLeap = true; --i; --this.monCyl;}

    if(offset<0){ offset += temp; --i; --this.monCyl; }

    this.month = i
    this.day = offset + 1
}

//==============================传回国历 y年某m+1月的天数
function solarDays(y,m) {
    if(m==1)
        return(((y%4 == 0) && (y%100 != 0) || (y%400 == 0))? 29: 28)
    else
        return(solarMonth[m])
}
//============================== 传入 offset 传回干支, 0=甲子
function cyclical(num) {
    return(Gan[num%10]+Zhi[num%12])
}

//============================== 月历属性
function calElement(sYear,sMonth,sDay,week,lYear,lMonth,lDay,isLeap,cYear,cMonth,cDay) {

    this.isToday    = false;
    //国历
    this.sYear      = sYear;
    this.sMonth     = sMonth;
    this.sDay       = sDay;
    this.week       = week;
    //农历
    this.lYear      = lYear;
    this.lMonth     = lMonth;
    this.lDay       = lDay;
    this.isLeap     = isLeap;
    //干支
    this.cYear      = cYear;
    this.cMonth     = cMonth;
    this.cDay       = cDay;

    this.color      = '';

    this.lunarFestival = ''; //农历节日
    this.solarFestival = ''; //国历节日
    this.solarTerms    = ''; //节气

}

//===== 某年的第n个节气为几日(从0小寒起算)
function sTerm(y,n) {
    var offDate = new Date( ( 31556925974.7*(y-1900) + sTermInfo[n]*60000  ) + Date.UTC(1900,0,6,2,5) )
    return(offDate.getUTCDate())
}

//============================== 传回月历物件 (y年,m+1月)
function calendar(y,m) {

    var sDObj, lDObj, lY, lM, lD=1, lL, lX=0, tmp1, tmp2
    var lDPOS = new Array(3)
    var n = 0
    var firstLM = 0

    sDObj = new Date(y,m,1)            //当月一日日期

    this.length    = solarDays(y,m)    //国历当月天数
    this.firstWeek = sDObj.getDay()    //国历当月1日星期几


    for(var i=0;i<this.length;i++) {

        if(lD>lX) {
            sDObj = new Date(y,m,i+1)    //当月一日日期
            lDObj = new Lunar(sDObj)     //农历
            lY    = lDObj.year           //农历年
            lM    = lDObj.month          //农历月
            lD    = lDObj.day            //农历日
            lL    = lDObj.isLeap         //农历是否闰月
            lX    = lL? leapDays(lY): monthDays(lY,lM) //农历当月最後一天

            if(n==0) firstLM = lM
            lDPOS[n++] = i-lD+1
        }

        //sYear,sMonth,sDay,week,
        //lYear,lMonth,lDay,isLeap,
        //cYear,cMonth,cDay
        this[i] = new calElement(y, m+1, i+1, nStr1[(i+this.firstWeek)%7],
                                 lY, lM, lD++, lL,
                                 cyclical(lDObj.yearCyl) ,cyclical(lDObj.monCyl), cyclical(lDObj.dayCyl++) )


        if((i+this.firstWeek)%7==0)   this[i].color = 'red'  //周日颜色
        if((i+this.firstWeek)%14==13) this[i].color = 'red'  //周休二日颜色
    }

    //节气
    tmp1=sTerm(y,m*2)-1
    tmp2=sTerm(y,m*2+1)-1
    this[tmp1].solarTerms = solarTerm[m*2]
    this[tmp2].solarTerms = solarTerm[m*2+1]
    if(m==3) this[tmp1].color = 'red' //清明颜色

    //国历节日
    for(i in sFtv)
        if(sFtv[i].match(/^(\d{2})(\d{2})([\s\*])(.+)$/))
            if(Number(RegExp.$1)==(m+1)) {
                this[Number(RegExp.$2)-1].solarFestival += RegExp.$4 + ' '
                if(RegExp.$3=='*') this[Number(RegExp.$2)-1].color = 'red'
            }

    //月周节日
    for(i in wFtv)
        if(wFtv[i].match(/^(\d{2})(\d)(\d)([\s\*])(.+)$/))
            if(Number(RegExp.$1)==(m+1)) {
                tmp1=Number(RegExp.$2)
                tmp2=Number(RegExp.$3)
                this[((this.firstWeek>tmp2)?7:0) + 7*(tmp1-1) + tmp2 - this.firstWeek].solarFestival += RegExp.$5 + ' '
            }

    //农历节日
    for(i in lFtv)
        if(lFtv[i].match(/^(\d{2})(.{2})([\s\*])(.+)$/)) {
            tmp1=Number(RegExp.$1)-firstLM
            if(tmp1==-11) tmp1=1
            if(tmp1 >=0 && tmp1<n) {
                tmp2 = lDPOS[tmp1] + Number(RegExp.$2) -1
                if( tmp2 >= 0 && tmp2<this.length) {
                    this[tmp2].lunarFestival += RegExp.$4 + ' '
                    if(RegExp.$3=='*') this[tmp2].color = 'red'
                }
            }
        }

    //黑色星期五
    if((this.firstWeek+12)%7==5)
        this[12].solarFestival += '黑色星期五 '

    //今日
    if(y==tY && m==tM) this[tD-1].isToday = true;

}

//====================== 中文日期
function cDay(d){
    var s;

    switch (d) {
        case 10:
            s = '初十'; break;
        case 20:
            s = '二十'; break;
            break;
        case 30:
            s = '三十'; break;
            break;
        default :
            s = nStr2[Math.floor(d/10)];
            s += nStr1[d%10];
    }
    return(s);
}

///////////////////////////////////////////////////////////////////////////////

var cld;



function drawCld(SY,SM) {
    var i,sD,s,size;
    cld = new calendar(SY,SM);

    if(SY>1874 && SY<1909) yDisplay = '光绪' + (((SY-1874)==1)?'元':SY-1874)
    if(SY>1908 && SY<1912) yDisplay = '宣统' + (((SY-1908)==1)?'元':SY-1908)
    if(SY>1911 && SY<1950) yDisplay = '民国' + (((SY-1911)==1)?'元':SY-1911)

    if(SY>1949) yDisplay = '';

    for(i=0;i<42;i++) {


        var sObj2='SD'+i+'-'+SM;
        var sObj=$('#'+sObj2);



        var lObj2='LD'+i+'-'+SM;
        //alert(lObj);

        var lObj=$('#'+lObj2);
        sObj.className = '';

        sD = i - cld.firstWeek;


        if(sD>-1 && sD<cld.length) { //日期内

            var dateOb=new Object();
            dateOb.gongli='dp'+cld[sD].sYear+'-'+(cld[sD].sMonth)+'-'+cld[sD].sDay;
            if(cld[sD].lDay==1) //显示农历月
            {

                dateOb.bhtml=( '<b>'+(cld[sD].isLeap?'闰':'') + cld[sD].lMonth + '月' + (monthDays(cld[sD].lYear,cld[sD].lMonth)==29?'小':'大')+'</b>');
            }

            else //显示农历日
            {
                dateOb.bhtml=('<b class="font_set">'+cDay(cld[sD].lDay)+'</b>');
            }
            arrDate.push(dateOb);

            s=cld[sD].lunarFestival;
            if(s.length>0) { //农历节日
                if(s.length>6) s = s.substr(0, 4)+'…';
                s = s.fontcolor('red');
            }
            else { //国历节日
                s=cld[sD].solarFestival;
                if(s.length>0) {
                    size = (s.charCodeAt(0)>0 && s.charCodeAt(0)<128)?8:4;
                    if(s.length>size+2) s = s.substr(0, size)+'…';
                    s = s.fontcolor('blue');
                }
                else { //廿四节气
                    s=cld[sD].solarTerms;
                    if(s.length>0) s = s.fontcolor('limegreen');
                }
            }
            if(s.length>0) {
                lObj.innerHTML=s;
                dateOb.bhtml=s;
            }
        }
        else { //非日期
            sObj.html( '');
            lObj.html( '');
        }
    }
}


var Today = new Date();
var tY = Today.getFullYear();
var tM = Today.getMonth();
var tD = Today.getDate();
//////////////////////////////////////////////////////////////////////////////

var width = "130";
var offsetx = 2;
var offsety = 16;

var x = 0;
var y = 0;
var snow = 0;
var sw = 0;
var cnt = 0;

var dStyle;
//document.onmousemove = mEvn;



function setCookie(name, value) {
    var today = new Date()
    var expires = new Date()
    expires.setTime(today.getTime() + 1000*60*60*24*365)
    document.cookie = name + "=" + escape(value)  + "; expires=" + expires.toGMTString()
}

function getCookie(Name) {
    var search = Name + "="
    if(document.cookie.length > 0) {
        offset = document.cookie.indexOf(search)
        if(offset != -1) {
            offset += search.length
            end = document.cookie.indexOf(";", offset)
            if(end == -1) end = document.cookie.length
            return unescape(document.cookie.substring(offset, end))
        }
        else return ""
    }
}

