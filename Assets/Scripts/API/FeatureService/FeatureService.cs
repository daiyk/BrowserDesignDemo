using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrowserDesign.Extension
{
    public class FeatureService
    {
        // v-labs
        public string serviceName { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool requireToken { get; set; } = false;
        public bool isPortalItem { get; set; } = false;
        public float currentVersion { get; set; }
        public string serviceDescription { get; set; }
        public string mapName { get; set; }
        public string description { get; set; }
        public string metadataLink { get; set; }//link to the underlying item in portal sharing or the orginal file.
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
        //a list of acceptable operation
        public List<string> authority { get; set; }


        //below properties specific to FeatureService response
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

    public class Spatialreference1
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }

    public class Spatialreference2
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }

    public class Initialextent
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
        public Spatialreference1 spatialReference { get; set; }
    }

    public class Fullextent
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
        public Spatialreference2 spatialReference { get; set; }
    }

    public class Documentinfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Comments { get; set; }
        public string Subject { get; set; }
        public string Category { get; set; }
        public string Keywords { get; set; }
    }

    public class Layer
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parentLayerId { get; set; }
        public bool defaultVisibility { get; set; }
        public object subLayerIds { get; set; }
        public double minScale { get; set; }
        public double maxScale { get; set; }
        public string type { get; set; }
        public string geometryType { get; set; }
    }

    public class Geotransform
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
        public bool transformForward { get; set; }
        public string name { get; set; }
    }

    public class Datumtransformation
    {
        public Geotransform[] geoTransforms { get; set; }
    }

    
    public class ItemInfo
    {
        public string id { get; set; }
        public string snippet { get; set; }
        public string thumbnail { get; set; }
        public string itemId { get; set; } //only for webmap item
        public List<ItemInfo> layers { get; set; } //only for webmap item
        public string layerType { get; set; } // onlt for webmap item
        public string owner { get; set; }
        public string total { get; set; }
        public bool isOrgItem { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public List<string> categories { get; set; }
        public string ownerFolder { get; set; }
    }

    public class WebMap
    {
        public string url { get; set; }
        public string description { get; set; }
        public string snippet { get; set;}
        public bool requireToken { get; set; } = false;
        public string serviceName { get; set; }
        public string thumbnail { get; set; }
        public List<ItemInfo> operationalLayers { get; set; }
        public Spatialreference spatialReference { get; set; }
    }

    public class ServiceDirectory
    {
        public List<string> folders { get; set; }
        public List<ServiceInfo> services { get; set; }
        public ErrorBody error { get; set; }

    }

    public class ServiceInfo
    {
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public ErrorBody error { get; set; }
    }

    public class ErrorBody
    {   
        public int code { get; set; }
        public string message { get; set; }
        public string messageCode { get; set; }
        public List<string> details { get; set; }
    }
    public class ErrorReponse
    {
       public ErrorBody error { get; set; }
    }
}

