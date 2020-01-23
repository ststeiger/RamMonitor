
namespace RamMonitorPrototype.Trash
{
    
    
    public class IPConverter
    {
        
        
        public static void Test()
        {
            Ip2Num(127, 0, 0, 1);
        }

        public static uint ConvertFromIpAddressToInteger(string ipAddress)
        {
            System.Net.IPAddress address = System.Net.IPAddress.Parse(ipAddress);
            byte[] bytes = address.GetAddressBytes();

            // flip big-endian(network order) to little-endian
            if (System.BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(bytes);
            }

            return System.BitConverter.ToUInt32(bytes, 0);
        }
        
        
        public static string ConvertFromIntegerToIpAddress(uint ipAddress)
        {
            byte[] bytes = System.BitConverter.GetBytes(ipAddress);

            // flip little-endian to big-endian(network order)
            if (System.BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(bytes);
            }

            // IP addresses are in network order (big-endian), while ints are little-endian on Windows,
            // so to get a correct value, you must reverse the bytes before converting on a little-endian system.

            return new System.Net.IPAddress(bytes).ToString();
        }
        
        
        public static void Ip2Num(int first, int second, int third, int fourth)
        {
            // int num =  (first << 24) | (second << 16) | (third << 8) | (fourth);
            int num = (fourth << 24) | (third << 16) | (second << 8) | (first);

            long numIP = System.Net.IPAddress.Parse($"{first}.{second}.{third}.{fourth}").Address;

            // Here's a pair of methods to convert from IPv4 to a correct integer and back:
            uint foo = ConvertFromIpAddressToInteger($"{first}.{second}.{third}.{fourth}");
            
            System.Console.WriteLine(num);
            System.Console.WriteLine(numIP);
            System.Console.WriteLine(foo);
        }
        
        
    }
}