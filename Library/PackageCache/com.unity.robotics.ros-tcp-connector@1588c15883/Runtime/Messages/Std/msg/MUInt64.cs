//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Std
{
    public class MUInt64 : Message
    {
        public const string RosMessageName = "std_msgs/UInt64";

        public ulong data;

        public MUInt64()
        {
            this.data = 0;
        }

        public MUInt64(ulong data)
        {
            this.data = data;
        }
        public override List<byte[]> SerializationStatements()
        {
            var listOfSerializations = new List<byte[]>();
            listOfSerializations.Add(BitConverter.GetBytes(this.data));

            return listOfSerializations;
        }

        public override int Deserialize(byte[] data, int offset)
        {
            this.data = BitConverter.ToUInt64(data, offset);
            offset += 8;

            return offset;
        }

        public override string ToString()
        {
            return "MUInt64: " +
            "\ndata: " + data.ToString();
        }
    }
}
