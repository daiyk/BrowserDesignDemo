using System.Collections.Generic;

namespace BrowserDesign.Extension
{
    public static class EsriExtensionMethods
    {
        public static int GetIDlayerFromLayerName(this FeatureService source, string layername)
        {
            foreach (var layer in source.layers)
            {
                if (layer.name == layername)
                    return layer.id;
            }

            return -1;
        }

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

