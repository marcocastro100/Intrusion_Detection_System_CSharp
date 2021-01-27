using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace First_System {
    class Program {
        static void Main(string[] args) {
            //Configuration Vars
            string path_sniffer = @"c:\VS_Projects\First_System\First_System\";// path to sniffer script
            Process.Start(path_sniffer+"Network_Sniffer.bat"); //cal the sniffer script with Tshark
            Thread.Sleep(10000); // pause the system for 10 secs to initialize the scrip correctly

            Sys obj_system = new Sys(); //System function to structure data into the sustem
            while (true) {
                obj_system.Check_network(); //Makes the structuration of the packages in the dump file to system strucutured data
                obj_system.Check_activity();//MOnitor the streams in the system in order to run the IA program to analise it            }
            }
            //Handle the parameters Entered in command line mode
        }
    }
    //tshark -i 5 -q -T fields -e tcp.stream -e udp.stream -e frame.time_relative -e ip.proto -e _ws.col.Protocol -e tcp.flags -e tcp.urgent_pointer -e frame.cap_len  -e ip.flags -e tcp.window_size_value -e tcp.srcport -e tcp.dstport -e udp.srcport -e udp.dstport -e ip.src -e ip.dst -E header=n -E separator=, -E occurrence=f > "C:\VS_Projects\First_System\network_dump.csv"

}
