using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utility;
public class AddServiceDirectoryUIPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_serviceURL;
    [SerializeField]
    private Toggle rememberMe;
    [SerializeField]
    private GameObject m_MessageBar;
    //Event arise when click the confirm button
    public event AddServiceDirectoryDelegate serviceDirectoryLoginEvent;
    //define the event that transfer the service directory login information
    public delegate void AddServiceDirectoryDelegate(string serviceDirectoryURL, bool rememberMe);

    void OnEnable()
    {
        InitializePanel();
    }

    public void InitializePanel()
    {
        //check if there are already service directory registered
        if(EsriManager.serviceDirectory_URL != null)
        {
            m_serviceURL.text = EsriManager.serviceDirectory_URL;
        }
    }

    public async void ConfirmButton()
    {
        if (m_serviceURL.text == "")
        {
            m_MessageBar.SetActive(true);
            m_MessageBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Error: Service Address cannot be empty!");
        }
        else
        {
            //add service directory url to the esrimanager and refresh the filebrowser
            var serviceDirectoryURL = m_serviceURL.text;
            serviceDirectoryURL = serviceDirectoryURL.Replace(" ", string.Empty);
            if (serviceDirectoryURL.StartsWith("https://"))
            {
                serviceDirectoryURL = serviceDirectoryURL.Replace("https://", string.Empty);
            }
            else if (serviceDirectoryURL.StartsWith("http://"))
            {
                serviceDirectoryURL = serviceDirectoryURL.Replace("http://", string.Empty);
            }
            if (serviceDirectoryURL.EndsWith("/"))
            {
                serviceDirectoryURL = serviceDirectoryURL.Remove(serviceDirectoryURL.Length - 1);
            }
            m_MessageBar.SetActive(true);
            m_MessageBar.GetComponent<TextMeshProUGUI>().text = LanguageManager.Translate("Service Directory is loading, please wait...");
            var result = await Utilities.GetServiceDirectory(serviceDirectoryURL);

            if (result.error != null)
            {
                m_MessageBar.GetComponent<TextMeshProUGUI>().text = $"Login Failed: {result.error.message}";
                return;
            }
            if (serviceDirectoryLoginEvent != null)
            {
                serviceDirectoryLoginEvent?.Invoke(serviceDirectoryURL, rememberMe.isOn);
            }
        }
    }
    public void ClosePanel()
    {
        Destroy(gameObject);
    }

    void OnDestory()
    {
        //unsubscribe all listener
        if (serviceDirectoryLoginEvent != null)
            foreach (var d in serviceDirectoryLoginEvent.GetInvocationList())
                serviceDirectoryLoginEvent -= (d as AddServiceDirectoryDelegate);
    }

}
