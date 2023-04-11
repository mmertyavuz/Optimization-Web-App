using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Logging;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.RcSmsService.Services;

public class RcSmsServiceTask : IScheduleTask
{
    #region Fields

    private readonly ILogger _logger;
    private readonly IQueuedSmsService _queuedSmsService;
    private readonly RcSmsServiceSettings _rcSmsServiceSettings;
    private readonly RCSmsService.IRcSmsService _client;

    #endregion

    #region Ctor

    public RcSmsServiceTask(ILogger logger, IQueuedSmsService queuedSmsService, RcSmsServiceSettings rcSmsServiceSettings, RCSmsService.IRcSmsService client)
    {
        _logger = logger;
        _queuedSmsService = queuedSmsService;
        _rcSmsServiceSettings = rcSmsServiceSettings;
        _client = client;
    }

    #endregion

    #region Methods

    public async Task ExecuteAsync()
    {
        var maxTries = 3;
        var queuedSmses = await _queuedSmsService.SearchSmsesAsync(null, null, null, true,
            true, maxTries, false, 0, 500);

        foreach (var queuedSms in queuedSmses)
        {
            try
            {
                await SendSmsAsync(queuedSms);
                queuedSms.SentOnUtc = DateTime.UtcNow;
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync($"Error sending e-sms. {exc.Message}", exc);
            }
            finally
            {
                queuedSms.SentTries += 1;
                await _queuedSmsService.UpdateQueuedSmsAsync(queuedSms);
            }
        }
    }

    public async Task<RCSmsService.SmsResponseDto> SendSmsAsync(QueuedSms queudedSms)
    {
        var smsSendList = new List<RCSmsService.SmsSendDto>
        {
            new RCSmsService.SmsSendDto
            {
                PhoneNumber = queudedSms.To
            }
        };

        RCSmsService.SmsRequestDto smsContrainer = new RCSmsService.SmsRequestDto
        {
            MessageText = queudedSms.Body,
            SendDate = DateTime.Now,
            ApplicationName = _rcSmsServiceSettings.ApplicationName,
            ServicePassword = _rcSmsServiceSettings.ServicePassword,
            SmsSendList = smsSendList.ToArray()
        };

        var result = await _client.SendSmsAsync(smsContrainer);

        if (result.RcSMSServiceReturnCode != RCSmsService.SmsServiceReturnCodeType.Success)
            throw new Exception("An error occurred while sending message to RC Sms Service. Queued Sms Id:" + queudedSms.Id + " Error: " + result.RcSMSServiceReturnCode);

        return result;
    }

    #endregion

}