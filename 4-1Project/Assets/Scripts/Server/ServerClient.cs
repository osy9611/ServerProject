using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net.Sockets;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ServerState
{
    NOT_CONNECTED,
    CONNECTED,
    LOGIN,
    CLOSING,
    ERROR,
};

public class ServerClient : MonoBehaviour
{
    public static ServerClient instance;
    //서버상태
    public ServerState ST;

    public InputField ipText;
    public string IP;
    public InputField portText;
    public int Port;
    
    PlayerInfo playerInfo;

    public float asyncTime; //캐릭터 위치 동기화 타이밍
    
    public class AsyncObject
    {
        public byte[] Buffer;
        public Socket WorkingSocket;
        public AsyncObject(int BufferSize)
        {
            Buffer = new byte[BufferSize];
        }
    }

    private bool g_Connected;
    private Socket m_ClientSocket = null;

    //AysncCallback : 해당 비동기 작업을 완료할 떄 호출되는 메서드를 참조
    private AsyncCallback m_fnReceiveHandler;
    private AsyncCallback m_fnSendHandler;
    
    Resolve resolve;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        //비동기 작업 매서드를 초기화
        m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
        m_fnSendHandler = new AsyncCallback(handlerDataSend);
        resolve = new Resolve();
    }

    public bool Connected
    {
        get
        {
            return g_Connected;
        }
    }

    bool isConnected;

    public void ConnectedToServer(string IP, int Port)
    {
        //TCP 통신을 위한 소켓생성
        m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        m_ClientSocket.NoDelay = true;  //소켓 통신의 딜레이를 없앰 안없애면 실시간 게임은 조금 힘듬...
        isConnected = false;

        try
        {
            //연결시도
            m_ClientSocket.Connect(IP, Port);

            //연결성공
            isConnected = true;
        }
        catch
        {
            //연결실패(연결 동중 오류가 발생됨)
            isConnected = false;
        }

        g_Connected = isConnected;

        if(isConnected)
        {
            //1024 바이트의 크기를 갖는 바이트 배열을 가진 AsyncObject 클래스를 생성한다
            AsyncObject ao = new AsyncObject(1024);

            //작업 중인 소켓을 저장하기 위해 socketClient 할당
            ao.WorkingSocket = m_ClientSocket;

            //비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 매서드 사용
            m_ClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);

            Debug.Log("연결 성공!");
        }
        else
        {
            Debug.Log("연결 실패!");
        }
    }

    public void StopClient()
    {
        //클라이언트 소켓을 닫는다
        m_ClientSocket.Close();
    }

    public void Send(string data)
    {
        //Senddata.Enqueue(data);
        //추가 정보를 넘기기위한 변수 선언
        AsyncObject ao = new AsyncObject(1);
        
        //해더 선언 제이슨 파일의 문자열 + 4(int)
        byte[] header = BitConverter.GetBytes(data.Length+4);

        //스트링 바이트화 =
        byte[] buffer = Encoding.UTF8.GetBytes(data);

        //제이슨 파일 문자열 길이와 int 4바이트 크기를 잡아서 동적할당
        byte[] packet = new byte[header.Length + buffer.Length];

        //해더 + 제이슨 문자열
        Array.Copy(header, 0, packet, 0, header.Length);
        Array.Copy(buffer, 0, packet, header.Length, buffer.Length);

        ao.Buffer = packet;
        ao.WorkingSocket = m_ClientSocket;
        
        //전송시작
        try
        {
            //BeginSend : 연결된 Socket 데이터를 비동기적으로 보낸다
            m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSendHandler, ao);
        }
        catch (Exception ex)
        {
            Debug.Log("전송 중 오류발생 메시지 : " + ex.Message.ToString());
        }
    }

    int m_nPacketBufferMark;
    byte[] PacketBuffer = new byte[2048];
    //IAsyncResult : 비동기 작업의 상태를 나타내는 인터페이스
    private void handleDataReceive(IAsyncResult ar)
    {
        //넘겨진 추가 정보를 가져온다
        //AysncState 속성의 자료형은 Object 형식이기 때문에 형 변환이 필요
        AsyncObject ao = (AsyncObject)ar.AsyncState;

        //받은 바이트 수 저장할 변수 선언
        int recvBytes =-0;

        try
        {
            //자료를 수신하고, 수신받은 바이트를 가져옴
            recvBytes = ao.WorkingSocket.EndReceive(ar);
        }
        catch
        {
            //예외가 발생하면 함수종료
            return;
        }

        //수신받은 자료의 크기가 1 이상일 때만 처리함
        if (recvBytes > 0)
        {
            //공백 문자들이 많이 발생할 수 있으므로, 받은 바이트 수 만큼 배열을 선언하고 복사함
            byte[] msgByte = new byte[recvBytes];

            Array.Copy(ao.Buffer, msgByte, recvBytes);

            resolve.ReadMessage(msgByte, recvBytes);
        }

        try
        {
            //자료 처리가 끝났으면 다시 데이터를 수신받기 위해서 수신 대리를 한다.
            //Begin 메서드를 이용해 비동기적으로 작업을 대기했다면
            //반드시 대리자 함수에서 End 메서드를 이용해 비동기 작업이 끝났다고 알려줘야함
            ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
        }
        catch(Exception ex)
        {
            //예외가 발생하면 예외 정보를 출력 후 함수를 종료한다.
            Debug.Log("자료 수신 대기 도중 오류가 발생하였습니다! 메시지 :" + ex.Message);
            return;
        }
    }


    private void handlerDataSend(IAsyncResult ar)
    {
        //넘겨진 추가 정보를 가져온다
        AsyncObject ao = (AsyncObject)ar.AsyncState;

        //보낸 바이트 수를 저장할 변수 선언 
        int sendBytes;

        try
        {
            //자료를 전송하고, 전송한 바이트를 가져온다
            sendBytes = ao.WorkingSocket.EndSend(ar);
        }
        catch(Exception ex )
        {
            //예외가 발생하면 예외 정보 출력 후 함수를 종료한다
            Debug.Log("자료 송신 도중 오류가 발생하였습니다! 메시지 : " + ex.Message);
            return;
        }

        if(sendBytes>0)
        {
            //마찬가지로 보낸 바이트 수 만큼 배열 선언 후 복사한다
            byte[] msgByte = new byte[sendBytes];
            Array.Copy(ao.Buffer, msgByte, sendBytes);
        }
    }

    public void Connect()
    {
        if(ipText != null)
            IP = ipText.text;

        if (portText != null)
            Port = int.Parse(portText.text);

        if(ST == ServerState.NOT_CONNECTED)
        {
            ConnectedToServer(IP, Port);
            if(!Connected)
            {
                Debug.Log("접속 중 오류 발생");
                return;
            }
            else
            {
                Debug.Log("접속 성공!");
                ST = ServerState.CONNECTED;

                //연결을 성공하면 유저 데이터를 전송한다
                playerInfo.Init(GameManager.instance.PlayerName, "abc");
                JsonData Data = JsonMapper.ToJson(playerInfo);
                Send(Data.ToString());
                GameManager.instance.SceneChange("Robby");
            }
        }
    }

    private void OnApplicationQuit()
    {
        StopClient();
    }
}
