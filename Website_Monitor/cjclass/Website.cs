using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Website_Monitor.cjclass
{
    public class Website
    {
        
        public List<WebPage> pages = new List<WebPage>();
        public int pageCoout { get { return pages.Count; } }
        
    }
}
