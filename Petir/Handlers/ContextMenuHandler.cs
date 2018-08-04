﻿
using CefSharp;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System;

namespace Petir
{
    internal class ContextMenuHandler : IContextMenuHandler {

		private const int ShowDevTools = 26501;
		private const int CloseDevTools = 26502;
		private const int SaveImageAs = 26503;
		private const int SaveAsPdf = 26504;
		private const int SaveLinkAs = 26505;
		private const int CopyLinkAddress = 26506;
		private const int OpenLinkInNewTab = 26507;

        private const int GoogleDork = 26508;
        private const int DorkInTitle = 26509;
        private const int DorkInUrl = 26510;
        private const int DorkInText = 26511;
        private const int DorkSite = 26512;
        private const int DorkLink = 26513;
        private const int DorkFileType = 26514;
        private const int DorkExt = 26515;
        private const int Search = 26516;

        private const int Automation = 26550;

        private const int auto = 26517;

        private const int CloseTab = 40007;
		private const int RefreshTab = 40008;
        MainForm myForm;
        
        private IBrowser bro;

        public ContextMenuHandler(MainForm form) {
			myForm = form;
		}

		public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			
			// clear the menu
			model.Clear();
            bro = browser;
            //enable expath locator mouse
            var selected = browserControl.EvaluateScriptAsync("document.activeElement.localName").ToString();
            //chek if form is exist
            //var formExists = browserControl.EvaluateScriptAsync("$('form').length");
            //if form exist enable autofill form
            //if(formExists != null)
            //{
                //enable autofill form
                //auto will be used in xcode or after request loaded
                // auto fill can be used for autobot, or like save password featured in google chrome
                //browserControl.ExecuteScriptAsync(File.ReadAllText(Js.autofill));
            //}
                    model.AddItem((CefMenuCommand)RefreshTab, "Refresh");
            // to copy text
            if (parameters.SelectionText.CheckIfValid()) {
                    model.AddItem(CefMenuCommand.Copy, "Copy");
                    model.AddItem((CefMenuCommand)Search, "Search");
			}
            
			//Removing existing menu item
			//bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option
			if (parameters.LinkUrl != "") {
				    model.AddItem((CefMenuCommand)OpenLinkInNewTab, "Open link in new tab");
				    model.AddItem((CefMenuCommand)CopyLinkAddress, "Copy link");
                    model.AddItem((CefMenuCommand)SaveLinkAs, "Save page");
			}

			if (parameters.HasImageContents && parameters.SourceUrl.CheckIfValid()) {
                // RIGHT CLICKED ON IMAGE
                    model.AddItem((CefMenuCommand)SaveImageAs, "Save Image");
            }

			if (parameters.SelectionText != null) {

                // TEXT IS SELECTED
            }
            if(selected != null)
            {
                model.AddSeparator();
                var x = model.AddSubMenu((CefMenuCommand)GoogleDork, "GoogleDork");
                x.AddItem((CefMenuCommand)DorkInTitle, "InTitle");
                x.AddItem((CefMenuCommand)DorkInUrl, "InUrl");
                x.AddItem((CefMenuCommand)DorkInText, "InText");
                x.AddItem((CefMenuCommand)DorkSite, "Site");
                x.AddItem((CefMenuCommand)DorkLink, "Link");
                x.AddItem((CefMenuCommand)DorkFileType, "FileType");
                x.AddItem((CefMenuCommand)DorkExt, "Ext");
            }
            
            model.AddSeparator();
            model.AddItem((CefMenuCommand)ShowDevTools, "Developer tools");
			model.AddItem(CefMenuCommand.ViewSource, "View source");
            model.AddItem((CefMenuCommand)CloseTab, "Close tab");

        }

		public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {

			int id = (int)commandId;
            if (id == auto)
            {
                frame.ExecuteJavaScriptAsync(File.ReadAllText(Js.autofill));
            }
            if (id == Search)
            {
                newtab(parameters.SelectionText);
            }
            if (id == DorkInTitle)
            {
                newtab("intitle:" + parameters.SelectionText);
            }
            if (id == DorkInUrl)
            {
                newtab("inurl:" + parameters.SelectionText);
            }
            if (id == DorkInText)
            {
                newtab("intext:" + parameters.SelectionText);
            }
            if (id == DorkSite)
            {
                newtab("site:" + parameters.SelectionText);
            }
            if (id == DorkLink)
            {
                newtab("link:" + parameters.SelectionText);
            }
            if (id == DorkFileType)
            {
                newtab("filetype:" + parameters.SelectionText);
            }
            if (id == DorkExt)
            {
                newtab("ext:" + parameters.SelectionText);
            }
            if (id == ShowDevTools) {
				browser.ShowDevTools();
			}
			if (id == CloseDevTools) {
				browser.CloseDevTools();
			}
			if (id == SaveImageAs) {
				browser.GetHost().StartDownload(parameters.SourceUrl);
			}
			if (id == SaveLinkAs) {
				browser.GetHost().StartDownload(parameters.LinkUrl);
			}
			if (id == OpenLinkInNewTab) {
                string u = parameters.LinkUrl;
                myForm.InvokeOnParent(delegate () { myForm.AddNewBrowser(u); });
            }
			if (id == CopyLinkAddress) {
				Clipboard.SetText(parameters.LinkUrl);
			}
            if (id == CloseTab) {
                //myForm.X.InvokeOnParent(delegate() { myForm.X.Dispose(); });
                myForm.X.InvokeOnParent(delegate () { myForm.Tes(); });
            }
            if (id == RefreshTab) {
                browser.Reload();
            }
			return false;
		}

        private void OpenLink(string linkUrl)
        {
            myForm.AddNewBrowser(linkUrl);
        }

        private void newtab(string url)
        {
            var newurl = "https://www.google.com/#q=" + HttpUtility.UrlEncode(url);
            myForm.InvokeOnParent(delegate () { myForm.AddNewBrowser(newurl); });
            //myForm.AddNewBrowser(newurl);
        }

		public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) {

		}

		public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) {

			// show default menu
			return false;
		}
        
	}
}
