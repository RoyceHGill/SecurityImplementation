// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using JokesMVC2023;
using JokesMVC2023.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace JokesMVC2023.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ProfilePictureUploader _profilePictureUploader;

        public IndexModel(
            ProfilePictureUploader profilePictureUploader,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profilePictureUploader = profilePictureUploader;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var profilePhotoFileName = user.ProfilePhotoFileName;

            LoadProfilePhoto(profilePhotoFileName);
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }



        public async Task<IActionResult> DownloadFile(string fileName)
        {
            byte[] fileBytes = await _profilePictureUploader.ReadFileIntoMemory(fileName);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return RedirectToAction("Index");
            }

            return File(fileBytes, "application/octet-stream", fileDownloadName: fileName);
        }

        public async Task<IActionResult> OnPostUploadProfilePhotoAsync(IFormFile file)
        {
            var validataionResult = ValidateFileUpload(file);

            if (validataionResult != null && validataionResult.Count > 0)
            {
                foreach (var error in validataionResult)
                {
                    ModelState.AddModelError("UpploadError", error);
                }
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);


            string fileName = await _profilePictureUploader.SaveFile(file);
            user.ProfilePhotoFileName = fileName;
            await _userManager.UpdateAsync(user);
            LoadProfilePhoto(user.ProfilePhotoFileName);
            return Page();
        }


        public async void LoadProfilePhoto(string fileName)
        {
            byte[] fileBytes = await _profilePictureUploader.ReadFileIntoMemory(fileName);
            var imageDate = Convert.ToBase64String(fileBytes);
            var fileExtention = _profilePictureUploader.GetFileExtentsion(fileName);


            ViewData["ImageSource"] = $"data:image/{fileExtention};base64,{imageDate}";
            ViewData["ImageAlt"] = "Image Loaded";


        }

        private List<string> ValidateFileUpload(IFormFile file)
        {


            List<string> errors = new List<string>();

            if (file == null)
            {
                errors.Add("No file selected");
                return errors;
            }

            if (file.Length > 10000000)
            {
                errors.Add("File exceeds the 10mb limit.");
            }

            if (file.FileName.Contains("."))
            {
                string[] acceptableExtensions = { "png", "bmp", "jpg", "jpeg" };
                string extention = file.FileName.Split('.').LastOrDefault();
                if (extention == null)
                {
                    errors.Add("File does not have an acceptable extention");
                }
                else
                {
                    if (!acceptableExtensions.Any(e => e.Equals(extention)))
                    {
                        errors.Add($"The file extension of {extention} is not allowed.");
                    }
                }
            }
            return errors;
        }


    }
}
