using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading;
using System.Threading.Tasks;
using MessagingReturnCode = SmallEarthTech.AntRadioInterface.MessagingReturnCode;

namespace GodotAntGrpc.Services
{
    internal partial class AntChannelService : IAntChannel
    {
        private readonly gRPCAntChannel.gRPCAntChannelClient _client;
        private readonly ILogger _logger;
        private readonly byte _channelNumber;

        public AntChannelService(ILogger logger, byte channelNumber, GrpcChannel grpcChannel)
        {
            _logger = logger;
            _channelNumber = channelNumber;
            _client = new(grpcChannel);
        }

        public byte ChannelNumber => _channelNumber;

        public event EventHandler<AntResponse> ChannelResponse;

        public async void HandleChannelResponseEvents(CancellationToken cancellationToken)
        {
            using AsyncServerStreamingCall<ChannelResponseUpdate> _response = _client.Subscribe(new SubscribeRequest { ChannelNumber = _channelNumber }, cancellationToken: cancellationToken);
            try
            {
                await foreach (ChannelResponseUpdate update in _response.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    ChannelResponse?.Invoke(this, new GrpcAntResponse(update));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("RpcException: operation cancelled");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("OperationCanceledException");
            }
        }

        public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool CloseChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing _response");
        }

        public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool OpenChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public ChannelId RequestChannelID(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SendBroadcastData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public Task<MessagingReturnCode> SendBurstTransferAsync(byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            throw new NotImplementedException();
        }

        public Task<MessagingReturnCode> SendAcknowledgedDataAsync(byte[] data, uint ackWaitTime)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            var reply = _client.SendExtAcknowledgedData(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        public async Task<MessagingReturnCode> SendExtAcknowledgedDataAsync(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            var reply = await _client.SendExtAcknowledgedDataAsync(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        public bool SendExtBroadcastData(ChannelId channelId, byte[] data)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public Task<MessagingReturnCode> SendExtBurstTransferAsync(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelPeriod(ushort messagePeriod, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool UnassignChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }
    }
}
