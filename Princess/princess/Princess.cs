using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication2.dto;

namespace Lab2;

public class Princess : IHostedService
{
    private readonly HttpClient _httpClient;
    private IServiceScopeFactory _serviceScopeFactory;

    private Simulator? _simulator;
    private TaskCompletionSource<bool> TaskCompletionSource { get; set; } = new TaskCompletionSource<bool>();
    private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    private ILogger<Princess>? Logger { get; }
    
    private Contender? _bestContender = null;
    private Contender? _secondContender = null;

    private IHostApplicationLifetime? ApplicationLifetime { get; }

    public Princess(ILogger<Princess> logger, IHostApplicationLifetime appLifetime,
        IServiceScopeFactory serviceScopeFactory, Simulator simulator)
    {
        Logger = logger;
        ApplicationLifetime = appLifetime;
        _simulator = simulator;
        _serviceScopeFactory = serviceScopeFactory;
        _httpClient = new HttpClient();
    }

    private bool IsBetterThenBest(ref Contender candidate)
    {
        if (_bestContender == null) throw new Exception("Invalid best candidate, check algorithm");
        return candidate.Value > _bestContender.Value.Value;
    }

    private bool IsBetterThenSecond(ref Contender candidate)
    {
        if (_secondContender == null) throw new Exception("Invalid second candidate, check algorithm");
        return candidate.Value > _secondContender.Value.Value;
    }
    
    private Contender? GetNextContender(int attemptId)
    {
        var candidateNameResponse = _httpClient.PostAsync("http://localhost:5041/hall/" + attemptId + "/next", new StringContent("", Encoding.UTF8, "application/json")).Result;
        candidateNameResponse.EnsureSuccessStatusCode();
        Contender? candidate = candidateNameResponse.Content.ReadFromJsonAsync<Contender?>().Result;
        return candidate;
    }


    private bool Compare(Contender first, Contender second, int attemptId)
    {
        var dto = new CompareCandidatesDto { First = first.Name, Second = second.Name };
        JsonContent content = JsonContent.Create(dto);
        var friendResponse = _httpClient.PostAsync("http://localhost:5041/friend/" + attemptId + "/compare", content);
        CandidateDto? result = friendResponse.Result.Content.ReadFromJsonAsync<CandidateDto>().Result;
        Logger.LogInformation("Friend Response: " + result?.Name);

        return result?.Name == first.Name;
    }

    private void Reset(int attemptId)
    {
        var friendResponse = _httpClient.PostAsync("http://localhost:5041/hall/" + attemptId + "/reset", null);
        friendResponse.Result.EnsureSuccessStatusCode();
        Logger.LogInformation("Reset: " + friendResponse.Result.Content.ReadAsStringAsync().Result);

    }

    private Contender? MakeChoice(int attemptId)
    {
        _bestContender = GetNextContender(attemptId);
        _secondContender = GetNextContender(attemptId);
        
        Debug.Assert(_bestContender != null, nameof(_bestContender) + " != null");
        Debug.Assert(_secondContender != null, nameof(_secondContender) + " != null");

        for (int i = 2; i < 37; i++)
        {
            Contender? candidate = GetNextContender(attemptId);
            Debug.Assert(candidate != null, nameof(candidate) + " != null");

            if (Compare(candidate.Value, _bestContender.Value, attemptId))
            {
                _secondContender = _bestContender;
                _bestContender = candidate;
            }
            else if (Compare(candidate.Value, _secondContender.Value, attemptId))
            {
                _secondContender = candidate;
            }
        }

        for (int i = 37; i < 68; i++)
        {
            Contender? candidate = GetNextContender(attemptId);
            Debug.Assert(candidate != null, nameof(candidate) + " != null");

            if (Compare(candidate.Value, _bestContender.Value, attemptId))
            {
                TaskCompletionSource.SetResult(true);
                return candidate;
            }

            if (Compare(candidate.Value, _secondContender.Value, attemptId))
            {
                _secondContender = candidate;
            }

        }

        for (int i = 68; i < 100; i++)
        {
            Contender? candidate = GetNextContender(attemptId);
            Debug.Assert(candidate != null, nameof(candidate) + " != null");
            
            if (Compare(candidate.Value, _bestContender.Value, attemptId))
            {
                TaskCompletionSource.SetResult(true);
                return candidate;
            }

            if (Compare(candidate.Value, _secondContender.Value, attemptId))
            {
                TaskCompletionSource.SetResult(true);
                return candidate;
            }
        }


        TaskCompletionSource.SetResult(true);
        return null;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        GetPrincessChoice();
        return Task.CompletedTask;
    }

    private Contender? GetPrincessChoice()
    {
        int attemptId = 5;
        Reset(attemptId);
        Task<Contender?> task = Task.Run(() => MakeChoice(attemptId));
        if (task.Result != null)
        {
            Contender candidate = task.Result.Value;
            Logger?.LogInformation("Name: " + candidate.Name + "; Happiness: " + candidate.Value);
        }
        else
        {
            Logger?.LogInformation("Happiness: 10");
        }
        
        Reset(attemptId);
        return task.Result;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}