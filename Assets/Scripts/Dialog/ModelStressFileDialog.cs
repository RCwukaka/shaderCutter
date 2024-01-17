using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ModelStressFileDialog : MonoBehaviour, IDialog<string>
{
    public Action<DialogResult<string>> CloseAction { get; set; }

    private Button OKButton, CancelButton;
    private Button ChooseModelFileBtn, ChooseStressFileBtn;

    private InputField ModelFileInput, StressFileInput;

    private String modelFilePath, stressFilePath;
    private void Awake()
    {
        OKButton = transform.Find("OKButton").GetComponent<Button>();
        CancelButton = transform.Find("CancelButton").GetComponent<Button>();
        OKButton.onClick.AddListener(OK);
        CancelButton.onClick.AddListener(Cancel);

        ChooseModelFileBtn = transform.Find("Model/ChooseModelFile").GetComponent<Button>();
        ChooseModelFileBtn.onClick.AddListener(OpenModelFileChoose);
        ChooseStressFileBtn = transform.Find("Stress/ChooseStressFile").GetComponent<Button>();
        ChooseStressFileBtn.onClick.AddListener(OpenStressFileChoose);

        StressFileInput = transform.Find("Stress/StressFileInput").GetComponent<InputField>();
    }

    private void OK()
    {
        var Result = new DialogResult<string>() { Status = DialogStatus.OK, Result = ModelFileInput.text };
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

    private void OpenModelFileChoose()
    {

        string path = FileDialog.OpenFilePanel();
        modelFilePath = path;
        ModelFileInput.text = path;
    }

    private void OpenStressFileChoose()
    {

        string path = FileDialog.OpenFilePanel();
        stressFilePath = path;
        StressFileInput.text = path;
    }
}
