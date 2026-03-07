namespace FourPrime.Application.DTOs;

public class Paging
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Propriedade calculada para skip
    public int Skip => (Page - 1) * PageSize;
}