namespace AgentApi.Models

{
    public class AgentDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string os { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string ip { get; set; }
        public string location { get; set; }
        public string[] resources { get; set; }
    }
}