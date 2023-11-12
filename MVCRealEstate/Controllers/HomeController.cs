using System.Data;
using System.Diagnostics;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVCRealEstate.Models;
using MVCRealEstateData;

namespace MVCRealEstate.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public IActionResult Index()
    {

        return View();
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

    [HttpGet]
    public async Task<IActionResult> GetProvinces()
    {

        var model = await context
            .Provinces
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();
        return Json(model);
    }
    [HttpGet]
    public async Task<IActionResult> GetDistricts(int id)
    {


        var model = await context
            .Districts
            .Where(p => p.ProvinceId == id)
            .Select(p => new { p.Id, p.Name })
            .ToListAsync();
        return Json(model);
    }


}

