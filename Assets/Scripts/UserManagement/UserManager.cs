using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BrowserDesign.UI;
/// <summary>
/// Handles the active user
/// </summary>
namespace BrowserDesign.users
{
    public class UserManager : MonoBehaviour
    {
        /// <summary>
        /// Connection token
        /// </summary>
        public static string token { get; set; }

        static HttpClient myClient = new HttpClient();

        /// <summary>
        /// path to the local profile folder
        /// </summary>
        private static string localProfile;

        /// <summary>
        /// Name for the local profile
        /// </summary>
        private static string localProfileName = "profile.json";

        /// <summary>
        /// The profile object for storing user data and setting
        /// </summary>
        public static Profile profile = new Profile();
        /// <summary>
        /// The data types that can be saved for quick loading
        /// </summary>
        public static Dictionary<string, Type> allowedUserItemTypes = new Dictionary<string, Type>()
        {
            {"KMLFile",typeof(KMLItem) },
            {"KMLPortal",typeof(KMLPortalItem) },
            {"FeatureService",typeof(FeatureServiceItem) },
            {"FeatureLayer",typeof(FeatureLayerItem) },
            {"WebMap",typeof(WebMapItem) },
            {"MapService", typeof(MapServiceItem) },
            {"ObjFile",typeof(ObjItem) },
            {"DXFFile",typeof(DXFItem) }
        };
        /// <summary>
        /// DataItems that user loaded and can be loaded in the next login
        /// </summary>
        public static HashSet<IBaseItem> UserFavoriteItems = new HashSet<IBaseItem>();

        private static User ActiveUser;
        /// <summary>
        /// This represent the current logged in user. If not connected null
        /// </summary>
        public static User activeUser
        {
            //get { return profile.SavedUsers[0]; }
            get
            {
                if (isOfflineLogin)
                    return ActiveUser;
                else
                    return profile.SavedUsers.Count > 0 ? profile.SavedUsers[0] : null;
            }
            private set { ActiveUser = value; }
        }

        /// <summary>
        /// Workaround to skip authenitcation
        /// </summary>
        public static string passwordOfflineCH = "6262da00ch";
        public static string passwordOfflineDE = "6262da00de";
        public static string passwordOfflineNL = "6262da00nl";
        public static string passwordOfflineAU = "6262da00au";
        public static bool isOfflineLogin = false;

        /// <summary>
        /// return the default local profile saving path
        /// </summary>
        public static string DefaultLocalProfilePath
        {
            get { return localProfile; }
        }

        public static async Task Init()
        {
            //find path to the profile!
#if UNITY_EDITOR
            localProfile = Path.Combine(DataFolders.sourceDirIO, localProfileName);
#else
            localProfile = Path.Combine(DataFolders.sourceDirIO.Path, localProfileName);
#endif
            profile.SavedUsers = new List<User>();
            profile.LastSaved = "";

            await LoadLocalProfileData();
            LoadUserItems();
        }

        public static User GetUserCredential(string user)
        {
            return profile.SavedUsers.FirstOrDefault(c => c.Name == user);
        }

        /// <summary>
        /// Read user credential from the local file
        /// </summary>
        public static async Task<bool> LoadLocalProfileData()
        {
            //check the existence and retrieve key information
            if (await DataIO.IsFileExistInSourceFolder(localProfile))
            {
                //if exist read file from the disk
                profile = await DataIO.ReadJsonFromFile<Profile>(localProfile);
                if (profile.SavedUsers.Count > 0)
                {
                    Debug.Log("DebugLog: loading file successful");
                    return true;
                }
                else
                {
                    Debug.LogError("Error: local user profile exist but load failed, it may due to the wrong format or damaged user file");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// copy esri login information from EsriManager to the user profile
        /// </summary>
        public static void LoadEsriPortalCredential()
        {
            /****connect to usermanager module to load user information from storage******/

        }

        public static void ClearPortalCredential()
        {
            if (UserManager.profile.SavedUsers.Count > 0)
            {
                UserManager.profile.SavedUsers.Clear();
                UserManager.profile.SavedUsers.Add(new User());
                UserManager.profile.LastSaved = DateTime.Now.ToString("h:mm:ss tt");
                ClearEsriCredential();
            }
        }

        public static void ClearServiceDirectoryCredential()
        {
            if (UserManager.profile.SavedUsers.Count > 0)
            {
                UserManager.profile.LastSaved = DateTime.Now.ToString("h:mm:ss tt");
                UserManager.profile.SavedUsers[0].ServiceDirectoryURL = "";
            }
        }

        /// <summary>
        /// Save profile data to the local storage
        /// </summary>
        public async static Task SaveUserCredential()
        {
            if (profile.SavedUsers.Count > 0)
            {
                //write down user favorite items
                if (UserFavoriteItems.Count > 0)
                {
                    LoadUserItemsToProfile();
                }
                var result = await DataIO.SaveJsonToFile(localProfile, profile).ConfigureAwait(false);
                if (!result)
                {
                    Debug.LogError("SaveUserCredential: Save local user profile failed!");
                }
            }
            //load user favorite item to profile

        }

        public static void LoadUserItems()
        {
            if (profile.SavedUsers.Count > 0)
            {
                if (profile.SavedUsers[0].userItems == null)
                {
                    profile.SavedUsers[0].userItems = new List<UserItem>();
                }
                foreach (var it in profile.SavedUsers[0].userItems)
                {
                    if (allowedUserItemTypes.ContainsKey(it.Type))
                    {
                        //create user selected dataitem from profile
                        var item = (IBaseItem)Activator.CreateInstance(allowedUserItemTypes[it.Type], new object[] { it.Name, it.Resource });
                        //add to loaded item
                        UserFavoriteItems.Add(item);
                    }
                }
            }
        }

        public static void LoadUserItemsToProfile()
        {
            //clear user items
            profile.SavedUsers[0].userItems.Clear();
            foreach (var item in UserFavoriteItems)
            {
                switch (item)
                {
                    case KMLItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.Path, "KMLFile"));
                        break;
                    case ObjItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.Path, "DXFFile"));
                        break;
                    case DXFItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.Path, "ObjFile"));
                        break;
                    case KMLPortalItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.URL, "KMLPortal"));
                        break;
                    case FeatureLayerItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.URL, "FeatureLayer"));
                        break;
                    case FeatureServiceItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.URL, "FeatureService"));
                        break;
                    case MapServiceItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.URL, "MapService"));
                        break;
                    case WebMapItem it:
                        profile.SavedUsers[0].userItems.Add(new UserItem(it.Name, it.URL, "WebMap"));
                        break;
                }

            }
        }



        private static void ClearEsriCredential()
        {
            UserManager.profile.SavedUsers[0].EsriDomain = "";
            UserManager.profile.SavedUsers[0].EsriName = "";
            UserManager.profile.SavedUsers[0].Encryption = "";
            UserManager.profile.SavedUsers[0].EncryptionVec = "";
            UserManager.profile.SavedUsers[0].refreshToken = null;
            UserManager.profile.SavedUsers[0].Oauth2ExpiresAt = null;
        }
    }
}
