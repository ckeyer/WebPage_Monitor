using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Net;

using AfterWork.Html;

namespace Website_Monitor.cjclass
{
    public class WebPage
    {
        public string Url;
        public KeyValuePair<int,string> Status;
        public Node RootNode;
        public string Html;

        private Stack<int> nodeState = new Stack<int>();

        public WebPage(string url)
        {
            this.Url = url;
            this.getHtml();
        }
        public void init()
        {
            this.RootNode = new Node();
            Node nowNode = RootNode;
            if (getHtml())
            {
                HtmlGrammarOptions options = new HtmlGrammarOptions();
                options.HandleCharacterReferences = true;
                options.DecomposeCharacterReference = true;
                options.HandleUnfinishedTags = true;
                HtmlGrammar grammar = new HtmlGrammar(options);
                HtmlReader reader = new HtmlReader(Html, grammar);
                reader.Builder.TokenChaning += delegate(TokenChangingArgs args)
                {
                    if (args.HasBefore)
                    {
                        KeyValuePair<string ,string > roll = 
                            new KeyValuePair<string,string>(args.Before.Id ,args.Before.Value);

                    }
                };
                HtmlReader.Read(reader, null);
            }
        }
        private bool getHtml()
        {
            string htmlTmp="";
            try
            {
                WebClient client = new WebClient();
                htmlTmp = Encoding.UTF8.GetString(client.DownloadData(Url));
            }
            catch (Exception ex)
            {
                Status = new KeyValuePair<int, string>(-1, ex.Message);
                return false ;
            }
            if (htmlTmp.IndexOf("<html") < 0)
            {
                Status = new KeyValuePair<int, string>(-1, "没有找到html标签");
                return false;
            }
            Html = htmlTmp.Substring(htmlTmp.IndexOf("<html"));
            Status = new KeyValuePair<int, string>(0, "文件下载完毕");
            return true;
        }
    }
}
