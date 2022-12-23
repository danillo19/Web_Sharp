namespace WebApplication2.models;

public class Attempt
{   
    public int Id { get; set; }
    public int PrincessHappiness { get; set; }
    public String? ChoiceName { get; set; }
    public int[]? CandidatesSequenceValues { get; set; }
    public String[]? CandidatesSequenceNames { get; set; }
    public int Next { get; set; }


}

public class Contender
{
    public String Name { get; set;}
    public int Value { get; set; }

}