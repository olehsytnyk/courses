using System;

namespace STP.RabbitMq
{
    public class RabbitMQOptions
    {
        // Client TODO services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
        //
        // Summary:
        //     Sets or gets the AMQP Uri to be used for connections.
        public string HostName { get; set; }
        //
        // Summary:
        //     Virtual host to access during this connection.
        public string VirtualHost { get; set; }
        //
        // Summary:
        //     Username to use when authenticating to the server.
        public string UserName { get; set; }
        //
        // Summary:
        //     Password to use when authenticating to the server.
        public string Password { get; set; }
        /// <summary>
        /// The port to connect on.
        /// </summary>
        public int Port { get; set; }
    }
}
