using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class NumbersOnly : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        // Get the InputField component attached to this GameObject
        inputField = GetComponent<TMP_InputField>();

        // Add a listener to the input field to invoke the ValidateInput method whenever the text is changed
        inputField.onValueChanged.AddListener(ValidateInput);
    }

    void ValidateInput(string input)
    {
        // Define a regular expression pattern to match only numbers
        string pattern = @"^\d*$";

        // Check if the input matches the pattern
        if (!Regex.IsMatch(input, pattern))
        {
            // If the input doesn't match, remove the invalid characters
            inputField.text = Regex.Replace(inputField.text, "[^0-9]", "");
        }
    }
}