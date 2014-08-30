using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Website_Monitor.cjclass
{
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;

    // html 文件的分解
    class HtmlResolve
    {
        public Node RootNode;
        public bool isAnalyzeOver = false; // 是否在单行类结束了该标签
        
        private string Html, lastAttr, textTmp="";
        private StringReader htmlReader;
        private KiVs lastCell;
        private int position = 0;
        private string line;
        private int lineNum=0;
        private Node nowNode;
        private KiVs err = new KiVs(0, null);
        private Stack<string> tagStack ;
        private StringBuilder strTmp;
        private bool inOneLine ;
        private KsVs ssText;
        private string[] tagsNoChild
            = { "script", "style", "meta", "title", "br", "iframe","img" };
    
        public HtmlResolve(string html)
        {
            this.Html = html;
            htmlReader = new StringReader(html);
            tagStack = new Stack<string>();
            strTmp = new StringBuilder();
        }
        public void Analyze()
        {
            while (err.Key>=0) // 按行读取
            {
                nextLine();
                while (line.Length > 0)
                {
                    if (position < 20)
                    {
                        string cellTmp = peekLineStr(1);
                        switch (cellTmp)
                        {
                            case "<":
                                position = 1;
                                lastCell = new KiVs(1, dequeueLineStr(1));
                                break;
                            case ">":
                                lastCell = new KiVs(2, dequeueLineStr(1));
                                position = 2;
                                break;
                            case "=":
                                lastCell = new KiVs(5, dequeueLineStr(1));
                                position = 5;
                                break;
                            case "/":
                                if (peekLineStr(2) != "//")
                                {
                                    if (lastCell.Key == 1)
                                    {
                                        lastCell = new KiVs(3, dequeueLineStr(1));
                                    }
                                    else
                                    {
                                        lastCell = new KiVs(4, dequeueLineStr(1));
                                        nowNode = nowNode.Parent;
                                        tagStack.Pop();
                                    }
                                }
                                else
                                {
                                    getString();
                                }
                                break;
                            default:
                                getString();
                                break;
                        }
                    }
                    else 
                    {
                        getString();
                    }
                }

            }
            this.isAnalyzeOver = true;
        }
        private void getString()
        {
            switch (lastCell.Key)
            {
                case 1:
                    if (line.IndexOf("!--") != 0)
                    {
                        getName(); // 获取tag名称，项nowNode中添加该子节点并指到该节点
                    }
                    else
                    {
                        getAnnotation(); // html注释部分提取
                    }
                    break;
                //case 2:
                //case 20:
                //case 22:
                //case 23:
                case 3:
                case 4:
                    getNameClosing(); // 节点结束，获取节点名称并是nowNode指到父节点
                    break;
                case 5:
                    getValue(); // 获取属性的值部分
                    break;
                case 10:
                    getAttr(); // 获取属性的名称部分
                    break;
                case 12:
                    getAttrLastNull(); // 上一个属性没有值
                    break;
                case 13:
                    getAttr(); // 获取属性的名称部分
                    break;
                case 21: // 多行的注释
                    getAnnotation();
                    break;
                default:
                    getText(); // 节点间内容
                    break;
            }
        }

        private void getName()
        {
            int minIndex = 10000;
            if (minIndex > line.IndexOf('/') && line.IndexOf('/') > 0)
            {
                minIndex = line.IndexOf('/');
            }
            if (minIndex > line.IndexOf(' ') && line.IndexOf(' ') > 0)
            {
                minIndex = line.IndexOf(' ');
            }
            if (minIndex > line.IndexOf('>') && line.IndexOf('>') > 0)
            {
                minIndex = line.IndexOf('>');
            }
            string tagname = dequeueLineStr(minIndex).ToLower();
            lastCell = new KiVs(10, tagname);
            if (lastCell.Key != 4)
            {
                if (RootNode == null)
                {
                    RootNode = new Node(tagname);
                    nowNode = RootNode;
                }
                else
                {
                    foreach (string tag in tagsNoChild)
                    {
                        if (tag == tagStack.Peek())
                        {
                            nowNode = nowNode.Parent;
                            tagStack.Pop();
                        }
                    }
                    nowNode.AddChild(tagname);
                    nowNode = nowNode.Child[nowNode.Child.Count - 1];
                }
                tagStack.Push(tagname);
            }
            else
            {
                getNameClosing(tagname);
            }
        }
        private void getNameClosing()
        {
            int minIndex = line.IndexOf('>');
            string tagname = dequeueLineStr(minIndex).ToLower();
           
            if (tagname == tagStack.Peek())
            {
                nowNode = nowNode.Parent;
                tagStack.Pop();
            }
            dequeueLineStr(1);
            lastCell = new KiVs(2, ">");

        }
        private void getNameClosing(string tagname)
        {
            if (tagname == tagStack.Peek())
            {
                nowNode = nowNode.Parent;
                tagStack.Pop();
            }
            else
            {
                
                while (true)
                {
                    if (tagStack.Pop() == tagname)
                    {
                        return ;
                    }
                }
            }
        }
        private void getText()
        {
            if (tagStack.Peek() == "script")
            {
                getTextScript();
            }
            if (tagStack.Peek() == "style")
            {
                getTextStyle();
            }
            else
            {
                int n = line.IndexOf('<');
                if (n < 0)
                {
                    textTmp += "\r\n" + line;
                    line = "";
                    position = 20;
                }
                else
                {
                    textTmp += "\r\n" + dequeueLineStr(n);
                    nowNode.AddText(textTmp);
                    textTmp = "";
                    position = 14;
                }
            }
        }
        private void getTextScript()
        {
            int n = line.IndexOf("</script>");
            int zhushi = line.IndexOf("//");
            if (n < 0 || (zhushi>=0 && zhushi <n))
            {
                textTmp += "\r\n"+ line;
                line = "";
                position = 22;
            }
            else
            {
                textTmp += "\r\n" + dequeueLineStr(n);
                nowNode.AddText(textTmp);
                textTmp = "";
                position = 16;
            }
        }
        private void getTextStyle()
        {
            int n = line.ToLower().IndexOf("</style>");
            int zhushi = line.IndexOf("//");
            if (n < 0 || (zhushi >= 0 && zhushi < n))
            {
                textTmp += "\r\n" + line;
                line = "";
                position = 23;
            }
            else
            {
                textTmp += "\r\n" + dequeueLineStr(n);
                nowNode.AddText(textTmp);
                textTmp = "";
                position = 17;
            };
        }
        private void getValue()
        {
            string value = "";
            if (peekLineStr(1) == "\"")
            {
                dequeueLineStr(1);
                value = dequeueLineStr(line.IndexOf("\""));
                dequeueLineStr(1);
            }
            else
            {
                int minIndex = 10000;
                if (minIndex > line.IndexOf("/>") && line.IndexOf("/>") > 0)
                {
                    minIndex = line.IndexOf("/>");
                }
                if (minIndex > line.IndexOf(' ') && line.IndexOf(' ') > 0)
                {
                    minIndex = line.IndexOf(' ');
                }
                if (minIndex > line.IndexOf('>') && line.IndexOf('>') > 0)
                {
                    minIndex = line.IndexOf('>');
                }
                value = dequeueLineStr(minIndex);
            }
            nowNode.AddAttribute(lastAttr, value);
            lastCell = new KiVs(13, value);
        }
        private void getAttr()
        {
            int minIndex = 10000;
            if (minIndex > line.IndexOf('/') && line.IndexOf('/') > 0)
            {
                minIndex = line.IndexOf('/');
            }
            if (minIndex > line.IndexOf(' ') && line.IndexOf(' ') > 0)
            {
                minIndex = line.IndexOf(' ');
            }
            if (minIndex > line.IndexOf('>') && line.IndexOf('>') >0)
            {
                minIndex = line.IndexOf('>');
            }
            if (minIndex > line.IndexOf('=') && line.IndexOf('=') > 0)
            {
                minIndex = line.IndexOf('=');
            }
            lastAttr = dequeueLineStr(minIndex);
            lastCell = new KiVs(12, lastAttr);
        }
        private void getAttrLastNull() // 上一个属性没有赋值项 
        {
            nowNode.AddAttribute(lastAttr, lastAttr);
            getAttr();
        }
        private void getAnnotation()
        {
            int n = line.IndexOf("-->");
            if (n < 0)
            {
                line = "";
                lastCell = new KiVs(21,"");
                position = 21;
            }
            else
            {
                dequeueLineStr(n + 3);
                lastCell = new KiVs(2, ">");
                position = 2;
            }
        }

        // line辅助方法
        private void nextLine()
        {
            try
            {
                line = htmlReader.ReadLine().Trim();
                lineNum++;
            }
            catch (Exception)
            {
                err = new KiVs(-1, "over");
            }

            // for TEST ************************
            if (lineNum >= 2010)
            {
                return;
            }
            // TEST over ************************
        }
        private string peekLineStr(int n)
        {
            if (n <= line.Length)
            {
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < n; i++)
                {
                    tmp.Append(line[i].ToString());
                }
                return tmp.ToString();
            }
            return "";
        }
        private string peekLineStr(int startIndex,int count)
        {
            if (count+startIndex <= line.Length)
            {
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    tmp.Append(line[startIndex+i].ToString());
                }
                return tmp.ToString();
            }
            return "";
        }
        private string dequeueLineStr(int n)
        {
            if (n <= line.Length)
            {
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < n; i++)
                {
                    tmp.Append(line[i].ToString());
                }
                line = line.Remove(0, n);
                line = line.Trim();
                return tmp.ToString();
            }
            return "";
        }
        
    }

}
