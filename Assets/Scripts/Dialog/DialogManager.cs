using System.Collections.Generic;
using UnityEngine;

public class DialogManager
{
    public static Dictionary<string, object> DialogDic = new Dictionary<string, object>();//�������жԻ��򣬿��Ը����Լ���Ҫ���й���
    private static Transform panelParent;//�Ի����Ŀ¼
    static DialogManager()
    {
        panelParent = GameObject.Find("DialogCanvas").transform;
        panelParent.localScale = new Vector3(1, 1, 1);
    }
    /// <summary>
    /// չʾ�Ի���
    /// </summary>
    /// <typeparam name="TDialog">�Ի�������ͣ�PrefabҲҪ���������������</typeparam>
    /// <typeparam name="TResult">�Ի��򷵻�ֵ������</typeparam>
    /// <param name="callback">�ص�</param>
    public static void Show<TDialog, TResult>(System.Action<DialogResult<TResult>> callback)
    {
        Show<TDialog, TResult>(null, callback);
    }
    /// <summary>
    /// չʾ�Ի���
    /// </summary>
    /// <typeparam name="TDialog">�Ի�������ͣ�PrefabҲҪ���������������</typeparam>
    /// <typeparam name="TResult">�Ի��򷵻�ֵ������</typeparam>
    /// <param name="par">����</param>
    /// <param name="callback">�ص�</param>
    public static void Show<TDialog, TResult>(object par, System.Action<DialogResult<TResult>> callback)
    {
        string key = typeof(TDialog).Name;
        IDialog<TResult> dialog;
        if (DialogDic.TryGetValue(key, out var dialogObj))//���ڣ���ȡ
        {
            dialog = dialogObj as IDialog<TResult>;
            dialog.OnStart(par);
        }
        else//�����ڣ��½�
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
