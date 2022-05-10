using System;

public static class Utils
{
    public static T[,,] Populate<T>(this T[,,] array, Func<T> provider)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                for (int k = 0; k < array.GetLength(2); k++)
                {
                    array[i, j, k] = provider();
                }
            }
        }
        return array;
    }
}
