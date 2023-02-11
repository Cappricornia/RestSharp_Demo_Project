namespace GitHubApiTests
{
    public class Issue
    {

        public int number { get; set; }
        public string title { get; set; }
        public string body { get; set; }


        public List<Labels> labels { get; set;}
    }
}