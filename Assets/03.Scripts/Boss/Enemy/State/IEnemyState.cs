public interface IEnemyState
{
    MiniGameDeliveryEnemy Enemy { get; set;}
    void EnterState(); // 상태 진입 시 동작
    void UpdateState(); // 상태 업데이트
    void ExitState(); // 상태 종료 시 동작
}