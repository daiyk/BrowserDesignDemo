using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    class OBJFileViewDescription : MonoBehaviour, IViewDescription
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

        public void InitializeDescription(ViewItem it, ItemLoadBrowser itemLoadBrowser)
        {
            Item = it;
            snippetBar.SetActive(true);
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The File is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
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
                Debug.LogError("Null FileBrowser cannot initialize the OBJFileViewDescription");
                return;
            }

            this.itemLoadBrowser = itemLoadBrowser;
            if (it.DataItem.GetType() != typeof(ObjItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is consistent with the UI type: ObjItem!");
                return;
            }

            loadServerButton.SetActive(!it.Loaded);
            removeServerButton.SetActive(it.Loaded);
            snippetBar.SetActive(false);
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
            }
        }
        public void ClosePanel()
        {

        }
        public void InitializePanel()
        {
        }
        public async void LoadItem()
        {

            if (Item.Loaded)
            {
                Debug.LogError($"Try to load a portal obj item that is already loaded {Item.Name}");
                return;
            }
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The file is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            snippetBar.SetActive(true);

            var objItem = (ObjItem)Item.DataItem;

            //load file
            await itemLoadBrowser.LoadViewItem(Item);

            //if it is still unloaded, then loading failed
            if (!Item.Loaded) 
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate($"Error: The file {objItem.Path} does not exist or not in acceptable directory......");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;
                return;
            }

            loadServerButton.SetActive(false);
            removeServerButton.SetActive(true);
            snippetBar.SetActive(false);


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
