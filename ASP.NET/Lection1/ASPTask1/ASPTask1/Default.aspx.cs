using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPTask1
{
    public partial class Default : System.Web.UI.Page
    {
        protected override void Construct()
        {
            log("Construct");
            base.Construct();
        }

        public override void ProcessRequest(HttpContext context)
        {
            log("ProcessRequest");
            //Page.Response еще не доступен, но он доступен через свойство - context.Response
            base.ProcessRequest(context);
        }

        protected override void InitializeCulture()
        {
            log("InitializeCulture");
            base.InitializeCulture();
        }

        protected override NameValueCollection DeterminePostBackMode()
        {
            log("DeterminePostBackMode");
            return base.DeterminePostBackMode();
        }

        protected override void OnPreInit(EventArgs e)
        {
            log("OnPreInit");
            base.OnPreInit(e);
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            log("Page_PreInit");
        }

        protected override void OnInit(EventArgs e)
        {
            log("OnInit");
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            log("Page_Init");
        }

        protected override void TrackViewState()
        {
            log("TrackViewState");
            base.TrackViewState();
        }

        protected override void OnInitComplete(EventArgs e)
        {
            log("OnInitComplete");
            base.OnInitComplete(e);
        }

        protected void Page_InitComplete(object sender, EventArgs e)
        {
            log("Page_InitComplete");
        }

        protected override object LoadPageStateFromPersistenceMedium()
        {
            log("LoadPageStateFromPersistenceMedium");
            return base.LoadPageStateFromPersistenceMedium();
        }

        protected override void LoadViewState(object savedState)
        {
            log("LoadViewState");
            base.LoadViewState(savedState);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            log("OnPreLoad");
            base.OnPreLoad(e);
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            log("Page_PreLoad");
        }

        protected override void OnLoad(EventArgs e)
        {
            log("OnLoad");
            base.OnLoad(e);
            if(!IsPostBack)
                Button1_OnClick(null, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            log("Page_Load");
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            log("RaisePostBackEvent");
            base.RaisePostBackEvent(sourceControl, eventArgument);
        }

        public override void Validate()
        {
            log("Validate");
            base.Validate();
        }

        public override void Validate(string validationGroup)
        {
            log("Validate(string)");
            base.Validate(validationGroup);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            log("OnLoadComplete");
            base.OnLoadComplete(e);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            log("Page_LoadComplete");
        }

        protected override void OnPreRender(EventArgs e)
        {
            log("OnPreRender");
            base.OnPreRender(e);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            log("Page_PreRender");
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            log("OnPreRenderComplete");
            base.OnPreRenderComplete(e);
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            log("Page_PreRenderComplete");
        }

        protected override object SaveViewState()
        {
            log("SaveViewState");
            return base.SaveViewState();
        }

        protected override void SavePageStateToPersistenceMedium(object state)
        {
            log("SavePageStateToPersistenceMedium");
            base.SavePageStateToPersistenceMedium(state);
        }

        protected override void OnSaveStateComplete(EventArgs e)
        {
            log("OnSaveStateComplete");
            base.OnSaveStateComplete(e);
        }

        protected void Page_SaveStateComplete(object sender, EventArgs e)
        {
            log("Page_SaveStateComplete");
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            log("RenderControl");
            base.RenderControl(writer);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            log("Render");
            base.Render(writer);
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            log("RenderChildren");
            base.RenderChildren(writer);
        }

        protected override void OnUnload(EventArgs e)
        {
            log("OnUnload");
            base.OnUnload(e);
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            log("Page_Unload");
        }

        public override void Dispose()
        {
            log("Dispose");
            base.Dispose();
        }

        protected void Button1_OnClick(object sender, EventArgs e)
        {
            /*  Page.Request.Form содержит коллекцию переменных формы.
             *  При запуске приложения там пустая коллекция. Но при отправке
             *  postBack запроса в эту коллекцию помещаются все элементы формы.
             *  Помимо обычных элементов формы(в данном случае Button1) эта коллекция
             *  еще содержит два ключа: __VIEWSTATE И __EVENTVALIDATION.
             *  __VIEWSTATE содержит десериализованные данные, в которых хранится состояние
             *  страницы во время последней обработки сервером.
             * __EVENTVALIDATION обеспечивает безопасность, предотвращает возможность отправки потенциально 
             * злонамеренных несанкционированных запросов, с клиентской стороны. На странице, производится
             * сравнение содержания запроса с информацией поля __EVENTVALIDATION, на предмет отсутствия 
             * дополнительных полей, добавленных на стороне клиента.
             */

            foreach (string i in Page.Request.Form.Keys)
            {
                addCells(i, Page.Request.Form[i]);
            }
        }

        private void addCells(string v1, string v2)
        {
            var row = new TableRow();
            row.Cells.AddRange(new[] { new TableCell() { Text = v1 }, new TableCell() { Text = v2 } });
            Table1.Rows.Add(row);
        }

        private void log(string message)
        {
            try
            {
                Page.Response.Write(string.Format("{0} {1}<br/>", message, DateTime.Now.ToString("G")));
            }
            catch (HttpException)
            {
                /* Обьект Response уже перестал быть доступен (ответ уже был отправлен пользователю)
                 * или еще не доступен. 
                 * Это происходит в методах\событиях: 
                 * ProcessRequest
                 * OnUnload
                 * Page_OnUnload
                 * Dispose
                 */
            }
            catch (NullReferenceException)
            {
                /* Обьект Response еще не был создан в методе:
                 * Construct
                 */
            }
        }
    }
}