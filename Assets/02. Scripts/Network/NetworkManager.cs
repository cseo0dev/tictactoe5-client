using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManager : Singleton<NetworkManager>
{
    // 회원가입
    public IEnumerator Signup(SignupData signupData, Action success, Action<int> failure)
    {
        string jsonString = JsonUtility.ToJson(signupData);
        byte[] byteRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signup",
            UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(byteRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                // TODO : 서버 연결 오류에 대해 알림
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);

                if (result.result == 2) // 로그인 성공
                {
                    success?.Invoke();
                }
                else
                {
                    failure?.Invoke(result.result);
                }
            }
        }
    }

    // 로그인
    public IEnumerator Signin(SigninData signinData, Action success, Action<int> failure)
    {
        string jsonString = JsonUtility.ToJson(signinData); // 구조체 값을 json 형식의 문자열로 바꾸기

        // Post 방식으로 값 전달 -> btye 타입이어야 함
        byte[] byteRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signin",
            UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(byteRaw);
            www.downloadHandler = new DownloadHandlerBuffer(); // 서버 응답 받아오기
            www.SetRequestHeader("Content-Type", "application/json"); // Http 프로토콜의 header 구조가 이럼

            yield return www.SendWebRequest(); // 서버에서 응답 오기 전까지 대기

            // 접속 에러가 발생했을 경우
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                // TODO : 서버 연결 오류에 대해 알림
            }
            else
            {
                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString); // Json을 구조체 형태로 바꾸기

                if (result.result == 2) // 로그인 성공
                {
                    var cookie = www.GetResponseHeader("set-cookie");
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        int lastIndex = cookie.LastIndexOf(';');
                        string sid = cookie.Substring(0, lastIndex);
                        // 저장
                        PlayerPrefs.SetString("sid", sid);
                    }

                    success?.Invoke();
                }
                else
                {
                    failure?.Invoke(result.result);
                }
            }
        };
    }

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        
    }
}
