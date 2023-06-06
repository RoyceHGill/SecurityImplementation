using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JokesMVC2023.Controllers
{
    /// <summary>
    /// An alternative to having an MVC controller recieve a request and return a response.
    /// API Controllers are better set up to handle requests containing JSOn and to respond to Async Fetch/AJAX requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class SettingsController : ControllerBase
    {
        /// <summary>
        /// Recieves a string that describes the new theme on the client
        /// </summary>
        /// <remarks>
        ///     This governs which CSS file will be returned to the client on the next response.
        /// </remarks>
        /// <param name="updatedTheme">either 'light' or 'dark' depending on which theme has just been set</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("SetTheme")]
        public async Task<IActionResult> SetTheme([FromBody] ThemeSetting updatedTheme)
        {
            HttpContext.Session.SetString("theme", updatedTheme.Theme);
            return Ok();
        }
    }

    /// <summary>
    /// Created to help the Framework with extracting the parameters from a fetch request from the client
    /// </summary>
    public class ThemeSetting
    {
        public string Theme { get; set; }
    }
}
