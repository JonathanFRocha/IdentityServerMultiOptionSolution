using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFAModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAViewModel viewModel { get; set; }

        [BindProperty]
        public bool Succeded { get; set; }

        public AuthenticatorWithMFAModel(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.viewModel = new SetupMFAViewModel();
            this.Succeded = false;
        }
        public async Task OnGet()
        {
            var user = await userManager.GetUserAsync(User);
            await this.userManager.ResetAuthenticatorKeyAsync(user);
            var authenticatorKey = await this.userManager.GetAuthenticatorKeyAsync(user);

            viewModel.Key = authenticatorKey;
            viewModel.QrCode = GenerateQRCodeBytes("MyApp", authenticatorKey, user.Email);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await userManager.GetUserAsync(User);
            var resultOk = await userManager.VerifyTwoFactorTokenAsync(
                user,
                userManager.Options.Tokens.AuthenticatorTokenProvider,
                viewModel.SecurityCode
                );
            if (resultOk)
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                this.Succeded = true;
            }else
            {
                ModelState.AddModelError("AuthenticationSetup", "Something went wrong with the authenticator setup.");
            }
            return Page();
        }

        private Byte[] GenerateQRCodeBytes(string provider, string secretKey, string userEmail)
        {

            var qrCodeGenerator = new QRCodeGenerator();

            var qrCodeData = qrCodeGenerator.CreateQrCode(
                $"otpauth://totp/{provider}:{userEmail}?secret={secretKey}&Issuer={provider}",
                QRCodeGenerator.ECCLevel.Q
                );

            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return BitMapToBytes(qrCodeImage);
        }

        private Byte[] BitMapToBytes(Bitmap image )
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }

        }
    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; }

        [Required]
        [Display(Name ="Security Code")]
        public string SecurityCode { get; set; }

        public Byte[] QrCode { get; set; }
    }
}
