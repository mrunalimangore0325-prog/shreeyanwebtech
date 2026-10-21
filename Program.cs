using Microsoft.AspNetCore.HttpOverrides;
using Shreeyan.Models;
using Shreeyan.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Contact form email delivery (sent via Brevo's HTTPS transactional API,
// not SMTP — many hosts, including Render's free tier, block outbound SMTP)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHttpClient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Cloud Run (and most cloud load balancers) terminate HTTPS at the edge and
// forward plain HTTP to the container, adding X-Forwarded-* headers so the
// app still knows the original request was secure. Trust those headers so
// UseHttpsRedirection/UseHsts and Url.Action behave correctly behind it.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Old "/Home/..." addresses permanently redirect to the clean URLs below, so
// Google only ever indexes one address per page (and sitelinks look tidy).
var legacyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["/Home"] = "/",
    ["/Home/Index"] = "/",
    ["/Home/Services"] = "/services",
    ["/Home/About"] = "/about-us",
    ["/Home/Portfolio"] = "/portfolio",
    ["/Home/Careers"] = "/careers",
    ["/Home/Contact"] = "/contact-us",
    ["/Home/Companies"] = "/group-companies",
    ["/Home/Privacy"] = "/privacy-policy",
};
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.TrimEnd('/') ?? "";
    if (HttpMethods.IsGet(context.Request.Method) && legacyPaths.TryGetValue(path, out var target))
    {
        context.Response.Redirect(target + context.Request.QueryString, permanent: true);
        return;
    }
    await next();
});

app.UseRouting();

app.UseAuthorization();

// Clean, keyword-friendly URLs (these are what Google shows as sitelinks).
app.MapControllerRoute(name: "services", pattern: "services", defaults: new { controller = "Home", action = "Services" });
app.MapControllerRoute(name: "about-us", pattern: "about-us", defaults: new { controller = "Home", action = "About" });
app.MapControllerRoute(name: "portfolio", pattern: "portfolio", defaults: new { controller = "Home", action = "Portfolio" });
app.MapControllerRoute(name: "careers", pattern: "careers", defaults: new { controller = "Home", action = "Careers" });
app.MapControllerRoute(name: "contact-us", pattern: "contact-us", defaults: new { controller = "Home", action = "Contact" });
app.MapControllerRoute(name: "group-companies", pattern: "group-companies", defaults: new { controller = "Home", action = "Companies" });
app.MapControllerRoute(name: "privacy-policy", pattern: "privacy-policy", defaults: new { controller = "Home", action = "Privacy" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();