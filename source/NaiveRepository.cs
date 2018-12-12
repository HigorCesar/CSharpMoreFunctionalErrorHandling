using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Newtonsoft.Json;

namespace HandlingOperationResult
{
    /*
     * It can throw exceptions and there is no mention about what can go wrong.
     */
public class NaiveRepository
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly string _sqsUrl;

    public NaiveRepository(IAmazonSQS amazonSqs, string sqsUrl)
    {
        _amazonSqs = amazonSqs;
        _sqsUrl = sqsUrl;
    }

    public async Task<IEnumerable<ImageDataPoint>> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
        return response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
    }
}
}
