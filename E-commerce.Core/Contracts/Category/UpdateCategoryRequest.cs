using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Core.Contracts.Category;

public record UpdateCategoryRequest(
    int Id,
    string Name,
    string Description
);

