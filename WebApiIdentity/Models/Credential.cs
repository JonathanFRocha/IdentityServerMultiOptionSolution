using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiIdentity.Models
{

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
