using TMPro;
using UnityEngine;

public struct SignupData
{
    public string nickname;
    public string username;
    public string password;
}

public class SignupPanelController : PanelController
{
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;

    public void OnClickConfirmButton()
    {
        string nickname = nicknameInputField.text;
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            // TODO : 누락된 값을 입력하도록 요청
            Shake();
            return;
        }

        // Confim Password 확인
        if (password.Equals(confirmPassword))
        {
            var signupData = new SignupData();
            signupData.nickname = nickname;
            signupData.username = username;
            signupData.password = password;

            StartCoroutine(NetworkManager.Instance.Signup(signupData,
            () => // 로그인 성공했을 때
            {
                GameManager.Instance.OpenConfirmPanel("회원가입에 성공했습니다.", () =>
                {
                    Hide();
                });
            },
            (result) => // 로그인 실패했을 때
            {
                if (result == 0)
                {
                    GameManager.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.", () =>
                    {
                        nicknameInputField.text = "";
                        usernameInputField.text = "";
                        passwordInputField.text = "";
                        confirmPasswordInputField.text = "";
                    });
                }
            }));
        }
    }

    public void OnClickCancelButton()
    {
        Hide();
    }
}
