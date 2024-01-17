using System.Collections.Generic;
using UnityEngine;

public class DialogManager
{
    public static Dictionary<string, object> DialogDic = new Dictionary<string, object>();//储存所有对话框，可以根据自己需要进行管理
    private static Transform panelParent;//对话框根目录
    static DialogManager()
    {
        panelParent = GameObject.Find("DialogCanvas").transform;
        panelParent.localScale = new Vector3(1, 1, 1);
    }
    /// <summary>
    /// 展示对话框
    /// </summary>
    /// <typeparam name="TDialog">对话框的类型（Prefab也要用这个类型命名）</typeparam>
    /// <typeparam name="TResult">对话框返回值的类型</typeparam>
    /// <param name="callback">回调</param>
    public static void Show<TDialog, TResult>(System.Action<DialogResult<TResult>> callback)
    {
        Show<TDialog, TResult>(null, callback);
    }
    /// <summary>
    /// 展示对话框
    /// </summary>
    /// <typeparam name="TDialog">对话框的类型（Prefab也要用这个类型命名）</typeparam>
    /// <typeparam name="TResult">对话框返回值的类型</typeparam>
    /// <param name="par">传参</param>
    /// <param name="callback">回调</param>
    public static void Show<TDialog, TResult>(object par, System.Action<DialogResult<TResult>> callback)
    {
        string key = typeof(TDialog).Name;
        IDialog<TResult> dialog;
        if (DialogDic.TryGetValue(key, out var dialogObj))//存在，获取
        {
            dialog = dialogObj as IDialog<TResult>;
            dialog.OnStart(par);
        }
        else//不存在，新建
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>(key), panelParent);
            obj.name = key;
            dialog = obj.GetComponent<IDialog<TResult>>();
            DialogDic.Add(key, dialog);
            dialog.OnStart(par);

            dialog.CloseAction = res =>
            {
                dialog.OnExit();
                callback?.Invoke(res);
            };
        }
    }
}
