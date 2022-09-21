using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Battlehub.UIControls;
using Utility;
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
    using Windows.Storage;
#endif

namespace BrowserDesign.UI
{
    /// <summary>
    /// Model: database for the file browser
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        //storage for all items
        HashSet<IBaseItem> dataItems = new HashSet<IBaseItem>();
        //UI items that are listed in the filebrowser UI
        public List<ViewItem> ViewItems { get; set; }
        //treeView's root that are managed by this ItemManager
        List<IBaseItem> rootItems = new List<IBaseItem>();


        // file types that are admissiable to be stored in DataItems container
        private Dictionary<string, Type> allowedDataFileTypes = new Dictionary<string, Type>()
        {
            { ".kmz",typeof(KMLItem) },
            { ".kml",typeof(KMLItem) },
            { ".obj",typeof(ObjItem) },
            { ".dxf",typeof(DXFItem) },
            { ".job",typeof(JOBItem) }
        };

        // portal item types that are admissiable to be stored in DataItems container
        private Dictionary<string, Type> allowedPortalTypes = new Dictionary<string, Type>()
        {
            {"KML",typeof(KMLPortalItem) },
            {"Feature Service",typeof(FeatureServiceItem) },
            {"Feature Layer",typeof(FeatureLayerItem) },
            {"Web Map",typeof(WebMapItem) },
            {"Map Service", typeof(MapServiceItem) }

        };

        //data type that are accepted inside the portal web map
        public Dictionary<string, Type> allowedLayerTypesinWebMap = new Dictionary<string, Type>()
        {
            {"ArcGISFeatureLayer",typeof(FeatureLayerItem) },
            {"KML",typeof(KMLPortalItem)},
            {"ArcGISMapServiceLayer",typeof(MapServiceItem) },
        };

        //data item that are accepted on the service directory
        public Dictionary<string, Type> allowedServiceDirectoryTypes = new Dictionary<string, Type>()
        {
            {"FeatureServer",typeof(FeatureServiceItem) },
            {"MapServer",typeof(MapServiceItem) },
        };


        //item types that are visiable in the filebrowser/ stored in ViewItems container
        public List<Type> allowedViewItemTypes = new List<Type>();

        public bool AddPortal { get; set; } = false;

        public bool AddServiceDirectory { get; set; } = false;

        public bool SetLoadUserContent { get; set; } = true;
        public bool SetLoadOrganizationContent { get; set; } = true;

        public void RemoveItem(IBaseItem itemRemove)
        {
            rootItems.Remove(itemRemove);
            dataItems.Remove(itemRemove);
        }
        public void AddItem(IBaseItem itemAdd)
        {
            dataItems.Add(itemAdd);
        }
        //This will add the exist item or reassign the exist item to the parameter item "itemAdd"
        public IBaseItem AddOrExistItem(IBaseItem itemAdd)
        {
            if (dataItems.Contains(itemAdd))
            {
                itemAdd = GetItem(itemAdd);
            }
            else
            {
                dataItems.Add(itemAdd);
            }
            return itemAdd;
        }
        public IBaseItem GetItem(IBaseItem itemSearch)
        {
            foreach(var it in dataItems)
            {
                if (it.Equals(itemSearch))
                {
                    return it;
                }
            }
            return null;
        }
        //check the existence of a item
        public bool ExistItem(IBaseItem item)
        {

            if (dataItems.Contains(item))
            {
                return true;
            }
            else
            {
                return false;
            }

           /* foreach(var it in items)
            {
                if (it.GetType() == item.GetType())
                {
                    if (item.Equals(it))
                    {
                        return true;
                    }
                }
            }
            return false;*/

        }

        //get the items at the root of dataitem hiearchey tree
        public List<IBaseItem> GetRootItems()
        {
            return rootItems;
        }
        //get the set of all dataitem
        public HashSet<IBaseItem> GetItems()
        {
            return dataItems;
        }
        public async Task Initialize()
        {
            rootItems.Clear();
            dataItems.Clear();
            ViewItems = new List<ViewItem>();

            var arcgisRoot = new RootItem("ArcGIS Services");
            arcgisRoot.Loaded = true;
            //1. add local file structure
            Directory localDirs = await GetLocalFileStructure();
            rootItems.Add(localDirs);
            rootItems.Add(arcgisRoot);
            //2. add portal items structure
            if (AddPortal) //if the portal is registered and we want to add portal
            {
                Portal portalDirs = await GetPortalItems();
                arcgisRoot.AddLayer(portalDirs);
                //rootItems.Add(portalDirs);
            }
            //3. add service directory if necessary
            if (AddServiceDirectory) // the server is registered and we want to add server
            {
                Server serviceDirs = await GetServiceDirectoryItems();
                arcgisRoot.AddLayer(serviceDirs);
            }
        }
        

