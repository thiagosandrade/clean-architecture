using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Todos.Breakdown;

public enum BreakdownComplexity
{
    Simple = 0,
    Standard = 1,
    Detailed = 2
}


public enum BreakdownStrategy
{
    Sequential = 0,
    Category = 1,
    Deliverables = 2,
    Checklist = 3
}
