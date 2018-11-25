using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ArianWebsiteMVC.Classes;
using DataLayer;
using DataLayer.Services;

namespace ArianWebsiteMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _db = new DataContext();
        private readonly IProductRepository _productRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly IContactUsRepository _contactUsRepository;
        private readonly INewsLatterRepository _newsLatterRepository;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerSatisfactionRepository _customerSatisfactionRepository;
        private readonly IPriceDateRepository _priceDateRepository;
        private readonly IProformaRepository _proformaRepository;
        public HomeController()
        {
            _productRepository = new ProductRepository(_db);
            _visitorRepository = new VisitorRepository(_db);
            _contactUsRepository = new ContactUsRepository(_db);
            _newsLatterRepository = new NewsLatterRepository(_db);
            _employmentRepository = new EmploymentRepository(_db);
            _supplierRepository = new SupplierRepository(_db);
            _customerSatisfactionRepository = new CustomerSatisfactionRepository(_db);
            _priceDateRepository = new PriceDateRepository(_db);
            _proformaRepository = new ProformaRepository(_db);

        }
        private readonly string _userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
        private static IEnumerable<string> EmailList()
        {

            var toList = new List<string>
            {
                "ghavami.it@gmail.com",
                "ariansattari@gmail.com",
                "arianEdarikar@gmail.com"
            };
            return toList;
        }
        private static IEnumerable<string> ManageEmailList()
        {

            var toList = new List<string>
            {
                "mohsenayyari@yahoo.com",
                "bahmangroup1@gmail.com",
                "ghavami.it@gmail.com",
                "ariansattari@gmail.com"
            };
            return toList;
        }
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Slider()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult BottomBar()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult ShowPrice()
        {
            ViewBag.PriceUpdate = _priceDateRepository.LastPriceUpdate().ToShamshi();
            return PartialView(_productRepository.ShowProductModelViews());
        }
        [ChildActionOnly]
        public ActionResult ShowAllProcuctLine()
        {
            return PartialView();
        }
        public ActionResult Visit()
        {
            var visitor = new Visitor()
            {
                DateTime = DateTime.Now,
                Ip = _userIp,
                Browser = Request.GetBrowser(),
                Windows = Request.UserAgent
            };
            _visitorRepository.InsertVisitor(visitor);
            _visitorRepository.Dispose();
            _db.Dispose();
            return null;
        }
        [Route("ContactUs")]
        public ActionResult ContactUs()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Contact()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Contact(ContactUs contactUs)
        {
            contactUs.Ip = _userIp;
            contactUs.DateTime = DateTime.Now;
            _contactUsRepository.InsertContactUs(contactUs);
            #region Send Email
            var toList = EmailList();
            var body = "<html><body dir='rtl'   style='font-family: tahoma'>"
                       + "<p>نام و نام خانوادگی   : " + contactUs.Name + "</p>" + "<hr/>" + ""
                       + "<p>ایمیل  : " + contactUs.Email + "</p>" + "<hr/>" + ""
                       + "<p>موضوع پیام   : " + contactUs.Subject + "</p>" + "<hr/>" + ""
                       + "<p>متن پیام   : " + contactUs.Text + "</p>" + ""
                       + "<p>IP : " + contactUs.Ip + "</p>" + ""
                       + "</body></html>";
            MailMessage mail = new MailMessage();
            //پارامتر این شی همان حالت معرفی شده در تنظیمات ایمیل سرور می‌باشد که پیشتر معرفی شد.
            SmtpClient smtpServer = new SmtpClient("mail.ariansteel.com");
            mail.Subject = "انتقادات و پشنهادات سایت";
            mail.From = new MailAddress("No_Reply@ariansteel.com");
            //ایمیل گیرنده نامه
            mail.To.Add(string.Join(",", toList));
            //متن نامه
            mail.Body = body;
            mail.IsBodyHtml = true;
            //شماره پورت در اینجا حالت ارسال معمولی و غیر رمز شده مد نظر بوده است
            smtpServer.Port = 587;
            //email address      ,email password
            smtpServer.Credentials = new NetworkCredential("No_Reply@ariansteel.com", "Sa@1304343");
            smtpServer.EnableSsl = false;
            smtpServer.Send(mail);
            #endregion
            _contactUsRepository.Dispose();
            _db.Dispose();
            TempData["MessageHeader"] = "با تشکر";
            TempData["AlertMessage"] = "پیام شما ارسال گردید.";
            return Redirect(Request.UrlReferrer?.ToString() ?? "/");
        }
        public ActionResult NewsLatter()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult NewsLatter(NewsLatter newsLatter)
        {
            if (ModelState.IsValid)
            {
                if (_newsLatterRepository.GetNewsLatterByEmail(newsLatter.EmailAdressNewslatter))
                {
                    TempData["MessageHeader"] = "عدم ثبت";
                    TempData["AlertMessage"] = "ایمل شما در سیستم موجود است";
                }
                else
                {
                    newsLatter.Ip = _userIp;
                    newsLatter.DateTime = DateTime.Now;
                    _newsLatterRepository.InsertNewsLatter(newsLatter);
                    _newsLatterRepository.Dispose();
                    _db.Dispose();
                    TempData["MessageHeader"] = "ثبت موفق";
                    TempData["AlertMessage"] = "ایمل شما با موفقیت ثبت گردید";
                } 
            }
            return Redirect(Request.UrlReferrer?.ToString() ?? "/");
        }
        [Route("Employment")]
        public ActionResult Employment()
        {
            return View();
        }
        public ActionResult AddEmployment()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddEmployment(Employment employment, string sex, string relation, string education, HttpPostedFileBase imgUp)
        {
            try
            {
                employment.Ip = _userIp;
                employment.DateTime = DateTime.Now;
                employment.Sex = sex;
                employment.Relation = relation;
                employment.Education = education;
                if (imgUp != null)
                {
                    if (imgUp.ContentType != "application/msword" && imgUp.ContentType != "application/pdf" &&
                        imgUp.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("imgUp", "فقط فایلهای doc و pdf و Jpg مجاز میباشد");
                    }
                    employment.ImageName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/ImgRezoome/" + employment.ImageName));
                }
                _employmentRepository.InsertEmployment(employment);
                #region Send Email
                var toList = EmailList();
                var body = "<html><body dir='rtl'   style='font-family: tahoma'>"
                           + "<p>نام   : " + employment.Name + "</p>" + "<hr/>" + ""
                           + "<p>نام پدر   : " + employment.FatherName + "</p>" + "<hr/>" + ""
                           + "<p>کد ملی   : " + employment.NationalId + "</p>" + "<hr/>" + ""
                           + "<p>سال تولد   : " + employment.BirthDate + "</p>" + "<hr/>" + ""
                           + "<p>محل تولد  : " + employment.BirthLocation + "</p>" + "<hr/>" + ""
                           + "<p>جنسیت  : " + employment.Sex + "</p>" + "<hr/>" + ""
                           + "<p>وضعیت تأهل  : " + employment.Relation + "</p>" + "<hr/>" + ""
                           + "<p>تحصیلات  : " + employment.Education + "</p>" + "<hr/>" + ""
                           + "<p>رشته تحصیلی   : " + employment.FieldStudy + "</p>" + "<hr/>" + ""
                           + "<p>شغل مورد علاقه   : " + employment.FavoriteJob + "</p>" + "<hr/>" + ""
                           + "<p>تلفن   : " + employment.Tel + "</p>" + "<hr/>" + ""
                           + "<p>موبایل  : " + employment.Mobile + "</p>" + "<hr/>" + ""
                           + "<p>ایمیل  : " + employment.Email + "</p>" + "<hr/>" + ""
                           + "<p>سوابق شغلی  : " + employment.LastJob + "</p>" + "<hr/>" + ""
                           + "<p>آدرس  : " + employment.Address + "</p>" + "<hr/>" + ""
                           + "<p>IP  : " + employment.Ip + "</p>" + "<hr/>" + ""
                           + "</body></html>";
                MailMessage mail = new MailMessage();
                //پارامتر این شی همان حالت معرفی شده در تنظیمات ایمیل سرور می‌باشد که پیشتر معرفی شد.
                SmtpClient smtpServer = new SmtpClient("mail.ariansteel.com");
                mail.Subject = "ارسال رزومه از سایت";
                mail.From = new MailAddress("No_Reply@ariansteel.com");
                //ایمیل گیرنده نامه
                mail.To.Add(string.Join(",", toList));
                //متن نامه
                mail.Body = body;
                mail.IsBodyHtml = true;
                //شماره پورت در اینجا حالت ارسال معمولی و غیر رمز شده مد نظر بوده است
                smtpServer.Port = 587;
                //email address      ,email password
                smtpServer.Credentials = new NetworkCredential("No_Reply@ariansteel.com", "Sa@1304343");
                smtpServer.EnableSsl = false;
                if (imgUp != null)
                {
                    Attachment attachment = new Attachment(imgUp.InputStream, imgUp.FileName);
                    mail.Attachments.Add(attachment);
                }
                smtpServer.Send(mail);
                #endregion
                _employmentRepository.Dispose();
                _db.Dispose();
                TempData["MessageHeader"] = "با تشکر";
                TempData["AlertMessage"] = "مشخصات شما ثبت گردید و پس از بررسی با شما تماس گرفته خواهد شد .";

                return Redirect(Request.UrlReferrer?.ToString() ?? "/");
            }
            catch (Exception)
            {
                TempData["MessageHeader"] = "عدم ثبت";
                TempData["AlertMessage"] = "لطفا مجدد ثبت نمایید.";
                return Redirect(Request.UrlReferrer?.ToString() ?? "/");
            }
        }
        [Route("Supplier")]
        public ActionResult Supplier()
        {
            return View();
        }

        public ActionResult AddSuplier()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddSuplier(Supplier supplier, string nationality, string ownership, string kind, HttpPostedFileBase imgUp)
        {
            try
            {
                supplier.Ip = _userIp;
                supplier.DateTime = DateTime.Now;
                supplier.Nationality = nationality;
                supplier.Ownership = ownership;
                supplier.Kind = kind;
                if (imgUp != null)
                {
                    supplier.ImageName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/ImgSupplier/" + supplier.ImageName));
                }
                _supplierRepository.InsertSupplier(supplier);
                #region Send Email
                var toList = EmailList();
                var body = "<html><body dir='rtl'   style='font-family: tahoma'>"
                           + "<p>نام مجموعه   : " + supplier.CompanyName + "</p>" + "<hr/>" + ""
                           + "<p>نام مدیرعامل   : " + supplier.CeoName + "</p>" + "<hr/>" + ""
                           + "<p>نام مدیر فروش   : " + supplier.SaleManagment + "</p>" + "<hr/>" + ""
                           + "<p>ملیت مجموعه   : " + supplier.Nationality + "</p>" + "<hr/>" + ""
                           + "<p>نوع مالکیت   : " + supplier.Ownership + "</p>" + "<hr/>" + ""
                           + "<p>نحوه تامین   : " + supplier.Kind + "</p>" + "<hr/>" + ""
                           + "<p>شهر   : " + supplier.City + "</p>" + "<hr/>" + ""
                           + "<p>تلفن   : " + supplier.Tel + "</p>" + "<hr/>" + ""
                           + "<p>فکس   : " + supplier.Fax + "</p>" + "<hr/>" + ""
                           + "<p>موبایل   : " + supplier.Mobile + "</p>" + "<hr/>" + ""
                           + "<p>ایمیل   : " + supplier.Emial + "</p>" + "<hr/>" + ""
                           + "<p>آدرس   : " + supplier.Address + "</p>" + "<hr/>" + ""
                           + "<p>کد پستی   : " + supplier.PostalCode + "</p>" + "<hr/>" + ""
                           + "<p>توضیحات   : " + supplier.Description + "</p>" + "<hr/>" + ""
                           + "<p>IP   : " + supplier.Ip + "</p>" + "<hr/>" + ""
                           + "</body></html>";
                MailMessage mail = new MailMessage();
                //پارامتر این شی همان حالت معرفی شده در تنظیمات ایمیل سرور می‌باشد که پیشتر معرفی شد.
                SmtpClient smtpServer = new SmtpClient("mail.ariansteel.com");
                mail.Subject = "ارسال فرم تامین کنندگان کالا از سایت";
                mail.From = new MailAddress("No_Reply@ariansteel.com");
                //ایمیل گیرنده نامه
                mail.To.Add("Ariantadarokat@gmail.com");
                //متن نامه
                mail.Body = body;
                mail.IsBodyHtml = true;
                //شماره پورت در اینجا حالت ارسال معمولی و غیر رمز شده مد نظر بوده است
                smtpServer.Port = 587;
                //email address      ,email password
                smtpServer.Credentials = new NetworkCredential("No_Reply@ariansteel.com", "Sa@1304343");
                smtpServer.EnableSsl = false;
                if (imgUp != null)
                {
                    Attachment attachment = new Attachment(imgUp.InputStream, imgUp.FileName);
                    mail.Attachments.Add(attachment);
                }
                smtpServer.Send(mail);
                #endregion
                _supplierRepository.Dispose();
                _db.Dispose();
                TempData["MessageHeader"] = "با تشکر";
                TempData["AlertMessage"] = "اطلاعات شما با موفقیت ثبت گردید.";
                return RedirectToAction("Supplier", "Home");
            }
            catch (Exception)
            {
                TempData["MessageHeader"] = "عدم ثبت";
                TempData["AlertMessage"] = "لطفا مجدد ثبت نمایید.";
                return Redirect(Request.UrlReferrer?.ToString() ?? "/");
            }
        }
        [Route("CustomerSatisfaction")]
        public ActionResult CustomerSatisfaction()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult AddCustomerSatisfaction()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddCustomerSatisfaction(CustomerSatisfaction customerSatisfaction, string quality, string packing, string price, string timeDelivery, string sale, string feedback, string rate)
        {
            try
            {
                customerSatisfaction.Ip = _userIp;
                customerSatisfaction.DateTime = DateTime.Now;
                customerSatisfaction.Quality = quality;
                customerSatisfaction.Packing = packing;
                customerSatisfaction.Price = price;
                customerSatisfaction.TimeDelivery = timeDelivery;
                customerSatisfaction.Sale = sale;
                customerSatisfaction.Rate = rate;
                customerSatisfaction.Feedback = feedback;
                _customerSatisfactionRepository.InsertCustomerSatisfaction(customerSatisfaction);
                #region Send Email
                var toList = ManageEmailList();
                var body = "<html><body dir='rtl'   style='font-family: tahoma'>"
                           + "<p>نام  : " + customerSatisfaction.CustomerSatisfactionName + "</p>" + "<hr/>" + ""
                           + "<p>تلفن   : " + customerSatisfaction.CustomerSatisfactionTel + "</p>" + "<hr/>" + ""
                           + "<p>نظر شما نسبت به کیفیت محصولات خریداری شده شرکت چیست؟   : " + customerSatisfaction.Quality + "</p>" + "<hr/>" + ""
                           + "<p>نظر شما نسبت به نحوه بسته بندی محصولات چیست؟   : " + customerSatisfaction.Packing + "</p>" + "<hr/>" + ""
                           + "<p>شما قیمت محصولات را در مقایسه با کیفیت محصولات چگونه ارزیابی می کنید؟   : " + customerSatisfaction.Price + "</p>" + "<hr/>" + ""
                           + "<p>شما زمان تحویل محصولات درخواستی را چگونه ارزیابی می کنید؟   : " + customerSatisfaction.TimeDelivery + "</p>" + "<hr/>" + ""
                           + "<p>نظر شما در مورد برخورد بخش فروش با مشتریان چگونه است ؟   : " + customerSatisfaction.Sale + "</p>" + "<hr/>" + ""
                           + "<p>نحوه پاسخگویی به نظرات، پیشنهادات، و شکایاتتان را چگونه ارزیابی می کنید؟   : " + customerSatisfaction.Feedback + "</p>" + "<hr/>" + ""
                           + "<p>امتیازی که در یک نگاه کلی به آریان فولاد می دهید؟   : " + customerSatisfaction.Rate + "</p>" + "<hr/>" + ""
                           + "<p>تولید کنندگان برتر از دیدگاه جنابعالی باید چه ویژگی هایی داشته باشند؟   : " + customerSatisfaction.Question1 + "</p>" + "<hr/>" + ""
                           + "<p>خواهشمند است با ارائه نقطه نظرات، انتقادات و پیشنهادات خود، ما را جهت ارائه خدمات مطلوب تر همراهی فرمائید   : " + customerSatisfaction.Question2 + "</p>" + "<hr/>" + ""
                           + "<p>IP  : " + customerSatisfaction.Ip + "</p>" + "<hr/>" + ""
                           + "</body></html>";
                MailMessage mail = new MailMessage();
                //پارامتر این شی همان حالت معرفی شده در تنظیمات ایمیل سرور می‌باشد که پیشتر معرفی شد.
                SmtpClient smtpServer = new SmtpClient("mail.ariansteel.com");
                mail.Subject = "سنجش رضایت مشتریان در سایت";
                mail.From = new MailAddress("No_Reply@ariansteel.com");
                //ایمیل گیرنده نامه
                mail.To.Add(string.Join(",", toList));
                //متن نامه
                mail.Body = body;
                mail.IsBodyHtml = true;
                //شماره پورت در اینجا حالت ارسال معمولی و غیر رمز شده مد نظر بوده است
                smtpServer.Port = 587;
                //email address      ,email password
                smtpServer.Credentials = new NetworkCredential("No_Reply@ariansteel.com", "Sa@1304343");
                smtpServer.EnableSsl = false;
                smtpServer.Send(mail);
                #endregion
                _customerSatisfactionRepository.Dispose();
                _db.Dispose();
                TempData["MessageHeader"] = "با تشکر";
                TempData["AlertMessage"] = "با تشکر از اینکه در نظرسنجی شرکت نمودید.";
                return Redirect(Request.UrlReferrer?.ToString() ?? "/");
            }
            catch (Exception)
            {
                TempData["MessageHeader"] = "عدم ثبت";
                TempData["AlertMessage"] = "لطفا مجدد ثبت نمایید.";
                return Redirect(Request.UrlReferrer?.ToString() ?? "/");
            }
        }

        public ActionResult Proforma()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Proforma(Proforma proforma, HttpPostedFileBase imgUp)
        {
            if (!ModelState.IsValid) return View(proforma);
            proforma.DateTime = DateTime.Now;
            proforma.Ip = _userIp;
            proforma.ImageName = Guid.NewGuid() + Path.GetExtension(imgUp.FileName);
            imgUp.SaveAs(Server.MapPath("/ImgProforma/" + proforma.ImageName));
            _proformaRepository.InsertProforma(proforma);
            #region Send Email
            var body = "<html><body dir='rtl'   style='font-family: tahoma'>"
                       + "<p>نام مجموعه  : " + proforma.CustomerCompany + "</p>" + "<hr/>" + ""
                       + "<p>نام هماهنگ کننده   : " + proforma.CustomerName + "</p>" + "<hr/>" + ""
                       + "<p>تلفن  : " + proforma.CustomerTel + "</p>" + "<hr/>" + ""
                       + "<p>فکس  : " + proforma.CustomerFax + "</p>" + "<hr/>" + ""
                       + "<p>موبایل  : " + proforma.CustomerMobile + "</p>" + "<hr/>" + ""
                       + "<p>شرح درخواست  : " + proforma.Request + "</p>" + "<hr/>" + ""
                       + "</body></html>";
            MailMessage mail = new MailMessage();
            //پارامتر این شی همان حالت معرفی شده در تنظیمات ایمیل سرور می‌باشد که پیشتر معرفی شد.
            SmtpClient smtpServer = new SmtpClient("mail.ariansteel.com");
            mail.Subject = "درخواست صدور پیش فاکتور از سایت";
            mail.From = new MailAddress("No_Reply@ariansteel.com");
            //ایمیل گیرنده نامه
            mail.To.Add("arinsattari1@gmail.com");
            //متن نامه
            mail.Body = body;
            mail.IsBodyHtml = true;
            //شماره پورت در اینجا حالت ارسال معمولی و غیر رمز شده مد نظر بوده است
            smtpServer.Port = 587;
            //email address      ,email password
            smtpServer.Credentials = new NetworkCredential("No_Reply@ariansteel.com", "Sa@1304343");
            smtpServer.EnableSsl = false;
            {
                Attachment attachment = new Attachment(imgUp.InputStream, imgUp.FileName);
                mail.Attachments.Add(attachment);
            }
            smtpServer.Send(mail);
            #endregion
            _db.Dispose();
            TempData["MessageHeader"] = "با تشکر";
            TempData["AlertMessage"] = "پیش فاکتور درخواستی بزودی تنظیم وبرای شما ارسال خواهد شد";
            return Redirect(Request.UrlReferrer?.ToString() ?? "/");
        }
    }
}
