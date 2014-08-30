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
using System.IO;

namespace Website_Monitor
{
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string testHtml = "<html>	<head>		<meta lang=\"zh_cn\">		<link type=\"style/css\" src=\"www.bootstreat.com/hero.css\"/>	</head>	<body>		<div>			<p>click<a>hero</a>...</p>			<p id=\"conten\">Fuck</p>		</div>		<!--		<<<<<<<			this is some description			>		-->		<div>			<a>hero</a>			<p id=\"conten\">Fuck</p>			<img src=\"hero.jpg\" name=\"heroPic\" id=\"imgId\"/>		</div>	</body></html>";
        public  WebBrowser webBrowser1 = new WebBrowser();
        public string html;
        private string popNextString(int n)
        {
            if (n > testHtml.Length)
            {
                return null;
            }
            string tmp = testHtml.Substring(0, n);
            testHtml = testHtml.Remove(0, n);
            return tmp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //txtHtmlWhole.Text = popNextString(testHtml.IndexOf('\"'));
            //WebClient client = new WebClient();
            //html = Encoding.UTF8.GetString(client.DownloadData("file://G:/test1.html"));
            
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://fc.wut.edu.cn:8086/");
            //html = html.Substring(html.IndexOf("<html"));
            //List<string> styels = new List<string>();
            //bool stylestaus = true;
            //HtmlGrammarOptions options = new HtmlGrammarOptions();
            //options.HandleCharacterReferences = true;
            //options.DecomposeCharacterReference = true;
            //options.HandleUnfinishedTags = true;
            //HtmlGrammar grammar = new HtmlGrammar(options);
            //HtmlReader reader = new HtmlReader(testHtml, grammar);
            //while (reader.Enumerator.IsDisposed) {
            //    txtHtmlWhole.Text += reader.Enumerator.MoveNext().ToString();
            //}
            //reader.Builder.TokenChaning += delegate(TokenChangingArgs args)
            //{
            //    if (args.HasBefore)
            //    {
            //        txtHtmlWhole.Text += (args.Before.Id + "\t#" + args.Before.Value + "#\r\n");
            //    }
            //};
            //HtmlReader.Read(reader, null);
            //for (int i = 0; i < styels.Count; i++)
            //{
            //    txtHtmlWhole.Text +="> "+i+" --- "+styels[i]+"\r\n > \r\n";
            //}
            //txtHtmlWhole.Text += reader.Status.ToString();
            //AfterWork.Html.Link link = new AfterWork.Html.Link(state, category);
            //txtHtmlWhole.Text +=  reader.State.Name;
            
            //webBrowser1.DocumentCompleted += 
            //    new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted); 

        }
        private void button2_Click(object sender, EventArgs e)
        {
            string url = "file://G:/test1.html";
            //url = "http://hbagri.gov.cn/";
            cjclass.WebPage page = new cjclass.WebPage(url);
            txtHtmlWhole.Text = page.RootNode.ToString();

            //txtHtmlWhole.Text += page.Status.Value;
            //txtHtmlWhole.Text += page.testGetAtom();
            //txtHtmlWhole.Text = page.Html;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //WebClient client = new WebClient();
            //html = Encoding.UTF8.GetString(client.DownloadData("file://G:/test1.html"));
            //html = html.Substring(html.IndexOf("<html"));
            //cjclass.HtmlResolve reader = new cjclass.HtmlResolve(html);
            char[] n = { ' ','\t'};
            string str = "\t as ";
            if (str.Trim() == "as")
            {
                MessageBox.Show("hhh");
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
