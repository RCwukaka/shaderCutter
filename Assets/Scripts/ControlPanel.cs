using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public Action<DialogResult<string>> CloseAction { get; set; }

    private Button HelpBtn, ExitBtn;
    private Button FileBtn, SaveBtn, SettingBtn;

    void Start()
    {
        FileBtn = transform.Find("ControlPanel/FileButton").GetComponent<Button>();
        SaveBtn = transform.Find("ControlPanel/SaveButton").GetComponent<Button>();
        SettingBtn = transform.Find("ControlPanel/SettingButton").GetComponent<Button>();        
        HelpBtn = transform.Find("ControlPanel/HelpButton").GetComponent<Button>();
        ExitBtn = transform.Find("ControlPanel/ExitButton").GetComponent<Button>();
        ExitBtn.onClick.AddListener(Exit);
        SettingBtn.onClick.AddListener(showSettingDialog);
        FileBtn.onClick.AddListener(showFileDialog);
        SaveBtn.onClick.AddListener(showSaveDialog);
        HelpBtn.onClick.AddListener(showHelpDialog);

    }

    private void Exit() {
        Application.Quit();
    }

    public void showFileDialog()
    {

        DialogManager.Show<ModelStressFileDialog, string>("传参", res =>
        {
            if (res.Status == DialogStatus.OK)
                Debug.Log(res.Result);
            else
                Debug.Log("取消");
        });
    }

    private void showSaveDialog()
    {

        DialogManager.Show<StressSaveFileDialog, string>("传参", res =>
        {
            if (res.Status == DialogStatus.OK)
                Debug.Log(res.Result);
            else
                Debug.Log("取消");
        });
    }

    private void showSettingDialog() {

        DialogManager.Show<ModelStressFileDialog, string>("传参", res =>
        {
            if (res.Status == DialogStatus.OK)
                Debug.Log(res.Result);
            else
                Debug.Log("取消");
        });
    }

    private void showHelpDialog()
    {

        DialogManager.Show<ModelStressFileDialog, string>("传参", res =>
        {
            if (res.Status == DialogStatus.OK)
                Debug.Log(res.Result);
            else
                Debug.Log("取消");
        });
    }

}
