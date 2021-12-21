using System.Collections;

namespace AdventOfCode;

public static class D16
{
    private static BitArray GetInput()
    {
        string str = File.ReadAllText("./Inputs/D16.txt");
        if (str.Length % 2 == 1)
            str += '0';

        BitArray bits = new(str.Length * 4);
        byte buffer;
        for (int i = 0; i < str.Length / 2; i++)
        {
            buffer = Convert.ToByte(str[(i * 2)..(i * 2 + 2)], 16);
            for (int j = 0; j < 8; j++)
                bits[i * 8 + j] = (buffer >> 7 - j & 1) == 1;
        }

        return bits;
    }

    private static IEnumerable<bool> GetBitsInRange(this BitArray array, int startIndex, int count)
    {
        if (startIndex + count > array.Length || startIndex < 0)
            throw new IndexOutOfRangeException();

        for (int i = startIndex; i < startIndex + count; i++)
            yield return array[i];
    }

    private static int GetIntInRange(this BitArray bits, int startIndex, int count)
    {
        if (count > 32)
            throw new InvalidOperationException($"Int max length is 32 bits. {count} bits were requested.");

        int v = 0;
        for (int i = startIndex, j = count - 1; i < startIndex + count; i++, j--)
            v |= Convert.ToByte(bits[i]) << j;

        return v;
    }

    /// <returns>Packet's BitArray actual length</returns>
    private static int ComputeLiteralValue(Packet packet)
    {
        long value = 0;
        bool end = false;
        int i = 6;
        while(true)
        {
            var bits = packet.RawData.GetBitsInRange(i, 5);
            if (bits.First() == false)
                end = true;

            foreach (bool b in bits.Skip(1))
                value = value << 1 | Convert.ToByte(b);

            i += 5;
            if (end) break;
        }
        packet.LiteralValue = value;
        packet.RawData = packet.RawData.GetRange(0, i);
        return packet.RawData.Length;
    }

    private static BitArray GetRange(this BitArray bits, int startIndex, int count)
    {
        if (count > bits.Length - startIndex)
            throw new IndexOutOfRangeException("Requested too many bits");

        BitArray newBits = new(count);
        for (int i = startIndex, j = 0; i < count + startIndex; i++, j++)
            newBits[j] = bits[i];
        return newBits;
    }

    private static BitArray GetRange(this BitArray bits, int startIndex) =>
        bits.GetRange(startIndex, count: bits.Length - startIndex);

    /// <returns>Packet's BitArray actual length</returns>
    private static int DecodeSubPacketsType0(Packet packet)
    {
        int subpacketsTotalLength = packet.RawData.GetIntInRange(7, 15);
        int currentSubpacketsTotalLength = 0;
        BitArray subArray = packet.RawData.GetRange(22, subpacketsTotalLength);
        List<Packet> subPackets = new();

        while (true)
        {
            (var subPacket, int subPacketLength) = DecodePacket(subArray);
            currentSubpacketsTotalLength += subPacketLength;
            subPackets.Add(subPacket);

            if (currentSubpacketsTotalLength == subpacketsTotalLength)
                break;
            subArray = subArray.GetRange(startIndex: subPacketLength);
        }

        packet.SubPackets = subPackets;
        packet.RawData = packet.RawData.GetRange(0, subpacketsTotalLength + 22);
        return packet.RawData.Length;
    }

    /// <returns>Packet's BitArray actual length</returns>
    private static int DecodeSubPacketsType1(Packet packet)
    {
        int subpacketsCount = packet.RawData.GetIntInRange(7, 11);
        BitArray subArray = packet.RawData.GetRange(18);
        List<Packet> subPackets = new();

        for (int i = 0; i < subpacketsCount; i++)
        {
            (var subPacket, int subPacketLength) = DecodePacket(subArray);
            subPackets.Add(subPacket);

            subArray = subArray.GetRange(startIndex: subPacketLength);
        }

        packet.SubPackets = subPackets;
        packet.RawData = packet.RawData.GetRange(0, packet.SubPackets.Sum(s => s.RawData.Length) + 18);
        return packet.RawData.Length;
    }

    /// <returns>Packet's BitArray actual length</returns>
    private static int DecodeSubPackets(Packet packet) => packet.LengthTypeId == 0 ?
        DecodeSubPacketsType0(packet) :
        DecodeSubPacketsType1(packet);


    private static (Packet packet, int stoppedAtIndex) DecodePacket(BitArray bits)
    {
        int version = bits.GetIntInRange(0, 3);
        int typeId = bits.GetIntInRange(3, 3);
        int? lengthTypeId = typeId == 4 ? null : Convert.ToInt32(bits[6]);
        Packet packet = new(bits) { Version = version, TypeId = typeId, LengthTypeId = lengthTypeId };
        if (typeId == 4)
            return (packet, ComputeLiteralValue(packet));
        return (packet, DecodeSubPackets(packet));
    }

    private static int VersionSum(Packet packet)
    {
        int sum = packet.Version;
        if (packet.TypeId == 4)
            return sum;

        foreach (var p in packet.SubPackets)
            sum += VersionSum(p);

        return sum;
    }

    public static int SolveA() => VersionSum(DecodePacket(GetInput()).packet);

    private static long Sum(Packet packet)
    {
        long sum = 0;
        foreach (var p in packet.SubPackets)
            sum += ComputeValue(p);
        return sum;
    }

    private static long ComputeValue(Packet packet) => packet.TypeId switch
    {
        0 => Sum(packet),
        1 => packet.SubPackets.Aggregate(1L, (agg, p) => agg * ComputeValue(p)),
        2 => packet.SubPackets.Min(ComputeValue),
        3 => packet.SubPackets.Max(ComputeValue),
        5 => ComputeValue(packet.SubPackets.First()) > ComputeValue(packet.SubPackets.Last()) ? 1L : 0L,
        6 => ComputeValue(packet.SubPackets.First()) < ComputeValue(packet.SubPackets.Last()) ? 1L : 0L,
        7 => ComputeValue(packet.SubPackets.First()) == ComputeValue(packet.SubPackets.Last()) ? 1L : 0L,
        _ => packet.LiteralValue ?? throw new ArgumentNullException("Packet type 4 has no value")
    };

    public static long SolveB() => ComputeValue(DecodePacket(GetInput()).packet);

    private class Packet
    {
        public int Version { get; init; }
        public int TypeId { get; init; }
        public int? LengthTypeId { get; init; }
        public BitArray RawData { get; set; }
        public long? LiteralValue { get; set; }
        public IEnumerable<Packet> SubPackets { get; set; }

        public Packet(BitArray bits)
        {
            RawData = bits;
            SubPackets = Array.Empty<Packet>();
        }
    }

}