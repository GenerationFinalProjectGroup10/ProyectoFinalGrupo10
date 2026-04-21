// PuzzleInputHandler.cs — agrega este script al jugador o a un Game Manager
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleInputHandler : MonoBehaviour
{
    [Header("UI del panel de código")]
    public TMP_InputField codeInputField;
    public Button submitButton;
    public TeddyBearPuzzle teddyPuzzle;

    private void Start()
    {
        submitButton.onClick.AddListener(SubmitCode);

        // También funciona con Enter
        codeInputField.onSubmit.AddListener(OnInputSubmit);
    }

    private void SubmitCode()
    {
        teddyPuzzle.OnCodeSubmitted(codeInputField.text);
        codeInputField.text = "";
    }

    private void OnInputSubmit(string value)
    {
        SubmitCode();
    }
}