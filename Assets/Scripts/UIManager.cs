using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Ensure you have this for UI components

public class UIManager : MonoBehaviour
{
    public GameObject loadingPanel;
    // Sign Up Input Fields
    public TMP_InputField signUpEmailInputField; // Email input field for sign up
    public TMP_InputField signUpPasswordInputField; // Password input field for sign up
    public TMP_InputField signUpConfirmPasswordInputField; // Confirm password input field for sign up

    // Sign In Input Fields
    public TMP_InputField signInEmailInputField; // Email input field for sign in
    public TMP_InputField signInPasswordInputField; // Password input field for sign in

    public TMP_Text signInErrorMessageText; // TextMesh Pro text for displaying error messages
    public TMP_Text signUpErrorMessageText; // TextMesh Pro text for displaying error messages



    public void OnSignUpButtonClicked()
    {
        StartCoroutine(SignUpCoroutine());
    }

    public void OnSignInButtonClicked()
    {
        StartCoroutine(SignInCoroutine());
    }

    private IEnumerator SignUpCoroutine()
    {
        loadingPanel.SetActive(true);
        signUpErrorMessageText.text = "";

        string email = signUpEmailInputField.text;
        string password = signUpPasswordInputField.text;
        string confirmPassword = signUpConfirmPasswordInputField.text;

        string validationError = ValidateSignUpInputs(email, password, confirmPassword);
        if (!string.IsNullOrEmpty(validationError))
        {
            DisplaySignUpError(validationError);
            loadingPanel.SetActive(false);

            yield break; // Exit the coroutine
        }

        var task = SignUp(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            DisplaySignUpError("Sign Up failed: " + task.Exception?.Message);
        }
        loadingPanel.SetActive(false);
    }

    private IEnumerator SignInCoroutine()
    {
        loadingPanel.SetActive(true);
        signInErrorMessageText.text = "";
        string email = signInEmailInputField.text;
        string password = signInPasswordInputField.text;

        string validationError = ValidateInputs(email, password);
        if (!string.IsNullOrEmpty(validationError))
        {
            DisplaySignInError(validationError);
            loadingPanel.SetActive(false);
            yield break; // Exit the coroutine
        }

        var task = SignIn(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            DisplaySignInError("Sign In failed: " + task.Exception?.Message);
            
        }
        loadingPanel.SetActive(false);
    }

    private string ValidateSignUpInputs(string email, string password, string confirmPassword)
    {
        string basicValidation = ValidateInputs(email, password);
        if (!string.IsNullOrEmpty(basicValidation))
            return basicValidation;

        if (password != confirmPassword)
            return "Passwords do not match.";

        return null;
    }
    private string ValidateInputs(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email cannot be empty.";
        if (string.IsNullOrWhiteSpace(password))
            return "Password cannot be empty.";
        if (!email.Contains("@"))
            return "Invalid email format.";
        if (password.Length < 6)
            return "Password must be at least 6 characters long.";

        return null; // No errors
    }

    private void DisplaySignInError(string message)
    {
        signInErrorMessageText.text = message; // Display the error message
    }
    private void DisplaySignUpError(string message)
    {
        signUpErrorMessageText.text = message; // Display the error message
    }

    private async Task SignUp(string email, string password)
    {
        bool success = await FirebaseManager.Instance.SignUp(email, password);
        if (success)
        {
            string userId = FirebaseManager.Instance.auth.CurrentUser.UserId;
            await FirebaseManager.Instance.StoreUserData(userId, email, "student");
            SceneManager.LoadScene(1);
        }
    }

    private async Task SignIn(string email, string password)
    {
        bool success = await FirebaseManager.Instance.SignIn(email, password);
        if (success)
        {
            string userId = FirebaseManager.Instance.auth.CurrentUser.UserId;
            await FirebaseManager.Instance.GetUserData(userId);
            SceneManager.LoadScene(1);
        }
    }
}
