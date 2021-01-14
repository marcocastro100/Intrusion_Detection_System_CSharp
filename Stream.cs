using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace First_System {
    class Stream {
        public DateTime last_modified { get; set; }
        public int index { get; set; }
        public int protocol_type { get; set; }
        public List<Package> package_list { get; set; }
        public double duration { get; set; }
        public int src_bytes { get; set; }
        public int dst_bytes { get; set; }
        public int land { get; set; }
        public string flag { get; set; }
        public string service { get; set; }
        public string protocol { get; set; }
        public int len { get; set; }
        public int win { get; set; }
        public int urg { get; set; }
        public int count { get; set; }
        public int srv_count { get; set; }
        public int serror_rate { get; set; }
        public int srv_serror_rate { get; set; }
        public int rerror_rate { get; set; }
        public int srv_rerror_rate { get; set; }
        public int same_srv_rate { get; set; }
        public int diff_srv_rate { get; set; }
        public int srv_diff_host_rate { get; set; }

        public Stream(Package package) {
            this.last_modified = DateTime.Now; //start to count the time to consider the stream dead or complete (after that the stream will be send to the IA to analisys)
            this.index = package.stream;
            this.package_list = new List<Package>();
            this.protocol_type = package.protocol_type;
        }

        public void Add_pkg(Package obj_package) { //add a package to the stream
            this.package_list.Add(obj_package);
            this.last_modified = DateTime.Now; //re-start the time to consider finalized
        }
        public void Generate_features() {
            Features obj_features = new Features(this.package_list); //instantiate a new features (class that will generate the IA information)
            this.duration = obj_features.Duration(this.package_list);
            int[] aux_src_dst_bytes = obj_features.Src_dst_bytes(this.package_list);
                this.src_bytes = aux_src_dst_bytes[0];
                this.dst_bytes = aux_src_dst_bytes[1];
            this.land = obj_features.Land(this.package_list);
            this.flag = obj_features.Flags(this.package_list);
            this.service = obj_features.Service(this.package_list);
            this.protocol = obj_features.Protocol(this.package_list);
            int[] aux_len_win_urg = obj_features.Len_Win_Urg_Clas(this.package_list);
                this.len = aux_len_win_urg[0];
                this.win = aux_len_win_urg[1];
                this.urg = aux_len_win_urg[2];
            int[] aux_srv = obj_features.srvcount(this.package_list);
            this.count = aux_srv[0];
            this.srv_count = aux_srv[1];
            this.serror_rate = aux_srv[2];
            this.srv_serror_rate = aux_srv[3];
            this.rerror_rate = aux_srv[4];
            this.srv_rerror_rate = aux_srv[5];
            this.same_srv_rate = aux_srv[6];
            this.diff_srv_rate = aux_srv[7];
            this.srv_diff_host_rate = aux_srv[8];
        }
        
    }
}
