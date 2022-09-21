using System;
using System.Collections.Generic;
using BrowserDesign.UI;
/// <summary>
/// Handles the user setting and items
/// </summary>
namespace BrowserDesign.Users
{
    public class UserManager
    {
        /// <summary>
        /// The data types that can be saved for quick loading
        /// </summary>
        public static Dictionary<string, Type> allowedUserItemTypes = new Dictionary<string, Type>()
        {
            {"KMLFile",typeof(KMLItem) },
            {"KMLPortal",typeof(KMLPortalItem) },
            {"FeatureService",typeof(FeatureServiceItem) },
            {"FeatureLayer",typeof(FeatureLayerItem) },
            {"WebMap",typeof(WebMapItem) },
            {"MapService", typeof(MapServiceItem) },
            {"ObjFile",typeof(ObjItem) },
            {"DXFFile",typeof(DXFItem) }
        };
        /// <summary>
        /// DataItems that user loaded and can be loaded in the next login
        /// </summary>
        public static HashSet<IBaseItem> UserFavoriteItems = new HashSet<IBaseItem>();

    }
}
