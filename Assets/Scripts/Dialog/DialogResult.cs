/// <summary>
/// �Ի��򷵻�ֵ
/// </summary>
/// <typeparam name="TResult">�Ի��򷵻�ֵ������</typeparam>
public class DialogResult<TResult>
{
    public DialogStatus Status { get; set; }
    public TResult Result { get; set; }
}

