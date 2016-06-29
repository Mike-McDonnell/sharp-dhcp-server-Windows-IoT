namespace DotNetProjects.DhcpServer
{
    /// <summary>DHCP relay information (option 82)</summary>
    internal sealed class RelayInfo
    {
        /// <summary>Agent circuit ID</summary>
        internal byte[] AgentCircuitID;
        /// <summary>Agent remote ID</summary>
        internal byte[] AgentRemoteID;
    }
}