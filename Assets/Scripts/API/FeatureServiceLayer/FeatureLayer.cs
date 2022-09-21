namespace BrowserDesign.API
{
    public class FeatureLayer
    {
        // credential information
        public string layerUrl { get; set; }
        public bool requireToken { get; set; } = false;
        public bool isPortalItem { get; set; } = false; //check whether it belongs to the portal item
        // esri
        public double currentVersion { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public ParentLayer parentLayer { get; set; }
        public string displayField { get; set; }
        public string description { get; set; }
        public string copyrightText { get; set; }
        public string subtypeField { get; set; }
        public string defaultSubtypeCode { get; set; }
        public bool defaultVisibility { get; set; }
        public object editFieldsInfo { get; set; }
        public object ownershipBasedAccessControlForFeatures { get; set; }
        public bool syncCanReturnChanges { get; set; }
        public object relationships { get; set; }
        public bool isDataVersioned { get; set; }
        public bool isDataArchived { get; set; }
        public bool isDataBranchVersioned { get; set; }
        public bool isDataReplicaTracked { get; set; }
        public bool isCoGoEnabled { get; set; }
        public bool supportsRollbackOnFailureParameter { get; set; }
        public ArchivingInfo archivingInfo { get; set; }
        public bool supportsStatistics { get; set; }
        public bool supportsAdvancedQueries { get; set; }
        public bool supportsCoordinatesQuantization { get; set; }
        public bool supportsDatumTransformation { get; set; }
        public string geometryType { get; set; }
        public object geometryProperties { get; set; }
        public float minScale { get; set; }
        public float maxScale { get; set; }
        public float effectiveMinScale { get; set; }
        public float effectiveMaxScale { get; set; }
        public object advancedQueryCapabilities { get; set; }
        public float standardMaxRecordCountNoGeometry { get; set; }
        public bool supportsAsyncCalculate { get; set; }
        public bool supportsFieldDescriptionProperty { get; set; }
        public object advancedEditingCapabilities { get; set; }
        public object userTypeExtensions { get; set; }
        public Extent extent { get; set; }
        public object heightModelInfo { get; set; }
        public object sourceHeightModelInfo { get; set; }
        public object sourceSpatialReference { get; set; }
        public object drawingInfo { get; set; }
        public bool hasM { get; set; }
        public bool hasZ { get; set; }
        public bool enableZDefaults { get; set; }
        public float zDefault { get; set; }
        public bool allowGeometryUpdates { get; set; }
        public object timeInfo { get; set; }
        public bool hasAttachments { get; set; }
        public object htmlPopupType { get; set; }
        public string objectIdField { get; set; }
        public string globalIdField { get; set; }
        public string typeIdField { get; set; }
        public Field[] fields { get; set; }
        public object geometryField { get; set; }
        public object types { get; set; }
        public object templates { get; set; }
        public object subtypes { get; set; }
        public float maxRecordCount { get; set; }
        public float standardMaxRecordCount { get; set; }
        public float tileMaxRecordCount { get; set; }
        public float maxRecordCountFactor { get; set; }
        public string supportedQueryFormats { get; set; }
        public bool hasMetadata { get; set; }
        public bool hasStaticData { get; set; }
        public string sqlParserVersion { get; set; }
        public bool isUpdatableView { get; set; }
        public string capabilities { get; set; }
    }

    public class ParentLayer
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class ArchivingInfo
    {
        public bool supportsQueryWithHistoricMoment { get; set; }
        public double? startArchivingMoment { get; set; }
    }

    public class Extent
    {
        public double xmin { get; set; }
        public double ymin { get; set; }
        public double xmax { get; set; }
        public double ymax { get; set; }
        public Spatialreference spatialReference { get; set; }
    }

    public class Spatialreference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
        public float vcsWkid { get; set; }
        public float latestVcsWkid { get; set; }
        public double xyTolerance { get; set; }
        public double zTolerance { get; set; }
        public double mTolerance { get; set; }
        public double falseX { get; set; }
        public double falseY { get; set; }
        public double xyUnits { get; set; }
        public double falseZ { get; set; }
        public double zUnits { get; set; }
        public double falseM { get; set; }
        public double mUnits { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public object type { get; set; }
        public string sqltype { get; set; }
        public string alias { get; set; }
        public object domain { get; set; }
        public bool editable { get; set; }
        public bool nullable { get; set; }
        public string length { get; set; }
        public string defaultValue { get; set; }
        public string modelName { get; set; }
    }
}