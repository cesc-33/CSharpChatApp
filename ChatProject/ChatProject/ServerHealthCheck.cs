using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatProject
{
    public class ServerHealthCheck
    {
        public static async Task<bool> IsServerAvailable(string host, int port, int timeout = 2000)
        {
            if (!await PingHost(host)) return false;
            return await IsPortOpen(host, port, timeout);
        }

        private static async Task<bool> PingHost(string host)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(host);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> IsPortOpen(string host, int port, int timeout)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = Task.Delay(timeout);
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    return completedTask == connectTask && client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
    }


}
