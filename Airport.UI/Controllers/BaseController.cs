using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airport.UI.Controllers;

[Authorize]
public class BaseController : Controller
{

}
