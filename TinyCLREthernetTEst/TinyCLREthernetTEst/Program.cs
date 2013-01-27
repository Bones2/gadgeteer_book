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


namespace TinyCLREthernetTEst
{
    public partial class Program
    {
        Gadgeteer.Networking.WebEvent hello;
        Gadgeteer.Networking.WebEvent takePicture;
        Gadgeteer.Networking.WebEvent seePicture;

        GT.Picture pic = null;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            ethernet.UseDHCP();

            ethernet.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);

            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            pic = picture;
        }

        void ethernet_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("IP:" + ethernet.NetworkSettings.IPAddress.ToString());

            Gadgeteer.Networking.WebServer.StartLocalServer(ethernet.NetworkSettings.IPAddress, 80);

            foreach (string s in sender.NetworkSettings.DnsAddresses)
                Debug.Print("Dns:" + s);

            hello = Gadgeteer.Networking.WebServer.SetupWebEvent("hello");
            hello.WebEventReceived += new WebEvent.ReceivedWebEventHandler(hello_WebEventReceived);

            takePicture = Gadgeteer.Networking.WebServer.SetupWebEvent("takepicture");
            takePicture.WebEventReceived += new WebEvent.ReceivedWebEventHandler(takePicture_WebEventReceived);

            seePicture = Gadgeteer.Networking.WebServer.SetupWebEvent("seepicture");
            seePicture.WebEventReceived += new WebEvent.ReceivedWebEventHandler(seePicture_WebEventReceived);
        }

        void seePicture_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            if (pic != null)
            {
                Bitmap b = new Bitmap(320, 240);
                b.DrawLine(Colors.Red, 20, 0, 0, 319, 239);
                byte[] buff = new byte[320 * 240 * 3 + 54];
                GHIElectronics.NETMF.System.Util.BitmapToBMPFile(b.GetBitmap(), 320, 240, buff);

                GT.Picture picture = new GT.Picture(buff, GT.Picture.PictureEncoding.BMP);

                responder.Respond(picture);
            }
            else
                responder.Respond("Take picture first");
        }

        void takePicture_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            responder.Respond("Taking picture");
            camera.TakePicture();
        }

        void hello_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            Debug.Print("Hello world");
            responder.Respond("Hello world");
        }
    }
}
