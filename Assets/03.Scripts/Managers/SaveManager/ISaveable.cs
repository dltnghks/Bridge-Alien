public interface ISaveable
{
    void Add(ISaveable saveable);

    // 현재 상태를 저장 가능한 객체로 캡처하여 반환
    object CaptureState();

    // 캡처된 상태 객체를 받아와서 현재 상태를 복원
    void RestoreState(object state);
}