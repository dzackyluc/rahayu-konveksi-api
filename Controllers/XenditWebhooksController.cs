using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/webhooks/xendit")]
public class XenditWebhookController(ILogger<XenditWebhookController> logger, IOptions<XenditConnectionSettings> xenditConnectionSettings, OrdersService ordersService) : ControllerBase
{
    private readonly ILogger<XenditWebhookController> _logger = logger;
    private const string XenditTokenHeader = "x-callback-token";
    private readonly string ExpectedToken = xenditConnectionSettings.Value.WebhookSecret;
    private readonly OrdersService _ordersService = ordersService;

    [HttpPost]
    public IActionResult Receive([FromBody] JsonElement payload)
    {
        // 1. Verify token
        if (!Request.Headers.TryGetValue(XenditTokenHeader, out var token) ||
            token != ExpectedToken)
        {
            _logger.LogWarning("Invalid Xendit webhook token");
            return Unauthorized();
        }

        // 2. Log and process
        if (payload.TryGetProperty("event", out var eventTypeElement))
        {
            string eventType = eventTypeElement.GetString() ?? string.Empty;
            _logger.LogInformation("Received Xendit event: {Event}", eventType);
            _logger.LogInformation("Payload: {Payload}", payload.ToString());

            // TODO: Handle various event types (e.g., "invoice.expired", "virtual_account.paid")
            // Example:
            // if (eventType == "invoice.expired") { ... }
            if (eventType == "payout_link.expired")
            {
                // Handle payout link expired event
                _logger.LogInformation("Payout link expired event received");
            }
            else if (eventType == "payout_link.claimed")
            {
                _logger.LogInformation("Payout link claimed event received");
            }
        }
        else
        {
            _logger.LogWarning("Event and not found in payload");
            _logger.LogInformation("Payload: {Payload}", payload.ToString());
            return BadRequest("Invalid payload");
        }

        return Ok();
    }

    [HttpPost("disbursement")]
    public IActionResult Disbursement([FromBody] JsonElement payload)
    {
        // 1. Verify token
        if (!Request.Headers.TryGetValue(XenditTokenHeader, out var token) ||
            token != ExpectedToken)
        {
            _logger.LogWarning("Invalid Xendit webhook token");
            return Unauthorized();
        }
        else
        {
            _logger.LogInformation("Received Xendit disbursement event");
            _logger.LogInformation("Payload: {Payload}", payload.ToString());

            if (payload.TryGetProperty("external_id", out var externalIdElement))
            {
                string externalId = externalIdElement.GetString() ?? string.Empty;
                // _logger.LogInformation("External ID: {ExternalId}", externalId);
                if (externalId.StartsWith("demo"))
                {
                    // Handle demo disbursement
                    _logger.LogInformation("Demo disbursement received with external ID: {ExternalId}", externalId);
                }
                else
                {
                    // Handle real disbursement
                    _logger.LogInformation("Real disbursement received with external ID: {ExternalId}", externalId);
                }
            }
            else
            {
                _logger.LogWarning("External ID not found in payload");
            }

            return Ok();
        }
    }

    [HttpPost("payment")]
    public IActionResult Payment([FromBody] JsonElement payload)
    {
        // 1. Verify token
        if (!Request.Headers.TryGetValue(XenditTokenHeader, out var token) ||
            token != ExpectedToken)
        {
            _logger.LogWarning("Invalid Xendit webhook token");
            return Unauthorized();
        }

        // 2. Log and process
        if (payload.TryGetProperty("id", out var IdElement))
        {
            string Id = IdElement.GetString() ?? string.Empty;
            _logger.LogInformation("Received Xendit payment event with ID: {Id}", Id);
            _logger.LogInformation("Payload: {Payload}", payload.ToString());
            if (payload.TryGetProperty("status", out var statusElement))
            {
                string status = statusElement.GetString() ?? string.Empty;

                // Handle payment status
                if (status != null)
                {
                    // Update order status in the database
                    var orderTask = _ordersService.GetOrdersByXenditRefAsync(Id);
                    var orders = orderTask.Result; // Or use await if you make the method async
                    if (orders != null && orders.Count != 0)
                    {
                        foreach (var order in orders)
                        {
                            order.Status = status;
                            if (!string.IsNullOrEmpty(order?.Id))
                            {
                                _ = _ordersService.UpdateOrderAsync(order.Id, order);
                                _logger.LogInformation("Order status updated to: {Status} for External ID: {ExternalId}", status, Id);
                                return Ok();
                            }
                            else
                            {
                                _logger.LogWarning("Order ID is null or empty. Cannot update order status for External ID: {ExternalId}", Id);
                                return BadRequest("Order ID is null or empty");
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Order not found for External ID: {ExternalId}", Id);
                        return NotFound("Order not found");
                    }
                }
                else
                {
                    _logger.LogWarning("Status not found in payload for External ID: {ExternalId}", Id);
                    return BadRequest("Status not found in payload");
                }
                return BadRequest("Invalid payload");
            }
            else
            {
                _logger.LogWarning("Status not found in payload for External ID: {ExternalId}", Id);
                return BadRequest("Status not found in payload");
            }
        }
        else
        {
            _logger.LogWarning("External ID not found in payload");
            return BadRequest("Invalid payload");
        }
    }
}