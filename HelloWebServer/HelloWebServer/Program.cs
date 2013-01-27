using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace HelloWebServer
{
    public partial class Program
    {
        GT.Networking.WebEvent sayHello;

        void ProgramStarted()
        {
            ethernet.UseDHCP();
            ethernet.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);
            ethernet.NetworkDown += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkDown);
            led.TurnBlue();
        }


        void ethernet_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            led.TurnGreen();
            string ipAddress = ethernet.NetworkSettings.IPAddress;
            WebServer.StartLocalServer(ipAddress, 80);
            sayHello = WebServer.SetupWebEvent("hello");
            sayHello.WebEventReceived += new WebEvent.ReceivedWebEventHandler(sayHello_WebEventReceived);
        }

        void sayHello_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            string content = "<html><body><h1>Hello World!!</h1></body></html>";
            byte[] bytes = new System.Text.UTF8Encoding().GetBytes(content);
            responder.Respond(bytes, "text/html");
        }


        void ethernet_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            led.TurnRed();
        }


    }
}
