using System.Collections.Generic;
namespace BrowserDesign.API
{
    public class MapService
    {
        // credential information
        public string url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool requireToken { get; set; } = false;

        public List<string> authority { get; set; } //a manually assigned variable for the service's capabilities

        //json parser fields
        public int id { get; set; } //only exist for map server?
        public float currentVersion { get; set; }
        public string serviceDescription { get; set; }
        public string mapName { get; set; }
        public string description { get; set; }
        public string metadataLink { get; set; }//link to the underlying item in portal sharing.
        public string keyword { get; set; }
        public string copyrightText { get; set; }
        public Spatialreference spatialReference { get; set; }
        public Layer[] layers { get; set; }
        public object[] tables { get; set; }
        public bool singleFusedMapCache { get; set; }
        public Initialextent initialExtent { get; set; }
        public Fullextent fullExtent { get; set; }
        public string units { get; set; }
        public string capabilities { get; set; }
        public string supportedImageFormatTypes { get; set; }
        public Documentinfo documentInfo { get; set; }
        public float minScale { get; set; }
        public float maxScale { get; set; }
        public bool exportTilesAllowed { get; set; }
        public int maxExportTilesCount { get; set; }
        public bool cacheOnDemand { get; set; }
        public string type { get; set; }
        
        //need tileinfo object parser
        //need changeTrackinginfo object parser
        //need scheduleInfo object parser
        //additional properties not included:
        // 1. created
        // 2. modified
        // 3. status
        // 4. access

        public bool hasVersionedData { get; set; }
        public bool hasArchivedData { get; set; }
        public bool supportsDisconnectedEditing { get; set; }
        public bool supportsQueryDataElements { get; set; }
        public bool supportsRelationshipsResource { get; set; }
        public bool syncEnabled { get; set; }
        public int maxRecordCount { get; set; }
        public int maxRecordCountFactor { get; set; }

        public bool allowGeometryUpdates { get; set; }
        public bool allowTrueCurvesUpdates { get; set; }
        public bool onlyAllowTrueCurveUpdatesByTrueCurveClients { get; set; }
        public bool supportsApplyEditsWithGlobalIds { get; set; }
        public bool supportsTrueCurve { get; set; }

        public bool supportsQueryDomains { get; set; }
        public object[] relationships { get; set; }
        public bool enableZDefaults { get; set; }
        public bool allowUpdateWithoutMValues { get; set; }
        public Datumtransformation[] datumTransformations { get; set; }
        public bool supportsDynamicLayers { get; set; } //only for query capable server
        public bool supportsDatumTransformation { get; set; } //only for query capable server
        public string supportedQueryFormats { get; set; } //only for query capable server
        public string supportedExtensions { get; set; } //only for query capable server
    }
}
