using System;

namespace ServiceProvider.Models
{
    [Serializable]
    public class SendMessageRequest
    {
        public Message inner_envelope;
        public string receiver;
        public string timestamp;
        public string sig_service;
    }
}