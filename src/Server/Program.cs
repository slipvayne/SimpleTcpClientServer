using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			//default or set in args
			int PORT_NO = 1988;
			IPAddress SERVER_IP = IPAddress.Any;
			if (args.Any())
			{
				SERVER_IP = IPAddress.Parse(args[0]);
				PORT_NO = int.Parse(args[1]);
			}

			var hostname = Dns.GetHostName();
			var host = Dns.GetHostEntry(hostname);
			Console.WriteLine($"Machine {hostname} ips: ");
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					Console.WriteLine(ip.ToString());
				}
			}

			TcpListener listener = new TcpListener(SERVER_IP, PORT_NO);
			Console.WriteLine($"\nListening in {SERVER_IP} {PORT_NO} ...");
			listener.Start();

			//---incoming client connected---
			TcpClient client = listener.AcceptTcpClient();
			Console.WriteLine("Connected " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
			//---get the incoming data through a network stream---
			NetworkStream nwStream = client.GetStream();
			byte[] buffer = new byte[client.ReceiveBufferSize];

			//---read incoming stream---
			int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

			//---convert the data received into a string---
			string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
			Console.WriteLine("Received : " + dataReceived);

			//---write back the text to the client---
			Console.WriteLine("Sending back : " + dataReceived);
			nwStream.Write(buffer, 0, bytesRead);

			client.Close();
			listener.Stop();
			Console.ReadLine();
		}
	}
}