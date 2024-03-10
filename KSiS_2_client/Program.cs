using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
        Console.WriteLine("Вы успешено вошли в чат.");
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
                int bytesRec = clientSocket.Receive(buffer);
                string receivedMessage = Encoding.UTF8.GetString(buffer);
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
            byte[] buf = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(buf);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
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
                case "/2":
                    Environment.Exit(0);
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
                Console.WriteLine("Введите Ваше имя:");
                clientName = Console.ReadLine();

                Console.WriteLine("Введите IP адрес сервера:");
                serverIP = Console.ReadLine();

                Console.WriteLine("Введите порт:");
                port = Convert.ToInt32(Console.ReadLine());

                remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
                clientSocket = new Socket(remoteEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(remoteEndPoint);
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