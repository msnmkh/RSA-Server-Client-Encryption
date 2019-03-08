using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace clientConsole
{
    class Client
    {
        private static int eServer;
        private static int nServer;
        private static int eClient;
        private static int nClient;
        private static int dClient;
        TcpClient client;
        private string localIP;
        private Int32 port;
        NetworkStream stream;
        string responseData;
        int i;

        // Buffer for reading data
        Byte[] bytes = new Byte[256];

        public Client(string localIP, Int32 port)
        {
            this.localIP = localIP;
            this.port = port;

            // Create list of primes;
            var primes = new Primes();

            // Generate key n,d,e with two random primes for encrypt and decrypt 
            var keys = primes.GetKey();

            // The public key is (e, n) and the private key is (d, n).
            dClient = keys[0];
            eClient = keys[1];
            nClient = keys[2];
        }

        public void connectToServer()
        {
            try
            {
                // Create a TcpClient.
                client = new TcpClient();

                // Connect to server.
                client.Connect(localIP, port);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();
                stream = client.GetStream();

                //---------------------------- Exchange n client and server ----------------------------------
                // Send client's public key to server => n
                bytes = System.Text.Encoding.ASCII.GetBytes(nClient.ToString());
                stream.Write(bytes, 0, bytes.Length);

                // Get server's public key => n
                i = stream.Read(bytes, 0, bytes.Length);
                responseData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                nServer = Int32.Parse(responseData, 0);

                //---------------------------------- Exchange e client and server --------------------------------
                // Send client's public key to server => e
                bytes = System.Text.Encoding.ASCII.GetBytes(eClient.ToString());
                stream.Write(bytes, 0, bytes.Length);

                // Get server's public key => e
                i = stream.Read(bytes, 0, bytes.Length);
                responseData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                eServer = Int32.Parse(responseData, 0);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public void sendMessage(string message)
        {
             try
             {
                // Encrypt message
                var encodeMessage = Rsa.Encrypt(message, nServer, eServer);

                // Convert array of inte to array of  byte
                byte[] encodeBytes = convertIntToByte(encodeMessage);

                // Send the message to the connected TcpServer. 
                stream.Write(encodeBytes, 0, encodeBytes.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.
                // Buffer to store the response bytes.
                bytes = new Byte[256];

                i = stream.Read(bytes, 0, bytes.Length);

                // deleteZero(byte[] variable) for delete null elements.
                Byte[] newbytes = deleteZeoBytes(bytes);

                // Converting array bytes to array ints.
                var cipherInts = convertByteToint(newbytes);

                // Encode message
                string encodeMsg = Rsa.Decrypt(cipherInts, nClient, dClient);

                Console.WriteLine("Received: {0}", encodeMsg);

            }
            catch (ArgumentNullException e)
            {
                 Console.WriteLine("ArgumentNullException: {0}", e);
              }
             catch (SocketException e)
             {
            Console.WriteLine("SocketException: {0}", e);
            }
        }

        // Convert array of int to array of byte
        public byte[] convertIntToByte(int[] encodeInt)
        {
            byte[] encodeBytes = new byte[encodeInt.Length * sizeof(int)];
            Buffer.BlockCopy(encodeInt, 0, encodeBytes, 0, encodeBytes.Length);

            return encodeBytes;
        }

        // Converting array bytes to array ints.
        public int[] convertByteToint(byte[] bts)
        {
            var size = bts.Count() / sizeof(int);
            var cipherInts = new int[size];
            for (var index = 0; index < size; index++)
            {
                cipherInts[index] = BitConverter.ToInt32(bts, index * sizeof(int));
            }

            return cipherInts;
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
    }
}