        public async Task<Directory> GetLocalFileStructure()
        {
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
            StorageFolder startingDir = DataFolders.sourceDirWSA;
            // add main dir
            var subItems = await GetSubFileStructureUWPAsync(startingDir);

            //create root directory
            Directory mainDirectory = new Directory(startingDir.Name, DataFolders.defaultSaveLibPath);
            foreach(var it in subItems)
            {
                mainDirectory.AddLayer(it);
            }
#else

            string path = DataFolders.sourceDirIO;
            string dirName = Path.GetFileName(path);

            var localDirStructure = GetSubFileStructure(path);
            // add main dir
            Directory mainDirectory = new Directory(dirName, path, localDirStructure);

            //add to dataItem collection
            AddItem(mainDirectory);

#endif
            return mainDirectory;
        }

        public List<IFileItem> GetSubFileStructure(string directoryPath)
        {
            List<IFileItem> res = new List<IFileItem>();
            DirectoryInfo mainDir = new DirectoryInfo(directoryPath);

            var fileInfos = mainDir.GetFileSystemInfos();
            List<DirectoryInfo> subDirInfos = new List<DirectoryInfo>();
            foreach(var fileInfo in fileInfos)
            {
                if(fileInfo is DirectoryInfo)
                {
                    List<IFileItem> subDirsList = GetSubFileStructure(fileInfo.FullName);
                    var dir = new Directory(fileInfo.Name, fileInfo.FullName, subDirsList);
                    //add to the dataitem collections
                    AddItem(dir);
                    res.Add(dir);
                }
                if(fileInfo is FileInfo)
                {
                    if(allowedDataFileTypes.ContainsKey(fileInfo.Extension))
                    {
                        var obj = Activator.CreateInstance(allowedDataFileTypes[fileInfo.Extension], new object[] { fileInfo.Name, fileInfo.FullName });
                        //add to dataItems collections
                        AddItem((IFileItem)obj);
                        res.Add((IFileItem)obj);
                    }
                }
            }
            return res;
        }



#if UNITY_WSA && ENABLE_WINMD_SUPPORT
        public async Task<List<IFileItem>> GetSubFileStructureUWPAsync(StorageFolder parentFolder)
        {
            List<IFileItem> res = new List<IFileItem>();

            // get subFiles and subFolders
            IReadOnlyList<IStorageItem> subItems = await parentFolder.GetItemsAsync();

            foreach(var storageItem in subItems)
            {
                if(storageItem.IsOfType(StorageItemTypes.Folder))
                {
                    List<IFileItem> subDirsList = await GetSubFileStructureUWPAsync((StorageFolder)storageItem);
                    var dir = new Directory(storageItem.Name,storageItem.Path,subDirsList);
                    res.Add(dir);
                    AddItem(dir);

                }
                if(storageItem.IsOfType(StorageItemTypes.File))
                {
                    var fileItem = (StorageFile)storageItem;
                    if(allowedDataFileTypes.ContainsKey(fileItem.FileType))
                    {
                        var obj = Activator.CreateInstance(allowedDataFileTypes[fileItem.FileType], new object[] { fileItem.Name, fileItem.Path });
                        res.Add((IFileItem)obj);
                        AddItem((IFileItem)obj);
                    }
               }
            }
            return res;
        }
#endif

        /// <summary>
        /// Get the item tree from the portal
        /// The current implementation will use credential from the esrimanager
        /// </summary>
        /// <returns></returns>
        public async Task<Portal> GetPortalItems()
        {
            //Get myContent portal items
            //UserContent uc = await Managers.esriUtils.GetUserContent(EsriManager.portal_userName, await EsriManager.GetToken());
            //create portal root
            //current portal path
            string base_uri = @"https://" + EsriManager.portal_Domain + @"/sharing/rest/content/users";
            string portal_url = base_uri + $"/{EsriManager.portal_userName}";
            Portal currentPortal = new Portal(EsriManager.portal_userName, portal_url);

            AddItem(currentPortal);
            return currentPortal;
        }

