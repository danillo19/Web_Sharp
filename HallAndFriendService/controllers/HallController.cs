using System.Diagnostics;
using System.Web.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.models;
using WebApplication2.services;
using JsonResult = System.Web.Mvc.JsonResult;

namespace WebApplication2.controllers;

[ApiController]
[Route("[controller]")]
public class HallController : ControllerBase
{
    private readonly ILogger<HallController> _logger;
    private readonly HallService _service;

    public HallController(ILogger<HallController> logger, HallService hallService)
    {
        _logger = logger;
        _service = hallService;
    }

    [HttpPost("{attemptId:int}/next")]
    public Contender? GetNextContender(int attemptId)
    {
        var contender = _service.GetNextContender(attemptId);
        
        _logger.LogInformation("Get next contender: " + contender?.Name);
        if (contender == null)
        {
            Response.StatusCode = 400;
        }
        return contender;
    }
    
    [HttpPost("{attemptId:int}/reset")]
    public IActionResult ResetHall(int attemptId)
    {   
        _logger.LogInformation("Resetting hall...");
        _service.ResetHall(attemptId);
        return Content("0");
    }
}