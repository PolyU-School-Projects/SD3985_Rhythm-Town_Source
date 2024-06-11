using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using Ricimi;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public GameObject SignUpPanel;
    public GameObject SignInPanel;
    public GameObject AvatarChoosing;

    string displayErrorMsg;
    bool isSignUp = false;

    [SerializeField]
    TMP_InputField signUpEmailAsUserName;

    [SerializeField]
    // InputField signUpPassword;
    TMP_InputField signUpPassword;

    [SerializeField]
    TMP_InputField signUpNickName;

    [SerializeField]
    TMP_InputField logInEmail;

    [SerializeField]
    TMP_InputField logInPassword;

    public GameObject dialogueBox;

    DialogueManager dialogueManager;

    void Start()
    {
        SignInPanel.gameObject.SetActive(true);
        SignUpPanel.gameObject.SetActive(false);
        AvatarChoosing.gameObject.SetActive(false);

        dialogueManager = dialogueBox.GetComponent<DialogueManager>();
        dialogueManager.SetText("Welcome to the Rhythm Town! May I have your ID ? ");
    }

    public async void On_SignUp_SignUpPressed()
    {
        await SignUpWithUsernamePasswordAsync(
            signUpEmailAsUserName.text.Replace("\u200B", ""),
            signUpPassword.text.Replace("\u200B", "")
        );
    }

    public async void On_LogIn_LogInPressed()
    {
        await SignInWithUsernamePasswordAsync(
            logInEmail.text.Replace("\u200B", ""),
            logInPassword.text.Replace("\u200B", "")
        );
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        isSignUp = true;
        await UnityServices.InitializeAsync();
        try
        {
            Debug.Log("username:" + username + " password: " + password);
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(
                username,
                password
            );
            Debug.Log("SignUp is successful.");
            Debug.Log("PlayerID: " + AuthenticationService.Instance.PlayerId);
            SignUpPanel.gameObject.SetActive(false);
            // SignInPanel.gameObject.SetActive(true);
            AvatarChoosing.gameObject.SetActive(true);
            await UpdateRecordAsync(
                signUpNickName.text.Replace("\u200B", ""),
                signUpEmailAsUserName.text.Replace("\u200B", ""),
                signUpPassword.text.Replace("\u200B", "")
            );
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.Log("authentication");
            Debug.LogException(ex);
            Debug.Log("(SignUp)authentication exception: " + ex.ErrorCode + "()msg: " + ex.Message);
            Debug.Log("--------------------current error code: " + ex.Message);
            DisplayErrorMsgInDialougueBox(ex.ErrorCode, ex.Message);

            // var webRequestException = ex.InnerException as WebRequestException;
            // if (webRequestException != null)
            // {
            //     // Handle the WebRequestException
            //     Debug.Log("WebRequestException: " + ex.InnerException.Message);

            //     // Access the response code
            //     long responseCode = webRequestException.GetResponseCode();
            //     Debug.Log("Response Code: " + responseCode);
            // }
            // else
            // {
            //     // Handle other types of AuthenticationException
            //     Debug.Log("AuthenticationException: " + ex.Message);
            // }
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.Log("requestfailed");
            Debug.LogException(ex);
            Debug.Log("(SignUp)request exception: " + ex.ErrorCode + "()msg: " + ex.Message);
            Debug.Log("--------------------current error code: " + ex.Message);
            DisplayErrorMsgInDialougueBox(ex.ErrorCode, ex.Message);
        }
        catch (WebException ex)
        {
            Debug.Log("&&&&&&&&&webException: " + ex.Response);
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        isSignUp = false;
        await UnityServices.InitializeAsync();
        try
        {
            Debug.Log("username:" + username + " password: " + password);
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(
                username,
                password
            );
            Debug.Log("SignIn is successful.");
            Debug.Log("PlayerID: " + AuthenticationService.Instance.PlayerId);
            await GameSettings.LoadSettings();
            GetComponent<SceneTransition>().PerformTransition();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            Debug.Log("(SignIn)authentication exception: " + ex.ErrorCode + "()msg: " + ex.Message);

            Debug.Log("--------------------current error code: " + ex.ErrorCode);
            DisplayErrorMsgInDialougueBox(ex.ErrorCode, ex.Message);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            Debug.Log("(SignIn)request exception: " + ex.ErrorCode + "()msg: " + ex.Message);

            Debug.Log("--------------------current error code: " + ex.ErrorCode);
            DisplayErrorMsgInDialougueBox(ex.ErrorCode, ex.Message);
        }
    }

    public async Task UpdateRecordAsync(string nickname, string username, string password)
    {
        Debug.Log(nickname + " saved to cloud");
        // Save the objectLevelRecord_Cloud to the cloud or perform any other necessary updates
        var data = new Dictionary<string, object>
        {
            { "inGameDisplayNickName", nickname },
            { "PlayerInfo_email", username },
            { "PlayerInfo_psw", password }
        };

        // Task CloudSaveService.Instance.Data.Player.SaveAsync(Dictionary<string, object> data)；
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }

    public void DisplayErrorMsgInDialougueBox(int errorCode, string errorMsg)
    {
        if (errorCode == AuthenticationErrorCodes.InvalidParameters)
        {
            displayErrorMsg = "The input is not in the correct format.";
        }
        else if (
            (errorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            || (errorCode == AuthenticationErrorCodes.AccountLinkLimitExceeded)
        )
        {
            displayErrorMsg = "account already exist, please use login.";
        }
        else if (errorCode == AuthenticationErrorCodes.ClientUnlinkExternalIdNotFound)
        {
            displayErrorMsg =
                "the developer doesnot understand what this is about, try again in the next version.";
        }
        else if (errorCode == AuthenticationErrorCodes.ClientInvalidProfile)
        {
            displayErrorMsg = "user name invalid...................";
        }
        else if (errorCode == AuthenticationErrorCodes.InvalidSessionToken)
        {
            displayErrorMsg = "Gosh! The session token is invalid.";
        }
        else if (errorCode == AuthenticationErrorCodes.InvalidProvider)
        {
            displayErrorMsg =
                "Bad luck~ API refused to process, the provider was invalid for the request";
        }
        else if (errorCode == AuthenticationErrorCodes.BannedUser)
        {
            displayErrorMsg = "Ooops you have been banned.";
        }
        else if (errorCode == AuthenticationErrorCodes.EnvironmentMismatch)
        {
            displayErrorMsg =
                "There is a mismatch between the requested environment and the one configured.";
        }
        else
        {
            if (errorCode != 10000)
            {
                if (
                    (isSignUp == false)
                    && (Convert.ToInt32(errorCode.ToString().Substring(0, 1)) != 2)
                )
                {
                    displayErrorMsg = "Invalid username or password.";
                }
                else if (errorMsg.Contains("Username"))
                {
                    displayErrorMsg =
                        "username(3-20 digits): Insert only letters, digits and symbols among . - _ @";
                }
                else if (errorMsg.Contains("Password"))
                    displayErrorMsg =
                        "password(3-30 digits):Insert at least 1 uppercase, 1 lowercase, 1 digit and 1 symbol.";
                else
                    displayErrorMsg = "Something went wrong.";
            }
        }
        dialogueManager.SetText(displayErrorMsg);
    }
}
