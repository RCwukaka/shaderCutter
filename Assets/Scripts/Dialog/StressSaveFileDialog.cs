using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class StressSaveFileDialog : MonoBehaviour, IDialog<string>
{
    public Action<DialogResult<string>> CloseAction { get; set; }

    private Button OKButton, CancelButton;
    private Button ChooseStressFileBtn;

    private InputField  StressFileInput;

    private String stressFilePath;
    private void Awake()
    {
        OKButton = transform.Find("OKButton").GetComponent<Button>();
        CancelButton = transform.Find("CancelButton").GetComponent<Button>();
        OKButton.onClick.AddListener(OK);
        CancelButton.onClick.AddListener(Cancel);

        ChooseStressFileBtn = transform.Find("Stress/ChooseStressFile").GetComponent<Button>();
        ChooseStressFileBtn.onClick.AddListener(OpenStressFileChoose);

        StressFileInput = transform.Find("Stress/StressFileInput").GetComponent<InputField>();
    }

    private void OK()
    {
        var Result = new DialogResult<string>() { Status = DialogStatus.OK, Result = StressFileInput.text };
        CloseAction.Invoke(Result);
    }
    private void Cancel()
    {
        var Result = new DialogResult<string>() { Status = DialogStatus.Cancel };
        CloseAction.Invoke(Result);
    }

    public void OnStart(object par = null)
    {
        //QuestionField.text = par?.ToString();

        gameObject.SetActive(true);
    }
    public void OnExit()
    {
        gameObject.SetActive(false);
    }
    private void OpenStressFileChoose()
    {

        OpenFileDir.OpenWinFile(StressFileInput);
    }
}
