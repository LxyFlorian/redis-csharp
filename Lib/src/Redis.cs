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
        #region Attributes
        Socket socket;
        BufferedStream stream;
        string CRLF = "\r\n";
        

        public string Host { get; private set; }
        public int Port { get; private set; }
        public int SendTimeout { get; set; }
        #endregion

        #region Constructor
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
        #endregion

        #region Utils Method
        /// <summary>
        /// Connect to the server
        /// </summary>
        private void ConnectSocket()
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

        /// <summary>
        /// Generate Data Command following the RESP specifications
        /// </summary>
        /// <param name="data">Data of the command</param>
        /// <param name="command">Redis command to call</param>
        /// <param name="args">Arguments to the redis command</param>
        /// <returns></returns>
        bool GenDataCommand(byte[] data, string command, params object[] args)
        {
            string resp = "*" + (2 + args.Length).ToString() + this.CRLF;
            resp += "$" + command.Length + CRLF + command + CRLF;

            foreach(object item in args)
            {
                int argsLength = Encoding.UTF8.GetByteCount(item.ToString());
                resp += "$" + argsLength + CRLF + item.ToString() + CRLF;
            }
            resp += "$" + data.Length + CRLF;

            return this.SendData(data, resp);
        }

        /// <summary>
        /// Send data to the server
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="resp">RESP string </param>
        /// <returns></returns>
        bool SendData(byte[] data, string resp)
        {
            //Check if the client is connected to the redis server
            if (this.socket is null) this.ConnectSocket();
            if (this.socket is null) return false;

            //Send command to the server
            byte[] bytes = Encoding.UTF8.GetBytes(resp);
            try
            {
                //Sending command to the server
                this.socket.Send(bytes);
                if(data != null)
                {
                    byte[] end = new byte[] { (byte)CRLF[0], (byte)CRLF[1] };
                    this.socket.Send(data);
                    this.socket.Send(end);
                }
            }
            catch (SocketException)
            {
                this.socket.Close();
                this.socket = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send command to the server
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="args">Parameters of the command</param>
        /// <returns></returns>
        bool SendCommand(string command, params object[] args)
        {
            //Check if the client is connected to the redis server
            if (this.socket is null) this.ConnectSocket();
            if (this.socket is null) return false;

            //Send command to the server
            string resp = "*" + (1 + args.Length).ToString() + CRLF;
            resp += "$" + command.Length + CRLF + command + CRLF;
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + "\r\n" + argStr + "\r\n";
            }

            byte[] b = Encoding.UTF8.GetBytes(resp);
            try
            {
                //Sending command to the server
                this.socket.Send(b);
            }
            catch (SocketException)
            {
                this.socket.Close();
                this.socket = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Read line return by the redis server
        /// </summary>
        /// <returns></returns>
        private string ReadServerLine()
        {
            StringBuilder sb = new StringBuilder();
            int c;

            while((c = stream.ReadByte()) != -1)
            {
                if (c.Equals('\r')) continue;
                if (c.Equals('\n')) break;

                sb.Append((char)c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read if the server return an error
        /// </summary>
        private void CheckSuccess()
        {
            int b = stream.ReadByte();
            if (b.Equals(-1))
                throw new Exception("No data to read");
            string s = this.ReadServerLine();
            if (b == '-')
                throw new Exception(s.StartsWith("ERR ") ? s.Substring(4) : s);

        }

        /// <summary>
        /// Read the data from server
        /// </summary>
        /// <returns></returns>
        private byte[] ReadDataResponse()
        {
            string s = ReadServerLine();
            if (s.Length == 0)
                throw new Exception("Zero length respose");

            char c = s[0];
            if (c == '-')
                throw new Exception(s.StartsWith("-ERR ") ? s.Substring(5) : s.Substring(1));

            if (c.Equals('$')) // Bulks string
            {
                if (s == "$-1")
                    return null;
                int n;

                if (Int32.TryParse(s.Substring(1), out n))
                {
                    byte[] retbuf = new byte[n];

                    int bytesRead = 0;
                    do
                    {
                        int read = stream.Read(retbuf, bytesRead, n - bytesRead);
                        if (read < 1)
                            throw new Exception("Invalid termination mid stream");
                        bytesRead += read;
                    }
                    while (bytesRead < n);
                    if (stream.ReadByte() != '\r' || stream.ReadByte() != '\n')
                        throw new Exception("Invalid termination");
                    return retbuf;
                }
                throw new Exception("Invalid length");
            }
            else if (c.Equals('+')) //simple string
            {
                return Encoding.UTF8.GetBytes(s);
            }
            throw new Exception("Unexpected reply: " + s);
        }

        private int ReadIntResponse(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args)) throw new Exception("Cannot connect to the server");

            int b = stream.ReadByte();
            if (b.Equals(-1))
                throw new Exception("No data retrieved");

            string s = this.ReadServerLine();
            if(b.Equals('-'))
                throw new Exception(s.StartsWith("-ERR ") ? s.Substring(5) : s.Substring(1));

            if (b.Equals(':'))
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
            }
            throw new Exception("Unexpected response : " + s);
        }

        private string ReadStringResponse(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args)) throw new Exception("Cannot connect to the server");
            return this.ReadServerLine();
        }

        /// <summary>
        /// Sent Data and return the data 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public byte[] PushCommand(string cmd, params object[] args)
        {
            if (cmd is null) throw new ArgumentNullException("key");

            if (!SendCommand(cmd, args))
                throw new Exception("Cannot connect to the server");

            return this.ReadDataResponse();
        }

        /// <summary>
        /// Dispose the connection
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the connection
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SendCommand("QUIT");
                ReadDataResponse();
                this.socket.Close();
                this.socket = null;
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// Set a KEY in Redis
        /// </summary>
        /// <param name="key">Name of the key to save</param>
        /// <param name="value">Value of the object to store</param>
        private void Set(string key, byte[] value)
        {
            if (key is null) throw new ArgumentNullException("key");
            if (value is null) throw new ArgumentNullException("value");

            if (!this.GenDataCommand(value, "SET", key))
                throw new Exception("Cannot connect to the server");

            this.CheckSuccess();

        }

        /// <summary>
        /// Set a simple string key/value record in the database
        /// </summary>
        /// <param name="key">Name of the key to save</param>
        /// <param name="value">Value of the object to store</param>
        public void Set(string key, string value)
        {
            if (key is null) throw new ArgumentNullException("key");
            if (value is null) throw new ArgumentNullException("value");

            this.Set(key, Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Get a String key
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <returns>String value of the key</returns>
        public string GetString(string key)
        {
            if (key.Equals(null)) throw new ArgumentNullException("key");

            return Encoding.UTF8.GetString(PushCommand("GET", key));
        }

        /// <summary>
        /// Delete a key
        /// </summary>
        /// <param name="key">key to delete</param>
        /// <returns>1 if deleted, 0 else</returns>
        public bool Delete(string key)
        {
            if (key.Equals(null)) throw new ArgumentNullException("key");

            return this.ReadIntResponse("DEL", key).Equals(1);
        }

        /// <summary>
        /// SET a timeout on key. After the timeout has expired, the key will automatically be deleted.
        /// </summary>
        /// <param name="key">The key to apply a EXPIRE</param>
        /// <param name="seconds">Seconds to live to the key</param>
        /// <returns>1 if the timeout was set; 0 if key does not exist</returns>
        public bool Expire(string key, int seconds)
        {
            if (key.Equals(null)) throw new ArgumentNullException("key");
            if (seconds.Equals(null)) throw new ArgumentNullException("seconds");

            return this.ReadIntResponse("EXPIRE", key, seconds).Equals(1);
        }

        /// <summary>
        /// Return the remanining time to live of a key that has a timeout
        /// </summary>
        /// <param name="key">Key to check the timeout</param>
        /// <returns>TTL in seconds</returns>
        public int TimeToLive(string key)
        {
            if (key.Equals(null)) throw new ArgumentNullException("key");

            return this.ReadIntResponse("TTL", key);
        } 

        /// <summary>
        /// Rename oldKey to newKey. 
        /// </summary>
        /// <param name="oldKey">Old name of the key to rename</param>
        /// <param name="newKey">New name to give </param>
        /// <returns></returns>
        public bool Rename(string oldKey, string newKey)
        {
            if (oldKey.Equals(null)) throw new ArgumentNullException("oldKey");
            if (newKey.Equals(null)) throw new ArgumentNullException("newKey");

            return this.ReadStringResponse("RENAME", oldKey, newKey)[0].Equals('+');
        }

/// <summary>
        /// Remove the existing timeout on key, turning the key from volatile to persistent key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Persist(string key)
        {
            if (key.Equals(null)) throw new Exception("key");

            return this.ReadIntResponse("PERSIST", key).Equals(1);
        }        
        #endregion
    }
}
