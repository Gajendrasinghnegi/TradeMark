using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using TMR_AgentRegistration.Application.Dtos;
using TMR_AgentRegistration.Application.Extensions;
using TMR_AgentRegistration.Application.IServices;
using TMR_AgentRegistration.Domain.IRepositories;

namespace TMR_AgentRegistration.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateRepository _templateRepository;
        private readonly string _endpointUrl;

        public EmailService(IEmailTemplateRepository templateRepository, IOptions<MailServiceSettings> mailSettings)
        {
            _templateRepository = templateRepository;
            _endpointUrl = mailSettings.Value.EndpointUrl;
        }

        public async Task<bool> SendEmailAsync(EmailRequestDto request)
        {
            // Step 1: Fetch BODY_HTML from ONLINE_EMAIL_TEMPLATE table
            var template = await _templateRepository.GetTemplateByCodeAsync(request.TemplateCode);
            if (template == null) return false;

            // Step 2: Replace {{PLACEHOLDER}} tokens
            string finalSubject = ReplacePlaceholders(template.Subject, request.Placeholders);
            string finalBody = ReplacePlaceholders(template.BodyHtml, request.Placeholders);

            // Step 3: Call the government WCF MailService directly
            string result = await CallMailServiceAsync(request.ToEmail, finalSubject, finalBody);

            string decisionCode = result.Contains('/') ? result.Substring(0, result.IndexOf('/')) : result;
            return decisionCode == "0";
        }

        private string ReplacePlaceholders(string source, Dictionary<string, string> placeholders)
        {
            foreach (var kvp in placeholders)
                source = source.Replace("{{" + kvp.Key + "}}", kvp.Value);
            return source;
        }
        // Actual WCF call - kept private here since no other

        private async Task<string> CallMailServiceAsync(string toEmail, string subject, string body)
        {
            ChannelFactory<IMailContract> factory = null;
            try
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                {
                    MaxReceivedMessageSize = int.MaxValue,
                    MaxBufferSize = int.MaxValue,
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    SendTimeout = TimeSpan.FromMinutes(1),
                    ReceiveTimeout = TimeSpan.FromMinutes(1)
                };
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

                var endpoint = new EndpointAddress(_endpointUrl);
                factory = new ChannelFactory<IMailContract>(binding, endpoint);
                var client = factory.CreateChannel();

                string result = await client.SendEmailAsync(
                    "noreply.ipo@nic.in",          
                    new[] { toEmail },
                    Array.Empty<string>(),
                    subject,
                    body,
                    true,
                    Array.Empty<MailAttachment>(),
                    "PATENT Agent",                          //confirm OfficeCd with department
                    "01",                            
                    "ONLINE"                         
                );

                ((IClientChannel)client).Close();
                factory.Close();
                return result;
            }
            catch (Exception ex)
            {
                factory?.Abort();
                return "ERROR/" + ex.Message;
            }
        }
        // WCF contract - private, only EmailService uses this
        // Matches the real WSDL: interface name is "IService"
   
        [ServiceContract(Namespace = "http://tempuri.org/", Name = "IService")]
        private interface IMailContract
        {
            [OperationContract( Action = "http://tempuri.org/IService/SendEmail", ReplyAction = "http://tempuri.org/IService/SendEmailResponse")]
            Task<string> SendEmailAsync(string emailFrom, string[] emailTo, string[] ccTo, string subject, string body, bool isBodyHtml, MailAttachment[] attachments, string officeCd, string corresType, string section);
        }

        [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/")]
        private class MailAttachment
        {
            [DataMember] public string FileContentBase64 { get; set; }
            [DataMember] public string FileName { get; set; }
        }
    }
}