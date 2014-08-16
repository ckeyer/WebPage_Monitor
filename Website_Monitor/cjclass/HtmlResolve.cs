using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website_Monitor.cjclass
{
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;
    // html 文件的分解
    class HtmlResolve
    {           
        private string Html;
        private int position = 0;
        private KsVs lastStatus,nowStatus;
        public HtmlResolve(string html)
        {
            this.Html = html;
        }
        public KsVs PopNextRoll()
        {
            KsVs next = new KsVs("OVER",null);
            char firstChar = Html.First();
            switch (firstChar)
            {
                case '<':
                    next = new KsVs("TAG_STARTS", popNextString(1));
                    this.position = 1;  
                    break;
                case '>':
                    next = new KsVs("TAG_ENDS", popNextString(1));
                    this.position = 4;  
                    break;
                case '=':
                    next = new KsVs("ASSIGN", popNextString(1));
                    this.position = 5;
                    break;
                case ' ':
                    next = new KsVs("WHITESPACE", popNextString(1));
                    break;
                case '/':
                    this.position = 3;
                    if (lastStatus.Key == "TAG_STARTS")
                    {
                        next = new KsVs("CLOSING", popNextString(1));
                    }
                    else
                    {
                        next = new KsVs("ATOM", popNextString(1));
                    }
                    break;
                default:
                    next = getNextText();
                    break;
            }
            this.lastStatus = next;
            return next;
        }
        private KsVs getNextText()
        {
            int minIndex;
            KsVs next = new KsVs("OVER", null);
            switch (this.lastStatus.Key)
            {
                case "TAG_STARTS":
                    minIndex = Html.IndexOf('/');
                    if (minIndex > Html.IndexOf(' '))
                    {
                        minIndex = Html.IndexOf(' ');
                    }
                    if (minIndex > Html.IndexOf('>'))
                    {
                        minIndex = Html.IndexOf('>');
                    }
                    this.position = 2;
                    next = new KsVs("NAME", popNextString(minIndex).Trim().ToLower());
                    break;
                case "CLOSING":
                    minIndex = Html.IndexOf('>');
                    this.position = 3;
                    next = new KsVs("NAME", popNextString(minIndex).Trim().ToLower());
                    break;
                case "ASSIGN":
                    popNextString(Html.IndexOf('\"')+1);
                    minIndex = Html.IndexOf('\"');
                    this.position = 2;
                    next = new KsVs("QUOTED_VALUE", popNextString(minIndex).Trim());
                    popNextString(1);
                    break;
                case "TAG_ENDS":
                    minIndex = Html.IndexOf('<');
                    this.position = 4;
                    string tmpStr = popNextString(minIndex).Trim();
                    if (tmpStr == "")
                    {
                        next = new KsVs("WHITESPACE", " ");
                    }
                    else
                    {
                        next = new KsVs("TEXT", tmpStr);
                    }
                    break;
                default:
                    switch (this.position)
                    {
                        case 2:
                            minIndex = Html.IndexOf('=');
                            this.position = 2;
                            next = new KsVs("ATTR", popNextString(minIndex).Trim().ToLower());
                            break;
                    }
                    break;
            }
            return next;
        }
        private string popNextString(int n)
        {
            if (n > Html.Length)
            {
                return null;
            }
            string tmp =Html.Substring(0, n);
            Html = Html.Remove(0, n);
            return tmp;
        }
    }
}
