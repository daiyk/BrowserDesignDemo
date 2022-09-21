using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Battlehub.UIControls;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    /// <summary>
    /// The controller for filebrowser which controls the whole system's data flow.
    /// </summary>
    [RequireComponent(typeof(ItemManager))]
    public class FileBrowserController : MonoBehaviour
    {
        //view model: thrid-party plug-in
        [SerializeField]
        public VirtualizingTreeView treeView;

        [Tooltip("Ui controller that accompany this file browser")]
        [SerializeField]
        private FileBrowserUIPanel UIControl;

        
        [Tooltip("Items manager that accompany this file browser")]
        [SerializeField]
        public ItemManager itemManager;

        #region Item Icons
        [SerializeField]
        private Sprite folderIcon;

        [SerializeField]
        private Sprite serviceIcon;

        [SerializeField]
        private Sprite defaultIcon;
        
        [SerializeField]
        private Sprite KMLIcon;

        [SerializeField]
        private Sprite esriLayerIcon;

        [SerializeField]
        private Sprite webMapIcon;

        [SerializeField]
        private Sprite featureSericeIcon;
        #endregion
        //switch on/off including portal
        public void ActivePortal(bool active)
        {
            itemManager.AddPortal = active;
        }
        //switch on/off including service directory
        public void ActiveServiceDirectory(bool active)
        {
            itemManager.AddServiceDirectory = active;
        }
        //switch on/off portal's user items loading
        public bool SetLoadPortalUserContent 
        {
            get { return itemManager.SetLoadUserContent; }
            set { itemManager.SetLoadUserContent = value; } 
        }
        //switch on/off portal's public items loading
        public bool SetLoadPortalOrganizationContent
        {
            get { return itemManager.SetLoadOrganizationContent; }
            set { itemManager.SetLoadOrganizationContent = value; }
        }
  
        //switch on/off user input field
        public bool ToggleInputField
        {
            get { return UIControl.inputFieldObject.activeSelf; }
            set { UIControl.inputFieldObject.SetActive(value); }
        }

        public string ConfirmButtonText
        {
            get { return UIControl.confirmButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text; }
            set { UIControl.confirmButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; }
        }

        public string CancelButtonText
        {
            get { return UIControl.cancelButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text; }
            set { UIControl.cancelButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; }
        }

        public string Title
        {
            get { return UIControl.title.text; }
            set { UIControl.title.text = value; }
        }
        //add event listener to button pressed events
        public void AddItemSelectedEventListener(FileBrowserUIPanel.ItemSelectedEventHandler actionHandler)
        {
            UIControl.confirmButtonPressed += actionHandler;
        }

        public void AddItemCancelEventListener(FileBrowserUIPanel.ItemSelectCancelEventHandler actionHandler)
        {
            UIControl.cancelButtonPressed += actionHandler;
        }

        public ViewItem CurrentSelection {
            get
            {
                return UIControl.CurrentSelection;
            }
        }
        private void Start()
        {
            InitializePanel();
        }
        private void OnDestroy()
        {
            ClosePanel();
        }

        /// <summary>
        /// Button of cancel and close the panel
        /// </summary>
        public void ClosePanel()
        {

        }

        public void OnDestory()
        {
            treeView.ItemDataBinding -= OnItemDataBinding;
            treeView.SelectionChanged -= OnItemSelectionChanged;
            treeView.ItemExpanding -= OnItemExpanding;
            treeView.ItemClick -= ToggleExpandCollapse;
            treeView.ItemsRemoved -= OnItemsRemoved;
        }

        public void InitializePanel()
        {
            // subscribe to events
            treeView.ItemDataBinding += OnItemDataBinding;
            treeView.SelectionChanged += OnItemSelectionChanged;
            treeView.ItemExpanding += OnItemExpanding;
            treeView.ItemClick += ToggleExpandCollapse;
            treeView.ItemsRemoved += OnItemsRemoved;
        }


        /// <summary>
        /// Refreshes the content of the UI// build UI elements of the panel
        /// </summary>
        public async Task Refresh()
        {
            if (itemManager != null)
            {
                await itemManager.Initialize();

                //TODO: move viewItem refresh to ItemManager
                //set reference to the UI current selection
                //UIControl.CurrentSelection = UIControl.CurrentSelection;

                List<ViewItem> viewItemRootTree = new List<ViewItem>();
                var items = itemManager.GetRootItems();
                foreach (var item in items)
                {
                    ViewItem parentViewIt = new ViewItem(item.Name, item);
                    var listOfViewIt = itemManager.GetSubViewItemTree(parentViewIt, treeView);
                    if (listOfViewIt != null)
                    {
                        foreach (var childViewItem in listOfViewIt)
                        {
                            parentViewIt.Children.Add(childViewItem);
                            treeView.AddChild(parentViewIt, childViewItem);
                        }
                        viewItemRootTree.Add(parentViewIt);
                    }
                }
                itemManager.ViewItems = viewItemRootTree;
                //initialize treeView and show in UI
                treeView.Items = itemManager.ViewItems;
            }
        }

        private void OnItemDataBinding(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            ViewItem item = (ViewItem)e.Item;

            Text text = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
            text.text = item.Name;

            // Changing icons
            if (item.DataItem is Directory)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = folderIcon;
            }
            else if (item.DataItem is PortalDirectory)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = folderIcon;
            }
            else if (item.DataItem is Portal) //arcgis portal
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = serviceIcon;
            }
            else if(item.DataItem is Server) //arcgis server
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = serviceIcon;
            }
            else if(item.DataItem is FeatureLayerItem)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = esriLayerIcon;
            }
            else if(item.DataItem is FeatureServiceItem)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = featureSericeIcon;
            }
            else if(item.DataItem is WebMapItem)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = webMapIcon;
            }
            else if(item.DataItem is KMLPortalItem)
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = KMLIcon;
            }
            else if(item.DataItem is RootItem)
            {
                //only setup arcgis services icon
                if (item.Name == "ArcGIS Services")
                {
                    Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                    img.sprite = serviceIcon;
                }

            }
            else
            {
                Image img = e.ItemPresenter.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Image>();
                img.sprite = defaultIcon;
            }
            // Notify the tree of the presence of child data items.
            e.HasChildren = item.Children != null && item.Children.Count > 0;
        }

        private void OnItemSelectionChanged(object sender, SelectionChangedArgs e)
        {
            UIControl.CurrentSelection = e.NewItem as ViewItem;
        }
        private void OnItemsRemoved(object sender, ItemsRemovedArgs e)
        {
            for (int i = 0; i < e.Items.Length; ++i)
            {
                ViewItem viewItem = (ViewItem)e.Items[i];
                if (viewItem.Parent != null)
                {
                    viewItem.Parent.Children.Remove(viewItem);
                }
                //.Remove(dataItem);
            }
        }
        private void ToggleExpandCollapse(object sender, ItemArgs e)
        {
            ViewItem item = (ViewItem)e.Items[0];
            treeView.ToggleCollapseExpand(item);
        }

        private void OnItemExpanding(object sender, VirtualizingItemExpandingArgs e)
        {
            //Debug.Log("OnItemExpanding FIRED: " + e.Item.ToString());
            ViewItem item = (ViewItem)e.Item;

            //Return children to the tree view
            e.Children = item.Children;
        }

    }
}
