using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;

using AfterWork.Html.Testing;
using AfterWork.Html;
using System.IO;

namespace Website_Monitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public  WebBrowser webBrowser1 = new WebBrowser();
        public string html;

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            html = Encoding.UTF8.GetString(client.DownloadData("http://fc.wut.edu.cn:8086/"));
            
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://fc.wut.edu.cn:8086/");
            html = html.Substring(html.IndexOf("<html"));
           
            HtmlGrammarOptions options = new HtmlGrammarOptions();
            options.HandleCharacterReferences = true;
            options.DecomposeCharacterReference = true;
            options.HandleUnfinishedTags = true;
            HtmlGrammar grammar = new HtmlGrammar(options);
            HtmlReader reader = new HtmlReader(html, grammar);
            while (reader.Enumerator.IsDisposed) {
                txtHtmlWhole.Text += reader.Enumerator.MoveNext().ToString();
            }
            reader.Builder.TokenChaning += delegate(TokenChangingArgs args)
            {
                if (args.HasBefore)
                {
                    txtHtmlWhole.Text += (args.Before.Id + " ## " + args.Before.Value + "\r\n");
                }
            };
            HtmlReader.Read(reader, null);
            //txtHtmlWhole.Text += reader.Status.ToString();
            //AfterWork.Html.Link link = new AfterWork.Html.Link(state, category);
            //txtHtmlWhole.Text +=  reader.State.Name;
            
            //webBrowser1.DocumentCompleted += 
            //    new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted); 
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
                if ((webBrowser1.StatusText == "完成"))
                {
                    txtHtmlWhole.Text += webBrowser1.StatusText + "\r\n@@";
                    txtHtmlWhole.Text += webBrowser1.Document.Body.InnerHtml;
                }
        }
        private void browser_DocumentCompleted(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            txtHtmlWhole.Text += e.Url.ToString();
        }

        private void Window_Error(object sender,
            HtmlElementErrorEventArgs e)
        {
            // 忽略该错误并抑制错误对话框    
            e.Handled = true;
        }
    }
}
