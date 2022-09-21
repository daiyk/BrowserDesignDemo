using System;
using System.Collections.Generic;
using UnityEngine;
using BrowserDesign.API;
using System.Threading.Tasks;
/// <summary>
/// Remote Server manager, this manages all relevant parameters for loading/unloading remote resources 
/// </summary>
public class RemoteServerManager : MonoBehaviour
{
    /// <summary>
    /// Contains the username/parsed 
    /// </summary>
    #region Portal
    public static string portal_Domain { get; set; }
    public static string portal_userName { get; set; }
    public static string portal_passWord { get; set; }
    public static string portal_Token { get; set; }

    public static bool IsOauth2Login { get; set; }

    public static string oauth2_clientId { get; set; } //client id used to connect to the portal

    public static int token_expiration { get; set; } = 180; //expiration time in minutes, some will always expires in 30 minutes
    public static string portal_Directory { get; set; } //local storage path for storing data downloaded from portal
    public static bool portal_Registered { get; set; } = false;
    public static DateTime expirationDate;//token expiration date


    /// <summary>
    /// Generate token by sending request, non-oauth2 or oauth2
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
    #endregion

    #region Service Directory
    public static bool serviceDirectory_Registered;
    public static string serviceDirectory_URL;
    #endregion

    /// <summary>
    /// List of layers that have been selected but still not loaded
    /// </summary>
    public static List<FeatureLayer> layers2load = new List<FeatureLayer>();

    /// <summary>
    /// List of layers that have been loaded
    /// </summary>
    public static List<FeatureLayer> layersLoaded = new List<FeatureLayer>();



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
        /*******Other remote login module*******/
        Tuple<bool, string> portalResult = new Tuple<bool,string>(false, "NonOauth2");
        //test whether portal add successful
        return portalResult;
       
    }
    
}
