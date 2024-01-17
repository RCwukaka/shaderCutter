/// <summary>
/// 对话框返回值
/// </summary>
/// <typeparam name="TResult">对话框返回值的类型</typeparam>
public class DialogResult<TResult>
{
    public DialogStatus Status { get; set; }
    public TResult Result { get; set; }
}

