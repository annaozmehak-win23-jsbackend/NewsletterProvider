using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions;

public class Subscribers(ILogger<Subscribers> logger, DataContext context)
{
    private readonly ILogger<Subscribers> _logger = logger;
    private readonly DataContext _context = context;


    [Function("Subscribers")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try 
        {
            var subscribers = await _context.Subscribers.OrderByDescending(c => c.Email).ToListAsync();
            return new OkObjectResult(new { Status = 200, subscribers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching subscribers.");
            return new BadRequestObjectResult(new { Status = 500, Message = "Unable to get all subscribers right now" });
        }
    }
}