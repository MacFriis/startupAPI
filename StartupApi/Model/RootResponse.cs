namespace StartupApi.Model
{
    internal class RootResponse : Resource
    {
        public Link Info { get; set; }

        // TODO: This is the sample data, you should add links to all your controllers here
        public Link AppData { get; set; }

        public Link Users { get; set; }
        public Form Token { get; set; }


        // TODO: implement ETAG?
    }
}