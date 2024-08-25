using System.Security.Claims;
using IdentityModel;
using IdentityServer.Models;
using IdentityServer.Pages.Account.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {

        public UserManager<ApplicationUser> _userManager;

        [BindProperty] public RegisterViewModel Input { get; set; }
        [BindProperty] public bool IsRegistered { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet(string returnUrl)
        {
            Input = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (Input.Button != "register") return Redirect("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user,
                    [
                        new Claim(JwtClaimTypes.Name, Input.FullName)
                    ]);
                }

                IsRegistered = true;
            }

            return Page();
        }
    }
}
