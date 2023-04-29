using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Client.Mvc.Models;
using Clients.Models;
using Newtonsoft.Json;
using Clients.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;

namespace Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        this._httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize]
    public async Task<IActionResult> Weather([FromServices] ITokenService tokenService)
    {

        var data = new List<WeatherData>();
        var tokenResponse = await tokenService.GetToken("weatherapi.read");
        _httpClient.SetBearerToken(tokenResponse.AccessToken!);
        //  data= await _httpClient.GetFromJsonAsync<List<WeatherData>>(@"http://localhost:5153/WeatherForecast");
        using var response = await _httpClient.GetAsync(@"http://localhost:5153/WeatherForecast");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        data = JsonConvert.DeserializeObject<List<WeatherData>>(content);
        return View(data);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
