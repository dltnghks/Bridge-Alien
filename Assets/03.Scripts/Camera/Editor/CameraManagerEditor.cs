using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraManager))]               // CameraManager 클래스에 대한 사용자 정의 인스펙터 생성

//! CameraManager 클래스에 대한 사용자 정의 인스펙터
public class CameraManagerEditor : Editor
{
    private SerializedProperty currentCameraType;   // 현재 카메라 타입 속성 (CameraManager 클래스의 currentCameraType 속성 참고)
    private SerializedProperty targetTransform;     // 카메라 타겟 속성 (CameraManager 클래스의 target 속성 참고)
    private SerializedProperty topDownSettings;    // 탑 다운 카메라 설정 속성 (CameraManager 클래스의 topDownSettings 속성 참고)
    private SerializedProperty thirdPersonSettings; // 세 번째 인치 카메라 설정 속성 (CameraManager 클래스의 thirdPersonSettings 속성 참고)
    private CameraManager cameraManager;            // CameraManager 인스턴스 (CameraManager 클래스의 인스턴스 참고)

    //~ 인스펙터가 활성화될 때 호출되는 메서드
    private void OnEnable()
    {
        currentCameraType = serializedObject.FindProperty("currentCameraType");         // currentCameraType 속성 찾기
        targetTransform = serializedObject.FindProperty("target");                      // target 속성 찾기
        topDownSettings = serializedObject.FindProperty("topDownSettings");             // topDownSettings 속성 찾기
        thirdPersonSettings = serializedObject.FindProperty("thirdPersonSettings");     // thirdPersonSettings 속성 찾기
        cameraManager = target as CameraManager;                                        // CameraManager 인스턴스 찾기
    }

    //~ 인스펙터 그래픽 인터페이스 그리기
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();                                                  // 변경 확인 시작

        serializedObject.Update();                                                     // 직렬화된 객체 업데이트

        EditorGUILayout.PropertyField(currentCameraType);                              // currentCameraType 속성 표시
        EditorGUILayout.PropertyField(targetTransform);                                // target 속성 표시

        EditorGUILayout.Space();                                                       // 공백 추가

        CameraManager.CameraType selectedType = (CameraManager.CameraType)currentCameraType.enumValueIndex; // 선택된 카메라 타입 가져오기
        switch (selectedType)                                                                               // 선택된 카메라 타입에 따라 해당하는 설정 표시
        {
            case CameraManager.CameraType.TopDown:                                                          // 탑 다운 카메라 타입인 경우
                EditorGUILayout.PropertyField(topDownSettings, true);                                       // topDownSettings 속성 표시
                break;
            case CameraManager.CameraType.ThirdPerson:                                                      // 세 번째 인치 카메라 타입인 경우
                EditorGUILayout.PropertyField(thirdPersonSettings, true);                                   // thirdPersonSettings 속성 표시
                break;
        }

        bool changed = EditorGUI.EndChangeCheck();                                                          // 변경 확인 종료
        serializedObject.ApplyModifiedProperties();                                                         // 직렬화된 객체 적용

        // 값이 변경되었을 때 현재 카메라 설정 업데이트
        if (changed && Application.isPlaying) { cameraManager.UpdateCurrentCameraSettings(); }              // 현재 카메라 설정 업데이트
    }
}