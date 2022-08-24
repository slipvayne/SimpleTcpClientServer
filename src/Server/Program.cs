using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public struct Received
	{
		public IPEndPoint Sender;
		public string Message;
	}

	public abstract class UdpBase
	{
		protected UdpClient Client;

		protected UdpBase()
		{
			Client = new UdpClient();
		}

		public async Task<Received> Receive()
		{
			var result = await Client.ReceiveAsync();
			return new Received()
			{
				Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
				Sender = result.RemoteEndPoint
			};
		}
	}

	class UdpListener : UdpBase
	{
		private IPEndPoint _listenOn;

		public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123))
		{
		}

		public UdpListener(IPEndPoint endpoint)
		{
			_listenOn = endpoint;
			Client = new UdpClient(_listenOn);
		}

		public void Reply(string message, IPEndPoint endpoint)
		{
			var datagram = Encoding.ASCII.GetBytes(message);
			Client.Send(datagram, datagram.Length, endpoint);
		}

	}

	class Program
	{
		static void Main(string[] args)
		{
			//default or set in args
			int PORT_NO = 32123;
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

			var server = new UdpListener();
			Console.WriteLine($"\nListening in {SERVER_IP} {PORT_NO} ...");


			//start listening for messages and copy the messages back to the client
			Task.Factory.StartNew(async () => {
				while (true)
				{
					var received = await server.Receive();
					server.Reply("copy " + received.Message, received.Sender);
					if (received.Message == "quit")
						break;
				}
			});

			Console.ReadLine();
		}
	}
}