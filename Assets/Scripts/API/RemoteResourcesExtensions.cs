using System.Collections.Generic;

namespace BrowserDesign.API
{
    public static class RemoteResourcesExtensions
    {

        /// <summary>
        /// Checks if the FeatureLayer is present
        /// </summary>
        /// <param name="source"></param>
        /// <param name="layername"></param>
        /// <returns></returns>
        public static bool ContainsFeatureLayer(this List<FeatureLayer> source, string layerURL)
        {
            foreach (var layer in source)
            {
                if (layer.layerUrl == layerURL)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Remove a feature layer from the given list by searching the given name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="layername"></param>
        public static void RemoveFeatureLayerByName(this List<FeatureLayer> source, string layername)
        {
            FeatureLayer fl2remove = new FeatureLayer();
            foreach (var layer in source)
            {
                if (layer.name == layername)
                {
                    fl2remove = layer;
                    break;
                }
            }

            // my way to check it has been found!
            if (fl2remove.name != "")
            {
                source.Remove(fl2remove);
            }

        }
        /// <summary>
        /// Remove a feature layer from the given list by searching the given URL
        /// </summary>
        /// <param name="source"></param>
        /// <param name="layerUrl"></param>
        public static void RemoveFeatureLayerByUrl(this List<FeatureLayer> source, string layerUrl)
        {
            FeatureLayer fl2remove = new FeatureLayer();
            foreach (var layer in source)
            {
                if (layer.layerUrl == layerUrl)
                {
                    fl2remove = layer;
                    break;
                }
            }

            // my way to check it has been found!
            if (fl2remove.name != "")
            {
                source.Remove(fl2remove);
            }
        }
    }
}

