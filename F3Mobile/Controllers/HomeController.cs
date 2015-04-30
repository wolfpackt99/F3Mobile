using F3.Business;
using F3.ViewModels;
using Ninject;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace F3Mobile.Controllers
{
    [RequireHttps]
    public partial class HomeController : Controller
    {
        [Inject]
        public ISubscribe ContactBiz { get; set; }

        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Fng()
        {
            ViewBag.Message = "FNG Signup";

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<ActionResult> Fng(Contact contact)
        {
            if (ModelState.IsValid)
            {
                var subscribed = await ContactBiz.Add(contact);
                ViewBag.Success = "FNG Added.";
            }
            return View(contact);
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
        //[Authorize]
        //public virtual async Task<ActionResult> ContactAsnyc(CancellationToken cancellationToken)
        //{
            
        //    var externalidentity =
        //        await this.HttpContext.GetOwinContext()
        //            .Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            

        //    //if (result.Credential == null)
        //    //    return new RedirectResult(result.RedirectUri);
        //    var accessToken =
        //        ((ClaimsIdentity) this.HttpContext.User.Identity).Claims.FirstOrDefault(
        //            atk => atk.Type.Equals("urn:googleplus:access_token"));

        //    if (accessToken == null)
        //    {
        //        throw new Exception("Access Token is null");
        //    }

        //    var rs = new RequestSettings("F3Test", accessToken.Value );
        //    rs.AutoPaging = true;
        //    var cr = new ContactsRequest(rs);

        //    var f = cr.GetContacts();
        //    var c = f.Entries.Select(e => new F3.ViewModels.Contact
        //    {
        //        Email = e.Emails.Any() ? e.Emails.First().Address : string.Empty, 
        //        FirstName = e.Name.GivenName, 
        //        LastName = e.Name.FamilyName, 
        //        Id = e.Id
        //    }).ToList();
        //    return View(c);
        //}
    }
}