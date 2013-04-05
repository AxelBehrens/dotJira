using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace DotJira
{
    public class Jira
    {
        #region Private const members
        private static string BaseUrl;
        private static string U;
        private static string P;
        #endregion
        #region enums
        public enum Resource
        {
            PROJECT,
            SEARCH,
            ISSUE
        }
        public enum Method
        {
            GET,
            POST,
            PUT
        }
        public enum Resolution
        {
            FIXED = 1,
            WONTFIX,
            DUPLICATE,
            INCOMPLETE,
            CANTREPRODUCE
        }
        public enum IssueType
        {
            BUG = 1,
            FEATURE = 2,
            TASK = 3,
            IMPROVEMENT = 4,
            EPIC = 5,
            STORY = 6
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">login info - user name</param>
        /// <param name="pass">login info - password</param>
        public Jira(string baseurl, string user, string pass)
        {
            BaseUrl = UrlExtensions.API(baseurl);
            U = user;
            P = pass;
        }
        /// <summary>
        /// get a list of projects
        /// </summary>
        /// <returns>A list of projects</returns>
        public static List<Projects> GetProjects(out string statuscode)
        {
            string result = SendRequest(out statuscode, Resource.PROJECT);
            return JsonConvert.DeserializeObject<List<Projects>>(result);
        }
        /// <summary>
        /// Search for issues using jql
        /// </summary>
        /// <param name="jql">Jira Query string</param>
        /// <param name="fields">list of fields to return</param>
        /// <param name="startAt">starting index</param>
        /// <param name="maxResult">max issues to return</param>
        /// <returns>A list of Issues</returns>
        public List<Issue> SearchIssues(out string statuscode, string jql, List<string> fields = null, int start = 0, int max = 500)
        {
            SearchRequest request = new SearchRequest();
            request.JQL = jql;
            request.Fields = fields ?? new List<string> { "summary", "status", "assignee", "comment" };
            request.StartAt = start;
            request.MaxResults = max;
            string data = JsonConvert.SerializeObject(request);
            string result = SendRequest(out statuscode, Resource.SEARCH, data: data, method: Method.POST);
            SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(result);

            return response.Issues;
        }
        /// <summary>
        /// Create a new Issue
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="projectID">ID of the project</param>
        /// <param name="issueType">The type of issue to create (Bug, Task, etc...)</param>
        /// <param name="summary"> Summary of the issue</param>
        /// <param name="description">Description of the issue</param>
        /// <param name="assignee"> who get the ticket</param>
        /// <param name="reporter">who reported this issue</param>
        /// <returns></returns>
        public string CreateIssue(out string statuscode, string projectID, IssueType issueType, string summary, string description, string assignee, string reporter)
        {
            string result = "";
            int type = (int)issueType;
            string data = "{\"fields\": {\"project\": {\"id\": \"" + projectID + "\"},\"issuetype\":{\"id\":\"" + type + "\"},\"summary\": \"" + summary + "\",\"description\": \"" + description + "\",\"assignee\": {\"name\": \"" + assignee + "\"},\"reporter\": {\"name\": \"" + reporter + "\"}}}";
            result = SendRequest(out statuscode, Resource.ISSUE, null, data, Method.POST);
            return result;
        }
        /// <summary>
        /// Add a comment to a jira ticket.
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="issueKey"> the ticket id</param>
        /// <param name="comment"> your comment</param>
        /// <returns></returns>
        public string AddComment(out string statuscode, string issueKey, string comment)
        {
            string result = String.Empty;
            string data = "{\"body\": \"" + comment + "\"}";
            result = SendRequest(out statuscode, Resource.ISSUE, UrlExtensions.Comment(issueKey), data, Method.POST);

            return result;
        }
        /// <summary>
        /// Add a watcher
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="issueKey"> the ticket id</param>
        /// <param name="watchername">User name of the watcher to be added</param>
        /// <returns></returns>
        public string AddWatcher(out string statuscode, string issueKey, string watchername)
        {
            string result = String.Empty;
            string data = "\"" + watchername + "\"";
            result = SendRequest(out statuscode, Resource.ISSUE, UrlExtensions.Watcher(issueKey), data, Method.POST);
            return result;
        }
        /// <summary>
        /// Transition an issue (open to resolved... etc)
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="issueKey"> the ticket id</param>
        /// <param name="transitionID">ID of the transition. use validTransitions to get valid IDs</param>
        /// <returns></returns>
        public string TransitionIssue(out string statuscode, string issueKey, int transitionID)
        {
            string result = String.Empty;

            string data = "{\"transition\":{\"id\": \"" + transitionID + "\"} }";
            result = SendRequest(out statuscode, Resource.ISSUE, UrlExtensions.Transitions(issueKey), data, Method.POST);
            return result;
        }
        /// <summary>
        /// Update a custom field
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="issueKey"> the ticket id</param>
        /// <param name="field"> the name of the </param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string UpdateCustomField(out string statuscode, string issueKey, string field, string value)
        {
            string results = String.Empty;
            string data = "{\"fields\": {\"" + field + "\": \"" + value + "\"}}";
            results = SendRequest(out statuscode, Resource.ISSUE, UrlExtensions.Issue(issueKey), data, Method.PUT);
            return results;
        }
        /// <summary>
        /// get a list of valid Transitions. 
        /// </summary>
        /// <param name="statuscode">Response code for the request</param>
        /// <param name="issueKey"> the ticket id</param>
        /// <returns>List of valid Transitions</returns>
        public List<Transitions> ValidTransitions(out string statuscode, string issueKey)
        {
            string result = String.Empty;
            result = SendRequest(out statuscode, Resource.ISSUE, UrlExtensions.Transitions(issueKey));
            Transition t = JsonConvert.DeserializeObject<Transition>(result);
            return t.Transitions;
        }

        #region Private methods
        /// <summary>
        /// Send the request to Jira
        /// </summary>
        /// <param name="responseCode"></param>
        /// <param name="resource"></param>
        /// <param name="argument"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string SendRequest(out string responseCode, Resource resource, string argument = null, string data = null, Method method = Method.GET)
        {
            string result = string.Empty;
            HttpWebResponse response;
            try
            {
                string url = UrlExtensions.ConCat(BaseUrl, resource.ToString().ToLower());
                if (argument != null)
                {
                    url = UrlExtensions.ConCat(url, argument);
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = method.ToString();

                if (data != null)
                {
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(data);
                    }
                }
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(U + ":" + P)));
                response = request.GetResponse() as HttpWebResponse;
                responseCode = response.StatusCode.ToString();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                responseCode = ex.Message;
            }

            return result;
        }
        #endregion
    }
}
