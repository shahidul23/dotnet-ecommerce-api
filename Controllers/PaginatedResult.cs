using System;
using System.Text.RegularExpressions;

namespace dotnet_ecommerce_api.Controllers;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items {get; set;} = new List<T>();
    public int TotalCount {get; set;}
    public int PageNumber {get; set;}
    public int PageSize {get;set;}
    public int TotalPages => (int) Math.Ceiling((double) TotalCount/PageSize);
}
