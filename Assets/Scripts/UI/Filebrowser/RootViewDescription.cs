using UnityEngine;

namespace BrowserDesign.UI {
    public class RootViewDescription : MonoBehaviour
    {
        ItemLoadBrowser itemLoadBrowser;
        ViewItem item;
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
        }

        public void RemovePortalButton(bool forget)
        {
            itemLoadBrowser.Controller.treeView.RemoveSelectedItems();

            //set now portal registration free so user can add new portal
            RemoteServerManager.portal_Registered = false;

            //remove any loaded item
            itemLoadBrowser.RemoveDataItem(item.DataItem);

            //refresh the filebrowser
            itemLoadBrowser.Refresh();

            if (forget)
            {
                /*** Clear user data ***/
            }
        }

        public void RemoveServiceDirectoryButtpn(bool forget)
        {
            itemLoadBrowser.Controller.treeView.RemoveSelectedItems();

            RemoteServerManager.serviceDirectory_Registered = false;

            //remove any loaded item
            itemLoadBrowser.RemoveDataItem(item.DataItem);

            //refresh the filebrowser
            itemLoadBrowser.Refresh();

            if (forget)
            {
                /*** Clear user data ***/
            }
        }
    }
}
