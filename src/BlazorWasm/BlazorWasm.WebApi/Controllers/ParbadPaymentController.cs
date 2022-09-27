using BlazorServer.Services;
using Microsoft.AspNetCore.Mvc;
using Parbad;
using Parbad.AspNetCore;
using Parbad.Gateway.ParbadVirtual;

namespace BlazorWasm.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ParbadPaymentController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IOnlinePayment _onlinePayment;
    private readonly IRoomOrderDetailsService _roomOrderService;

    public ParbadPaymentController(
        IConfiguration configuration,
        IOnlinePayment onlinePayment,
        IRoomOrderDetailsService roomOrderService)
    {
        _configuration = configuration;
        _onlinePayment = onlinePayment ?? throw new ArgumentNullException(nameof(onlinePayment));
        _roomOrderService = roomOrderService ?? throw new ArgumentNullException(nameof(roomOrderService));
    }

    [HttpGet]
    public async Task<IActionResult> PayRoomOrder(int orderId, long amount)
    {
        var verifyUrl = Url.Action(
                                   nameof(VerifyRoomOrderPayment),
                                   nameof(ParbadPaymentController)
                                       .Replace("Controller", string.Empty, StringComparison.Ordinal),
                                   null, Request.Scheme);

        var result = await _onlinePayment.RequestAsync(invoiceBuilder =>
                                                           invoiceBuilder.UseAutoIncrementTrackingNumber()
                                                                         .SetAmount(amount)
                                                                         .SetCallbackUrl(verifyUrl)
                                                                         .UseParbadVirtual()
                                                      );

        if (result.IsSucceed)
        {
            await _roomOrderService.UpdateRoomOrderTrackingNumberAsync(orderId, result.TrackingNumber);

            // It will redirect the client to the gateway.
            return result.GatewayTransporter.TransportToGateway();
        }

        return Redirect(GetClientReturnUrl(orderId, result.TrackingNumber, result.Message));
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> VerifyRoomOrderPayment()
    {
        var invoice = await _onlinePayment.FetchAsync();
        var orderDetail = await _roomOrderService.GetOrderDetailByTrackingNumberAsync(invoice.TrackingNumber);
        if (orderDetail is null)
        {
            return Redirect("/404");
        }

        if (invoice.Status == PaymentFetchResultStatus.AlreadyProcessed)
        {
            return Redirect(GetClientReturnUrl(orderDetail.Id, invoice.TrackingNumber,
                                               "The payment is already processed."));
        }

        var verifyResult = await _onlinePayment.VerifyAsync(invoice);
        if (verifyResult.Status == PaymentVerifyResultStatus.Succeed)
        {
            var result =
                await _roomOrderService.MarkPaymentSuccessfulAsync(verifyResult.TrackingNumber, verifyResult.Amount);
            if (result == null)
            {
                await _onlinePayment.CancelAsync(invoice);
                return Redirect(GetClientReturnUrl(orderDetail.Id, verifyResult.TrackingNumber,
                                                   "Can not mark payment as successful"));
            }

            return Redirect(GetClientReturnUrl(orderDetail.Id, verifyResult.TrackingNumber, verifyResult.Message));
        }

        return Redirect(GetClientReturnUrl(orderDetail.Id, verifyResult.TrackingNumber, verifyResult.Message));
    }

    private string GetClientReturnUrl(int orderId, long trackingNumber, string errorMessage)
    {
        var clientBaseUrl = _configuration.GetValue<string>("Client_URL");
        return new Uri(new Uri(clientBaseUrl),
                       Invariant($"/payment-result/{orderId}/{trackingNumber}/{Uri.EscapeDataString(errorMessage)}"))
            .ToString();
    }
}