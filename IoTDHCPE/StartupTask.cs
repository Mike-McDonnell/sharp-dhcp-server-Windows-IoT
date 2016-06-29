using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using DotNetProjects.DhcpServer;
using System.Net;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IoTDHCPE
{
    public sealed class StartupTask : IBackgroundTask
    {
 

        public void Run(IBackgroundTaskInstance taskInstance)
        {

            var server = new DHCPServer(new System.Net.IPAddress(new byte[] {192, 168, 137, 1}));
            
            server.ServerName = "fontanaWTEDHCP";
            server.BroadcastAddress = IPAddress.Broadcast.ToString();  //"192.168.137.255";

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            taskInstance.Canceled += (o, e) => { server.Dispose(); deferral.Complete(); };

            server.OnDataReceived += (sender, dhcpRequest) =>
            {
                try
                {
                    var type = dhcpRequest.GetMsgType();
                    var ip = new System.Net.IPAddress(new byte[] { 192, 168, 137, 11 });

                    var replyOptions = new DHCPReplyOptions();
                    replyOptions.SubnetMask = IPAddress.Parse("255.255.255.0");
                    replyOptions.DomainName = server.ServerName;
                    replyOptions.ServerIdentifier = IPAddress.Parse("192.168.137.1");
                    replyOptions.RouterIP = IPAddress.Parse("192.168.137.1");
                    replyOptions.DomainNameServers = new IPAddress[]
                    {IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.4.4")};


                    if(type == DHCPMsgType.DHCPDISCOVER)
                        dhcpRequest.SendDHCPReply(DHCPMsgType.DHCPOFFER, ip, replyOptions);
                    if (type == DHCPMsgType.DHCPREQUEST)
                        dhcpRequest.SendDHCPReply(DHCPMsgType.DHCPACK, ip, replyOptions);
                }
                catch
                { }

            };

            server.Start();
      
        }
    }
}
