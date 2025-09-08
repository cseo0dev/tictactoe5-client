using TMPro;
using UnityEngine;

public struct SigninData
{
    public string username;
    public string password;
}

public struct SigninResult
{
    public int result; // Json 타입의 키 값과 이름 동일해야함
}

public class SigninPanelController : PanelController
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    public void OnClickConfirmButton()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // TODO : 누락된 값을 입력하도록 요청
            Shake();
            return;
        }

        var signinData = new SigninData();
        signinData.username = username;
        signinData.password = password;

        StartCoroutine(NetworkManager.Instance.Signin(signinData,
            () => // 로그인 성공했을 때
            {
                Hide();
            },
            (result) => // 로그인 실패했을 때
            {
                if (result == 0)
                {
                    GameManager.Instance.OpenConfirmPanel("유저 이름이 유효하지 않습니다.", () =>
                    {
                        usernameInputField.text = "";
                        passwordInputField.text = "";
                    });
                }
                else if (result == 1)
                {
                    GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.", () =>
                    {
                        usernameInputField.text = "";
                        passwordInputField.text = "";
                    });
                }
            }));
    }

    public void OnClickJoinButton()
    {
        GameManager.Instance.OpenSignupPanel();
    }
}
