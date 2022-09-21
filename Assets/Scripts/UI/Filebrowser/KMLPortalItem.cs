
namespace BrowserDesign.UI
{
    class KMLPortalItem : IPortalItem
    {

        //info about the mapservice
        public string Description { get; set; }
        public string Snippt { get; set; }

        public string LocalPath { get; set; }
        public KMLPortalItem(string name, string url)
        {
            base.name = name;
            base.url = url;
        }
        public KMLPortalItem(string name, string url, bool requireToken)
        {
            base.name = name;
            base.url = url;
            base.requireToken = requireToken;
        }
    }
}
