namespace Customer.Common
{
    public class SwaggerSettings<TInfo>
    {
        public string JsonUrl { get; set; }

        public string Stage { get; set; }

        public string Description { get; set; }

        public string SwaggerDocName { get; set; }

        public string XmlCommentsFile { get; set; }
    }
}