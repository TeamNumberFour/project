﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using WebApplication2.Models.Requests;
using WebApplication2.Data;
using WebApplication2.Models.AccountViewModels;
using WebApplication2.Services;
using Microsoft.EntityFrameworkCore;
using VkNet.Utils;
using Shark.PdfConvert;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;


namespace WebApplication2.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly string _externalCookieScheme;
        private readonly ApplicationDbContext context;
        private readonly IParsingService parsingService;
        
        public RequestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            ApplicationDbContext context,
            IParsingService parsingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            this.context = context;
            this.parsingService = parsingService;
        }



        
        public async Task<IActionResult> Choose()
        {
            
            var Univer = await this.context.Universities.ToListAsync();
            var Faculty= await this.context.Faculties.ToListAsync();
            ViewBag.Logout = "Выйти";
            ViewBag.Find = "Найти";
            ViewBag.Un = "Университет";
            ViewBag.Fa = "Факультет";
            ViewBag.Uns = "Университеты";
            ViewBag.Us = "Пользователи";



            var model = new Choose
            {
                Universities = (ICollection<University>)Univer,
                Faculties = (ICollection<Faculty>)Faculty


            };

            return this.View(model);

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choose(Choose model, string un, string fa, bool locker)
        {
            if (!locker)
            {
                ViewBag.Logout = "Выйти";
                ViewBag.Find = "Найти";
                ViewBag.Un = "Университет";
                ViewBag.Fa = "Факультет";
                ViewBag.Uns = "Университеты";
                ViewBag.Us = "Пользователи";
                var Univer = await this.context.Universities.ToListAsync();
                var Faculty = await this.context.Faculties.ToListAsync();
                var model1 = new Choose
                {
                    Universities = (ICollection<University>)Univer,
                    Faculties = (ICollection<Faculty>)Faculty,
                    University = un


                };
                this.ViewBag.University = un;
                return this.View(model1);
            }
            ViewBag.Logout = "Выйти";
            ViewBag.Us = "Пользователи";
            ViewBag.Uns = "Университеты";
            ViewBag.Negative = "Отрицательные";
            ViewBag.Neutral = "Нейтральные";
            ViewBag.Positive = "Положительные";
            ViewBag.Undefined = "Неопределённые";
            ViewBag.Export = "Сохранить";
            ViewBag.Startover = "Начать заново";
            ViewBag.Filters = "Фильтры";
            ViewBag.Target = "Целевая аудитория";
            ViewBag.Emp1 = "Работники";
            ViewBag.Emp2 = "Работодатели";
            ViewBag.Stu = "Студенты";
            ViewBag.Ab = "Абитуриенты";
            ViewBag.Grad = "Выпускники";
            ViewBag.Nick = "Имя пользователя";
            ViewBag.Em = "Эмоциональная окраска";
            ViewBag.Key = "Ключевые слова";
            ViewBag.Time = "Временной промежуток";
            ViewBag.From = "От";
            ViewBag.To = "До";
            ViewBag.Accept = "Принять";



            Post[] posts =await parsingService.Common(un + " " + fa, un, fa);
            
                var itemsToDelete = context.Set<Post>();
                context.Posts.RemoveRange(itemsToDelete);
                await context.SaveChangesAsync();
            int Count = 0;
            double vkc = 0;
            double gazetac = 0;
            double vestic = 0;
            double riac = 0;
            double twitc = 0;
            double newsc = 0;
            double profc = 0;
            double regnumc = 0;
            double positivec = 0;
            double negativec = 0;
            double neutralc = 0;
            double uncertainc = 0;
            foreach (var postitem in posts)
            {
                var post = new Post
                {
                    ownersName=postitem.ownersName,
                    text=postitem.text,
                    date=postitem.date,
                    comments=postitem.comments,
                    link=postitem.link,
                    emo=postitem.emo,
                    pass=postitem.pass,
                    source=postitem.source
                };
                Count++;
                
                switch (post.source)
                {
                    case "vk":
                        vkc++;
                        break;
                    case "gazeta":
                        gazetac++;
                        break;
                    case "vesti":
                        vestic++;
                        break;
                    case "prof":
                        profc++;
                        break;
                    case "regnum":
                        regnumc++;
                        break;
                    case "news.ru":
                        newsc++;
                        break;
                    case "twitter":
                        twitc++;
                        break;
                    case "ria":
                        riac++;
                        break;


                }
                switch (post.emo)
                {
                    case 0:
                        positivec++;
                        break;
                    case 1:
                        negativec++;
                        break;
                    case 2:
                        neutralc++;
                        break;
                    case 3:
                        uncertainc++;
                        break;
                    


                }
                context.Posts.Add(post);
            }
            this.ViewBag.vkc = vkc;
            this.ViewBag.newsc = newsc;
            this.ViewBag.riac = riac;
            this.ViewBag.twitc = twitc;
            this.ViewBag.gazetac = gazetac;
            this.ViewBag.vestic = vestic;
            this.ViewBag.profc = profc;
            this.ViewBag.regnumc = regnumc;
            this.ViewBag.positivec = positivec;
            this.ViewBag.negativec = negativec;
            this.ViewBag.neutralc = neutralc;
            this.ViewBag.uncertainc = uncertainc;
            if (vkc > 0) ViewBag.vkp = (int)(vkc / Count * 100); else ViewBag.vkp = 0;
            if (riac > 0) ViewBag.riap = (int)(riac / Count * 100); else ViewBag.riap = 0;
            if (newsc > 0) ViewBag.newsp = (int)(newsc / Count * 100); else ViewBag.newsp = 0;
            if (twitc > 0) ViewBag.twitp = (int)(twitc / Count * 100); else ViewBag.twitp = 0;
            if (gazetac > 0) ViewBag.gazetap = (int)(gazetac / Count * 100); else ViewBag.gazetap = 0;
            if (vestic > 0) ViewBag.vestip = (int)(vestic / Count * 100); else ViewBag.vestip = 0;
            if (profc > 0) ViewBag.profp = (int)(profc / Count * 100); else ViewBag.profp = 0;
            if (regnumc > 0) ViewBag.regnump = (int)(regnumc / Count * 100); else ViewBag.regnump = 0;
            if (positivec > 0) ViewBag.positivep = (int)(positivec / Count * 100); else ViewBag.positivep = 0;
            if (negativec > 0) ViewBag.negativep = (int)(negativec / Count * 100); else ViewBag.negativep = 0;
            if (neutralc > 0) ViewBag.neutralp = (int)(neutralc / Count * 100); else ViewBag.neutralp = 0;
            if (uncertainc > 0) ViewBag.uncertainp = (int)(uncertainc / Count * 100); else ViewBag.uncertainp = 0;


            await this.context.SaveChangesAsync();

            var Postmass = await this.context.Posts.ToListAsync();
            return this.View("List", new List
                 {
                     
                     Posts=(ICollection<Post>)Postmass,
                     Query=un+" "+fa
                     
                 });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filtered(List model, string query, string export, string nickname, bool employer, bool grad, bool abitur, bool student, bool employee, bool neutral, bool negative, bool positive, bool uncertain, string data1, string data2, string key)
        {
            ViewBag.Logout = "Выйти";
            ViewBag.Us = "Пользователи";
            ViewBag.Uns = "Университеты";
            ViewBag.Negative = "Отрицательные";
            ViewBag.Neutral = "Нейтральные";
            ViewBag.Positive = "Положительные";
            ViewBag.Undefined = "Неопределённые";
            ViewBag.Export = "Сохранить";
            ViewBag.Startover = "Начать заново";
            ViewBag.Filters = "Фильтры";
            ViewBag.Target = "Целевая аудитория";
            ViewBag.Emp1 = "Работники";
            ViewBag.Emp2 = "Работодатели";
            ViewBag.Stu = "Студенты";
            ViewBag.Ab = "Абитуриенты";
            ViewBag.Grad = "Выпускники";
            ViewBag.Nick = "Имя пользователя";
            ViewBag.Em = "Эмоциональная окраска";
            ViewBag.Key = "Ключевые слова";
            ViewBag.Time = "Временной промежуток";
            ViewBag.From = "От";
            ViewBag.To = "До";
            ViewBag.Accept = "Принять";


            if (export != null)
            { 
                export = this.parsingService.getstr(export);
                var res = "<!DOCTYPE html> <html lang=\"ru\"> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/> </head> " + "<div class=\"bg-blue\">" ;
                res += "<div>" + model.Query + "</div>";
                res += "<div> Целевая аудитория:";
                if (model.employee) res += "Работники ";
                if (model.student && model.employee) res += ",Студенты ";
                else if (student) res += "Students ";
                if (abitur && (student || employee)) res += ",Абитуриенты ";
                else if (abitur) res += "Abiturients ";
                if (grad && (abitur || student || employee)) res += ",Выпускники ";
                else if (grad) res += "Graduates ";
                if (employer && (grad || abitur || student || employee)) res += ",Работодатели ";
                else if (employer) res += "Работодатели ";
                res += "</div>";
                res += "<div> Эмоциональная окраска:";

                if (positive) res += "Положительные  ";
                if (negative && positive) res += ",Отрицательные  ";
                else if (negative) res += "Отрицательные  ";
                if (neutral && (negative || positive)) res += ",Нейтральные  ";
                else if (neutral) res += "Нейтральные  ";
                if (uncertain && (neutral || negative || positive)) res += ",Неопределённые  ";
                else if (uncertain) res += "Неопределённые  ";
                res += "</div>";


                res += "<div> Ключевые слова - " + key + "</div>";

                if(data1 != "01.01.0001")
                res += "<div> От - " + data1 + " До - " + data2 + "</div>";


                res += export + "</div></html>";

                var convertSettings = new PdfConversionSettings()
                {
                    PdfToolPath = Path.Combine(Directory.GetCurrentDirectory(),
           "wkhtmltopdf.exe"),
                    Title = "Salg",
                    Content = res
                };
                using (var memoryStream = new MemoryStream())
                {
                    PdfConvert.Convert(convertSettings, memoryStream);
                    return File(memoryStream.ToArray(), "application/pdf");
                }

            }



            int Count = 0;
            double vkc = 0;
            double gazetac = 0;
            double vestic = 0;
            double profc = 0;
            double regnumc = 0;
            double riac = 0;
            double twitc = 0;
            double newsc = 0;
            double positivec = 0;
            double negativec = 0;
            double neutralc = 0;
            double uncertainc = 0;
            List<Post> Filt = new List<Post>();
            var Poststofilter = await this.context.Posts.ToListAsync() ;
            foreach (var item in Poststofilter)
            {
                if(model.positive || model.negative || model.neutral || model.uncertain ) switch (item.emo)
                {
                    case 0:
                        if (!model.positive) item.pass = false;
                        break;
                    case 1:
                        if (!model.negative) item.pass = false;
                        break;
                    case 2:
                        if (!model.neutral) item.pass = false;
                        break;
                    case 3:
                        if (!model.uncertain) item.pass = false;
                        break;
                }
                
                if (!item.source.Equals("regnum") &&model.data1>=DateTime.Now.AddDays(-21) && model.data2 <= DateTime.Now) if (DateTime.Parse(item.date) < model.data1 || DateTime.Parse(item.date)> model.data2) item.pass = false;
                if(model.nickname!=null)if (string.Compare(item.ownersName,model.nickname)!=0) item.pass = false;
                if (model.key != null) if (!item.text.Contains(model.key)) item.pass = false;
                List<string> check = new List<string>();
                if (model.abitur) { check.Add("vk"); check.Add("prof"); }
                if (model.student) check.Add("vk");
                if (model.employee) { check.Add("regnum"); check.Add("vesti"); }
                if (model.employer) { check.Add("regnum"); check.Add("vesti"); check.Add("vk"); check.Add("prof"); check.Add("gazeta"); }
                if (model.grad) { check.Add("regnum"); check.Add("vesti"); check.Add("vk"); check.Add("prof"); }
                if (!check.Contains(item.source) && check.Count>0) { item.pass = false; }



                if (item.pass)
                {
                    Count++;

                    switch (item.source)
                    {
                        case "vk":
                            vkc++;
                            break;
                        case "gazeta":
                            gazetac++;
                            break;
                        case "vesti":
                            vestic++;
                            break;
                        case "prof":
                            profc++;
                            break;
                        case "regnum":
                            regnumc++;
                            break;
                        case "news.ru":
                            newsc++;
                            break;
                        case "twitter":
                            twitc++;
                            break;
                        case "ria":
                            riac++;
                            break;

                    }
                    switch (item.emo)
                    {
                        case 0:
                            positivec++;
                            break;
                        case 1:
                            negativec++;
                            break;
                        case 2:
                            neutralc++;
                            break;
                        case 3:
                            uncertainc++;
                            break;



                    }
                    Filt.Add(item);
                }

            }
            this.ViewBag.vkc = vkc;
            this.ViewBag.newsc = newsc;
            this.ViewBag.riac = riac;
            this.ViewBag.twitc = twitc;
            this.ViewBag.gazetac = gazetac;
            this.ViewBag.vestic = vestic;
            this.ViewBag.profc = profc;
            this.ViewBag.regnumc = regnumc;
            this.ViewBag.positivec = positivec;
            this.ViewBag.negativec = negativec;
            this.ViewBag.neutralc = neutralc;                           
            this.ViewBag.uncertainc = uncertainc;
            if (vkc > 0) ViewBag.vkp = (int) (vkc / Count * 100); else ViewBag.vkp=0;
            if (riac > 0) ViewBag.riap = (int)(riac / Count * 100); else ViewBag.riap = 0;
            if (newsc > 0) ViewBag.newsp = (int)(newsc / Count * 100); else ViewBag.newsp = 0;
            if (twitc > 0) ViewBag.twitp = (int)(twitc / Count * 100); else ViewBag.twitp = 0;
            if (gazetac > 0) ViewBag.gazetap = (int)(gazetac / Count * 100); else ViewBag.gazetap = 0;
            if (vestic > 0) ViewBag.vestip = (int)(vestic / Count * 100); else ViewBag.vestip = 0;
            if (profc > 0) ViewBag.profp = (int)(profc / Count * 100); else ViewBag.profp = 0;
            if (regnumc > 0) ViewBag.regnump = (int)(regnumc / Count * 100); else ViewBag.regnump = 0;
            if (positivec > 0) ViewBag.positivep = (int)(positivec / Count * 100); else ViewBag.positivep = 0;
            if (negativec > 0) ViewBag.negativep = (int)(negativec / Count * 100); else ViewBag.negativep = 0;
            if (neutralc > 0) ViewBag.neutralp = (int)(neutralc / Count * 100); else ViewBag.neutralp = 0;
            if (uncertainc > 0) ViewBag.uncertainp = (int)(uncertainc / Count * 100); else ViewBag.uncertainp = 0;

            return this.View("List", new List
            {
                filtered = true,
                employee=model.employee,
                student=model.student,
                abitur=model.abitur,
                employer=model.employer,
                grad=model.grad,
                nickname=model.nickname,
                key=key,
                positive=model.positive,
                negative=model.negative,
                neutral=model.neutral,
                uncertain=model.uncertain,
                data1=model.data1,
                data2=model.data2,
                Posts=(ICollection<Post>)Filt,
                Query=model.Query

            });



        }

    }
}
