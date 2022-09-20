using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BrowserDesign.Extension;

namespace BrowserDesign.UI
{
    class FeatureServiceViewDescription : MonoBehaviour,IViewDescription 
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

        // this will be manually called by the fileBrowserController
        public async void InitializeDescription(ViewItem it,ItemLoadBrowser itemLoadController)
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
            
            if(itemLoadController == null)
            {
                Debug.LogError("Null FileBrowser initialize the FeatureServiceDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadController;

            if (it.DataItem.GetType() != typeof(FeatureServiceItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is not with the same type of UI type: FeatureServiceItem!");
                return;
            }

            FeatureServiceItem featureServiceItem = (FeatureServiceItem)it.DataItem;
            
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            if (Item.DataItem.Loaded)
            {
                if(featureServiceItem.Description != null)
                {
                    description.text = featureServiceItem.Description;
                }

                //check and load thumbnail
                if(featureServiceItem.ThumbNailUrl != null)
                {
                    thumbNail.SetActive(true);
                    var imageTexture = await Utilities.GetImage(featureServiceItem.ThumbNailUrl, await EsriManager.GetToken(),featureServiceItem.RequireToken);
                    if(imageTexture != null)
                    {
                        thumbNail.GetComponent<Image>().sprite = Sprite.Create(imageTexture, new Rect(0f, 0f, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
                    }
                }
                else
                {
                    thumbNail.SetActive(false);
                }

                //check and input snippet(summary)
                if(featureServiceItem.Snippt != null)
                {
                    snippetBar.GetComponent<TextMeshProUGUI>().text = featureServiceItem.Snippt;
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
                Debug.LogError($"Try to load a feature service that is already loaded {Item.Name}");
                return;
            }
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            var featureServiceItem = (FeatureServiceItem)Item.DataItem;

            //get feature service
            FeatureService fs = await Utilities.GetFeatureService(featureServiceItem.URL, featureServiceItem.RequireToken);
            if (fs == null)
            {
                return;
            }

            //check feature service capability
            if (!fs.authority.Contains("Query"))
            {
                Debug.LogError($"Portal: Try to load {fs.serviceName} but failed, because it doesn't support query operation");
    
                snippetBar.GetComponent<TextMeshProUGUI>().text = $"{fs.serviceName}" + LanguageManager.Translate(" is unloaded because it doesn't support query operation");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                return;
            }

            descriptionBar.SetActive(true);
            featureServiceItem.Description = fs.description;
            
            description.text = featureServiceItem.Description;

            if(featureServiceItem.ThumbNailUrl != null)
            {
                thumbNail.SetActive(true);
                //build uri
                var imageTexture = await Utilities.GetImage(featureServiceItem.ThumbNailUrl, await EsriManager.GetToken(), featureServiceItem.RequireToken);
                if (imageTexture != null)
                {
                    thumbNail.GetComponent<Image>().sprite = Sprite.Create(imageTexture, new Rect(0f, 0f, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
                }
            }
            else
            {
                thumbNail.SetActive(false);
            }
            
            //load feature layers, maybe need to set parent
            for (int i = 0; i < fs.layers.Length; i++)
            {
                var layer = new FeatureLayerItem(fs.layers[i].name, featureServiceItem.URL + "/" + fs.layers[i].id, featureServiceItem.RequireToken);
                //check if it is already loaded
                if (itemLoadBrowser.Controller.itemManager.ExistItem(layer))
                {
                    layer = (FeatureLayerItem)itemLoadBrowser.Controller.itemManager.GetItem(layer);
                }
                featureServiceItem.AddLayer(layer);

                //add new created item
                itemLoadBrowser.Controller.itemManager.AddItem(layer);
            }
                    
            Item.Children.Clear();
            if (featureServiceItem.GetLayers().Count > 0)
            {
                //add created children layers to the ViewItem
                foreach (var childItem in featureServiceItem.GetLayers())
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
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Loading failed: This Feature Service doesn't contain any layer with supported data formats (Currently only feature service, web map and .KML files are supported!).");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                Item.Loaded = false;
                Item.DataItem.Loaded = false;
                return;
            }


            //set up summary description
            if (featureServiceItem.Snippt != null)
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = featureServiceItem.Snippt;
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("No Summary");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
            }

            /////load success, change UI//////
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

            //remove the item and its childs from scene
            itemLoadBrowser.RemoveViewItem(Item);

            // toggle buttons
            descriptionBar.SetActive(Item.Loaded);
            loadServerButton.SetActive(!Item.Loaded);
            removeServerButton.SetActive(Item.Loaded);
            thumbNail.SetActive(Item.Loaded);
        }
    }
}
