using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPNET_mvc_core.Models;
using System.Threading;
using System.Timers;
namespace ASPNET_mvc_core.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("index")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Route("maxim")]
        public IActionResult Maxim([FromBody] string content)
        {
            ViewData["Message"] = $"Сим-сим-сим, идёт по улице Максим. На кого пальцем покажет - тому... ";
            return View();
        }

        [Route("nextDestiny")]
        public IActionResult NextDestiny()
        {
            Thread.Sleep(10000);
            return new JsonResult(MaximModel.Destiny);
        }

        [Route("addDestiny/{destiny}")]
        public IActionResult AddDestiny(string destiny)
        {
            MaximModel.Destiny = destiny;
            PlayerModel.Announcement.Add((DateTime.Now.TimeOfDay + TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(Convert.ToDouble(destiny))));
            return null;
        }
        [Route("nextTrack")]
        public IActionResult NextTrack()
        {
            Thread.Sleep(PlayerModel.getDelayMS());
            var result = PlayerModel.NextSource;
            return new JsonResult(new
            {
                src = result.Item1,
                volume = result.Item3,
                time = result.Item4.TotalSeconds
            });
        }

        [Route("startOver")]
        public IActionResult StartOver()
        {
            var result = PlayerModel.GetCurrent();
            return new JsonResult(new
            {
                src = result.Item1,
                volume = result.Item3,
                time = (int)result.Item4.TotalSeconds
            });
        }
    }
}
