namespace JetBlack.MessageBus.AuthFeedBus.Messages
{
    public enum MessageType : byte
    {
        InteractorAdvertisement,
        MulticastData,
        UnicastData,
        ForwardedMulticastData,
        ForwardedUnicastData,
        ForwardedSubscriptionRequest,
        NotificationRequest,
        SubscriptionRequest,
        AuthorizationRequest,
        AuthorizationResponse
    }
}
