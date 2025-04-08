using MassTransit;

namespace Persistance.Masstransit;

public class ClientResponseWorker
{
    private static TimeSpan _timeout = TimeSpan.FromSeconds(2);
    
    public static async Task<TRes> GetRabbitMessageResponse<TReq, TRes>(TReq request, IBusControl busControl,
        Uri rabbitQueueName, CancellationToken cancellationToken = default)
        where TReq : class
        where TRes : class
    {
#if DEBUG
        _timeout = TimeSpan.FromHours(1);
#endif

        var client   = busControl.CreateRequestClient<TReq>(rabbitQueueName, timeout: _timeout);
        var response = await client.GetResponse<TRes>(request, cancellationToken);

        return response.Message;
    }
}