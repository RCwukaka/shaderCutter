/// <summary>
/// �Ի���ӿڣ����жԻ���Ҫ�̳д˽ӿ�
/// </summary>
/// <typeparam name="TResult">�Ի��򷵻�ֵ������</typeparam>
public interface IDialog<TResult>
{
    System.Action<DialogResult<TResult>> CloseAction { get; set; }
    void OnStart(object par = null);
    void OnExit();
}
