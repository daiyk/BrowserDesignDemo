using System;
using System.Collections.Generic;
using UnityEngine;
using BrowserDesign.Extension;
using System.Threading.Tasks;
using System.Linq;
using BrowserDesign.UI;
/// <summary>
/// EsriManager, this manages all relevant parameters for loading/unloading remote resources 
/// </summary>
public class EsriManager : MonoBehaviour
{
    /// <summary>
    /// Contains the username/parsed 
    /// </summary>
    #region Esri_Portal
    public static string portal_Domain { get; set; }
    public static string portal_userName { get; set; }
    public static string portal_passWord { get; set; }
    public static string portal_Token { get; set; }

    public static bool IsOauth2Login { get; set; }

    //token expiration time, default to 3 hours
    //public static string oauth2_clientId { get; set; } = "f1VKEtFqxpY8h1yN"; //clientid for vlabs arcgis online
    //public static string oauth2_clientId { get; set; } = "sN4b5FVA1rQoLXGS"; //clientid for brabentwater arcgis online
    public static string oauth2_clientId { get; set; } //client id used to connect to the portal
    //deeplink rediect_url, must be equal to the redirect_url registered on the portal AND match the protocol name in unity setting
    public static string oauth2_redirectUrl { get; set; } = "argosxr://auth";
    public static string oauth2_authenticationCode { get; set; }
    public static string oauth2_refreshToken { get; set; }

    // the time when refresh token expires, need to check this before login
    public static string oauth2_refreshTokenExpiresAt { get; set; }
    public static int token_expiration { get; set; } = 180; //expiration time in minutes, some will always expires in 30 minutes
    public static string portal_Directory { get; set; } //local storage path for storing data downloaded from portal
    public static bool portal_Registered { get; set; } = false;
    public static DateTime expirationDate;//token expiration date

    public static HashSet<IBaseItem> LoadedDataItem = new HashSet<IBaseItem>(); // contains loaded item in the scene

    public static Dictionary<string, WebMap> knownWebMaps = new Dictionary<string, WebMap>();


    /// <summary>
    /// Generate token by sending request, non-oauth2 only
    /// </summary>
    /// <returns></returns>
    public static async Task<string> GetToken()
    {
        if (portal_Token == null || DateTime.UtcNow > expirationDate)
        {
            if (IsOauth2Login)//oauth2 login
            {
                /**********Oauth2Login process**************/

                /**********Oauth2Login process**************/
            }
            else //non-oauth2 login
            {
               /***Non-auth2login****/
            }
        }
        return portal_Token;
    }
    /// <summary>
    /// Adds the webmap to the lit of valid webmap container
    /// </summary>
    public static void AddWebMap(WebMap wm)
    {
        if (wm.serviceName == "null")
        {
            wm.serviceName = "ESRI_" + esriCounter.ToString();
            esriCounter += 1;
        }

        // check if already added
        if (knownWebMaps.ContainsKey(wm.serviceName))
        {
            //Debug.Log("SERVICE IS ALREADY ADDED");
            return;
        }
        // add it
        knownWebMaps.Add(wm.serviceName, wm);
    }
    #endregion

    #region Service Directory
    public static bool serviceDirectory_Registered;
    public static string serviceDirectory_URL;
    #endregion

    /// <summary>
    /// Contains all the valid/parsed WebMaps that have been added: [name of the service:webmap name]
    /// </summary>
    public static Dictionary<string, FeatureService> knownFeatureServices = new Dictionary<string, FeatureService>();

    /// <summary>
    /// List of layers that have been selected but still not loaded
    /// </summary>
    public static List<FeatureLayer> layers2load = new List<FeatureLayer>();

    /// <summary>
    /// List of layers that have been loaded
    /// </summary>
    public static List<FeatureLayer> layersLoaded = new List<FeatureLayer>();


    /// <summary>
    /// just a counter for naming thing without a name!
    /// </summary>
    private static int esriCounter = 1;

    /// <summary>
    /// Adds the fs to the lit of valid fs's
    /// </summary>
    public static void AddFeatureService(FeatureService fs2add)
    {
        if (fs2add.serviceName == "null")
        {
            fs2add.serviceName = "ESRI_" + esriCounter.ToString();
            esriCounter += 1;
        }

        // check if already added
        if (knownFeatureServices.ContainsKey(fs2add.serviceName))
        {
            //Debug.Log("SERVICE IS ALREADY ADDED");
            return;
        }

        // add it
        knownFeatureServices.Add(fs2add.serviceName, fs2add);
    }

    public async static Task<Tuple<bool, string>> AttemptLogin()
    {
        if (portal_Registered)
        {
            return new Tuple<bool, string>(false, "a portal is already registered!");
        }

        if (!IsOauth2Login && (portal_userName == null || portal_passWord == null || portal_Domain == null))
        {
            return new Tuple<bool, string>(false, "Non-oauth2 requires all input fields (username and password) not empty!");
        }
        
        if(IsOauth2Login&&(oauth2_clientId==null || portal_Domain == null))
        {
            return new Tuple<bool, string>(false, "oauth2 set-up is not complete, some of required configurations(e.g. clientid and redirect_url) are missing! ");
        }
        /*******Other EsriLogin module*******/
        Tuple<bool, string> portalResult = new Tuple<bool,string>(false, "NonOauth2");
        //test whether portal add successful
        return portalResult;
       
    }

    //login via oauth2, open web browser for authentication, REMEMBER to subscribe Application.deepLinkActivated to receive response, see EsriAddPanel.deepLinkActivated.
    public static bool AttempOauthLogin(string url)
    {
        if (oauth2_clientId != null)
        {
            //build oauth2 end-point
            url += "/sharing/rest/oauth2/authorize";
            //build url
            string fullUrl = "https://"+url + "?" + $"client_id={oauth2_clientId}" + $"&redirect_uri={oauth2_redirectUrl}"+"&response_type=code";

#if UNITY_WSA
            UnityEngine.WSA.Launcher.LaunchUri(fullUrl, false);
#else
            Application.OpenURL(fullUrl);
#endif
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
