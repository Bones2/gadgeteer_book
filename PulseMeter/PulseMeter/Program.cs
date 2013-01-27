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
using Gadgeteer.Modules.Seeed;

namespace PulseMeter
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            pulseOximeter.Heartbeat += new PulseOximeter.HeartbeatHandler(pulseOximeter_Heartbeat);
            DisplayPulseRate(0);
        }

        void pulseOximeter_Heartbeat(PulseOximeter sender, PulseOximeter.Reading reading)
        {
            lED7R.Animate(50, true, true, false);
            DisplayPulseRate(reading.PulseRate);
        }

        private void DisplayPulseRate(int p)
        {
            display.SimpleGraphics.Clear();
            display.SimpleGraphics.DisplayText("Pluse: " + p,
                Resources.GetFont(Resources.FontResources.NinaB), Colors.Green, 20, 100);
        }
    }
}
