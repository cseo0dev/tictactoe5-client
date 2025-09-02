using TMPro;
using UnityEngine;

public class ConfirmPanelController : PanelController
{
    [SerializeField] private TMP_Text messageText;

    public void Show(string message)
    {
        messageText.text = message;
        base.Show();
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}
