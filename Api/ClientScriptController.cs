using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.EpisodeLimit.Api;

/// <summary>
/// Serves the client-side JavaScript for the player UI.
/// </summary>
[ApiController]
[Route("EpisodeLimit")]
public class ClientScriptController : ControllerBase
{
    /// <summary>
    /// Get the client-side JavaScript for the player UI.
    /// This can be loaded by the web client to add the episode limit button.
    /// </summary>
    /// <returns>The JavaScript file.</returns>
    [HttpGet("episodelimit.js")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/javascript")]
    public ActionResult GetClientScript()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Jellyfin.Plugin.EpisodeLimit.Web.episodelimit.js";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return NotFound("Client script not found");
        }
        
        using var reader = new StreamReader(stream);
        var script = reader.ReadToEnd();
        
        return Content(script, "application/javascript");
    }
}
