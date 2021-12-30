using System;
using System.Net;
using Hazel;
using Hazel.Udp;

namespace BaldiServer
{
	class Program
	{
		static ConnectionListener listener;

		static void Main(string[] args)
		{
			IPEndPoint endpoint = new IPEndPoint(0, 25565);
			listener = new UdpConnectionListener(endpoint);
			listener.NewConnection += NewConnectionHandler;
			listener.Start();
			CommandLoop();
		}

		static void CommandLoop()
		{
			string thingy = Console.ReadLine();
			CommandLoop();
		}


		static void NewConnectionHandler(NewConnectionEventArgs args)
		{
			Console.WriteLine(args);
		}


	}
}
