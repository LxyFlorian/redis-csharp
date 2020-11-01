using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace redis_csharp.src
{
    public class Redis
    {
        Socket socket;
        BufferedStream stream;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public int SendTimeout { get; set; }

        /// <summary>
        /// Constructor with parameters by default
        /// </summary>
        public Redis() : this("localhost", 6379)
        {
        }

        /// <summary>
        /// Constructor to define host
        /// </summary>
        /// <param name="host">host of the redis server</param>
        public Redis(string host) : this(host, 6379)
        {
        }
        
        /// <summary>
        /// Constructor to define port
        /// </summary>
        /// <param name="port">port of the redis server</param>
        public Redis(int port): this("localhost", port)
        {
        }

        /// <summary>
        /// Constructor to define host & post
        /// </summary>
        /// <param name="host">host of the redis server</param>
        /// <param name="port">port of the redis server</param>
        public Redis(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            this.Host = host;
            this.Port = port;
        }

        public void ConnectSocket()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.SendTimeout = this.SendTimeout;
            //connect to the redis server
            this.socket.Connect(this.Host, this.Port);

            //check to see if our socket is connected to the server
            if (!socket.Connected)
            {
                this.socket.Close();
                this.socket = null;
                return;
            }
            this.stream = new BufferedStream(new NetworkStream(this.socket), 16384);
        }
    }
}
