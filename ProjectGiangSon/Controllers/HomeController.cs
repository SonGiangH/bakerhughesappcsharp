using Microsoft.AspNetCore.Mvc;
using ProjectGiangSon.Models;
using System.Diagnostics;
using ProjectGiangSon.Data;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.SqlServer.Diagnostics.Internal;


namespace ProjectGiangSon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            GiangSonContext context = new GiangSonContext();

            var list = context.Slides.ToList();

            var ListSlide = context.Slides.Select(p => p.SurveyDepth).ToList();

            ViewBag.SensorOffset = context.Sensors.FirstOrDefault().Offset;
            ViewBag.Bur30m = context.Sensors.FirstOrDefault().Bur30m;

            list = Calculator(list);

            var list1 = context.Slides.ToList();

            return View(list1);
        }

        [HttpPost]
        public IActionResult AddDarkSlide(double survey_depth, double inc, double azi)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            int id = giangSonContext.Slides.Count() + 1;
            Slide tmpSlide = new Slide();
            tmpSlide.Id = id;
            tmpSlide.SurveyDepth = survey_depth;
            tmpSlide.Inc = inc;
            tmpSlide.Azo = azi;
            var sensoroffset = giangSonContext.Sensors.FirstOrDefault().Offset;
            if (sensoroffset == null) { sensoroffset = 0; }
            tmpSlide.BitDepth = Math.Floor(((double)sensoroffset + survey_depth)*100)/100;
            giangSonContext.Add(tmpSlide);
            giangSonContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddGreenSlide(string ToolFace, double St, double Ed)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            List<GreenSlide> Greenlist = new List<GreenSlide>();
            var list = giangSonContext.Slides.ToList();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.St == 0 || item.Ed == 0)
                    { continue; }
                    GreenSlide tmpgreenSlide = new GreenSlide();
                    tmpgreenSlide.ToolFace = item.ToolFace;
                    tmpgreenSlide.From = item.St;
                    tmpgreenSlide.To = item.Ed;
                    Greenlist.Add(tmpgreenSlide);
                }
                GreenSlide newGreenSlide = new GreenSlide();
                newGreenSlide.ToolFace = ToolFace;
                newGreenSlide.From = St;
                newGreenSlide.To = Ed;
                Greenlist.Add(newGreenSlide);
                Greenlist.Sort((x, y) => x.From.CompareTo(y.From));
                for (int i = 0; i < Greenlist.Count(); i++)
                {
                    list[i].ToolFace = Greenlist[i].ToolFace;
                    list[i].St = Greenlist[i].From;
                    list[i].Ed = Greenlist[i].To;
                }
                giangSonContext.SaveChanges();
            }
            return RedirectToAction("Index");
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

        static double FindClosest(List<double> doubleList, double a)
        {
            foreach (double d in doubleList)
            {
                if (d > a) return d;
            }
            return 0;
        }

        public IActionResult ClearData()
        {
            GiangSonContext context = new GiangSonContext();
            var list = context.Slides.ToList();
            foreach (var slide in list)
            {
                context.Slides.Remove(slide);
            }
            context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            GiangSonContext ctx = new GiangSonContext();
            for (int i = id; i <= ctx.Slides.Count() - 1; i++)
            {
                Slide delSlide = ctx.Slides.Find(id);
                Slide delSlide1 = ctx.Slides.Find(id + 1);
                delSlide = delSlide1;
                ctx.SaveChanges();
            }
            Slide slide = ctx.Slides.ToList().Last();
            ctx.Remove(slide);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetSensorOffset(double offset)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            var sensor = giangSonContext.Sensors.FirstOrDefault();
            sensor.Offset = offset;
            giangSonContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            var Slide = giangSonContext.Slides.Find(id);
            return View(Slide);
        }

        [HttpPost]
        public IActionResult SaveEdit(int id, Slide newSlide)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            var tmpDSlide = giangSonContext.Slides.Find(id);
            tmpDSlide.SurveyDepth = newSlide.SurveyDepth;
            tmpDSlide.Inc = newSlide.Inc;
            tmpDSlide.Azo = newSlide.Azo;
            tmpDSlide.ToolFace = newSlide.ToolFace;
            tmpDSlide.St = newSlide.St;
            tmpDSlide.Ed = newSlide.Ed;
            tmpDSlide.Total = newSlide.Ed - newSlide.St;
            tmpDSlide.DpLength = newSlide.BitDepth + (double)giangSonContext.Sensors.FirstOrDefault().Offset;
            giangSonContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult SetDefineBur(double bur30m)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            giangSonContext.Sensors.FirstOrDefault().Bur30m = bur30m;
            giangSonContext.SaveChanges();
            return RedirectToAction("Index");
        }

        static List<Slide> Calculator(List<Slide> slides)
        {
            GiangSonContext giangSonContext = new GiangSonContext();
            if (slides.Count == 0) return new List<Slide>();
            foreach (var slide in slides)
            {
                var item = giangSonContext.Slides.Find(slide.Id);

                //bit depth
                slide.BitDepth = slide.SurveyDepth + (double)giangSonContext.Sensors.FirstOrDefault().Offset;
                item.BitDepth = slide.BitDepth;
                giangSonContext.SaveChanges();

                // dp depth
                if (slide.Id == 1) slide.DpLength = 0;
                else slide.DpLength = Math.Floor((slide.BitDepth - giangSonContext.Slides.Find(slide.Id - 1).BitDepth)*100)/100;
                item.DpLength = slide.DpLength;
                giangSonContext.SaveChanges();

                //total did slide
                slide.Total = Math.Floor((slide.Ed - slide.St)*100)/100;
                item.Total = slide.Total;
                giangSonContext.SaveChanges();

                //tmp1,tmp2
                var SurveyList = giangSonContext.Slides.Select(s => s.SurveyDepth).ToList();
                double b = 0.00;
                b = FindClosest(SurveyList, slide.St);
                if (b == 0)
                {

                    slide.Tmp1 = 0;
                    slide.Tmp2 = 0;

                }
                else
                {
                    if (slide.Ed < b)
                    {
                        slide.Tmp1 = slide.Total;
                        slide.Tmp2 = 0;
                    }
                    else
                    {
                        slide.Tmp1 = Math.Floor((b - slide.St)*100)/100;
                        slide.Tmp2 = Math.Floor((slide.Total - slide.Tmp1)*100)/100;
                    }

                }
                if (slide.Ed == 0)
                {
                    slide.Tmp1 = 0;
                    slide.Tmp2 = 0;
                }
                item.Tmp1 = slide.Tmp1;
                item.Tmp2 = slide.Tmp2;
                giangSonContext.SaveChanges();

                // metterseen
                if (slide.Id == 3)
                {
                    slide.MetterSeen = Math.Floor((giangSonContext.Slides.Find(slide.Id - 2).Tmp1)*100)/100;
                    item.MetterSeen = slide.MetterSeen;
                    giangSonContext.SaveChanges();
                }
                if (slide.Id > 3)
                {
                    slide.MetterSeen = Math.Floor((giangSonContext.Slides.Find(slide.Id - 3).Tmp2 + giangSonContext.Slides.Find(slide.Id - 2).Tmp1)*100)/100;
                    item.MetterSeen = slide.MetterSeen;
                    giangSonContext.SaveChanges();
                }


                //burm bur30m

                if (slide.Id > 1)
                {
                    if (slide.MetterSeen == 0)
                    {
                        slide.Bur30m = (double)giangSonContext.Sensors.FirstOrDefault().Bur30m;
                        slide.Burm = Math.Floor((slide.Bur30m / 30)*100)/100;
                        item.Burm = slide.Burm;
                        item.Bur30m = slide.Burm;
                        giangSonContext.SaveChanges();
                    }
                    else
                    {
                        slide.Burm = Math.Floor(((giangSonContext.Slides.Find(slide.Id).Inc - giangSonContext.Slides.Find(slide.Id - 1).Inc) / giangSonContext.Slides.Find(slide.Id).MetterSeen)*100)/100;
                        slide.Bur30m = Math.Floor((slide.Burm * 30)*100)/100;
                        item.Burm = slide.Burm;
                        item.Bur30m = slide.Bur30m;
                        giangSonContext.SaveChanges();
                    }
                }

                //inc@bit

                if (slide.Id == 2 || slide.Id==3)
                {
                    if (slide.MetterSeen == 0)
                    {
                        if (slide.Incbit == 0)
                            slide.Incbit = (Math.Floor(giangSonContext.Slides.Find(slide.Id - 1).Total * slide.Burm + slide.Inc)*100)/100;
                        item.Incbit = slide.Incbit;
                        giangSonContext.SaveChanges();
                    }
                    else
                    {
                        if (slide.Incbit == 0)
                            slide.Incbit = Math.Floor(((giangSonContext.Slides.Find(slide.Id - 1).Total + giangSonContext.Slides.Find(slide.Id - 2).Tmp2) * slide.Burm + slide.Inc)*100)/100;
                        item.Incbit = slide.Incbit;
                        giangSonContext.SaveChanges();
                    }
                }
                if (slide.Id > 3)
                {
                    if (slide.Incbit == 0)
                        slide.Incbit = Math.Floor(((giangSonContext.Slides.Find(slide.Id - 1).Total + giangSonContext.Slides.Find(slide.Id - 2).Tmp2) * (slide.Burm+giangSonContext.Slides.Find(slide.Id-1).Burm)/2 + slide.Inc)*100)/100;

                    item.Incbit = slide.Incbit;
                    giangSonContext.SaveChanges();
                }
            }

            return slides;
        }
    }
}
