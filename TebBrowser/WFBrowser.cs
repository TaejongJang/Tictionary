using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TebBrowser {
    public class WFBrowser : WebBrowser {
        public bool Completed { get; private set; }

        public WFBrowser() {
            this.Completed = false;
            this.DocumentCompleted += WFBrowser_DocumentCompleted;
        }
        public WFBrowser(string Url) {
            this.Completed = false;
            this.DocumentCompleted += WFBrowser_DocumentCompleted;
            this.Navigate(Url);
        }
        public WFBrowser(Uri Uri) {
            this.Completed = false;
            this.DocumentCompleted += WFBrowser_DocumentCompleted;
            this.Navigate(Uri);
        }

        private void WFBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            if(e.Url.ToString().Equals(this.Url.AbsoluteUri.Replace("%20", " ")) || e.Url.ToString().Equals(this.Url.AbsoluteUri.Replace("%20", " ") + "/"))
            {
                this.Completed = true;
            }
        }

        public new void Navigate(string Url) {
            this.Completed = false;
            this.Navigate(new Uri(Url));
        }

        public HtmlAgilityPack.HtmlDocument GetDocByHAP() {
            HtmlAgilityPack.HtmlDocument returnDoc = new HtmlAgilityPack.HtmlDocument();
            returnDoc.Load(this.DocumentStream);
            return returnDoc;
        }
    }

    public static class SupportBrowser {
        public static HtmlElement FindChild(this WebBrowser WebBrowser, string Attribute, string Value) {
            foreach (HtmlElement Element in WebBrowser.Document.All)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                    return Element;
            }

            return null;
        }

        public static HtmlElement FindChild(this HtmlElement TargetElement, string Attribute, string Value) {
            foreach (HtmlElement Element in TargetElement.Children)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                    return Element;
                else if(Element.Children.Count > 0)
                {
                    HtmlElement returnElement = Element.FindChild(Attribute, Value);
                    if (returnElement != null)
                        return returnElement;
                }
            }

            return null;
        }

        public static IEnumerable<HtmlElement> FindChildren(this WebBrowser WebBrowser, string Attribute, string Value) {
            List<HtmlElement> returnList = new List<HtmlElement>();

            foreach (HtmlElement Element in WebBrowser.Document.All)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                    returnList.Add(Element);
            }

            return returnList;
        }

        public static IEnumerable<HtmlElement> FindChildren(this WebBrowser WebBrowser, string Attribute, string Value, int Limit) {
            List<HtmlElement> returnList = new List<HtmlElement>();

            foreach (HtmlElement Element in WebBrowser.Document.All)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                    returnList.Add(Element);

                if (returnList.Count == Limit)
                    break;
            }

            return returnList;
        }

        public static IEnumerable<HtmlElement> FindChildren(this HtmlElement TargetElement, string Attribute, string Value) {
            List<HtmlElement> returnList = new List<HtmlElement>();

            foreach (HtmlElement Element in TargetElement.Children)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                    returnList.Add(Element);
                else if (Element.Children.Count > 0)
                {
                    List<HtmlElement> returnElements = Element.FindChildren(Attribute, Value).ToList();
                    if (returnElements.Count > 0)
                        returnList.AddRange(returnElements);
                }
            }

            return returnList;
        }

        public static IEnumerable<HtmlElement> FindChildren(this HtmlElement TargetElement, string Attribute, string Value, int limit) {
            List<HtmlElement> returnList = new List<HtmlElement>();

            foreach (HtmlElement Element in TargetElement.Children)
            {
                if (Element.GetAttribute(Attribute).ToString().Equals(Value))
                {
                    returnList.Add(Element);
                    if (returnList.Count == limit)
                        return returnList;
                }
                else if (Element.Children.Count > 0)
                {
                    List<HtmlElement> returnElements = Element.FindChildren(Attribute, Value, limit - returnList.Count).ToList();
                    if (returnElements.Count > 0)
                        returnList.AddRange(returnElements);

                    if (returnList.Count == limit)
                        return returnList;
                }
            }

            return returnList;
        }
    }
}
