using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BrowserDesign.UI
{
    public class FavoriteItemsViewDescription : MonoBehaviour
    {
        // Start is called before the first frame update
        ItemLoadBrowser itemLoadBrowser;
        ViewItem item;

        [SerializeField]
        private GameObject snippet;
        [SerializeField]
        private GameObject userItem;
        [SerializeField]
        private GameObject itemsContent;
        public void InitializePanel()
        {

        }
        public void ClosePanel()
        {

        }

        public void InitializeDescription(ViewItem item, ItemLoadBrowser itemLoadManager)
        {
            //initialize itemload manager
            this.itemLoadBrowser = itemLoadManager;
            this.item = item;
            snippet.SetActive(false);
            //initialize the items container
            RefreshItemList();
            
        }

        //buttons favorite item relative
        public void LoadAllFavoriteItems()
        {
            itemLoadBrowser.LoadFavoriteItems();
            snippet.SetActive(true);
            snippet.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Your data is loading in progress, Please wait for the file browser to refresh....");
            snippet.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        public void RemoveFavoriteItems()
        {
            itemLoadBrowser.ClearFavoriteItems();
            snippet.SetActive(true);
            snippet.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Favorite items are all clear!");
            snippet.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            RefreshItemList();
        }

        public void RefreshItemList()
        {
            //clear list in the favorite item list
            foreach(Transform child in itemsContent.transform)
            {
                Destroy(child.gameObject);
            }
            var allItems = itemLoadBrowser.Controller.itemManager.GetItems();
            var favoriteItems = itemLoadBrowser.GetFavoriteItems();

            //update container items since the favorite items contains different reference
            foreach(var it in allItems)
            {
                if (favoriteItems.Contains(it))
                {
                    favoriteItems.Remove(it);
                    favoriteItems.Add(it);
                }
            }
            bool allloaded = true;

            //populate items to favorite item list container
            foreach (var it in favoriteItems)
            {
                if (!allItems.Contains(it))
                {
                    //this means this item is already unloaded
                    it.Loaded = false;
                }
                if (!(it is IContainerLabelItem))
                {
                    var itemobj = Instantiate(userItem, itemsContent.transform);
                    itemobj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = it.Name;

                    if (it.Loaded)
                    {
                        itemobj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
                    }
                    else
                    {
                        allloaded = false;
                        itemobj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = Color.yellow;
                    }
                }     
            }
            if (!allloaded)
            {
                snippet.SetActive(true);
                snippet.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("You have items that are not loaded");
                snippet.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            }
        }
    }
}
