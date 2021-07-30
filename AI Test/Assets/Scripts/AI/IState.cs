
public interface IState
{
    /// <summary>
    /// 开始
    /// </summary>
    void OnEnter();


    /// <summary>
    /// 执行时
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// 退出时
    /// </summary>
    void OnExit();
}
