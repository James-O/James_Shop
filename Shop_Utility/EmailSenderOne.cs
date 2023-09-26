using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace Shop_Utility
{
    public class EmailSenderOne : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public MailJetSettings _mailJetSettings { get; set; }
        public EmailSenderOne(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        //public void Send(string from, string to, string subject, string messageText)
        //{
        //    throw new NotImplementedException();
        //}
        //public void Send(MailMessage message)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Send(IEnumerable<MailMessage> messages)
        //{
        //    throw new NotImplementedException();
        //}
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }
        public async Task Execute(string email, string subject, string body) 
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();
            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey)
            {
                //Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", "ogbonnasunday43@outlook.com"},
        {"Name", "Ogbonna"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }, {
          "Name",
          "JovicsTech"
         }
        }
       }
      }, {
       "Subject",
       subject
      }, {
       "HTMLPart",
       body
      }
     }
             });
           await client.PostAsync(request);
        }  
    }
}
