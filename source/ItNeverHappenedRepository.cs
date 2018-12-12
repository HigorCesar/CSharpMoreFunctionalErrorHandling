using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace HandlingOperationResult
{
    /*
     * Here the client never knows if a error happened.
     * The business case of empty response is mixed with error scenario and null is used to represent failures.
     */
public class ItNeverHappenedRepository
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly string _sqsUrl;

    public ItNeverHappenedRepository(IAmazonSQS amazonSqs, string sqsUrl)
    {
        _amazonSqs = amazonSqs;
        _sqsUrl = sqsUrl;
    }

    public async Task<IEnumerable<ImageDataPoint>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
            if (response.HttpStatusCode != HttpStatusCode.OK)
                return null;

            return response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
        }
        catch (OverLimitException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
}