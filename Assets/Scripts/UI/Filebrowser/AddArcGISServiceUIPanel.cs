using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrowserDesign.UI;
using UnityEngine.UI;
using TMPro;

public class AddArcGISServiceUIPanel : MonoBehaviour
{

    [SerializeField]
    private Button m_portalButton;
    [SerializeField]
    private TextMeshProUGUI m_portalSummary;
    [SerializeField]
    private Button m_serviceDirectory;
    [SerializeField]
    private TextMeshProUGUI m_serviceSummary;
    [SerializeField]
    private GameObject m_portalLoginPanelObject;
    [SerializeField]
    private GameObject m_serviceDirectoryLoginPanelObject;

    private GameObject m_serverLoginObj;
    private GameObject m_portalLoginObj;

    private ItemLoadBrowser controller;
    public void InitializePanel()
    {
        m_portalSummary.text = "";
        m_serviceSummary.text = "";
        if (RemoteServerManager.portal_Registered)
        {
            m_portalButton.interactable = false;
            m_portalSummary.text = LanguageManager.Translate("A portal is already registered, please log out the current portal before new login");
        }
        else
        {
            m_portalButton.interactable = true;
        }
        if (RemoteServerManager.serviceDirectory_Registered)
        {
            m_serviceDirectory.interactable = false;
            m_serviceSummary.text = LanguageManager.Translate("A service directory is already registered, please log out the current service directory before new login");
        }
        else
        {
            m_serviceDirectory.interactable = true;
        }

        

    }

    public void ConfirmButton_AddPortal()
    {
        m_portalLoginObj = Instantiate(m_portalLoginPanelObject, controller.gameObject.transform);
        //subscribe to add portal event
        m_portalLoginObj.GetComponent<AddPortalUIPanel>().portalLoginEvent += addPortalEventListener;
    }
    public void ConfirmButton_AddServiceDirectory()
    {
        //create object and add to the main screen
        m_serverLoginObj = Instantiate(m_serviceDirectoryLoginPanelObject, controller.gameObject.transform);
        //subscribe to add service directory event
        m_serverLoginObj.GetComponent<AddServiceDirectoryUIPanel>().serviceDirectoryLoginEvent += addServiceDirectoryEventListener;
    }
    public async void addPortalEventListener(string serviceDirectoryURL, bool isRemember)
    {
        controller.Refresh();
    }
        //listener for add service directory event, when click add service directory button
    public async void addServiceDirectoryEventListener(string serviceDirectoryURL, bool isRemember)
    {
        //register service directory
        RemoteServerManager.serviceDirectory_URL = serviceDirectoryURL; // record the service directory url.
        RemoteServerManager.serviceDirectory_Registered = true; // tell esrimanager that we have one sd registered and no more service directory can be registered.

        //refresh the filebrwoser and wait for its refresh
        controller.Refresh();
    }
    public void ClosePanel()
    {

    }

    public void InitializeDescription(ItemLoadBrowser itemLoadBrowser)
    {
        controller = itemLoadBrowser;
        InitializePanel();
    }

    private void OnDestroy()
    {
        //that means the loading process is finished, we can destroy any login panel object
        if (m_portalLoginObj)
        {
            Destroy(m_portalLoginObj);
        }
        if (m_serverLoginObj)
        {
            Destroy(m_serverLoginObj);
        }
    }

}