        private async Task<Server> GetServiceDirectoryItems()
        {
            string serviceDirectoryURL = "https://" + EsriManager.serviceDirectory_URL;
            var sdUri = new Uri(serviceDirectoryURL);
            //build service directorty root 
            var sdRoot = new PortalDirectory(sdUri.Host, serviceDirectoryURL, false);
            var newServer = new Server(await GetSubServiceDirectoryTree(sdRoot));
            AddItem(newServer);
            return newServer;
        }

        private async Task<PortalDirectory> GetSubServiceDirectoryTree(PortalDirectory pd)
        {
            var sd = await Utilities.GetServiceDirectory(pd.URL);
            if (sd.error!=null)
            {
                Debug.Log($"{pd.Name} loading failed: {sd.error.message}");
                return pd;

            }
            if (sd != null)
            {
                foreach (var folder in sd.folders)
                {
                    //create new item and add to the child of current portal directory
                    var childPd = new PortalDirectory(folder, pd.URL + "/" + folder,false);
                    childPd = await GetSubServiceDirectoryTree(childPd);
                    pd.AddLayer(childPd);
                    AddItem(childPd);
                }
                foreach (var service in sd.services)
                {
                    if (allowedServiceDirectoryTypes.ContainsKey(service.type))
                    {
                        IPortalItem childService;
                        if (service.url != null)
                        {
                            childService = (IPortalItem)Activator.CreateInstance(allowedServiceDirectoryTypes[service.type], new object[] { service.name, service.url, false });
                        }
                        else
                        {
                            //create uri
                            string absoluteUri = pd.URL;
                            string relativeUri = service.name;
                            //create url from relative path
                            if (!absoluteUri.EndsWith("/"))
                            {
                                absoluteUri = absoluteUri + "/";
                            }
                            var serviceUri = absoluteUri + relativeUri.Split('/').Last();

                            //service url should be the serviceUri+service.type(e.g.MapServr, FeatureServer)
                            childService = (IPortalItem)Activator.CreateInstance(allowedServiceDirectoryTypes[service.type], new object[] { service.name, serviceUri.ToString() + "/" + service.type, false });
                        }

                        childService = (IPortalItem)AddOrExistItem(childService);
                        pd.AddLayer(childService);
                    }
                }
            }
            return pd;
        }

        //build viewItem tree for the provided viewItem, and filter by the allowedViewItem
        //returns
        // 1. null for unsupported item
        // 2. empty if item is not a directory
        // 3. List of viewItem if item is a directory
        // Update this function to include more data types for UI element
        public List<ViewItem> GetSubViewItemTree(ViewItem item, VirtualizingTreeView treeView)
        {
            //if it is not in the filter then  return null
            if (allowedViewItemTypes.Count > 0 && !allowedViewItemTypes.Contains(item.DataItem.GetType()))
            {
                return null;
            }

            List<ViewItem> viewItemTree = new List<ViewItem>();


            //inner function for reuse the code
            void AddViewItemChild(IBaseItem it, Type folderType)
            {
                if (allowedViewItemTypes.Count==0||allowedViewItemTypes.Contains(it.GetType()))
                {
                    ViewItem viewIt = new ViewItem(it.Name, it);
                    if (it.GetType() == folderType)
                    {
                        var listOfChilds = GetSubViewItemTree(viewIt, treeView);
                        foreach (var child in listOfChilds)
                        {
                            viewIt.Children.Add(child);
                            treeView.AddChild(viewIt, child);
                        }
                    }
                    viewIt.Parent = item;
                    viewItemTree.Add(viewIt);
                }
            }


            if(item.DataItem is Directory)
            {
                foreach(var it in (item.DataItem as Directory).GetLayers())
                {
                    AddViewItemChild(it,typeof(Directory));                   
                }
            }
            if(item.DataItem is PortalDirectory)
            {
                foreach(var it in (item.DataItem as PortalDirectory).GetLayers())
                {
                    AddViewItemChild(it, typeof(PortalDirectory));
                }
            }
            if(item.DataItem is Portal)
            {
                foreach (var it in (item.DataItem as Portal).GetLayers())
                {
                    AddViewItemChild(it,typeof(PortalDirectory));    
                }
            }
            if (item.DataItem is Server) 
            {
                foreach (var it in (item.DataItem as Server).GetLayers())
                {
                    AddViewItemChild(it, typeof(PortalDirectory));
                }
            }
            if(item.DataItem is RootItem)
            {
                foreach(var it in (item.DataItem as RootItem).GetLayers())
                {
                    AddViewItemChild(it, it.GetType());
                }
            }
            return viewItemTree;
        }

        

    }
}
