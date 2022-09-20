using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BrowserDesign.UI;

public class FileBrowserUIPanel : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField]
    public GameObject confirmButtonObject;

    [SerializeField]
    public GameObject cancelButtonObject;

    [SerializeField]
    public GameObject inputFieldObject;

    [SerializeField]
    public TextMeshProUGUI title;

    //whether allow input field to be empty
    public bool allowEmptyInputField = true;

    /// <summary>
    /// Fired when the confirm (save) button is pressed
    /// </summary>
    public event ItemSelectedEventHandler confirmButtonPressed;

    /// <summary>
    /// Fired when cancel button is pressed
    /// </summary>
    public event ItemSelectCancelEventHandler cancelButtonPressed;

    public delegate void ItemSelectedEventHandler(IBaseItem dataItem, string fileName);
    public delegate void ItemSelectCancelEventHandler();

    public ViewItem CurrentSelection
    {
        get { return currentSelection; }
        set { currentSelection = value; }
    }

    //viewItem for current selection
    private ViewItem currentSelection;

    void Start()
    {
        InitializePanel();
    }

    public void InitializePanel()
    {

    }

    /// <summary>
    /// Adds a suggested filename
    /// </summary>
    public void AddSuggestedFilename(string suggestedFilename)
    {
        nameInput.text = suggestedFilename;
    }

    public virtual void ConfirmButton()
    {
        nameInput.image.color = Color.white;
        if (currentSelection != null)
        {
            if (nameInput.text == ""&& !allowEmptyInputField)
            {
                nameInput.image.color = Color.red;
                nameInput.placeholder.GetComponent<TMPro.TMP_Text>().text = LanguageManager.Translate("Please Enter Name...");
                return;
            }
            var selectedItem = currentSelection.DataItem;
            if (selectedItem != null)
            {
                confirmButtonPressed?.Invoke(selectedItem, nameInput.text);
            }
        }
        else
        {
            return;
        }
        //ClosePanel();
    }

    void OnDestroy()
    {
        ClosePanel();
    }

    public virtual void CancelButton()
    {
        cancelButtonPressed.Invoke();
        //ClosePanel();
    }
    
    /// <summary>
    /// Button of cancel and close the panel, 
    /// </summary>
    public virtual void ClosePanel()
    {
        //unsubsribe from all subscriber
        if (confirmButtonPressed != null)
        {
            foreach (var subscriber in confirmButtonPressed.GetInvocationList())
            {
                confirmButtonPressed -= (ItemSelectedEventHandler)subscriber;
            }
        }
        if (cancelButtonPressed != null)
        {
            foreach (var subscriber in cancelButtonPressed.GetInvocationList())
            {
                cancelButtonPressed -= (ItemSelectCancelEventHandler)subscriber;
            }
        }
    }
}
