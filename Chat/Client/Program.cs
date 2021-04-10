﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        public static void StartClient(string address, int port, string message)
        {
            try
            {
                IPAddress ipAddress = address == "localhost" ? IPAddress.Loopback : IPAddress.Parse(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    Console.WriteLine("Удалённый адрес подключения сокета: {0}",
                        sender.RemoteEndPoint.ToString());

                    // SEND
                    int bytesSent = sender.Send(Encoding.UTF8.GetBytes(message));

                    // RECEIVE
                    byte[] buf = new byte[1024];
                    int bytesRec = sender.Receive(buf);
                    var history = JsonSerializer.Deserialize<List<string>>(Encoding.UTF8.GetString(buf, 0, bytesRec));
                    
                    Console.WriteLine("История сообщений:");
                    foreach (var msg in history)
                    {
                        Console.WriteLine(msg);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            StartClient(args[0], Int32.Parse(args[1]), args[2]);
        }
    }
}