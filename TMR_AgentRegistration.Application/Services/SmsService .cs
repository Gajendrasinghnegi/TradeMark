using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.Extensions;
using TMR_AgentRegistration.Application.IServices;

namespace TMR_AgentRegistration.Application.Services
{
    public class SmsService: ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly SmsServiceSettings _smsSettings;
        public SmsService(HttpClient httpClient, IOptions<SmsServiceSettings> smsSettings)
        {
            _httpClient = httpClient;
            _smsSettings = smsSettings.Value;
        }
        public async Task<bool> SendSmsAsync(SmsRequestDto request)
        {
            // NOTE: exact SOAP method name/parameters depend on the WSDL of SMSService.svc
            // This is a placeholder structure - same as MailService, update once WSDL is confirmed
            string soapEnvelope = BuildSoapEnvelope(request.MobileNumber, request.Message);
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var response = await _httpClient.PostAsync(_smsSettings.EndpointUrl, content);

            return response.IsSuccessStatusCode;
        }

        private string BuildSoapEnvelope(string mobileNumber, string message)
        {
            // TODO: Replace "SendSMS" and parameter names with actual method from the WSDL
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <SendSMS xmlns=""http://tempuri.org/"">
                      <mobileNumber>{mobileNumber}</mobileNumber>
                      <message>{message}</message>
                    </SendSMS>
                  </soap:Body>
                </soap:Envelope>";
             }
       }
}
    
