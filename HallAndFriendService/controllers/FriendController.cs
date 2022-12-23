using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.dto;
using WebApplication2.models;
using WebApplication2.services;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace WebApplication2.controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class FriendController : ControllerBase
{
    private readonly FriendService _friendService;
    private readonly ILogger<FriendController> _logger;

    public FriendController(ILogger<FriendController> logger, FriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }
    [Microsoft.AspNetCore.Mvc.HttpPost("{attemptId:int}/compare")]
    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    public CandidateDto GetBetterContender([FromBody] CompareCandidatesDto compareDto,  int attemptId)
    {   
        _logger.LogInformation("Comparing " + compareDto.First + " vs "  + compareDto.Second);
        try
        {
            return _friendService.Compare(compareDto.First, compareDto.Second, attemptId)
                ? new CandidateDto(compareDto.First)
                : new CandidateDto(compareDto.Second);

        }
        catch (DataException e)
        {
            Response.StatusCode = 400;
            return new CandidateDto("");
        }
    }
}