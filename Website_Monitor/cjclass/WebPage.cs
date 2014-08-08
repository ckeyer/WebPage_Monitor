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
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;
    public class WebPage
    {
        public string Url;
        public KiVs Status;
        public Node RootNode;
        public string Html;
        public int PageDepth = 0;

        public WebPage(string url)
        {
            this.Url = url;
            this.getHtml();
            this.Analyze();
        }
        public WebPage(string url ,bool analyze)
        {
            this.Url = url;
            this.getHtml();
            if (analyze)
            {
                this.Analyze();
            }
        }
        public void Analyze()
        {
            if (Status.Key > 0)
            {
                this.AnalyzeHtml();
            }
        }
        public string testGetAtom()
        {
            HtmlGrammarOptions options = new HtmlGrammarOptions();
            options.HandleCharacterReferences = true;
            options.DecomposeCharacterReference = true;
            options.HandleUnfinishedTags = true;
            HtmlGrammar grammar = new HtmlGrammar(options);
            HtmlReader reader = new HtmlReader(Html, grammar);
            string str = "";
            reader.Builder.TokenChaning += delegate(TokenChangingArgs args)
            {
                if (args.HasBefore)
                {
                    str += args.Before.Id + "#" + args.Before.Value + "#\r\n";
                }
            };
            return str;
        }
        private void AnalyzeHtml()
        {
            // 解析Html是用到的栈
            Stack<string> outStack = new Stack<string>();
            Stack<string> inStack = new Stack<string>();
            int operate = 0;
            Node nowNode = RootNode;

            HtmlGrammarOptions options = new HtmlGrammarOptions();
            options.HandleCharacterReferences = true;
            options.DecomposeCharacterReference = true;
            options.HandleUnfinishedTags = true;
            HtmlGrammar grammar = new HtmlGrammar(options);
            HtmlReader reader = new HtmlReader(Html, grammar);
            Status = new KiVs(2, "AnalyzeHtmling...");
            reader.Builder.TokenChaning += delegate(TokenChangingArgs args)
            {
                if (args.HasBefore && Status.Key>0)
                {
                    KsVs roll = new KsVs(args.Before.Id ,args.Before.Value);
                    string tmpStr;
                    switch (roll.Key)
                    {
                        case "TAG_STARTS":
                            if (operate == 0)
                            {
                                operate = 1;
                            }
                            else
                            {
                                Status = new KiVs(-4, operate + "#" + roll.Key + "#" + roll.Value);
                            }
                            break;
                        case "TAG_ENDS":
                            if (operate == 4)
                            {
                                operate = 0;
                            }
                            else
                            {
                                tmpStr = outStack.Peek();
                                if (tmpStr == "meta" || tmpStr == "link" || tmpStr == "param")
                                {
                                    outStack.Pop();
                                    operate = 0;
                                    nowNode = nowNode.Parent;
                                }
                            }
                            operate = 0;
                            break;
                        case "NAME": // 添加节点到栈，新建ChildNode
                            if (RootNode == null)
                            {
                                RootNode = new Node();
                                nowNode = RootNode;
                                nowNode.name = roll.Value.ToLower();
                                outStack.Push(roll.Value.ToLower());
                            }
                            else 
                            {
                                if (operate == 4) // 节点结束
                                {
                                    if (roll.Value.ToLower() == "html")
                                    {
                                        if (outStack.Peek() == "html")
                                        {
                                            Status = new KiVs(0, "Success");
                                        }
                                        else
                                        {
                                            Status = new KiVs(-5, "Error htmlOver");
                                        }
                                    }
                                    else
                                    {
                                        if (outStack.Peek() == roll.Value.ToLower())
                                        {
                                            outStack.Pop();
                                            nowNode = nowNode.Parent;
                                        }
                                        else// if(false)
                                        {
                                            while (outStack.Count > 0)
                                            {
                                                if (outStack.Peek() == roll.Value.ToLower())
                                                {
                                                    outStack.Pop();
                                                    nowNode = nowNode.Parent;
                                                    break;
                                                }
                                                else 
                                                {
                                                    outStack.Pop();
                                                }
                                            }
                                            if (outStack.Count == 0)
                                            {
                                                Status = new KiVs(-6, "没有与 <" + roll.Value + "> 对应的标签");
                                            }
                                        }
                                    }
                                }
                                else // 节点创建
                                {
                                    outStack.Push(roll.Value.ToLower());
                                    nowNode.AddChild(roll.Value);
                                    nowNode = nowNode.Child[nowNode.Child.Count - 1];
                                }
                            }
                            operate = 1;
                            break;
                        case "ATTR": // 属性名
                            operate = 2;
                            inStack.Push(roll.Value);
                            break;
                        case "QUOTED_VALUE": // 属性内容
                            if (inStack.Count == 1)
                            {
                                string strtmp = roll.Value;
                                if (strtmp[0] == '"' && strtmp[strtmp.Length-1] == '"')
                                {
                                    strtmp = strtmp.Remove(strtmp.Length - 1);
                                    strtmp = strtmp.Remove(0,1);
                                }
                                nowNode.AddAttribute(inStack.Pop(), strtmp);
                            }
                            break;
                        case "TEXT":
                            if (roll.Value.Trim() != "")
                            {
                                nowNode.AddText(roll.Value);
                            }
                            break;
                        case "STYLE":
                            if (roll.Value.Trim() != "")
                            {
                                nowNode.AddText(roll.Value);
                            }
                            break;
                        case "SCRIPT":
                            if (roll.Value.Trim() != "")
                            {
                                nowNode.AddText(roll.Value);
                            }
                            break;
                        case "CLOSING":
                            if (nowNode.Depth > 0)
                            {
                            }
                            else
                            {
                                if (outStack.Count == 0)
                                {
                                    Status = new KiVs(0, "Over");
                                }
                                else
                                {
                                    //Status = new KiVs(-3, outStack.Pop()+nowNode.Depth);
                                }
                            }
                                operate = 4;
                            break;
                        case "ATOM":
                            operate = 4;
                            nowNode = nowNode.Parent;
                            outStack.Pop();
                            break;
                        case "CHAR_REF_STARTS": // &
                            nowNode.AddAttribute(nowNode.Attribute[nowNode.Attribute.Count - 1].Key, roll.Value);
                            break;
                        case "CHAR_REF_ENTITY": // &后面的属性
                            nowNode.AddAttribute(nowNode.Attribute[nowNode.Attribute.Count - 1].Key, roll.Value);
                            break;
                        case "CHAR_REF_ENDS": // &后面的属性
                            nowNode.AddAttribute(nowNode.Attribute[nowNode.Attribute.Count - 1].Key, roll.Value);
                            break;
                        case "COMMENT_STARTS":
                            break;
                        case "COMMENT_BODY":
                            nowNode.AddNote(roll.Value);
                            break;
                        case "COMMENT_ENDS":
                            break;
                        case "ASSIGN": // 赋值 =
                            break;
                        case "WHITESPACE":// 空格符
                            break;
                        default:
                            Status = new KiVs(-2, roll.Key+"#"+roll.Value);
                            break;
                    }
                }
            };
            HtmlReader.Read(reader, null);
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
            Status = new KeyValuePair<int, string>(1, "文件下载完毕");
            return true;
        }
        public string GetHtmlMD5()
        {
            if (Html != null)
                return CJMainServer.EncryptMd5(Html);
            else
            {
                return "";
            }
        }
        public override string ToString()
        {
            try
            {
                return this.RootNode.ToString();
            }
            catch (Exception)
            {
                if (Html != null)
                    return Html;
                else
                {
                    return "";
                }
            }
        }
        public List<string> getSrc()
        {
            return RootNode.getQuotedByAttr("src");
        }
        public List<string> getHref()
        {
            return RootNode.getQuotedByAttr("href");
        }
        public List<string> getTextContent()
        {
            return RootNode.getTextContent();
        }
        public List<string> getQuotedByNode_Attr(string nodeName, string attr)
        {
            return RootNode.getQuotedByNode_Attr(nodeName, attr);
        }
    }
}
