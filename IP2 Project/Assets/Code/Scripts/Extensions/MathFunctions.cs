using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions
{
    // x = (-b +/- sqrt(b^2 - 4ac) / 2a)
    /// <returns>0 for no valid answer. 1 for discrimenent greater than 0. 2 for discrimenent equal to 0.</returns>
    public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
    {
        var discriminant = (b * b) - (4 * a * c);

        if (discriminant < 0)
        {
            // No valid answer.
            root1 = Mathf.Infinity;
            root2 = -root1;
            return 0;
        }

        float sqrt = Mathf.Sqrt(discriminant);
        root1 = (-b + sqrt) / (2 * a);
        root2 = (-b - sqrt) / (2 * a);

        return discriminant > 0 ? 2 : 1;
    }
}
