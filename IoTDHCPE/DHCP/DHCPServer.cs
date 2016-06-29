using System;
using System.Net;
//using System.Net;
using System.Net.Sockets;
using System.Threading;
using Windows.Networking.Sockets;

namespace DotNetProjects.DhcpServer
{
    /// <summary>
    /// DHCP Server
    /// </summary>
    internal sealed class DHCPServer //: IDisposable
    {
        /// <summary>Delegate for DHCP message</summary>
        //internal delegate void DHCPDataReceivedEventHandler(DHCPRequest dhcpRequest);

        /// <summary>Will be called on any DHCP message</summary>
        internal event EventHandler<DHCPRequest> OnDataReceived; // = delegate { };
        /// <summary>Will be called on any DISCOVER message</summary>
        internal event EventHandler<DHCPRequest> OnDiscover; // = delegate { };
        /// <summary>Will be called on any REQUEST message</summary>
        internal event EventHandler<DHCPRequest> OnRequest; // = delegate { };
        /// <summary>Will be called on any DECLINE message</summary>
        internal event EventHandler<DHCPRequest> OnDecline; // = delegate { };
        /// <summary>Will be called on any DECLINE released</summary>
        internal event EventHandler<DHCPRequest> OnReleased; // = delegate { };
        /// <summary>Will be called on any DECLINE inform</summary>
        internal event EventHandler<DHCPRequest> OnInform; // = delegate { };

        /// <summary>Server name (optional)</summary>
        internal string ServerName { get; set; }

        private DatagramSocket socket = null;
        //private Thread receiveDataThread = null;
        private const int PORT_TO_LISTEN_TO = 67;
        private IPAddress _bindIp;

        internal event EventHandler<Exception> UnhandledException;

        internal string BroadcastAddress { get; set; }

        /// <summary>
        /// Creates DHCP server, it will be started instantly
        /// </summary>
        /// <param name="bindIp">IP address to bind</param>
        internal DHCPServer(IPAddress bindIp)
        {
            _bindIp = bindIp;
        }

        /// <summary>Creates DHCP server, it will be started instantly</summary>
        internal DHCPServer() : this(IPAddress.Any)
        {
            BroadcastAddress = IPAddress.Broadcast.ToString();
        }

        internal async void Start()
        {
            try
            {
                var DataSocket = new Windows.Networking.Sockets.DatagramSocket();

                DataSocket.MessageReceived += DataSocket_MessageReceived;

                await DataSocket.BindEndpointAsync(new Windows.Networking.HostName(_bindIp.ToString()), PORT_TO_LISTEN_TO.ToString());

            }
            catch(Exception ex)
            {
                if (UnhandledException != null)
                    UnhandledException(this, ex);
            }
        }

        /// <summary>Disposes DHCP server</summary>
        internal void Dispose()
        {
            if (socket != null)
            {
                //socket.Close();
                socket.Dispose();
                socket = null;
            }
        }

        //private void DataReceived(object o)
        private void DataSocket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                var reader = args.GetDataReader();
               
                byte[] data = new byte[reader.UnconsumedBufferLength];
                args.GetDataReader().ReadBytes(data);

                this.socket = sender;

                var dhcpRequest = new DHCPRequest(data, socket, this);
                //ccDHCP = new clsDHCP();


                //data is now in the structure
                //get the msg type
                OnDataReceived(this, dhcpRequest);
                var msgType = dhcpRequest.GetMsgType();
                switch (msgType)
                {
                    case DHCPMsgType.DHCPDISCOVER:
                        OnDiscover(this, dhcpRequest);
                        break;
                    case DHCPMsgType.DHCPREQUEST:
                        OnRequest(this, dhcpRequest);
                        break;
                    case DHCPMsgType.DHCPDECLINE:
                        OnDecline(this, dhcpRequest);
                        break;
                    case DHCPMsgType.DHCPRELEASE:
                        OnReleased(this, dhcpRequest);
                        break;
                    case DHCPMsgType.DHCPINFORM:
                        OnInform(this, dhcpRequest);
                        break;
                    //default:
                    //    Console.WriteLine("Unknown DHCP message: " + (int)MsgTyp + " (" + MsgTyp.ToString() + ")");
                    //    break;
                }
            }
            catch (Exception ex)
            {
                if (UnhandledException != null)
                    UnhandledException(this, ex);
            }
        }
    }
}