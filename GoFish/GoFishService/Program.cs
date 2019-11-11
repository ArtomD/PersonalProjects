using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;  // WCF types
using GoFishLibrary;
using System.Net;

namespace GoFishService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost servHost = null;
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Console.WriteLine(myIP);
            try
            {
                // Register the service Address
                //servHost = new ServiceHost(typeof(FishService), new Uri());
                NetTcpBinding binding = new NetTcpBinding();
                binding.Name = "FishService";
                binding.Security.Mode = SecurityMode.None;
                string uri = "net.tcp://" + myIP + ":13200/CardsLibrary/FishService";
                servHost = new ServiceHost(typeof(FishService));
                // Register the service Contract and Binding
                // servHost.AddServiceEndpoint(typeof(IFishService), new NetTcpBinding(), "FishService");

                // Run the service
                servHost.AddServiceEndpoint(typeof(IFishService), binding, uri);
                servHost.Open();
                Console.WriteLine("Service started. Press any key to quit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Wait for a keystroke
                Console.ReadKey();
                if (servHost != null)
                    servHost.Close();
            }
        }
    }
}


