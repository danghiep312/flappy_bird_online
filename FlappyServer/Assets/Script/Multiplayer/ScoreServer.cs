using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

public class ScoreServer : Singleton<ScoreServer>
{
    public string host;
    public int port;
    private List<TcpClient> listConnectedClients = new List<TcpClient>(new TcpClient[0]);
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    public static Dictionary<int, PlayerScore> Scores = new Dictionary<int, PlayerScore>();
    
    
    private void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests))
        {
            IsBackground = true
        };
        tcpListenerThread.Start();
    }

    private void Update () { 		
    	// if (Input.GetKeyDown(KeyCode.Space)) {          
    	// 	// message = CreateMessageDataScore();
    	// 	SendScore();         
    	// } 	
    } 

    
    private void ListenForIncommingRequests()
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        ThreadPool.QueueUserWorkItem(this.ListenerWorker, null);
    }

    private void ListenerWorker(object token)
    {
        while (tcpListener != null)
        {
            print("Its here");
            connectedTcpClient = tcpListener.AcceptTcpClient();
            listConnectedClients.Add(connectedTcpClient);
            // Thread thread = new Thread(HandleClientWorker);
            // thread.Start(connectedTcpClient);
            ThreadPool.QueueUserWorkItem(this.HandleClientWorker, connectedTcpClient);
        }
    }

    private void HandleClientWorker(object token)
    {
        Byte[] bytes = new Byte[1024];
        using (var client = token as TcpClient)
        using (var stream = client.GetStream())
        {
            Debug.Log("New Client connected");
            int length;
            // Read incomming stream into byte arrary.                      
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);
                // Convert byte array to string message.                          
                string clientMessage = Encoding.ASCII.GetString(incommingData);
                Debug.Log(clientMessage);
                // msg = clientMessage;
            }

            if (connectedTcpClient == null)
            {
                return;
            }
        }
        //  ThreadPool.QueueUserWorkItem(this.SendMessage, connectedTcpClient);
    }

    private void SendMessage(object token, string msg)
    {
        if (connectedTcpClient == null)
        {
            Debug.Log("Problem connected TCPClient null");
            return;
        }

        var client = (TcpClient)token;
        {
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    // Get a stream object for writing.    
                    // Convert string message to byte array.              
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(msg);
                    // Write byte array to socketConnection stream.            
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Debug.Log("Server sent his message - should be received by client");
                }
            }
            catch (Exception socketException)
            {
                listConnectedClients.Remove(client);
                Debug.Log("Socket exception: " + socketException);
                return;
            }
        }
    }

    public void SendScore()
    {
        foreach (TcpClient client in listConnectedClients)
        {
            SendMessage(client, CreateMessageDataScore());
        }
    }
    
    private string CreateMessageDataScore()
    {
        return JsonConvert.SerializeObject(Scores, Formatting.Indented);
    }
    
    
    // #region private members 	
    // /// <summary> 	
    // /// TCPListener to listen for incomming TCP connection 	
    // /// requests. 	
    // /// </summary> 	
    // private TcpListener tcpListener; 
    // /// <summary> 
    // /// Background thread for TcpServer workload. 	
    // /// </summary> 	
    // private Thread tcpListenerThread;  	
    // /// <summary> 	
    // /// Create handle to connected tcp client. 	
    // /// </summary> 	
    // private TcpClient connectedTcpClient; 	
    // #endregion
    //
    // public string message;
    
    // 	
    // // Use this for initialization
    // void Start () { 		
    // 	// Start TcpServer background thread 		
    // 	tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
    // 	tcpListenerThread.IsBackground = true; 		
    // 	tcpListenerThread.Start(); 	
    // }  	
    //
    // // Update is called once per frame
    // void Update () { 		
    // 	if (Input.GetKeyDown(KeyCode.Space)) {          
    // 		message = CreateMessageDataScore();
    // 		SendMessage();         
    // 	} 	
    // }  	
    //
    // /// <summary> 	
    // /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    // /// </summary> 	
    // private void ListenForIncommingRequests () { 		
    // 	try {    
    // 		tcpListener = new TcpListener(IPAddress.Parse(host), port); 			
    // 		tcpListener.Start();              
    // 		Debug.Log("Server is listening");              
    // 		Byte[] bytes = new Byte[1024];  			
    // 		while (true) { 				
    // 			using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
    // 				// Get a stream object for reading 					
    // 				using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
    // 					int length; 						
    // 					// Read incomming stream into byte arrary. 						
    // 					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
    // 						var incommingData = new byte[length]; 							
    // 						Array.Copy(bytes, 0, incommingData, 0, length);  							
    // 						// Convert byte array to string message. 							
    // 						string clientMessage = Encoding.ASCII.GetString(incommingData); 							
    // 						Debug.Log("client message received as: " + clientMessage); 						
    // 					} 					
    // 				} 				
    // 			} 			
    // 		} 		
    // 	} 		
    // 	catch (SocketException socketException) { 			
    // 		Debug.Log("SocketException " + socketException.ToString()); 		
    // 	}     
    // }  	
    //
    //
    // /// <summary> 	
    // /// Send message to client using socket connection. 	
    // /// </summary> 	
    // public void SendMessage() { 		
    // 	if (connectedTcpClient == null) {             
    // 		return;         
    // 	}  		
    // 	
    // 	try { 			
    // 		message = CreateMessageDataScore();
    // 		// Get a stream object for writing. 			
    // 		NetworkStream stream = connectedTcpClient.GetStream(); 			
    // 		if (stream.CanWrite) {                 
    // 			string serverMessage = "This is a message from your server."; 			
    // 			// Convert string message to byte array.                 
    // 			byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message); 				
    // 			// Write byte array to socketConnection stream.               
    // 			stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
    // 			Debug.Log("Server sent his message - should be received by client " + message);           
    // 		}       
    // 	} 		
    // 	catch (SocketException socketException) {             
    // 		Debug.Log("Socket exception: " + socketException);         
    // 	} 	
    // }
    //
    
}