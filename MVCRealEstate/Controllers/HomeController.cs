using System.Data;
using System.Diagnostics;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Index()
    {
        await PopulateDropdowns();
        ViewBag.Latest = await context.Posts.OrderByDescending(p => p.Date).Select(p => new GetPostViewModel {
            Id = p.Id,Name = p.Name,Image = p.Image, Price = p.Price
        }).Take(20).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Post(Guid id)
    {
        var post = await context.Posts.FindAsync(id);
        return View(post);
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
    [HttpGet]

    [HttpGet]
    public async Task<IActionResult> Search(SearchViewModel model)
    {
        await PopulateDropdowns();
        var result = await context
            .Posts
            .Where(p =>
            (p.DistrictId == model.DistrictId || model.DistrictId == null) &&
            (p.Price >= model.MinPrice || model.MinPrice == null) &&
            (p.Price <= model.MaxPrice || model.MaxPrice == null) &&
            (p.CategoryId == model.CategoryId || model.CategoryId == null) &&
            (p.Type == model.PostType || model.PostType == null)
            )
            .ToListAsync();
        ViewBag.SearchModel = model;
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> PostsByPage(int page)
    {
        var results = await context
            .Posts
            .OrderByDescending(p => p.Date)
            .Skip((page - 1) * 20)
            .Take(20)
            .Select(p => new { p.Id, p.Name, p.Descriptions, p.Price, p.Image })
            .ToListAsync();

        return Json(results);
    }

    private async Task PopulateDropdowns()
    {
        ViewBag.Categories = new SelectList(await context.Categories.ToListAsync(), "Id", "Name");
        ViewBag.Districts = new SelectList(await context.Districts.Select(p => new { p.Id, p.Name, ProvinceName = p.Province.Name }).ToListAsync(), "Id", "Name", null, "ProvinceName");
    }
}

