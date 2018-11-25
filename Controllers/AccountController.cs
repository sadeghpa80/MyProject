using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataLayer;
using DataLayer.ViewModels;

namespace ArianWebsiteMVC.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;
        readonly DataContext _db = new DataContext();

        public AccountController()
        {
            _userRepository = new UserRepository(_db);
        }
        public AccountController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        // GET: Account
        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(LoginViewModel login, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                if (_userRepository.IsExistUser(login.Username, login.Password))
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    return Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("Username", "کاربری یافت نشد");
                }
            }
            return View(login);
        }
    }
}