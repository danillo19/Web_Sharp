using System.Data;
using WebApplication2.models;
using WebApplication2.repository;

namespace WebApplication2.services;

public class FriendService
{
    public bool Compare(String first, String second, int attemptId)
    {
        using DatabaseContext db = new DatabaseContext();
        Attempt attempt =  db.Attempts.Find(attemptId) ?? throw new DataException("No such attempt");
        
        int indexFirst = Array.FindIndex(attempt.CandidatesSequenceNames, n => n == first);
        int indexSecond = Array.FindIndex(attempt.CandidatesSequenceNames, n => n == second);

        if (indexFirst >= attempt.Next || indexSecond >= attempt.Next)
        {
            throw new DataException("Princess hasn't met such contender yet.");
        }
        return attempt.CandidatesSequenceValues[indexFirst] > attempt.CandidatesSequenceValues[indexSecond];
    }
}