using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website_Monitor.cjclass
{
    public class Node
    {
        public string name;
        public int Depth = 0;
        public List<KeyValuePair<string, string>> Attribute= new List<KeyValuePair<string,string>>();
        public List<Node> Child= new List<Node>();
        public Node Parent;
        public int NodesCount {
            get 
            {
                return this.getNodeCount();
            } 
        }
        public string TextContent;
        public Node()
        {
            ;
        }
        public Node(string name)
        {
            this.name = name;
        }

        public void AddChild(Node child)
        {
            child.Depth = this.Depth + 1;
            child.Parent = this;
            Child.Add(child);
        }
        public void AddChild(string childName)
        {
            Node child = new Node(childName);
            child.Depth = this.Depth + 1;
            child.Parent = this;
            Child.Add(child);
        }
        public void AddAttribute(string key,string value)
        {
            int i = 0;
            for (; i < this.Attribute.Count; i++)
            {
                if (Attribute[i].Key == key)
                {
                    break;
                }
            }
            if (i < Attribute.Count)
            {
                value = Attribute[i].Value + value;
                Attribute.RemoveAt(i);
            }
            Attribute.Add(new KeyValuePair<string, string>(key, value));
        }
        public void AddText(string text)
        {
            Node Tmp = new Node();
            Tmp.name = "text";
            Tmp.TextContent = text;
            this.AddChild(Tmp);
        }
        public void AddNote(string text)
        {
            Node Tmp = new Node();
            Tmp.name = "note";
            Tmp.TextContent = text;
            this.AddChild(Tmp);
        }
        public override string ToString()
        {
            if (name == "text")
            {
                return TextContent + "\r\n";
            }
            else if (name == "note")
            {
                return "<!--" + TextContent + "-->\r\n";
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
                    tmp += "/>\r\n";
                }
                else
                {
                    tmp += ">\r\n";
                    for (int i = 0; i < Child.Count; i++)
                    {
                        tmp+=Child[i].ToString();
                    }
                    tmp += "</" + this.name + ">\r\n";
                }
                return tmp;
            }
        }
        public int getNodeCount()
        {
            int count = 1;
            foreach (Node node in this.Child)
            {
                count += node.NodesCount;
            }
            return count;
        }
        public string getNodeMd5()
        {
            return CJMainServer.EncryptMd5(this.ToString());
        }
        public List<string> getQuotedByAttr(string attr)
        {
            List<string> quote = new List<string>();
            foreach (KeyValuePair<string,string> vk in Attribute)
            {
                if (vk.Key == attr)
                {
                    quote.Add(vk.Value);
                }
            }
            foreach (Node node in Child)
            {
                quote.AddRange(node.getQuotedByAttr(attr));
            }
            return quote;
        }
        public List<string> getQuotedByNode_Attr(string nodeName,string attr)
        {
            List<string> quote = new List<string>();
            if (this.name == nodeName)
            {
                foreach (KeyValuePair<string, string> vk in Attribute)
                {
                    if (vk.Key == attr)
                    {
                        quote.Add(vk.Value);
                    }
                }
            }
            foreach (Node node in Child)
            {
                quote.AddRange(node.getQuotedByNode_Attr(nodeName,attr));
            }
            return quote;
        }
        public List<string> getTextContent()
        {
            List<string> quote = new List<string>();
            if (this.name == "text")
            {
                quote.Add(this.TextContent);
            }
            foreach (Node node in Child)
            {
                quote.AddRange(node.getTextContent());
            }
            return quote;
        }
    }
}
