using AntChannelGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace GodotAntGrpc.Services
{
    public class GrpcAntResponse : AntResponse
    {
        public GrpcAntResponse(ChannelResponseUpdate response)
        {
            ChannelId = response.ChannelId != null ? new ChannelId((uint)response.ChannelId) : null;
            ChannelNumber = (byte)response.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)response.ThresholdConfigurationValue;
            Payload = response.Payload.ToByteArray();
            ResponseId = (byte)response.ResponseId;
            Rssi = (sbyte)response.Rssi;
            Timestamp = (ushort)response.Timestamp;
        }
    }
}
