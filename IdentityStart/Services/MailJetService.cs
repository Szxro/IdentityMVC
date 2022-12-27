using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using Newtonsoft.Json.Linq;

namespace IdentityStart.Services
{
    public class MailJetService : IEmailSender
    {
        private readonly IConfiguration _config;
        private MailJetModel _mailJetModel;

        public MailJetService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        { 
            //Getting the section and putting in the Apikey and the SecretKey like JsonDeserializer
            _mailJetModel = _config.GetSection("MailJet")
                              .Get<MailJetModel>();

            MailjetClient client = new MailjetClient(_mailJetModel.ApiKey,_mailJetModel.SecretKey);
           
            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", "szxrocode@gmail.com"},
        {"Name", "Sebastian Vargas"}
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
          "Sebastian"
         }
        }
       }
      }, {
       "Subject",
        subject
      },{
       "HTMLPart",
       htmlMessage
      }
     }
             });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
