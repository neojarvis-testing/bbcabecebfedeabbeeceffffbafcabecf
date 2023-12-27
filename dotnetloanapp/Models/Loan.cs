using System;
using System.Collections.Generic;

namespace BookStoreDBFirst.Models;
public class Loan
{
    public int LoanID { get; set; }

    public string? LoanType { get; set; }

    public string? Description { get; set; }
//https://github.com/neo-stark-team/fullstack_project_react-dotnet_backend.git
    public decimal? InterestRate { get; set; }

    public decimal? MaximumAmount  { get; set; }

}