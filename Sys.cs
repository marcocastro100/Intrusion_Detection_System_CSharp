using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using First_System;
//using Package,learning,stream,features,common

namespace First_System {
    class Sys {
        //Settings
        private string src_dump = @"C:\VS_Projects\First_System\network_dump.csv"; //Location of dump file
        private string dst_dump = @"C:\VS_Projects\First_System\dump_read.csv"; //Location of dump file
        private int max_hold_time = 1; //Time to consider a stream finalized or dead

        //Control Vars
        int last_pkg_readed = 0; //holds the last package read to not insert packages infinitely in the system
        private List<Stream> streams_tcp = new List<Stream>();   //holds the streams (packages of a same connection)
        private List<int> existing_streams_tcp = new List<int>(); //holds the stream number active on the system
        private List<int> done_streams_tcp = new List<int>(); //holds stream number of already analised streams

        public Sys() {} //Contructor

        public void Check_network() {  
            File.Delete(this.dst_dump); //delete the copy file if he already exists (otherwise error while copying)
            File.Copy(this.src_dump, this.dst_dump); //Copy the dump file to a read_dump file (to not interfere into the sniffing process)
            IEnumerable<string> allines = File.ReadLines(this.dst_dump);//read all lines in the network package dump file
            IEnumerable<string> cutDown = allines.Skip(this.last_pkg_readed); //skip already readed lines
            List<string> lines = new List<string>();
            foreach (string s in cutDown) { lines.Add(s); } //adds the lines in a list
            this.last_pkg_readed += lines.Count; //updates var to not read the same packages again

            Package obj_package = new Package(); // instiate a package obj to use the functions
            List<Package> assembled_packages = obj_package.Assemble_packages(lines); //convert dump lines to packages            
            this.Redirect_packages(assembled_packages); //sends the now packages to the strucutrator method
        }

        public void Redirect_packages(List<Package> assembled_packages_list) {
            int tcp_protocol = 6; //Code convention for tcp
            foreach (Package obj_package in assembled_packages_list) { //Insert each package in the corresponding stream
                if (!this.existing_streams_tcp.Contains(obj_package.stream) && !this.done_streams_tcp.Contains(obj_package.stream)) { //If stream number is already in the sistem and isnt done yet
                    if (obj_package.protocol_type == tcp_protocol && obj_package.flag == "0x00000002") { //Check if this is the syn (first Package) #Start a stream with a non stream package dont make sense in network
                        Stream obj_stream = new Stream(obj_package); //instantiate a new stream
                        obj_stream.Add_pkg(obj_package); //add the current package in the list of packages of the stream
                        this.existing_streams_tcp.Add(obj_package.stream); //add the stream number in the existing streams list for control
                        this.streams_tcp.Add(obj_stream); //add the object stream in the stream list for control
                    }
                }
                else if (!this.done_streams_tcp.Contains(obj_package.stream)) { //If the stream alread exists, only insert the package in it
                    for (int count = 0; count < this.streams_tcp.Count; count++) {
                        if (this.streams_tcp[count].index == obj_package.stream) { //look for a matching index
                            this.streams_tcp[count].Add_pkg(obj_package);
                        }
                    }
                }
            }
        }

        public void Check_activity() {
            foreach(Stream obj_stream in streams_tcp) {
                if(obj_stream.package_list.Count > 0 && !this.done_streams_tcp.Contains(obj_stream.index)) { //Check if the stream has packages and if the stream has already been analised
                    if ((DateTime.Now.Subtract(obj_stream.last_modified)).TotalSeconds >= this.max_hold_time){
                        obj_stream.Generate_features();

                        Console.WriteLine("Stream "+obj_stream.index+" engenharia reversa completa => IA");
                        this.done_streams_tcp.Add(obj_stream.index);
                    }
                }
            }
            //Cant remove the obj_stream while in the loop.. hold all the finalized to remove after the loop
            //if so, generate features, send to the IA, predict, show and add to the complete streams list
        }

    }
}
