using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PV293WebApplication.Controllers.Admin;

[Authorize(Roles = "Admin")]
public abstract class AdminController : Controller
{
}
