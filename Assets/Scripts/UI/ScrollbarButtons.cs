using Battlehub.UIControls;
using UnityEngine;
using UnityEngine.UI;

// TODO: make it scroll exactly one line. Avoid at the top bottom a squeezing of the handle
[RequireComponent(typeof(VirtualizingScrollRect))]
public class ScrollbarButtons : MonoBehaviour
{
    /// <summary>
    /// Amount of lines to scroll with one push of a button;
    /// </summary>
    [SerializeField]
    private int numberOfLinesToScroll = 1;

    /// <summary>
    /// Height of one line. Saw through VirtualizingScrollRect.contentSize 
    /// </summary>
    private int heightOfLine = 25;

    /// <summary>
    /// The scroll rect to be used
    /// </summary>
    VirtualizingScrollRect rect;

    /// <summary>
    /// The scrollbar for the scrolling
    /// </summary>
    public Scrollbar scrollbar;

    public GameObject ButtonUp;
    public GameObject ButtonDown;

    private void Awake()
    {
        rect = gameObject.GetComponent<VirtualizingScrollRect>();
    }

    private void Update()
    {
        if (scrollbar.isActiveAndEnabled)
            EnableButtons();
        else
            DisableButtons();
    }
    public void Up()
    {
        float total = rect.content.sizeDelta.y;
        // this is not perfect. I think it depends on the size of the handle
        float delta = (1 / total) * (numberOfLinesToScroll * heightOfLine);
        scrollbar.value += delta;
    }


    public void Down()
    {
        float total = rect.content.sizeDelta.y;
        float delta = (1 / total) * (numberOfLinesToScroll * heightOfLine);
        scrollbar.value += -delta;
    }

    public void DisableButtons()
    {
        ButtonUp.gameObject.SetActive(false);
        ButtonDown.gameObject.SetActive(false);
    }

    public void EnableButtons()
    {
        ButtonUp.gameObject.SetActive(true);
        ButtonDown.gameObject.SetActive(true);
    }
}
