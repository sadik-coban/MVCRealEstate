using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Security.Claims;

namespace MVCRealEstate.Controllers
{
    public abstract class MVCRealEsateController : Controller
    {
        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
