using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotJira
{
    public static class UrlExtensions
    {
        private const string _URLExt_API = "{0}/rest/api/latest/";
        private const string _URLExt_Issue = "{0}";
        private const string _URLExt_ConCat = "{0}{1}/";
        private const string _URLExt_Comment = "{0}/comment";
        private const string _URLExt_Watcher = "{0}/watchers";
        private const string _URLExt_Transitions = "{0}/transitions?expand=transitions.fields";
        private const string _URLExt_Search = "?jql={0}";
        public static string Issue(string value)
        {
            return String.Format(_URLExt_Issue, value);
        }
        public static string Comment(string value)
        {
            return String.Format(_URLExt_Comment, value);
        }
        public static string Transitions(string value)
        {
            return String.Format(_URLExt_Transitions, value);
        }
        public static string Search(string value)
        {
            return String.Format(_URLExt_Search, value);
        }
        public static string Watcher(string value)
        {
            return String.Format(_URLExt_Watcher, value);
        }
        public static string API(string value)
        {
            return String.Format(_URLExt_API, value);
        }
        public static string ConCat(string value1, string value2)
        {
            string value = String.Empty;
            if (value1.Contains("?") || value2.Contains("?"))
            {
                value = String.Format("{0}{1}", value1, value2);
            }
            else
            {
                value = String.Format(_URLExt_ConCat, value1, value2);
            }
            return value;
        }
    }
}
