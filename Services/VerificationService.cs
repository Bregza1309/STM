using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using StudentTransportManagement_STM_.Server.Context;
using StudentTransportManagement_STM_.Shared.DataModels;
using Twilio;
using Twilio.Rest.Verify.V2;
using Twilio.Rest.Verify.V2.Service;
namespace STM.Services
{
    public class VerificationService
    {
        const string ACCOUNTSID = "ACf2286354a7f39046ed4853119ecd8dd0";
        const string AUTHTOKEN = "ffd223f8ac2c80a5c12fab0ec42a9491";
        StmContext Context { get; set; }
        public VerificationService(StmContext context)
        {
            this.Context = context;
            
        }
        public void Add(Verification verification)
        {
            Context.Verifications.Add(verification);
            Context.SaveChanges();
        }
        public void Remove(int Id)
        {
            Verification verification = Context.Verifications.Single( v => v.Id == Id);
            Context.Verifications.Remove(verification);
            Context.SaveChanges();
        }
        public List<Verification> Get(string? Id = null)
        {
            if(Id == null){
                return Context.Verifications.ToList();
            }
            else
            {
                return Context.Verifications.Where(v => v.Driver!.UserId == Id).ToList();
            }
        }
        public void Update(Verification verification)
        {
            Context.ChangeTracker.Clear();
            Context.Verifications.Update(verification);
            Context.SaveChanges();
        }
        public bool SendVerificationSms(string cellNumber)
        {
            TwilioClient.Init(ACCOUNTSID, AUTHTOKEN);
            var verification = VerificationResource.Create(
                        to: cellNumber,
                        channel: "sms",
                        pathServiceSid: "VAdb2a012a5783309cd8574acd71a113bd"
                    );
            return verification.Status == "pending";
           
        }
        public bool VerifyUser(string cellNumber , string otp)
        {
            
            var verificationCheck = VerificationCheckResource.Create(
                   to: cellNumber,
                   code: otp,
                   pathServiceSid: "VAdb2a012a5783309cd8574acd71a113bd"
               );
            return verificationCheck.Status == "approved";
        }
    }
}
