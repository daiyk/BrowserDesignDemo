using Battlehub.UIControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BrowserDesign.Extension;
using Utility;
using BrowserDesign.users;
using System.Linq;

namespace BrowserDesign.UI
{
    public class ItemLoadBrowser : MonoBehaviour
    {
        [Tooltip("Assign the original filebrowser object to this field")]
        [SerializeField]
        private FileBrowserController fileBrowserController;

        //panel that shows the description
        [SerializeField]
        private GameObject descriptionPanel;

        [Tooltip("ScrollView content object for description")]
        [SerializeField]
        private GameObject descriptionContent;

        [SerializeField]
        private GameObject featureServiceViewDescription;

        [SerializeField]
        private GameObject featureLayerViewDescription;

        [SerializeField]
        private GameObject webMapViewDescription;

        [SerializeField]
        private GameObject mapServiceDescription;

        [SerializeField]
        private GameObject kmlServiceDescription;

        [SerializeField]
        private GameObject kmlFileViewDescription;

        [SerializeField]
        private GameObject objFileViewDescription;

        [SerializeField]
        private GameObject dxfFileViewDescription;

        [SerializeField]
        private GameObject jobFileViewDescription;

        [SerializeField]
        private GameObject addArcGISServiceButtons;

        [SerializeField]
        private GameObject serviceDirectoryLoginDescription;

        [SerializeField]
        private GameObject favoriteItemsViewDescription;

        [SerializeField]
        private GameObject portalLoginDescription;

        [SerializeField]
        private Sprite favoriteStar;

        private static HashSet<IBaseItem> LoadedDataItem = new HashSet<IBaseItem>();

        public FileBrowserController Controller { get { return fileBrowserController; } }
        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        async void Initialize()
        {
            //set up UI
            fileBrowserController.Title = "Load Resource";
            fileBrowserController.ConfirmButtonText = "Load";
            fileBrowserController.ToggleInputField = false;

            //add additional treeView event consumer to the treeView Item
            fileBrowserController.treeView.SelectionChanged += LoadItemSelectionChanged;
            fileBrowserController.treeView.ItemDataBinding += LoadItemBinding;
            fileBrowserController.AddItemSelectedEventListener(LaunchButton);
            fileBrowserController.AddItemCancelEventListener(CancelButton);
            descriptionPanel.SetActive(false);
            Refresh();
            
        }

