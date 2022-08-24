using Server;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

	class UdpUser : UdpBase
	{
		private UdpUser() { }

		public static UdpUser ConnectTo(string hostname, int port)
		{
			var connection = new UdpUser();
			connection.Client.Connect(hostname, port);
			return connection;
		}

		public void Send(string message)
		{
			var datagram = Encoding.ASCII.GetBytes(message);
			Client.Send(datagram, datagram.Length);
		}

	}
	class Program
	{
		static void Main(string[] args)
		{
			//default or set in args
			int PORT_NO = 32123;
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

			var client = UdpUser.ConnectTo(SERVER_IP, PORT_NO);

			//wait for reply messages from server and send them to console 
			Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					try
					{
						var received = await client.Receive();
						Console.WriteLine(received.Message);
						if (received.Message.Contains("quit"))
							break;
					}
					catch (Exception ex)
					{
						Console.Write(ex);
					}
				}
			});

			//type ahead :-)
			string read;
			do
			{
				read = Console.ReadLine();
				client.Send(read);
			} while (read != "quit");
		}
	}
}