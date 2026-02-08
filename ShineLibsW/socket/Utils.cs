using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;

namespace shine.libs.socket
{
	public static class soutils
	{
		public static void printIp(System.Net.Sockets.Socket socket)
		{
			IPEndPoint remoteP = socket.RemoteEndPoint as IPEndPoint;
			IPEndPoint localP = socket.LocalEndPoint as IPEndPoint;
			
			if (remoteP != null)
			{
			    Console.WriteLine("RemoteEnd Ip / Port = {0} / {1}", remoteP.Address, remoteP.Port);
			}
			
			if (localP != null)
			{
			    Console.WriteLine("LocalEnd Ip / Port = {0} / {1}", localP.Address, localP.Port);
			}
		}		
		public static void printIpRemote(System.Net.Sockets.Socket socket)
		{
			IPEndPoint remoteP = socket.RemoteEndPoint as IPEndPoint;
			
			if (remoteP != null)
			{
			    Console.WriteLine("RemoteEnd Ip / Port = {0} / {1}", remoteP.Address, remoteP.Port);
			}
		}		
	}	
}