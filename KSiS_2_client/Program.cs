using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    string clientName;
    string serverIP;
    int port;
    Socket clientSocket;
    IPEndPoint remoteEndPoint;
    private void DisplayData()
    {
        Console.WriteLine("Список команд:");
        Console.WriteLine("/1. Написать пользователю.");
        Console.WriteLine("/2. Выйти из программы.");
    }
    private void GetMessage()
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int numOfBytes = clientSocket.Receive(buffer);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, numOfBytes);
                Console.WriteLine(receivedMessage);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Ошибка соединения");
        }
    }
    private void ChooseReceiver(string message)
    {
        try
        {
            SendMessage(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        Console.WriteLine("Введите ID пользователя, которому хотите написать.");
        do
        {
            string mes = Console.ReadLine();
            SendMessage(mes);
        } while (message != "/exit");
    }

    private void SendMessage(string message)
    {
        try
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(byteMessage);
        }
        catch (Exception) 
        {
            Console.WriteLine("Не удалось отправить запрос на сервер, " +
                "ошибка номер 52.");
        }
    }
    private void StartCommunication()
    {
        byte[] nameBuffer = Encoding.UTF8.GetBytes(clientName);
        clientSocket.Send(nameBuffer);
        Task.Run(() => GetMessage());
        while (true)
        {
            string message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message))
                continue;
            switch (message)
            {
                case "/1":
                    ChooseReceiver(message);
                    break;
                default:
                    SendMessage(message);
                    break;
            }           
        }
    }
    private void StartConnection()
    {
        bool isCorrect;
        do
        {
            isCorrect = true;
            try
            {
                Console.WriteLine("Добро пожаловать в приложение ChatSirinity!");
                Console.WriteLine("Введите Ваше имя:");
                clientName = Console.ReadLine();

                Console.WriteLine("Введите IP адрес сервера:");
                serverIP = Console.ReadLine();

                Console.WriteLine("Введите порт:");
                port = Convert.ToInt32(Console.ReadLine());

                remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
                clientSocket = new Socket(remoteEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(remoteEndPoint);
                Console.WriteLine("Вы успешено вошли в чат.");
                DisplayData();
                StartCommunication();
            }
            catch (SocketException)
            {
                Console.WriteLine("Некорректный IP и/или номер порта. Повторите попытку.");
                isCorrect = false;
            }
            catch (Exception) 
            { 
                Console.WriteLine("Ошибка ввода!");
                isCorrect = false;
            }
            finally
            {
                if (isCorrect)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);//if there are any data 
                    clientSocket.Close();//destroy socket//    //that aren't recieved yet
                }
            }
        }
        while (!isCorrect);
    }
    public void ConnectToChat()
    {
        Console.OutputEncoding = Encoding.UTF8;
        do
        {
            StartConnection();
        } while (true);
    }
}