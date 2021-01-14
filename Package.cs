using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_System {
    class Package {
        public double relative_time { get; set; }
        public int protocol_type { get; set; }
        public string service { get; set; }
        public string flag { get; set; }
        public int length { get; set; }
        public string ip_flag { get; set; }
        public string ip_src { get; set; }
        public string ip_dst { get; set; }
        public int src_port { get; set; }
        public int dst_port { get; set; }
        public int stream { get; set; }
        public int window_size { get; set; }
        public int urgent { get; set; }

        public Package() { }
        public Package(string[] lines) { //takes the package as string lines to assemble the package (used in assemble_packages function
            try {
                this.relative_time = double.Parse(lines[2]);
                this.protocol_type = int.Parse(lines[3]);
                this.service = lines[4];
                this.flag = lines[5];
                this.length = int.Parse(lines[7]);
                this.ip_flag = lines[8];
                this.ip_src = lines[14];
                this.ip_dst = lines[15];
                if (this.protocol_type == 6 || (this.protocol_type == 1 && lines[0] != "")) { // checks if the protocol is tcp or icmp
                    this.stream = int.Parse(lines[0]);
                    this.src_port = int.Parse(lines[10]);
                    this.dst_port = int.Parse(lines[11]);
                    this.window_size = int.Parse(lines[9]);
                    this.urgent = int.Parse(lines[6]);
                }
                else if (this.protocol_type == 17 || (this.protocol_type == 1 && lines[1] != "")) {//udp or icmp
                    this.stream = int.Parse(lines[1]);
                    this.src_port = int.Parse(lines[12]);
                    this.dst_port = int.Parse(lines[13]);
                    this.window_size = 0;
                    this.urgent = 0;
                }
            }
            finally {
                //print error message
            }
        }
        
        public List<Package> Assemble_packages(List<string> lines) {
            List<Package> assembled_packages = new List<Package>();
            List<string[]> raw_packages = new List<string[]>();
            foreach(string line in lines) {
                string[] aux = line.Split(','); //Every attribute of the package are separated by commas
                raw_packages.Add(aux); // each position in raw_packages have a vector that contains every attribute of every package
            }
            //Console.WriteLine(raw_packages[0][1]);
            for (int single_line = 0; single_line < raw_packages.Count; single_line++) {
                if(raw_packages[single_line].Length == 16) { //Check if the package was totatly captured (16 attributes) #see sniffing process
                    if(raw_packages[single_line][3] == "6" || raw_packages[single_line][3] == "17" || raw_packages[single_line][3]=="1") { //check if tcp, udp or icmp
                        if(raw_packages[single_line][0] != "" || raw_packages[single_line][1] != "") {// check if the package has a valid stream number (0=tcp,1=udp)
                            Package obj_package = new Package(raw_packages[single_line]); //transform the vector of attributes in a package itself
                            assembled_packages.Add(obj_package); //add the package in the assebled vector
                        }
                    }
                }
            }
            return (assembled_packages); //return the new packages to the main system
        }
        public override string ToString() {
            return ("Package Obj: "+this.stream + "," + this.protocol_type + "," + this.ip_src);
        }
    }
}
