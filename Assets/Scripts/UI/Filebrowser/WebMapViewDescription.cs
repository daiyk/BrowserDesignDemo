using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    class WebMapViewDescription:MonoBehaviour, IViewDescription
    {
        public ViewItem Item { get; set; }

        //UI elements
        [SerializeField]
        private GameObject loadWebMapButton;
        [SerializeField]
        private GameObject removeWebMapButton;
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
            //in case item loaded/unloaded in other place
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
                Debug.LogError("Null FileBrowser initialize the WebMapDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadBrowser;

            if (it.DataItem.GetType() != typeof(WebMapItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is not with the same type of UI type: WebMapItem!");
                return;
            }

            WebMapItem webMapItem = (WebMapItem)it.DataItem;
            
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            if (Item.DataItem.Loaded)
            {

                if (webMapItem.Description != null)
                {
                    description.text = webMapItem.Description;
                }

                //check and input snippet(summary)
                if (webMapItem.Snippt != null)
                {
                    snippetBar.GetComponent<TextMeshProUGUI>().text = webMapItem.Snippt;
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
            loadWebMapButton.SetActive(!it.Loaded);
            removeWebMapButton.SetActive(it.Loaded);
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
        public async void LoadItem()
        {
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            var webMapItem = (WebMapItem)Item.DataItem;

            //add children items to the item
            //TODO: update viewItem UI (Maybe invoke a event consumer in specific ViewItem observer?)
            Item.Children.Clear();
            if (webMapItem.GetLayers().Count > 0) //means we load some items into the layer
            {
                //add created children layers to the ViewItem
                foreach (var childItem in webMapItem.GetLayers())
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
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Loading failed: This Web Map doesn't contain any layer with supported data formats (Currently only feature service, web map and .KML files are supported!).");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                Item.Loaded = false;
                Item.DataItem.Loaded = false;
                return;
            }
            
            /////load success, change UI//////
            //retrieve the binding gameobject and set the color 
            var bindingObj = itemLoadBrowser.Controller.treeView.GetItemContainer(Item);
            Text title = bindingObj.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
            title.color = Color.green;
            Item.Loaded = true;
            Item.DataItem.Loaded = true;

            //change UI button
            descriptionBar.SetActive(Item.Loaded);
            loadWebMapButton.SetActive(false);
            removeWebMapButton.SetActive(true);
        }
        public void UnLoadItem()
        {
            //check if it is loaded
            if (!Item.Loaded)
            {
                Debug.LogError($"Try to unload a {Item.DataItem.GetType().ToString()} that is not loaded yet {Item.Name}");
                return;
            }

            //remove dataitems from DataItem collection and remove ViewItems
            itemLoadBrowser.RemoveViewItem(Item);

            itemLoadBrowser.Controller.treeView.DataBindItem(Item);

            // toggle buttons
            descriptionBar.SetActive(Item.Loaded);
            loadWebMapButton.SetActive(!Item.Loaded);
            removeWebMapButton.SetActive(Item.Loaded);
            thumbNail.SetActive(Item.Loaded);
        }
    }
}
