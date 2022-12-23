namespace WebApplication2.dto;

public class CandidateDto
{
    public String Name { get; set; }

    public CandidateDto(String name)
    {
        Name = name;
    }
}