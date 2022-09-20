using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrowserDesign.UI;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    enum Choice
    {
        BaseFileBrowser,
        FileLoader
    }
    [SerializeField]
    private Choice choice;
    [SerializeField]
    private GameObject browserController;
    [SerializeField]
    private GameObject itemLoader;
    
    void Start()
    {
        if (choice == Choice.BaseFileBrowser)
        {
            var browserControllerObj = Instantiate(browserController, gameObject.transform.parent.transform);
            var fileBrowserController = browserControllerObj.GetComponent<FileBrowserController>();
            fileBrowserController.ActiveServiceDirectory(EsriManager.serviceDirectory_Registered ? true : false);
            fileBrowserController.InitializePanel();
            ////add restrict condition
            //if (!fileBrowserController.itemManager.allowedViewItemTypes.Contains(typeof(BrowserDesign.UI.Directory)))
            //{
            //    fileBrowserController.itemManager.allowedViewItemTypes.Add(typeof(BrowserDesign.UI.Directory));
            //}
            fileBrowserController.Refresh();
            fileBrowserController.treeView.Items = fileBrowserController.itemManager.ViewItems;
        }
        else if(choice == Choice.FileLoader)
        {
            var browserControllerObj = Instantiate(itemLoader, gameObject.transform.parent.transform);
        }
    }
}
