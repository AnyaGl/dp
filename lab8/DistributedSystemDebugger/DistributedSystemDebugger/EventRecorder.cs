using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace DistributedSystemDebugger
{
    class EventRecorder : IEventRecorder
    {
        private readonly int _runId;
        private readonly int _processId;
        private readonly string _aggreagorUrl;

        public delegate void UpdateGlobalTime(int processId);
        public delegate List<int> GlobalTimeProvider();

        private UpdateGlobalTime _globalTimeUpdater;
        private GlobalTimeProvider _globalTimeProvider;

        private const byte INTERNAL_EVENT = 0;
        private const byte SEND_EVENT = 1;
        private const byte RECEIVE_EVENT = 2;

        public EventRecorder(int runId, int processId, string aggreagorUrl, UpdateGlobalTime updater, GlobalTimeProvider provider)
        {
            _runId = runId;
            _processId = processId;
            _aggreagorUrl = aggreagorUrl;
            _globalTimeUpdater = updater;
            _globalTimeProvider = provider;
        }
        public void DebugInternal(string description)
        {
            _globalTimeUpdater(_processId);
            SendEventInfo(description, INTERNAL_EVENT);
        }

        public string DebugReceive(string description, DebugPackage receivedMessage)
        {
            _globalTimeUpdater(_processId);
            SendEventInfo(description, RECEIVE_EVENT);
            return receivedMessage.message;
        }

        public DebugPackage DebugSend(string description, string message)
        {
            _globalTimeUpdater(_processId);
            SendEventInfo(description, SEND_EVENT);
            return new DebugPackage(message, _globalTimeProvider());
        }

        private void SendEventInfo(string description, byte kind)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Content-Type", "application/json");

                var e = new Event();
                e.Description = description;
                e.Kind = kind;
                e.ProcessId = _processId;
                e.RunId = _runId;
                e.Time = JsonSerializer.Serialize(_globalTimeProvider());

                webClient.UploadString(_aggreagorUrl, JsonSerializer.Serialize(e));
            }
        }
    }
}
