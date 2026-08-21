using System;

namespace dotnet_ecommerce_api.Helpers;

public class QueryParameters
{
    private const int MaxPageSize = 100;
    public int PageNumber {get; set;} = 1;
    public int PageSize {get; set;} = 10;
    public string? Search {get; set;} 
    public string? SortOrder {get; set;}

    public QueryParameters Validate()
    {
        if(PageNumber < 1)
        {
            PageNumber = 1;
        }
        if (PageSize <1)
        {
            PageSize = 10;
        }
        if (PageSize > MaxPageSize)
        {
            PageSize = MaxPageSize;
        }
        return this;

    }
}
