using System;
using System.Collections.Generic;
using System.Text;
using Pgvector;

namespace Application.Abstractions.Extensions;

public static class VectorExtensions
{
    public static Vector ToVector(this float[] array) => new(array);
}
