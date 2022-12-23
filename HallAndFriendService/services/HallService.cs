using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.dto;
using WebApplication2.models;
using WebApplication2.repository;

namespace WebApplication2.services;

public class HallService
{
    public Contender? GetNextContender(int attemptId)
    {
        using DatabaseContext db = new DatabaseContext();
        Attempt attempt = db.Attempts.Find(attemptId) ?? throw new DataException("No such attempt into database");
        int next = attempt.Next;

        if (next >= 100) return null;
        
        attempt.Next++;
        db.Entry(attempt).State = EntityState.Modified;
        db.SaveChanges();

        return new Contender
            { Name = attempt.CandidatesSequenceNames[next], Value = attempt.CandidatesSequenceValues[next] };

    }

    public void ResetHall(int attemptId)
    {
        using DatabaseContext db = new DatabaseContext();
        Attempt attempt = db.Attempts.Find(attemptId) ?? throw new DataException("No such attempt into database");
        attempt.Next = 0;
        db.Entry(attempt).State = EntityState.Modified;
        db.SaveChanges();
    }
}