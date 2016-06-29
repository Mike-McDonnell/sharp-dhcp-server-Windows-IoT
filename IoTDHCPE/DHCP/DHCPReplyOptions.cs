using System;
using System.Collections.Generic;
using System.Net;

namespace DotNetProjects.DhcpServer
{
    /// <summary>Reply options</summary>
    internal sealed class DHCPReplyOptions
    {
        /// <summary>IP address</summary>
        internal IPAddress SubnetMask = null;
        /// <summary>Next Server IP address (bootp)</summary>
        internal IPAddress ServerIpAddress = null;
        /// <summary>IP address lease time (seconds)</summary>
        internal UInt32 IPAddressLeaseTime = 60 * 60 * 24;
        /// <summary>Renewal time (seconds)</summary>
        internal UInt32? RenewalTimeValue_T1 = 60 * 60 * 24;
        /// <summary>Rebinding time (seconds)</summary>
        internal UInt32? RebindingTimeValue_T2 = 60 * 60 * 24;
        /// <summary>Domain name</summary>
        internal string DomainName = null;
        /// <summary>IP address of DHCP server</summary>
        internal IPAddress ServerIdentifier = null;
        /// <summary>Router (gateway) IP</summary>
        internal IPAddress RouterIP = null;
        /// <summary>Domain name servers (DNS)</summary>
        internal IPAddress[] DomainNameServers = null;
        /// <summary>Log server IP</summary>
        internal IPAddress LogServerIP = null;
        /// <summary>Static routes</summary>
        internal NetworkRoute[] StaticRoutes = null;
        /// <summary>Other options which will be sent on request</summary>
        internal Dictionary<DHCPOption, byte[]> OtherRequestedOptions = new Dictionary<DHCPOption, byte[]>();
    }
}