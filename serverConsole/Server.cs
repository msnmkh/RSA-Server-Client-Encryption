using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace serverConsole
{
    class Server
    {
        private static int eServer;
        private static int dServer;
        private static int nServer;
        private static int eClient;
        private static int nClient;
        TcpClient client;
        TcpListener tcpListener;
        NetworkStream stream;
        Int32 port;
        IPAddress iPAddress;
        string responseData;
        int i;
        // Buffer for reading data
        Byte[] bytes;

        public Server(string ip, Int32 port)
        {
            iPAddress = IPAddress.Parse(ip);
            this.port = port;

            // Create list of primes;
            var prime = new Primes();

            // Generate key n,d,e with two random primes for encrypt and decrypt 
            var keys = prime.GetKey();

            // The public key is (e, n) and the private key is (d, n).
            dServer = keys[0];
            eServer = keys[1];
            nServer = keys[2];
        }

        public void StartListening()
        {
            // Exchage client's public key and server's public key
            getClietnPubAndSendServerPub();

            while (true)
            {
                // Recive Message
                try
                {
                    bytes = new Byte[256];
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // deleteZero(byte[] variable) for delete null elements.
                        Byte[] newbytes = deleteZeoBytes(bytes);

                        // Converting array bytes to array ints.
                        var cipherInts = convertByteToint(newbytes);

                        // Decrypt message
                        string encodeMessage = Rsa.Decrypt(cipherInts, nServer, dServer);

                        Console.WriteLine("Received: {0}", encodeMessage);

                        // Process the data sent by the client.
                        string responseData = encodeMessage + encodeMessage;

                        // Encrypt response message
                        var encodeMsg = Rsa.Encrypt(responseData, nClient, eClient);

                        // Convert array of inte to array of  byte
                        byte[] encodeBytes = convertIntToByte(encodeMsg);

                        // Send the message to the connected TcpServer. 
                        stream.Write(encodeBytes, 0, encodeBytes.Length);

                        Console.WriteLine("Sent: {0}", responseData);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
        }

        public void getClietnPubAndSendServerPub() {
            try
            {
                bytes = new Byte[256];

                // TcpListener server = new TcpListener(iPAddress,port);
                tcpListener = new TcpListener(iPAddress, port);

                // TcpListener Start
                tcpListener.Start();

                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                client = tcpListener.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                stream = client.GetStream();

   //---------------------------- Exchange n client and server  ----------------------------------
                // get cleient => n
                i = stream.Read(bytes, 0, bytes.Length);
                responseData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                nClient = Int32.Parse(responseData, 0);

                // Send server,s public key to client => n
                bytes = System.Text.Encoding.ASCII.GetBytes(nServer.ToString());
                stream.Write(bytes, 0, bytes.Length);

   //---------------------------- Exchange e client and server  ----------------------------------
                // get client => e
                i = stream.Read(bytes, 0, bytes.Length);
                responseData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                eClient = Int32.Parse(responseData, 0);

                // Send server,s public key to client => e
                bytes = System.Text.Encoding.ASCII.GetBytes(eServer.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }

        // deleteZero(byte[] variable) for delete null elements.
        public byte[] deleteZeoBytes(byte[] packet)
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }

        // Converting array bytes to array ints.
        public int[] convertByteToint(byte[] bts) {
            var size = bts.Count() / sizeof(int);
            var cipherInts = new int[size];
            for (var index = 0; index < size; index++)
            {
                cipherInts[index] = BitConverter.ToInt32(bts, index * sizeof(int));
            }

            return cipherInts;
        }

        // Convert array of int to array of byte
        public byte[] convertIntToByte(int[] encodeInt)
        {
            byte[] encodeBytes = new byte[encodeInt.Length * sizeof(int)];
            Buffer.BlockCopy(encodeInt, 0, encodeBytes, 0, encodeBytes.Length);

            return encodeBytes;
        }
    }
}
