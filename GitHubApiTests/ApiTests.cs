
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace GitHubApiTests
{
    public class ApiTests
    {

        // Demo Sample Restsharp Testing api.github repo issues
        // baseurl -> api.github.com
        // repo -> username -> project-name -> issues
        // username -> Github username
        // password -> Github password

        private RestClient client;
        private const string baseUrl = "https://api.github.com";
        private const string partialUrl = "/repos/{user}/{project_name}/issues";
        private const string username = "{username}";
        private const string password = "{password}";


        [SetUp]
        public void SetUp()
        {
            this.client = new RestClient(baseUrl);
            this.client.Authenticator = new HttpBasicAuthenticator(username, password);   
           
        } 

        [Test]
        [Timeout(2000)] 
        public void Test_GetSingleIssue()
        {

            var request = new RestRequest($"{partialUrl}/1", Method.Get);

            var response = client.Execute(request);


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Http Status Code property");

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            // The assertion message will differ depending on the tested repo
            Assert.That(issue.title, Is.EqualTo("First issue 1233443355ws"), "First issue updated");
            Assert.That(issue.number, Is.EqualTo(1));
        }

        [Test]
        public void Test_GetSingleIssueWithLabels()
        {

            var request = new RestRequest($"{partialUrl}/1", Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Http Status Code property");

           
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            for (int i = 0; i < issue.labels.Count; i++)
            {
                // get current label properties
                Assert.That(issue.labels[i].name, Is.Not.Null, "Name");
                Assert.That(issue.labels[i].id, Is.GreaterThan(0));
                Assert.That(issue.labels[i].color, Is.Not.Null, "Color");  
            }

        }


        [Test]
        public void Test_GetAllIssues()
        {

            var request = new RestRequest(partialUrl, Method.Get);

            var response = client.Execute(request);


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Http Status Code property");

            // parse response
            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);


            foreach (var issue in issues)
            {
                Assert.That(issue.title, Is.Not.Empty);
                Assert.That(issue.number, Is.GreaterThan(0));

            }
                
        }

        [Test]
        public void Test_CreateNewIssue()
        {
            // Arrange
            var request = new RestRequest(partialUrl, Method.Post);


            // example code, will differ
            var issueBody = new
            {
                title = "A new test issue from RestSharp by User" + DateTime.Now.Ticks,
                body = "Some new issue 38848492939943",
                labels = new string[] { "bug", "critical", "release" }
            };

            request.AddBody(issueBody);

            var response = this.client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created), "HTTP Status Code property");
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title)); // "A new test issue from RestSharp"
            Assert.That(issue.body, Is.EqualTo(issueBody.body)); // "other issue ....",

        }

    }
}