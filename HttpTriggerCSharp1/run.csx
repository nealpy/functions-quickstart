#r "Newtonsoft.Json"
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    // parse query parameter
    var content = req.Content;

    string jsonContent = await content.ReadAsStringAsync(); 
    log.Info($"Received Event with payload: {jsonContent}");

    IEnumerable<string> headerValues;
    if (req.Headers.TryGetValues("Aeg-Event-Type", out headerValues))
    {
        var validationHeaderValue = headerValues.FirstOrDefault();
        if(validationHeaderValue == "SubscriptionValidation")
        {
            var events = JsonConvert.DeserializeObject<GridEvent[]>(jsonContent);
            var code = events[0].Data["validationCode"];
            return req.CreateResponse(HttpStatusCode.OK,
            new { validationResponse = code });
       }
   }

    return jsonContent == null
    ? req.CreateResponse(HttpStatusCode.BadRequest, "Pass a name on the query string or in the request body")
    : req.CreateResponse(HttpStatusCode.OK, "Hello " + jsonContent);
}

public class GridEvent
{
    public string Id { get; set; }
    public string EventType { get; set; }
    public string Subject { get; set; }
    public DateTime EventTime { get; set; }
    public Dictionary<string, string> Data { get; set; }
    public string Topic { get; set; }
}

