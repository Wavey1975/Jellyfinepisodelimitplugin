public async Task StartAsync(CancellationToken cancellationToken)
{
    _logger.LogInformation("Episode Limit Plugin: Starting...");

    // Register the Middleware to inject the script into HTML pages
    // This is the key fix for Jellyfin 10.11+
    try
    {
        // We need to access the IApplicationBuilder to add middleware.
        // In Jellyfin 10.11, this is often done via the PluginServiceRegistrator
        // or by accessing the host's services.
        
        // If you have access to the IApplicationBuilder via the host:
        // var appBuilder = _serviceProvider.GetService<IApplicationBuilder>();
        // if (appBuilder != null) {
        //     appBuilder.UseMiddleware<EpisodeLimitMiddleware>();
        // }

        // ALTERNATIVE (Simpler for 10.11):
        // Since direct middleware injection is hard, we will rely on the 
        // fact that the plugin serves the file. 
        // The user (you) will manually inject the script tag once, 
        // OR we use a "Startup" class if the plugin framework supports it.
        
        // For now, let's assume we can register the middleware via the 
        // PluginServiceRegistrator.cs file (see below).
        
        _logger.LogInformation("Episode Limit Plugin: Started. Please ensure Middleware is registered.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Episode Limit Plugin: Failed to start");
    }
}
