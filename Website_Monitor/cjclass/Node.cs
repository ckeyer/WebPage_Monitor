using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website_Monitor.cjclass
{
    public class Node
    {
        public string name;
        public List<KeyValuePair<string, string>> Attribute= new List<KeyValuePair<string,string>>();
        public List<Node> Child= new List<Node>();
        public Node Parent;
        public string TextContent;

        public void AddChild(Node child)
        {
            Child.Add(child);
            child.Parent = this;
        }
        public void AddAttribute(string key,string value)
        {
            Attribute.Add(new KeyValuePair<string, string>(key, value));
        }
        public void AddText(string text)
        {
            Node textTmp = new Node();
            textTmp.name = "text";
            textTmp.TextContent = text;
            this.AddChild(textTmp);
        }
        public void AddNote(string text)
        {
            Node textTmp = new Node();
            textTmp.name = "note";
            textTmp.TextContent = text;
            this.AddChild(textTmp);
        }
        public override string ToString()
        {
            if (name == "text")
            {
                return TextContent;
            }
            else if (name == "note")
            {
                return "<!--" + TextContent + "-->";
            }
            else 
            {
                string tmp = "<" + this.name;
                for (int i = 0; i < this.Attribute.Count; i++)
                {
                    tmp += " " + Attribute[i].Key + "=" + Attribute[i].Value;
                }
                if (this.Child.Count == 0)
                {
                    tmp += "/>";
                }
                else
                {
                    tmp += ">";
                    for (int i = 0; i < Child.Count; i++)
                    {
                        tmp+=Child[i].ToString();
                    }
                    tmp += "</" + this.name + ">";
                }
                return tmp;
            }
        }
    }
}
