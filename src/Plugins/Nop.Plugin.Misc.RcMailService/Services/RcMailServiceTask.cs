using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService.Services
{
    public class RcMailServiceTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly RcMailServiceSettings _rcMailServiceSettings;
        private readonly IDownloadService _downloadService;
        private readonly RCEmailService.IRCEmailService _client;

        #endregion

        #region Ctor

        public RcMailServiceTask(ILogger logger, IEmailAccountService emailAccountService, IQueuedEmailService queuedEmailService, RcMailServiceSettings rcMailServiceSettings, IDownloadService downloadService = null, RCEmailService.IRCEmailService client = null)
        {
            _logger = logger;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
            _rcMailServiceSettings = rcMailServiceSettings;
            _downloadService = downloadService;
            _client = client;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var maxTries = 3;
            var queuedEmails = await _queuedEmailService.SearchEmailsAsync(null, null, null, null,
                true, true, maxTries, false, 0, 500);

            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                            ? null
                            : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                            ? null
                            : queuedEmail.CC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    await SendEmailAsync(
                        emailAccount: await _emailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId),
                        queudedEmail: queuedEmail,
                        cc: cc,
                        bcc: bcc
                        );

                    queuedEmail.SentOnUtc = DateTime.UtcNow;

                }
                catch (Exception exc)
                {
                    await _logger.ErrorAsync($"Error sending e-mail. {exc.Message}", exc);
                }
                finally
                {
                    queuedEmail.SentTries += 1;
                    await _queuedEmailService.UpdateQueuedEmailAsync(queuedEmail);
                }

            }


            #endregion

        }

        public async Task<bool> SendEmailAsync(EmailAccount emailAccount, QueuedEmail queudedEmail, string[] bcc, string[] cc)
        {
            RCEmailService.CT_Mail mailContainer = new RCEmailService.CT_Mail
            {
                ApplicationName = _rcMailServiceSettings.ApplicationName,
                From = emailAccount.Email,
                Body = queudedEmail.Body,
                To = new string[] { queudedEmail.To },
                Subject = queudedEmail.Subject,
                IsHtml = true,
                Bcc = bcc,
                Cc = cc

            };

            if (queudedEmail.AttachedDownloadId > 0)
            {
                var download = await _downloadService.GetDownloadByIdAsync(queudedEmail.AttachedDownloadId);

                if (download != null)
                {
                    var attachment = new RCEmailService.CT_Attachment();
                    attachment.FileName = download.Filename;
                    attachment.ContentType = download.ContentType;
                    attachment.FileStream = download.DownloadBinary;

                    mailContainer.Attachments = new RCEmailService.CT_Attachment[] { attachment };
                }

            }

            var result = await _client.SendMailAsync(mailContainer);
            
            if(result == false)
                throw new Exception("An error occurred while sending message to RC Mail Service. Queued Mail Id:" + queudedEmail.Id);
            
            return result;

        }
    }
}
