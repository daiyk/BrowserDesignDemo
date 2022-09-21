using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrowserDesign.UI
{
    /// <summary>
    /// UI Script for managing arcgis portal login
    /// </summary>
    public class AddPortalUIPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleField;
        [SerializeField]
        private Button loginBTN;
        [Tooltip("The server url that host your portal service")]
        [SerializeField]
        private TMP_InputField url;
        [SerializeField]
        private TMP_InputField userName;
        [SerializeField]
        private TMP_InputField passWord;
        [SerializeField]
        private Image internetIcon;
        [SerializeField]
        private Toggle rememberMe;
        [SerializeField]
        private Toggle oauth2;
        [SerializeField]
        private GameObject messageBar;

        [SerializeField]
        private Image usernameBackground;
        [SerializeField]
        private Image passwordBackground;
        [SerializeField]
        private Image hostBackground;
        [SerializeField]
        private Sprite ShowPasswordIcon;
        [SerializeField]
        private Sprite HidePasswordIcon;

        bool loginProcess = false;
        bool hasInternet = false;
        /// <summary>
        /// Checks if we should enable the login button (when fields are populated)
        /// </summary>
        void Update()
        {
            if (hasInternet)
            {
                if (oauth2.isOn && userName.text != "" && !loginProcess)
                {
                    loginBTN.interactable = true;
                }
                else if (!oauth2.isOn && userName.text != "" && passWord.text != "" && !loginProcess)
                    loginBTN.interactable = true;
                else
                    loginBTN.interactable = false;
            
                if (oauth2.isOn)
                {
                    passWord.interactable = false;
                    url.interactable = false;
                }
                else
                {
                    passWord.interactable = true;
                    url.interactable= true;
                }
                userName.interactable = true;
            }
            else
            {
                loginBTN.interactable = false;
                userName.interactable = false;
                passWord.interactable = false;
                url.interactable = false;
            }
        }

        //Event arise when click the confirm button
        public event AddPortalDelegate portalLoginEvent;
        //define the event that transfer the service directory login information
        public delegate void AddPortalDelegate(string portalURL, bool rememberMe);

        private void OnEnable()
        {
            InitializePanel();
        }
        private void OnDisable()
        {
            ClosePanel();
        }
        public void InitializePanel()
        {

            loginBTN.interactable = false;
            messageBar.SetActive(false);
            url.text = "";
            userName.text = "";
            passWord.text = "";

            //initialize deeplink listener
            Application.deepLinkActivated += DeepLinkActivated;

            /***Load user profile from the local/remote****/

            CheckInternetConnection();
            // start searching internet
            InvokeRepeating("CheckInternet", 0.0f, 3.0f);
            InvokeRepeating("CheckInternetConnection", 0.5f, 2.0f);
        }
        public void ClosePanel()
        {
            //delete deeplink listener 
            Application.deepLinkActivated -= DeepLinkActivated;
        }
        /// <summary>
        /// Toggle password field if Oauth2 is enabled
        /// </summary>
        /// <param name="check"></param>
        public void ToggleOauth2()
        {
            if (oauth2.isOn)
            {
                passWord.interactable = false;
            }
            else
            {
                passWord.interactable = true;
            }
        }
        /// <summary>
        /// used for OAUTh2 Deep linked method that is called by redirect url, when login successful through web browser by oauth2 
        /// </summary>
        /// <param name="url"></param>
        void DeepLinkActivated(string url)
        {
            //process the deeplink
            //Deep Link processed
        }

        /// <summary>
        /// Checks if we should enable the login button (when fields are populated)
        /// </summary>
        public void CheckIfEnableLoginButton()
        {
            if (userName.text != "" && passWord.text != "")
                loginBTN.interactable = true;
            else
                loginBTN.interactable = false;
        }

        /// <summary>
        /// OnClick function for online portal login UI button. This checks if the service is valid
        /// </summary>
        public async void BTN_SaveESRI()
        {

            usernameBackground.GetComponent<Image>().color = Color.white;
            passwordBackground.GetComponent<Image>().color = Color.white;
            hostBackground.GetComponent<Image>().color = Color.white;
            loginProcess = true;
         
            messageBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("login in progress, please wait.....");
            messageBar.SetActive(true);
            // TODO: implement wanings when url, user and pass are wrong

            // it call UI to close the add esriloginPanel
            //AddValidEsriFeatureService(featureService);*/
            //records esri credentials
            if (url.text == "")
            {
                RemoteServerManager.portal_Domain = $"www.arcgis.com";
            }
            else
            {
                //remove blank space and remove slash at the end
                url.text = url.text.Replace(" ", string.Empty);
                if (url.text.StartsWith("https://"))
                {
                    url.text = url.text.Replace("https://", string.Empty);
                }
                else if (url.text.StartsWith("http://"))
                {
                    url.text = url.text.Replace("http://", string.Empty);
                }
                if (url.text.EndsWith("/"))
                {
                    url.text = url.text.Remove(url.text.Length - 1);
                }
                RemoteServerManager.portal_Domain = url.text;
            }
            RemoteServerManager.IsOauth2Login = oauth2.isOn;
            RemoteServerManager.portal_userName = userName.text;
            RemoteServerManager.portal_passWord = passWord.text;

            //login use the provided credentials
            var logResult = await RemoteServerManager.AttemptLogin();
            if (logResult.Item1)
            {
                CancelInvoke("CheckInternetConnection");
                if (logResult.Item2 == "NonOauth2" || logResult.Item2 == "Oauth2Refresh")
                {
                    RemoteServerManager.portal_Registered = true;
                    messageBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Authentication success! please wait for the app to refresh...");
                   
                    portalLoginEvent?.Invoke(url.text, rememberMe.isOn);
                }
           
                /*** Process other situation of login to portal ***/
                
                
            }
            else
            {

                Debug.LogError($"Portal: {userName.text} try to login but failed: {logResult.Item2}.");
                RemoteServerManager.portal_Registered = false;
                loginProcess = false;
                messageBar.SetActive(true);
                messageBar.GetComponent<TextMeshProUGUI>().text = logResult.Item2;
                usernameBackground.GetComponent<Image>().color = Color.red;
                passwordBackground.GetComponent<Image>().color = Color.red;
                hostBackground.GetComponent<Image>().color = Color.red;
            }
        }

        /// <summary>
        /// check internet and enable the field only if internet is connected
        /// </summary>
        private void CheckInternetConnection()
        {
            // check internet button
            if (hasInternet)
            {
                internetIcon.color = Color.green;
                // enable fields
                userName.interactable = true;
                passWord.interactable = true;
                url.interactable = true;
                hasInternet = true;
                titleField.text = LanguageManager.Translate("Remote Portal Sign in");
                titleField.color = Color.white;

            }
            else
            {
                internetIcon.color = Color.red;
                // show the message that we need the connection
                userName.interactable = false;
                passWord.interactable = false;
                url.interactable = false;
                hasInternet = false;
                titleField.text = LanguageManager.Translate("Internet Connection Failed");
                titleField.color = Color.red;
            }
        }

        private void CheckInternet()
        {
            StartCoroutine(CheckInternetConnection((hasConnection) =>
            {
                if (hasConnection)
                    hasInternet = true;
                else
                    hasInternet = false;
            }));
        }
        IEnumerator CheckInternetConnection(Action<bool> action)
        {
            WWW www = new WWW("http://bing.com");
            yield return www;
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }
        public void BTN_Cancel()
        {
            Destroy(gameObject);
        }
    }
}

