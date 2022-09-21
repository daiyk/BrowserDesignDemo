using UnityEngine;
using BrowserDesign.API;
using TMPro;
using BrowserDesign.Utility;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    public class FeatureLayerViewDescription : MonoBehaviour,IViewDescription
    {

        [SerializeField]
        private GameObject loadLayerButton;
        [SerializeField]
        private GameObject removeLayerButton;
        [SerializeField]
        private GameObject snippetBar;
        [SerializeField]
        private GameObject content;
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
        
        //template for html
        [SerializeField]
        private GameObject table_2_column;
        [SerializeField]
        private GameObject table_line;

        private ItemLoadBrowser itemLoadBrowser;

        public ViewItem Item { get; set; }
        public void InitializePanel()
        {
            
        }
        
        public void ClosePanel()
        {

        }

        public async void InitializeDescription(ViewItem it, ItemLoadBrowser itemLoadBrowser)
        {
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: Feature layer is loading, please wait.....");
            if (itemLoadBrowser == null)
            {
                Debug.LogError("Null FileBrowser initialize the FeatureLayerDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadBrowser;

            if (it.DataItem.GetType() != typeof(FeatureLayerItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is not with the same type of UI type: FeatureLayerItem!");
                Debug.LogError("Error: inner dataItem is not with the same type of UI type: FeatureLayerItem!");
                return;
            }
            
            FeatureLayerItem featureLayerItem = (FeatureLayerItem)it.DataItem;
            Item = it;

            if (featureLayerItem.Loaded)
            {
                if(featureLayerItem.Description != null)
                {
                    featureLayerItem.Description = featureLayerItem.Description;
                   
                    
                    snippetBar.GetComponent<TextMeshProUGUI>().text = featureLayerItem.Description; ;
                    snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
                    
                }
                else
                {
                    snippetBar.SetActive(false);
                }
            }
            else
            {
                snippetBar.SetActive(false);
            }
            loadLayerButton.SetActive(!featureLayerItem.Loaded);
            removeLayerButton.SetActive(featureLayerItem.Loaded);
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
                //and also check its parent, featureservice/mapservice/webmap whether is in here
                itemLoadBrowser.AddFavoriteItem(Item.DataItem);
                //check its parent
                if(Item.Parent.DataItem is FeatureServiceItem || Item.Parent.DataItem is MapServiceItem)
                {
                    itemLoadBrowser.AddFavoriteItem(Item.Parent.DataItem);
                    //webmap can contains service
                    if(Item.Parent.Parent.DataItem is WebMapItem)
                    {
                        itemLoadBrowser.AddFavoriteItem(Item.Parent.Parent.DataItem);
                    }
                }
                //webmap can directly contains feature layer
                if(Item.Parent.DataItem is WebMapItem)
                {
                    itemLoadBrowser.AddFavoriteItem(Item.Parent.DataItem);
                }
            }
        }
        public async void LoadItem()
        {
            //get layer 
            var featureLayerItem  = (FeatureLayerItem)Item.DataItem;
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: Feature layer is loading, please wait.....");
            var featureLayer = await Utilities.GetFeatureLayer(featureLayerItem.URL,featureLayerItem.RequireToken);
            
            if (featureLayer != null)
            {
                if(featureLayer.description != null)
                {
                    snippetBar.SetActive(true);
                    featureLayerItem.Description = featureLayer.description;
          
                    snippetBar.GetComponent<TextMeshProUGUI>().text = featureLayer.description;
                    snippetBar.GetComponent<TextMeshProUGUI>().color = Color.white;
                    
                }

                //add to loaded layers
                await itemLoadBrowser.LoadViewItem(Item);
                if (!RemoteServerManager.layers2load.ContainsFeatureLayer(featureLayerItem.URL) && !RemoteServerManager.layersLoaded.ContainsFeatureLayer(featureLayerItem.URL))
                {
                    RemoteServerManager.layers2load.Add(featureLayer);
                }

                if (Item.Loaded)
                {
                    /////load success, change UI//////
                    //retrieve the binding gameobject and set the color 
                    var bindingObj = itemLoadBrowser.Controller.treeView.GetItemContainer(Item);
                    Text title = bindingObj.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
                    title.color = Color.green;

                    Item.Loaded = true;
                    Item.DataItem.Loaded = true;

                    loadLayerButton.SetActive(false);
                    removeLayerButton.SetActive(true);
                }
            }
            else
            {
                snippetBar.SetActive(true);
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: feature layer loading fail.");
            }
            
        }

        public void UnLoadItem()
        {
            if (!Item.Loaded)
            {
                Debug.LogError($"Try to unload a {Item.DataItem.GetType().ToString()} that is not loaded yet {Item.Name}");
                return;
            }
            //remove any layer data loaded in the scene
            itemLoadBrowser.RemoveViewItem(Item);

            snippetBar.GetComponent<TextMeshProUGUI>().text = "";
            snippetBar.SetActive(false);

            removeLayerButton.SetActive(Item.Loaded);
            loadLayerButton.SetActive(!Item.Loaded);


        }

    }
}
