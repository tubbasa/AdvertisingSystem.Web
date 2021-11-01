using System.Threading.Tasks;
using AdvertisingSystem.Web.Models;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingSystem.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }
        // GET
        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }
        
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null) 
                {
                    ModelState.AddModelError("UserExist", "An user is exist with this email.");
                    return View(model);
                }
                
                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser =  await _userManager.CreateAsync(user, model.Password); // if we dont send the password to the manager, cognito will
                                                                      // create a password for this user and will send the user
                                                                      // after the user first login they have to change their password
                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound","A user with the given mail address was not found.");
                    return View(model);
                }

                var result =
                    await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true); //https://github.com/aws/aws-aspnet-cognito-identity-provider/issues/146
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
              
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
            }
            return View(model);
        }
        
        public IActionResult Confirm()
        {
            return View(new ConfirmModel());
        }

        public IActionResult SignIn()
        {
            return View(new SignInModel());
        }

        [ActionName("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Login error", "Email or password does not matched.");
                }
            }
            return View("SignIn",model);
        }
    }
}