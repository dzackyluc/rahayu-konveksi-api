using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/webhooks/xendit")]
public class XenditWebhookController(ILogger<XenditWebhookController> logger, IOptions<XenditConnectionSettings> xenditConnectionSettings) : ControllerBase
{
    private readonly ILogger<XenditWebhookController> _logger = logger;
    private const string XenditTokenHeader = "x-callback-token";
    private readonly string ExpectedToken = xenditConnectionSettings.Value.WebhookSecret;

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
        }
        else
        {
            _logger.LogWarning("Event and not found in payload");
            _logger.LogInformation("Payload: {Payload}", payload.ToString());
            return BadRequest("Invalid payload");
        }

        return Ok();
    }
}