using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BrowserDesign.Extension;
using Utility;

namespace BrowserDesign.UI
{

    public class MapServiceViewDescription : MonoBehaviour, IViewDescription
    {
        public ViewItem Item { get; set; }

        //UI elements in the Panel
        [SerializeField]
        private GameObject loadServerButton;
        [SerializeField]
        private GameObject removeServerButton;
        [SerializeField]
        private GameObject snippetBar;
        [SerializeField]
        private GameObject thumbNail;
        [SerializeField]
        private GameObject descriptionBar;
        [SerializeField]
        private TextMeshProUGUI description;
        [SerializeField]
        private Sprite starred;
        [SerializeField]
        private Sprite unstarred;
        [SerializeField]
        private Image starFavorite;

        //toggle for add/remove favorite item
        private bool favoriteItem;
        private bool FavoriteItem
        {
            get { return favoriteItem; }
            set
            {
                if (value != favoriteItem)
                {
                    favoriteItem = value;
                    if (value)
                    {
                        starFavorite.sprite = starred;
                    }
                    else
                    {
                        starFavorite.sprite = unstarred;
                    }
                }
            }
        }

        private ItemLoadBrowser itemLoadBrowser;

        public void InitializePanel()
        {

        }
        public void ClosePanel()
        {

        }
        public async void InitializeDescription(ViewItem it, ItemLoadBrowser itemLoadBrowser)
        {
            //set the view item reference value
            Item = it;
            //check whether it has been loaded/unloaded somewhere else, then update the underlying structure
            if (it.DataItem.Loaded != it.Loaded)
            {
                if (it.DataItem.Loaded)
                {
                    LoadItem();
                }
                else
                {
                    UnLoadItem();
                }
                it.Loaded = it.DataItem.Loaded;
            }

            if (itemLoadBrowser == null)
            {
                Debug.LogError("Null FileBrowser initialize cannot the MapServiceViewDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadBrowser;

            if (it.DataItem.GetType() != typeof(MapServiceItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is not consistent with the UI type: MapServiceItem!");
                return;
            }

            MapServiceItem mapServiceItem = (MapServiceItem)it.DataItem;
           
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            if (Item.DataItem.Loaded)
            {

                if (mapServiceItem.Description != null)
                {
                    description.text = mapServiceItem.Description;
                }

                //check and input snippet(summary)
                if (mapServiceItem.Snippt != null)
                {
                    snippetBar.GetComponent<TextMeshProUGUI>().text = mapServiceItem.Snippt;
                    snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                else
                {
                    snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("No Summary");
                    snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }
            else
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = "";
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            descriptionBar.SetActive(it.Loaded);
            loadServerButton.SetActive(!it.Loaded);
            removeServerButton.SetActive(it.Loaded);
            if (itemLoadBrowser.ContainFavoriteItem(it.DataItem))
            {
                FavoriteItem = true;
            }
            else
            {
                FavoriteItem = false;
            }
        }
        public void ToggleFavorite()
        {
            if (FavoriteItem)
            {
                FavoriteItem = false;
                itemLoadBrowser.RemoveFavoriteItem(Item.DataItem);
            }
            else
            {
                FavoriteItem = true;
                //and also check its parent, webmap whether is in here
                itemLoadBrowser.AddFavoriteItem(Item.DataItem);
                //check its parent
                if (Item.Parent.DataItem is WebMapItem)
                {
                    itemLoadBrowser.AddFavoriteItem(Item.Parent.DataItem);
                }
            }
        }

        //button load service
        public async void LoadItem()
        {
            if (Item.Loaded)
            {
                Debug.LogError($"Try to load a map service that is already loaded {Item.Name}");
                return;
            }
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            var mapServiceItem = (MapServiceItem)Item.DataItem;

            //TODO: update viewItem UI (Maybe invoke a event consumer in specific ViewItem observer?)
            //get map service
            MapService ms = await Utilities.GetMapService(mapServiceItem.URL, mapServiceItem.RequireToken);
            if (ms == null)
            {
                return;
            }

            //check map service capability
            if (!ms.authority.Contains("Query"))
            {
                Debug.LogError($"Portal: Try to load {ms.mapName} but failed, because it doesn't support query operation");

                snippetBar.GetComponent<TextMeshProUGUI>().text = $"{ms.mapName}" + LanguageManager.Translate(" is unloaded because it doesn't support query operation");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                return;
            }

            descriptionBar.SetActive(true);
            mapServiceItem.Description = ms.description;

            description.text = mapServiceItem.Description;

            //load feature layers, maybe need to set parent
            for (int i = 0; i < ms.layers.Length; i++)
            {
                var layer = new FeatureLayerItem(ms.layers[i].name, mapServiceItem.URL + "/" + ms.layers[i].id, mapServiceItem.RequireToken);

                layer = (FeatureLayerItem)itemLoadBrowser.Controller.itemManager.AddOrExistItem(layer);
                mapServiceItem.AddLayer(layer);

            }

            Item.Children.Clear();
            if (mapServiceItem.GetLayers().Count > 0)
            {
                //add created children layers to the ViewItem
                foreach (var childItem in mapServiceItem.GetLayers())
                {
                    var it = new ViewItem(childItem.Name, childItem);
                    Item.Children.Add(it);
                    it.Parent = Item;

                    //update UI viewTree
                    itemLoadBrowser.Controller.treeView.AddChild(Item, it);
                }

                itemLoadBrowser.Controller.treeView.Expand(Item);
                itemLoadBrowser.Controller.treeView.DataBindItem(Item);
            }
            else
            {
                //there is no supported layers
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Loading failed: This Map Service doesn't contain any layer with supported data formats (Currently only feature service, web map and .KML files are supported!).");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                Item.Loaded = false;
                Item.DataItem.Loaded = false;
                return;
            }



            //set up summary description
            if (mapServiceItem.Snippt != null)
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = mapServiceItem.Snippt;
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("No Summary");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
            }

            //retrieve the binding gameobject and set the color 
            var bindingObj = itemLoadBrowser.Controller.treeView.GetItemContainer(Item);
            Text title = bindingObj.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
            title.color = Color.green;

            Item.Loaded = true;
            Item.DataItem.Loaded = true;

            //change UI button
            loadServerButton.SetActive(false);
            removeServerButton.SetActive(true);
        }

        public void UnLoadItem()
        {
            //check if it is loaded
            if (!Item.Loaded)
            {
                Debug.LogError($"Try to unload a {Item.DataItem.GetType().ToString()} that is not loaded yet {Item.Name}");
                return;
            }

            //remvoe item and its child from the scene
            itemLoadBrowser.RemoveViewItem(Item);

            // toggle buttons
            descriptionBar.SetActive(Item.Loaded);
            loadServerButton.SetActive(!Item.Loaded);
            removeServerButton.SetActive(Item.Loaded);
            thumbNail.SetActive(Item.Loaded);
        }

    }
}
