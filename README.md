# StarDust-Server-
4학년 1학기 프로젝트/ 5월 기준 클라이언트 별도의 깃에서 관리중
+ 사용 기술
  + Unity 2D
  + C/C++, C#
  + MySQL
  + Boost.Asio
  
+ 작업내용
  + Client
    + c# 비동기 소켓을 사용해 서버 통신구현
    + 서버로 데이터를 보낼 때 패킷 헤더에 패킷 사이즈를 넣어서 보냄
    + 서버가 보낸 데이터 중 패킷 데이터 일부가 도착한 경우 다음에 받은 패킷 데이터와 합쳐 데이터 처리를 하도록 구현
    
  + Server
    + Boost.Asio 비동기 소켓을 이용해 Server Session 구현
    + ADO를 이용해 DB 연동 작업을 진행
    + 대기 방을 관리하는 RoomManager 구현
    + 보스 패턴, HP 등을 관리gksms BossManager 구현
