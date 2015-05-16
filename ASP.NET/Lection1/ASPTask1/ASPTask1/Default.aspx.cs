using System;
using System.Web;
using System.Web.UI.WebControls;

namespace ASPTask1
{
    public partial class Default : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            log("OnInit");
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            log("OnLoad");
            base.OnLoad(e);
        }

        protected override void OnInitComplete(EventArgs e)
        {
            log("OnInitComplete");
            base.OnInitComplete(e);
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            log("Page_PreInit");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            log("Page_Init");
        }

        protected void Page_InitComplete(object sender, EventArgs e)
        {
            log("Page_InitComplete");
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            log("Page_PreLoad");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            log("Page_Load");
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            log("Page_LoadComplete");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            log("Page_PreRender");
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            log("Page_PreRenderComplete");
        }

        protected void Page_SaveStateComplete(object sender, EventArgs e)
        {
            log("Page_SaveStateComplete");
        }
        protected void Page_Disposed(object sender, EventArgs e)
        {
            log("Page_SaveStateComplete");
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                log("Page_Unload");
            }
            catch (HttpException)
            {
                /* Обьект Response уже перестал быть доступен, так как ответ отправился пользователю.
                 * Поэтому невозможно залогировать вызов этого метода.
                 * Обрабатывать это событие необходимо для освобождения ресурсов.
                 * Аналогично для события Page_Disposed не будет доступен обьект Response, так как это
                 * событие возникает когда сборщик мусора убирает эту страницу, а в этот момент уже точно
                 * ответ отправлен пользователю.
                 */
            }
        }

        protected void Button1_OnClick(object sender, EventArgs e)
        {
            /*  Page.Request.Form содержит коллекцию переменных формы, обратно
             *  отправляемых странице.
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
            Page.Response.Write(string.Format("{0} {1}<br/>", message, DateTime.Now.ToString("G")));
        }
    }
}