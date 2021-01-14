using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_System {
    class Features {

        public string source { get; set; }
        public string destination { get; set; }
        public Features(List<Package> package_list) {
            this.source = package_list[0].ip_src; //Defines the machine source
            this.destination = package_list[0].ip_dst; //Defines the macine destination
        }
        public double Duration(List<Package> package_list) {
            return (package_list[package_list.Count - 1].relative_time - package_list[0].relative_time); //Last - First relative time =  total time of the connection
        }
        public int[] Src_dst_bytes(List<Package> package_list) {
            int src_bytes = 0; //total bytes sended by source
            int dst_bytes = 0; //total bytes sended by destination
            for (int count = 0; count < package_list.Count; count++) {
                if (package_list[count].ip_src == this.source) { src_bytes += package_list[count].length; }
                else { dst_bytes += package_list[count].length; }
            }
            int[] aux_vector = { src_bytes, dst_bytes };
            return (aux_vector); //returns both value inside a vector
        }
        public int Land(List<Package> package_list) {
            if (this.source == this.destination) { return (1); }
            else { return (0); }
        }
        public string Flags(List<Package> package_list) {
            int buffer_syn_src = 0; ////buffers stores the state of the conection depending on every flag of every package
            int buffer_synack_dst = 0;
            int buffer_ack_src = 0;
            int buffer_ack_dst = 0;
            int buffer_fin_src = 0;
            int buffer_fin_dst = 0;
            int buffer_rst_src = 0;
            int buffer_rst_dst = 0;
            for (int count = 0; count < package_list.Count; count++) {
                if (package_list[count].flag == "0x00000002") { ////check the code for the flag
                    package_list[count].flag = "SYN"; ////translated to be used later
                    if (package_list[count].ip_src == this.source) { buffer_syn_src = 1; } ////register the existance of the flag
                }
                else if (package_list[count].flag == "0x00000012") {
                    package_list[count].flag = "SYN-ACK";
                    if (package_list[count].ip_src != this.source) { buffer_synack_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000010") {
                    package_list[count].flag = "ACK";
                    if (package_list[count].ip_src == this.source) { buffer_ack_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_ack_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000018") {
                    package_list[count].flag = "PSH-ACK";
                    if (package_list[count].ip_src == this.source) { buffer_ack_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_ack_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000011") {
                    package_list[count].flag = "FIN";
                    if (package_list[count].ip_src == this.source) { buffer_fin_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_fin_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000019") {
                    package_list[count].flag = "FIN-PSH-ACK";
                    if (package_list[count].ip_src == this.source) { buffer_fin_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_fin_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000004") {
                    package_list[count].flag = "RST";
                    if (package_list[count].ip_src == this.source) { buffer_rst_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_rst_dst = 1; }
                }
                else if (package_list[count].flag == "0x00000038") {
                    package_list[count].flag = "PSH-ACK_URG";
                    if (package_list[count].ip_src == this.source) { buffer_ack_src = 1; }
                    if (package_list[count].ip_src != this.source) { buffer_ack_dst = 1; }
                }
                else if (package_list[count].flag == "") {
                    package_list[count].flag = "UDP-Null";
                }
                else if (package_list[count].flag == "0x00000014") {
                    package_list[count].flag = "RST-ACK";
                }
            }
            //Process the flags registered in the conection to return the corresponding behavior of the packages
            if (buffer_syn_src == 1 && buffer_synack_dst == 0) { return ("S0"); } //connection tryed.. no answer
            else if (buffer_syn_src == 1 && buffer_synack_dst == 1) {//conexão estabelecida
                if (buffer_rst_src == 1 && buffer_rst_dst == 0) { return ("RSTO"); }//source aborted the connection
                else if (buffer_rst_src == 0 && buffer_rst_dst == 1) { return ("RSTR"); }//destination aborted the connection
                else if (buffer_fin_src == 0 && buffer_fin_dst == 0) { return ("S1"); }//connected, not finished
                else if (buffer_fin_src == 1 && buffer_fin_dst == 0) { return ("S2"); }//connected, finished, no answer from destination
                else if (buffer_ack_src == 0 && buffer_ack_dst == 1) { return ("S3"); }//connected, finished, no answer from source
                else if (buffer_syn_src == 1 && buffer_synack_dst == 1) { return ("SF"); } //Conexão normal
            }
            else if (buffer_syn_src == 1 && buffer_rst_src == 1 && buffer_synack_dst == 0) { return ("RSTRH"); }//dst answered and aborted
            else if (buffer_syn_src == 1 && buffer_fin_src == 1 && buffer_synack_dst == 0) { return ("SH"); }//src send a syn and aborted
            else if (buffer_syn_src == 0 && buffer_fin_src == 0 && buffer_fin_dst == 0) { return ("OTH"); }//traffic without SYN
            else if (buffer_syn_src == 1 && buffer_synack_dst == 0 && buffer_rst_dst == 1) { return ("REJ"); }//connection rejected
            return ("NaN");
        }
        public string Service(List<Package> package_list) {
            foreach(Package pkg in package_list) {
                if(pkg.service != "TCP") { return (pkg.service); }
            }
            return (package_list[0].service);
        }
        public string Protocol(List<Package> package_list) {
            if (package_list[0].protocol_type == 6) { return ("TCP"); }
            else if (package_list[0].protocol_type == 17) { return ("UDP"); }
            else if (package_list[0].protocol_type == 1) { return ("ICMP"); }
            return ("None");
        }
        public int[] Len_Win_Urg_Clas(List<Package> package_list) {
            int total_window = 0;
            int total_length = 0;
            int urgent = 0;
            int classe = 0;
            foreach(Package pkg in package_list) {
                total_window += pkg.window_size;
                total_length += pkg.length;
                if(pkg.urgent == 1) { urgent = 1; }
            }
            int[] aux = { total_length, total_window, urgent };
            return (aux);
        }
        public int[] srvcount(List<Package> package_list) {
            double start_time1 = 0;
            double start_time2 = 0;
            double end_time1 = 2;
            double end_time2 = 2;

            int feat_count = 0;
            int serror_rate = 0;
            int rerror_rate = 0;
            int same_srv_rate = 0;
            int diff_srv_rate = 0;

            int srv_count = 0;
            int srv_serror_rate = 0;
            int srv_rerror_rate = 0;
            int srv_diff_host_rate = 0;

            for (int count = 0; count < package_list.Count; count++) {
                if (package_list[count].ip_src == this.source) {
                    if (package_list[count].relative_time >= start_time1 && package_list[count].relative_time <= end_time1) {
                        feat_count += 1;
                        if (package_list[count].flag == "SYN") {
                            if (package_list[count + 1].flag == "RST") { rerror_rate += 1; }
                            else if (package_list[count + 1].flag != "SYN-ACK") { serror_rate += 1; }
                        }
                        if(package_list[count].src_port == package_list[count].dst_port) { same_srv_rate += 1; }
                        else { diff_srv_rate += 1; }
                    }
                    else { 
                        start_time1 = package_list[count].relative_time;
                        end_time1 = package_list[count].relative_time + 2;
                    }
                }
                if(package_list[count].relative_time >= start_time2 && package_list[count].relative_time <= end_time2) {
                    if(package_list[count].src_port == package_list[count].dst_port) {
                        srv_count += 1;
                        if(package_list[count].flag == "SYN") {
                            if(package_list[count+1].flag == "RST") { srv_rerror_rate += 1; }
                            else if(package_list[count+1].flag == "SYN-ACK") { srv_serror_rate += 1; }
                        }
                        if(package_list[count].ip_src != package_list[count].ip_dst) { srv_diff_host_rate += 1; }
                    }
                }
                else {
                    start_time2 = package_list[count].relative_time;
                    end_time2 = package_list[count].relative_time + 2;
                }
            }
            int[] aux = { feat_count, srv_count, serror_rate, srv_serror_rate, rerror_rate, srv_rerror_rate, same_srv_rate, diff_srv_rate, srv_diff_host_rate };
            return (aux);
        }
    }
}
