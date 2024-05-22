using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions;

public class UpdateSubscriber(ILogger<UpdateSubscriber> logger, DataContext context)
{
    private readonly ILogger<UpdateSubscriber> _logger = logger;
    private readonly DataContext _context = context;


    [Function("UpdateSubscriber")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SubscribeEntity>(requestBody);

            var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == data!.Email);
            if (subscriber == null)
            {
                return new NotFoundResult();
            }

            subscriber.DailyNewsletter = data!.DailyNewsletter;
            subscriber.AdvertisingUpdates = data.AdvertisingUpdates;
            subscriber.WeekInReview = data.WeekInReview;
            subscriber.EventUpdates = data.EventUpdates;
            subscriber.StartupsWeekly = data.StartupsWeekly;
            subscriber.Podcasts = data.Podcasts;

            await _context.SaveChangesAsync();

            return new OkObjectResult(subscriber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to update a subscriber");
            return new BadRequestObjectResult(new { Status = 500, Message = "Unable to update a subscriber right now" });
        }
        
    }
}
