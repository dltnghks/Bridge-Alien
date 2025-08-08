using System.Threading.Tasks;
 
// 데이터 저장소의 표준(계약)을 정의하는 인터페이스
public interface IDataStorage
{
    // 데이터를 저장하는 비동기 메소드
    Task SaveAsync(string data);
 
    // 데이터를 로드하는 비동기 메소드
   Task<string> LoadAsync();
    
   // 데이터 존재 여부를 확인하는 메소드 (선택 사항)
   bool Exists();
    
   // 데이터 삭제 메소드 (선택 사항)
   void Delete();
}