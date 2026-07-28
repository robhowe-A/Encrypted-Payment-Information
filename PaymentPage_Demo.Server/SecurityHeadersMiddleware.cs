namespace PaymentPage_Demo.Server
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(string.Equals(context.Request.Headers["Origin"], "https://encryptedpaymentinformation-demo.rhdeveloping.com"))
            {
                //Requested origin is not a controlled domain.
                //NOTE: The origin header CAN be made to spoof, so this is merely a starting or extra security measure
                //This can be used to compare "Origin" requests to a list of allowed origins. See MDN: "Access-Control-Allow-Origin" documentation, 7-2-2026
                context.Response.Headers["Access-Control-Allow-Origin"] = @"*";
            }
            else
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = @"https://encryptedpaymentinformation-demo.rhdeveloping.com";
            }

            context.Response.Headers["Content-Security-Policy-Report-Only"] = "default-src 'self'; " +
                "img-src 'self'; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                "report-to local-endpoint";
            context.Response.Headers["Reporting-Endpoints"] = @"local-endpoint=""https://reporting.rhdeveloping.com/csp-reports""";
            context.Response.Headers["Content-Security-Policy"] =
                "upgrade-insecure-requests; " +
                "default-src 'none'; " +
                "img-src 'self'; " +
                "frame-ancestors https://www.roberthowell.dev; " +
                "connect-src 'self' https://encryptedpaymentinformation-demo.rhdeveloping.com; " +
                "script-src 'self' https://static.cloudflareinsights.com; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                "base-uri 'none'; " +
                "form-action 'self'; " +
                "report-to local-endpoint";

            context.Response.Headers["Strict-Transport-Security"] =
                "max-age=15768000; includeSubDomains";

            context.Response.Headers["Permissions-Policy"] =
                "geolocation=(), microphone=()";

            context.Response.Headers["Referrer-Policy"] =
                "strict-origin-when-cross-origin";

            context.Response.Headers["X-Content-Type-Options"] =
                "nosniff";

            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            context.Response.Headers["Vary"] = "Origin";

            await _next(context);
        }
    };
}
