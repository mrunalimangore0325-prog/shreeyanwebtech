using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shreeyan.Models;
using Shreeyan.Services;

namespace Shreeyan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View(GetServices());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View(GetServices());
        }




      



        public IActionResult Companies()
        {
            return View(GetCompanies());
        }

        [HttpGet]
        public IActionResult Careers()
        {
            return View(new CareerApplicationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Careers(CareerApplicationViewModel model)
        {
            var resumeError = model.ValidateResumeFile();
            if (resumeError != null)
            {
                ModelState.AddModelError(nameof(model.ResumeFile), resumeError);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation(
                "New career application from {Name} <{Email}> | Interest: {RoleInterest}",
                model.Name, model.Email, model.RoleInterest);

            try
            {
                await _emailSender.SendCareerApplicationAsync(model, model.ResumeFile);
                TempData["CareerSuccess"] = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Career application email failed to send.");
                TempData["CareerError"] = true;
            }

            return RedirectToAction(nameof(Careers));
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation(
                "New enquiry from {Name} <{Email}> | Project type: {ProjectType}",
                model.Name, model.Email, model.ProjectType);

            try
            {
                await _emailSender.SendContactMessageAsync(model);
                TempData["ContactSuccess"] = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact form email failed to send.");
                TempData["ContactError"] = true;
            }

            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ---- Placeholder content. Edit freely - nothing here touches the views. ----

        private static List<ServiceItem> GetServices() => new()
        {
            new ServiceItem
            {
                IconKey = "globe",
                Title = "Website Development",
                Description = "Modern, responsive websites designed and built around what your business actually needs.",
                Includes = new() { "Custom layout & UX", "Mobile responsive", "Fast, clean builds", "SEO-friendly markup" }
            },
            new ServiceItem
            {
                IconKey = "layers",
                Title = "Business Websites",
                Description = "Professional websites that establish credibility and help businesses connect with customers.",
                Includes = new() { "Brand-led design", "Lead & contact forms", "Content structure", "Analytics-ready" }
            },
            new ServiceItem
            {
                IconKey = "cart",
                Title = "E-Commerce Development",
                Description = "Modern online stores designed for smooth, secure, and conversion-focused shopping experiences.",
                Includes = new() { "Product catalogue", "Cart & checkout", "Payment integration", "Order management" }
            },
            new ServiceItem
            {
                IconKey = "code",
                Title = "Custom Web Applications",
                Description = "Web-based applications built around specific business workflows, .",
                Includes = new() { "Custom workflows", "Admin dashboards", "Database-backed", "Role-based access" }
            },
            new ServiceItem
            {
                IconKey = "spark",
                Title = "Portfolio & Professional Sites",
                Description = "Modern websites for professionals, creators, and individuals who want to stand out.",
                Includes = new() { "Personal branding", "Case study layouts", "Fast load times", "Simple to update" }
            },
            new ServiceItem
            {
                IconKey = "wrench",
                Title = "Website Maintenance",
                Description = "Ongoing improvements, updates, and technical support so your site keeps running well.",
                Includes = new() { "Regular updates", "Uptime monitoring", "Security patches", "Priority support" }
            },
            new ServiceItem
            {
                IconKey = "lock",
                Title = "Cyber Security Solutions",
                Description = "A dedicated security team hardening, monitoring, and protecting your applications and infrastructure.",
                Includes = new() { "Security audits", "Vulnerability testing", "Data protection", "Ongoing monitoring" }
            },
            new ServiceItem
            {
                IconKey = "spark",
                Title = "Special Purpose Software",
                Description = "Purpose-built software and automation for workflows that off-the-shelf tools can't handle.",
                Includes = new() { "Custom automation", "Industry-specific tools", "Process digitisation", "Full customisation" }
            },
            new ServiceItem
            {
                IconKey = "target",
                Title = "Meta Ads Management",
                Description = "Facebook & Instagram ad campaigns built to drive qualified leads and sales, backed by ongoing optimisation.",
                Includes = new() { "Campaign strategy & setup", "Audience targeting", "Creative & copy", "Performance tracking & optimisation" }
            },
        };

        private static List<Company> GetCompanies() => new()
        {
            new Company
            {
                Name = "ShreeYaan Web Tech",
                Tagline = "Websites & web applications",
                IconKey = "code",
                Description = "The group's software arm — building websites, custom web applications, and cyber security solutions .",
                Established = "Since 2022",
                Highlights = new() { "Website & app development", "Cyber security", "Special purpose software" },
                Url = "/"
            },
            new Company
            {
                Name = "Crystal Pro Automation",
                Tagline = "Industrial automation solutions",
                IconKey = "building",
                Description = "Designs and installs industrial automation systems for manufacturing and process industries.",
                Established = "Group company",
                Highlights = new() { "PLC & SCADA systems", "Process automation", "Panel design & build" },
                Url = null
            },
            new Company
            {
                Name = "Shivam Enterprise",
                Tagline = "Enterprise & trading solutions",
                IconKey = "briefcase",
                Description = "Handles enterprise trading and supply solutions across a range of industrial clients.",
                Established = "Group company",
                Highlights = new() { "B2B trading", "Supply chain support", "Vendor sourcing" },
                Url = null
            },
            new Company
            {
                Name = "Tara Green Energy",
                Tagline = "Solar panels & LED lighting",
                IconKey = "spark",
                Description = "Supplies and installs solar power systems and energy-efficient LED lighting solutions.",
                Established = "Group company",
                Highlights = new() { "Solar installation", "LED lighting", "Energy efficiency consulting" },
                Url = null
            },
            new Company
            {
                Name = "Bharmal Preci Works",
                Tagline = "Precision engineering works",
                IconKey = "target",
                Description = "Precision engineering and machining services for industrial and manufacturing clients.",
                Established = "Group company",
                Highlights = new() { "CNC machining", "Precision tooling", "Custom fabrication" },
                Url = null
            },
        };
    }
}



