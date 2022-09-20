using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    public class JOBFileViewDescription : MonoBehaviour, IViewDescription
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

            if (itemLoadBrowser == null)
            {
                Debug.LogError("Null FileBrowser cannot initialize the KMLPortalViewDescription");
                return;
            }
            this.itemLoadBrowser = itemLoadBrowser;
            if (it.DataItem.GetType() != typeof(JOBItem))
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: inner dataItem is consistent with the UI type: KMLPortalItem!");
                return;
            }

            // enable buttons
            loadServerButton.SetActive(!it.Loaded);
            removeServerButton.SetActive(it.Loaded);
            // not used for local files
            snippetBar.SetActive(false);

            // check for favorites
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
            //if (Item.Loaded)
            //{
            //    Debug.LogError($"Try to load a portal kml item that is already loaded {Item.Name}");
            //    return;
            //}
            snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Warning: The service is loading, please wait......");
            snippetBar.GetComponent<TextMeshProUGUI>().color = Color.yellow;

            var jobItem = (JOBItem)Item.DataItem;

            string filePath = jobItem.Path;
            if (filePath != null)
            {
                snippetBar.SetActive(true);


                /////load success, change UI//////
                //retrieve the binding gameobject and set the color 
                var bindingObj = itemLoadBrowser.Controller.treeView.GetItemContainer(Item);
                Text title = bindingObj.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
                title.color = Color.green;

                Item.Loaded = true;
                Item.DataItem.Loaded = true;
                loadServerButton.SetActive(false);
                removeServerButton.SetActive(true);
                snippetBar.SetActive(false);

            }
            else
            {
                snippetBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate($"Error: The file {jobItem.Path} does not exist or not in acceptable directory......");
                snippetBar.GetComponent<TextMeshProUGUI>().color = Color.red;

            }
            loadServerButton.SetActive(!Item.Loaded);
            removeServerButton.SetActive(Item.Loaded);
        }
        public void UnLoadItem()
        {
            //first need to check whether it is loaded and where
            var kmlItem = (JOBItem)Item.DataItem;

            //remove loaded item from the list
            itemLoadBrowser.RemoveLoadedItem(kmlItem);

            ///unload sucessful, change UI///
            var bindingObj = itemLoadBrowser.Controller.treeView.GetItemContainer(Item);
            Text title = bindingObj.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
            title.color = Color.white;
            Item.Loaded = false;

            // toggle buttons
            loadServerButton.SetActive(true);
            removeServerButton.SetActive(false);
            snippetBar.SetActive(false);
        }
    }
}
