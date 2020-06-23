using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Email
{
   
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password     {get; set; }

        public bool smtp_authentication { get; set; }
        public bool starttls_enable { get; set; }
        public bool ssl_enable { get; set; }
        public bool ssl_checkserveridentity { get; set; }
       

        public string SSL_trust { get; set; }


        
    }
}