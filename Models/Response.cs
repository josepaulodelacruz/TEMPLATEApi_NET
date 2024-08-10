namespace TemplateAPI.Models
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
        public dynamic Body { get; set; }
    }
}
