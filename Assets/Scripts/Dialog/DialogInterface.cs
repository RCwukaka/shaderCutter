/// <summary>
/// 对话框接口，所有对话框都要继承此接口
/// </summary>
/// <typeparam name="TResult">对话框返回值的类型</typeparam>
public interface IDialog<TResult>
{
    System.Action<DialogResult<TResult>> CloseAction { get; set; }
    void OnStart(object par = null);
    void OnExit();
}
