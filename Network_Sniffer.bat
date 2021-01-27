@ECHO OFF
cd "c:\Program Files\Wireshark"
ECHO "Interfaces Disponíveis:"
tshark -D
ECHO
ECHO
ECHO "Chosing network interface 5: (Change in the network_sniffer.bat file)"
tshark -i 6 -q -T fields -e tcp.stream -e udp.stream -e frame.time_relative -e ip.proto -e _ws.col.Protocol -e tcp.flags -e tcp.urgent_pointer -e frame.cap_len  -e ip.flags -e tcp.window_size_value -e tcp.srcport -e tcp.dstport -e udp.srcport -e udp.dstport -e ip.src -e ip.dst -E header=n -E separator=, -E occurrence=f > "C:\VS_Projects\First_System\network_dump.csv"