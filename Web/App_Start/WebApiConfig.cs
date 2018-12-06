using IMS.Web.App_Start.Filter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace IMS.Web.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        { 
            config.MapHttpAttributeRoutes();

         //   config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );//.RouteHandler = new SessionControllerRouteHandler();

            GlobalConfiguration.Configuration.EnsureInitialized();
            //config.MessageHandlers.Add(new CustomErrorMessageDelegatingHandler());
            //config.Filters.Add(new ApiSYSAuthorizationFilter());
            ReturnJsonSerializerSettings(); //json返回格式化设置
        }

        public class SessionRouteHandler : HttpControllerHandler, IRequiresSessionState
        {
            public SessionRouteHandler(RouteData routeData)
                : base(routeData)
            {
            }
        }
        public class SessionControllerRouteHandler : HttpControllerRouteHandler
        {
            protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new SessionRouteHandler(requestContext.RouteData);
            }
        }
        /// <summary>
        /// json返回格式化设置
        /// </summary>
        private static void ReturnJsonSerializerSettings()
        {
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用，如果设置为Error，则遇到循环引用的时候报错（建议设置为Error，这样更规范）
            json.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";//日期格式化
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//json中属性开头字母小写的驼峰命名
            json.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        }
    }
}