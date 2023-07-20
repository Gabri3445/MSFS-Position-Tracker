using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace PositionTrackerPlugin
{
    internal class Program
    {
        private static Timer _timer;
        private static SimConnect simconnect = null;
        
        static void Main(string[] args)
        {

            const int WM_USER_SIMCONNECT = 0x0402;
            try
            {
                simconnect = new SimConnect("Managed Scenario Controller", IntPtr.Zero, 0x402, null, 0);
            }
            catch (COMException ex)
            {

            }
            
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "HEADING INDICATOR", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "INDICATED ALTITUDE CALIBRATED", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GROUND ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "GROUND VELOCITY", "Knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);
            simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecvSimobjectDataBytype);

            _timer = new Timer(Timer_Tick, null, 0, 500);
            
            while (true)
            {
                
            }
        }

        private static void Timer_Tick(object? o)
        {
            simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            simconnect.ReceiveMessage();
        }
        
        private enum DATA_REQUESTS
        {
            REQUEST_1
        }

        private static void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            Console.WriteLine("giglo");
            if (data.dwRequestID == 0)
            {
                Struct1 struct1 = (Struct1)data.dwData[0];
                Console.WriteLine("Lat: " + struct1.latitude);
                Console.WriteLine("Lon: " + struct1.longitude);
                Console.WriteLine("Hdg: " + struct1.trueheading);
                Console.WriteLine("Alt: " + struct1.altitude);
                Console.WriteLine("Grd Alt: " + struct1.groundaltitude);
                Console.WriteLine("Spd: " + struct1.groundvelocity);
            }
        }
        
        private enum DEFINITIONS
        {
            Struct1
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Struct1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string title;
            public double latitude;
            public double longitude;
            public double trueheading;
            public double altitude;
            public double groundaltitude;
            public double groundvelocity;
        }
    }
}