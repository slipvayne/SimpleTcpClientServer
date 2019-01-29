using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			//default or set in args
			int PORT_NO = 1988;
			string SERVER_IP = "127.0.0.1";
			if (args.Any())
			{
				SERVER_IP = args[0];
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

			Console.WriteLine($"\nConnecting to {SERVER_IP} {PORT_NO} ...");

			//---data to send to the server---
			string textToSend = DateTime.Now.ToString();

			//---create a TCPClient object at the IP and port no.---
			TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
			NetworkStream nwStream = client.GetStream();
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

			//---send the text---
			Console.WriteLine("Sending : " + textToSend);
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);

			//---read back the text---
			byte[] bytesToRead = new byte[client.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
			Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
			Console.ReadLine();
			client.Close();
		}
	}
}