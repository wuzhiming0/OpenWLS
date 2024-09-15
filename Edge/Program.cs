// See https://aka.ms/new-console-template for more information

using OpenWLS.Edge;
using System.Data.Entity.Core.Metadata.Edm;

EdgeServer server = new EdgeServer();
server.StartServer();
while (!server.StopServer)
{
    Thread.Sleep(1000);
}