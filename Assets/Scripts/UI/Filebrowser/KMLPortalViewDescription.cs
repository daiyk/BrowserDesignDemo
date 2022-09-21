using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    class KMLPortalViewDescription : MonoBehaviour, IViewDescription
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
            // if it is loaded in somewhere else, then load the data
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
                Debug.LogError("Null FileBrowser cannot initialize the KMLPortalViewDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadBrowser;
            if (it.DataItem.GetType() != typeof(KMLPortalItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is consistent with the UI type: KMLItem!");
                return;
            }

            KMLPortalItem kmlPortalItem = (KMLPortalItem)it.DataItem;

            
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            if (Item.DataItem.Loaded)
            {

                if (kmlPortalItem.Description != null)
                {
                    description.text = kmlPortalItem.Description;
                }

                //check and input snippet(summary)
                if (kmlPortalItem.Snippt != null)
                {
                    snippetBar.GetComponent<TextMeshProUGUI>().text = kmlPortalItem.Snippt;
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
                //and also check its parent, featureservice/mapservice/webmap whether is in here
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
            if (Item.Loaded)
            {
                Debug.LogError($"Try to load a portal kml item that is already loaded {Item.Name}");
                return;
            }
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;

            
            
            snippetBar.GetComponent<TextMeshProUGUI>().text += $"\n{Item.Name}" + LanguageManager.Translate(" download failed and was unloaded. Please try again later!");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
              
            
            descriptionBar.SetActive(Item.Loaded);
            loadServerButton.SetActive(!Item.Loaded);
            removeServerButton.SetActive(Item.Loaded);
        }
        public void UnLoadItem()
        {
            //check if it is loaded
            if (!Item.Loaded)
            {
                Debug.LogError($"Try to unload a {Item.DataItem.GetType().ToString()} that is not loaded yet {Item.Name}");
                return;
            }

            //remove loaded item from the list

            itemLoadBrowser.RemoveViewItem(Item);


            // toggle buttons
            loadServerButton.SetActive(true);
            removeServerButton.SetActive(false);
            snippetBar.SetActive(false);
        }



    }
}
