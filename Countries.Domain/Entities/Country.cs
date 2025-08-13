using System;
using System.Collections.Generic;

namespace Countries.Infrastructure.Models;

public class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool IsDeleted { get; set; } = false;
}