        #region treeView(fileBrowser) eventListener functions
        // this function will initialize the right panel for description
        private void LoadItemSelectionChanged(object sender, SelectionChangedArgs e)
        {
            ViewItem item = e.NewItem as ViewItem;
            //clean any content that remain in the description panel
            DestroyFileList();
            if(item == null)
            {
                return;
            }
            //initialize right panel based on item type
            if(item.DataItem is KMLItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(kmlFileViewDescription, descriptionContent.transform).GetComponent<KMLFileViewDescription>();
                cmp.InitializeDescription(item,this);
            }
            else if (item.DataItem is FeatureServiceItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(featureServiceViewDescription, descriptionContent.transform).GetComponent<FeatureServiceViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is FeatureLayerItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(featureLayerViewDescription, descriptionContent.transform).GetComponent<FeatureLayerViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is WebMapItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(webMapViewDescription, descriptionContent.transform).GetComponent<WebMapViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is MapServiceItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(mapServiceDescription, descriptionContent.transform).GetComponent<MapServiceViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is KMLPortalItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(kmlServiceDescription, descriptionContent.transform).GetComponent<KMLPortalViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if(item.DataItem is ObjItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(objFileViewDescription, descriptionContent.transform).GetComponent<OBJFileViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if(item.DataItem is DXFItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(dxfFileViewDescription, descriptionContent.transform).GetComponent<DXFFileViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is JOBItem)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(jobFileViewDescription, descriptionContent.transform).GetComponent<JOBFileViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if (item.DataItem is RootItem)
            {
                //arcgis root manage page
                if (item.Name == "ArcGIS Services")
                {
                    descriptionPanel.SetActive(true);
                    var cmp = Instantiate(addArcGISServiceButtons, descriptionContent.transform).GetComponent<AddArcGISServiceUIPanel>();
                    cmp.InitializeDescription(this);
                }
                //my favorite manage page
                if(item.Name == "My Favorite")
                {
                    descriptionPanel.SetActive(true);
                    var cmp = Instantiate(favoriteItemsViewDescription, descriptionContent.transform).GetComponent<FavoriteItemsViewDescription>();
                    cmp.InitializeDescription(item, this);
                }
            }
            else if(item.DataItem is Server)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(serviceDirectoryLoginDescription, descriptionContent.transform).GetComponent<RootViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else if(item.DataItem is Portal)
            {
                descriptionPanel.SetActive(true);
                var cmp = Instantiate(portalLoginDescription, descriptionContent.transform).GetComponent<RootViewDescription>();
                cmp.InitializeDescription(item, this);
            }
            else
            {
                descriptionPanel.SetActive(false);
            }
        }
        private void LoadItemBinding(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            
            //check if the item is loaded
            var viewItem = e.Item as ViewItem;
            if (viewItem == null)
            {
                return;
            }
            if(viewItem.DataItem is Directory || viewItem.DataItem is PortalDirectory || viewItem.DataItem is Portal || viewItem.DataItem is Server || viewItem.DataItem is RootItem)
            {
                viewItem.Loaded = true;
            }
            //setup icon for "My Favorite"
            if (viewItem.DataItem is RootItem)
            {
                //my favorite manage page
                if (viewItem.Name == "My Favorite")
                {
                    e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>().sprite = favoriteStar;
                }
            }

            //change UI for item that is loaded
            if (viewItem.Loaded != viewItem.DataItem.Loaded)
            {
                if (viewItem.DataItem.Loaded) //if a identical item is already loaded in somewhere else/ OR this item is loaded manually
                {
                    LoadViewItem(viewItem,false);
                }
                else // if a identical item is already unloaded in somewhere else
                {
                    RemoveDataItem(viewItem.DataItem);
                    viewItem.Loaded = viewItem.DataItem.Loaded;
                    //unload and change UI if needed
                    for (int i = viewItem.Children.Count - 1; i >= 0; i--)
                    {
                        //remove ViewItems and its children from UI
                        Controller.treeView.RemoveChild(viewItem.Children[i].Parent, viewItem.Children[i]);

                    }
                }
            }

            var text = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<UnityEngine.UI.Text>();

            //turn item to green if it is loaded and exclude portal, local directory since they cannot be loaded. 
            if (viewItem.DataItem.Loaded && !(viewItem.DataItem is Portal) && !(viewItem.DataItem is RootItem) && !(viewItem.DataItem is Server) && !(viewItem.DataItem is PortalDirectory) && !(viewItem.DataItem is Directory))
            {
                text.color = Color.green;
            }
            else
            {
                text.color = Color.white;
            }
        }
        #endregion

        public async Task Refresh()
        {

            //if have portal registered we than add the portal/service directory

            int selectedIndex = fileBrowserController.treeView.SelectedIndex;

            fileBrowserController.ActivePortal(EsriManager.portal_Registered ? true : false);
            fileBrowserController.ActiveServiceDirectory(EsriManager.serviceDirectory_Registered ? true : false);
            var myFavorite = new RootItem("My Favorite");
            myFavorite.Loaded = true;
            var myFavoriteVI = new ViewItem(myFavorite.Name, myFavorite);
            await fileBrowserController.Refresh();
            
            fileBrowserController.itemManager.GetRootItems().Add(myFavorite); // add to root items
            fileBrowserController.itemManager.ViewItems.Add(myFavoriteVI);

            //add "My Favorite"
            //var dataContainer = fileBrowserController.treeView.Add(myFavoriteVI);
            

            //update UI so it includes the favorite items
            fileBrowserController.treeView.Items = fileBrowserController.itemManager.ViewItems;

            //fileBrowserController.treeView.Add(myFavoriteVI);


            /*foreach (var it in LoadedDataItem)
            {
                if (!itemCollection.Contains(it))
                {
                    fileBrowserController.itemManager.AddItem(it);
                }
            }*/
            //scan and toggle items that are loaded
            var listofItem = fileBrowserController.itemManager.GetItems().ToList();
            foreach (var it in listofItem)
            {
                if (LoadedDataItem.Contains(it))
                {
                    await LoadDataItem(it);
                    if(it is IPortalItem && it is IContainerLabelItem)
                    {
                        var childsLayer = (IEnumerable)it.GetType().GetMethod("GetLayers").Invoke(it, null);

                        foreach (var child in childsLayer)
                        {
                            var childItem = child as IBaseItem;
                            if (LoadedDataItem.Contains(childItem))
                            {
                                await LoadDataItem(childItem);
                            }
                        }
                    }
                }
            }
            DestroyFileList();

            //rechoose previous selection
            if(selectedIndex < fileBrowserController.treeView.ItemsCount)
                fileBrowserController.treeView.SelectedIndex = selectedIndex;
            
        }

        #region Item loading/removing functions
        /// <summary>
        /// remove a already loaded viewItem (UI item in treeview), also remove the loaded dataitem behind
        /// </summary>
        /// <param name="vi"></param>
        public void RemoveViewItem(ViewItem vi)
        {
            if (vi.Loaded)
            {
                //remove dataitem and then change the UI
                RemoveDataItem(vi.DataItem);

                if(!vi.DataItem.Loaded)
                    vi.Loaded = false;
                //databinding item that is removed from the storage
                Controller.treeView.DataBindItem(vi);

                // if it is a container item, e.g. directory, feature service
                if (vi.Children != null)
                {
                    //change UI 
                    for (int i = vi.Children.Count - 1; i >= 0; i--)
                    {
                        //remove ViewItems and its children from UI
                        Controller.treeView.RemoveChild(vi.Children[i].Parent, vi.Children[i]);
                    }
                    vi.Children.Clear();
                }

            }
        }

        public void RemoveDataItem(IBaseItem item)
        {
            //use reflection to check if the item has childlayers, then we need nested check every layer
            if (item.Loaded) 
            {
                if (item is IContainerLabelItem)
                {
                    var methodInfo = item.GetType().GetMethod("GetLayers");
                   
                    var result = methodInfo.Invoke(item, null);
                    var childsLayer = (IEnumerable)result;
                    foreach (var child in childsLayer)
                    {
                        var castedChild = child as IBaseItem;
                        if (castedChild.Loaded)
                        {
                            RemoveDataItem(castedChild);
                        }
                    }
                    //we need to ensure that it has GetLayers and ClearLayers method
                    //TODO: implements interface for container object
                    var removeMethodInfo = item.GetType().GetMethod("ClearLayers");
                    removeMethodInfo.Invoke(item, null); //clear all item hiearchey relationship
                    
                }
                else
                {
                    RemoveLoadedItem(item);
                }
                item.Loaded = false;
            }
        }

        //Function that is used to remove any loaded item in the scene(both portal and local)
        //Add more item type if needed
        public async void RemoveLoadedItem(IBaseItem item)
        {
            item.Loaded = false;
            switch (item)
            {
                case KMLPortalItem it:
                    /*******remove this data source from low-level data system********/
                    it.LocalPath = null;
                    break;
                case FeatureLayerItem it:
                    EsriManager.layers2load.RemoveFeatureLayerByUrl(it.URL);
                    EsriManager.layersLoaded.RemoveFeatureLayerByUrl(it.URL);
                    break;
                case KMLItem it:
                    /*******remove this data source from low-level data system********/
                    break;
                case ObjItem it:
                    /*******remove this data source from low-level data system********/
                    break;
                case DXFItem it:
                    /*******remove this data source from low-level data system********/
                    break;
                default:
                    Debug.LogError($"Try to unload Item {item.Name} that is not supported.");
                    return;
            }
        }
        /// <summary>
        /// function that load data item to the scene
        /// </summary>
        /// <param name="item"></param>
        public async Task LoadViewItem(ViewItem item, bool callDatabinding = true)
        {
            await LoadDataItem(item.DataItem);

            //change UI
            if (item.DataItem.Loaded)
            {
                if (item.DataItem is IContainerLabelItem)
                {
                    var methodInfo = item.DataItem.GetType().GetMethod("GetLayers");
                    var result = methodInfo.Invoke(item.DataItem, null);
                    var childsLayer = (IEnumerable)result;
                   
                    //create view item and add to UI
                    item.Children.Clear();
                    //add created children layers to the ViewItem
                    foreach (var childItem in childsLayer)
                    {
                        var castedChildItem = childItem as IBaseItem;
                        var viewIt = new ViewItem(castedChildItem.Name, castedChildItem);
                        item.Children.Add(viewIt);
                        viewIt.Parent = item;
                        
                        //update UI viewTree, and load any layer that need to be load
                        Controller.treeView.AddChild(item, viewIt);
                    }
                    
                }
                //if item.dataitem is loaded then turn viewitem to true
                item.Loaded = true;
                if (callDatabinding) { 
                Controller.treeView.DataBindItem(item);
                }
            } 
            
        }

        async Task LoadDataItem(IBaseItem item)
        {
            if (!item.Loaded)
            {
                switch (item)
                {
                    case FeatureLayerItem it:
                        var featureLayer = await Utilities.GetFeatureLayer(it.URL, it.RequireToken);
                        if (featureLayer != null)
                        {
                            if (!EsriManager.layers2load.ContainsFeatureLayer(it.URL) && !EsriManager.layersLoaded.ContainsFeatureLayer(it.URL))
                            {
                                EsriManager.layers2load.Add(featureLayer);
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case FeatureServiceItem it:
                        FeatureService fs = await Utilities.GetFeatureService(it.URL, it.RequireToken);
                        if (fs == null)
                        {
                            return;
                        }
                        for (int i = 0; i < fs.layers.Length; i++)
                        {
                            var layer = new FeatureLayerItem(fs.layers[i].name, it.URL + "/" + fs.layers[i].id, it.RequireToken);
                            //check if it is already loaded
                            if (Controller.itemManager.ExistItem(layer))
                            {
                                layer = (FeatureLayerItem)Controller.itemManager.GetItem(layer);
                            }
                            it.AddLayer(layer);

                            //add new created item
                            Controller.itemManager.AddItem(layer);
                        }
                        break;
                    case MapServiceItem it:
                       /****implementation for MapService****/
                        break;
                    case WebMapItem it:
                        /****implementation for webmap****/
                        break;
                    case KMLPortalItem it:
                        /****implementation for file****/
                        break;
                    case KMLItem it:
                        /****implementation for file****/
                        break;
                    case ObjItem it:
                        /****implementation for file****/
                        break;
                    case DXFItem it:
                        /****implementation for file****/
                        break;

                }
                item.Loaded = true;
            }
        }
        #endregion

        #region User items(favorite) manipulate functions
        /// <summary>
        /// Functions that manipulate user items/favorite items
        /// </summary>
        public async void LoadFavoriteItems()
        {
            var listOfObject = Controller.itemManager.GetItems().ToList();
            foreach (var it in listOfObject)
            {
                if (UserManager.UserFavoriteItems.Contains(it))
                {
                    await LoadDataItem(it); //wait for loading the feature service
                    LoadedDataItem.Add(it);
                    if(it is IPortalItem && it is IContainerLabelItem)
                    {
                        var childsLayer = (IEnumerable)it.GetType().GetMethod("GetLayers").Invoke(it,null);
                        //check if there is any sub feature layer should be loaded
                        foreach(var child in childsLayer)
                        {
                            var childItem = child as IBaseItem;
                            if (UserManager.UserFavoriteItems.Contains(childItem))
                            {
                                await LoadDataItem(childItem);
                                LoadedDataItem.Add(childItem);
                            }
                        }
                    }
                }
            }
            //refresh the browser
            await Refresh();

            //construct new view of favorite items

        }
        public void AddFavoriteItem(IBaseItem it)
        {
            UserManager.UserFavoriteItems.Add(it);
        }
        public void RemoveFavoriteItem(IBaseItem it)
        {
            UserManager.UserFavoriteItems.Remove(it);
        }
        public void ClearFavoriteItems()
        {
            UserManager.UserFavoriteItems.Clear();
        }
        public void RemoveFromFavoriteItems(IBaseItem it)
        {
            UserManager.UserFavoriteItems.Remove(it);
        }
        public bool ContainFavoriteItem(IBaseItem it)
        {
            return UserManager.UserFavoriteItems.Contains(it);
        }
        public HashSet<IBaseItem> GetFavoriteItems()
        {
            return UserManager.UserFavoriteItems;
        }
        #endregion

        #region ItemBrowser UI functions
        //launch button should also be confirm button for favorite items
        void LaunchButton(IBaseItem dataItem, string fileName)
        {
            //save profile for saving favorite items
            UserManager.SaveUserCredential();
            Debug.Log("Shows the next level menu");
        }
        void CancelButton()
        {
            Debug.Log("Shows the upper level menu");
        }
        
        void DestroyFileList()
        {
            foreach (Transform tr in descriptionContent.transform)
            {
                Destroy(tr.gameObject);
            }
        }
        void OnDestroy()
        {
            LoadedDataItem.Clear();
            //store items that is loaded during this period
            foreach(var it in Controller.itemManager.GetItems())
            {
                if (it.Loaded)
                {
                    LoadedDataItem.Add(it);
                }
            }
            fileBrowserController.treeView.SelectionChanged -= LoadItemSelectionChanged;
            fileBrowserController.treeView.ItemDataBinding -= LoadItemBinding;
            //remove selection
            
        }
        #endregion
    }
}
