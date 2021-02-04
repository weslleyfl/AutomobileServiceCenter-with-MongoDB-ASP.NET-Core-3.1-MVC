using ASC.Web.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Services.SMS
{
    public class AuthSmsSender : ISmsSender
    {
        private IOptions<ApplicationSettings> _settings;

        public AuthSmsSender(IOptions<ApplicationSettings> settings)
        {
            _settings = settings;
        }
        public async Task SendSmsAsync(string number, string message)
        {

            //return await Task.Run(() => string.Empty).ConfigureAwait(false);
            await Task.Factory.StartNew(() => string.Empty);

            // TODO: SMS esta desabilitado - pois é pago, verificar um free
            // using Twilio;
            //TwilioClient.Init(_settings.Value.TwilioAccountSID, _settings.Value.TwilioAuthToken);

            //var smsMessage = await MessageResource.CreateAsync(
            //    to: new PhoneNumber(number),
            //    from: new PhoneNumber(_settings.Value.TwilioPhoneNumber),
            //    body: message);
        }

    }
}
