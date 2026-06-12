namespace noKeyCloud.api.CustomMiddleware
{
    public class ContentSecurityPolicyMiddleware
    {
        private readonly RequestDelegate requestDelegate;

        public ContentSecurityPolicyMiddleware(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/scalar"))
            {
                await requestDelegate(context);
                return;
            }

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self'; " +
                    "style-src 'self'; " +
                    "img-src 'self'; " +
                    "font-src 'self'; " +
                    "connect-src 'self'; " +
                    "frame-src 'self'; " +
                    "object-src 'none'; " +
                    "base-uri 'self'; " +
                    "form-action 'self'; " +
                    "frame-ancestors 'none'; " +
                    "upgrade-insecure-requests;");
            }

            await requestDelegate(context);
        }
    }

    public static class ContentSecurityPolicyMiddlewareExtensions
    {
        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
        }
    }
}
