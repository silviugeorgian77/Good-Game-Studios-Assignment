using UnityEngine;
using System;

public class RandomUtils : MonoBehaviour
{
    /// <summary>
    /// Generates random numbers that add up to a given sum.
    /// </summary>
    /// <param name="count">Represents the size of the returned array</param>
    /// <param name="sum">Represents the sum that the numbers in the returned
    /// array add up to</param>
    /// <param name="lowerBound">The numbers in the returned array won't be
    /// less than this value</param>
    /// <param name="upperBound">The numbers in the returned array won't be
    /// greater than this value</param>
    /// <returns>Returns an array with random numbers that add up to the given
    /// sum</returns>
    public static int[] GenerateRandomNumbersThatAddUpToSum(
        int sum,
        int count,
        int lowerBound,
        int upperBound)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than 0.");
        }
        if (sum <= 0)
        {
            throw new ArgumentException("Sum must be greater than 0.");
        }

        int[] result = new int[count];

        int currentSum = 0;
        int currentNumber;
        for (int i = 0; i < count; i++)
        {
            currentNumber = UnityEngine.Random.Range(lowerBound, upperBound);
            result[i] = currentNumber;
            currentSum += currentNumber;
        }

        float arrayScale = sum / currentSum;

        currentSum = 0;
        for (int i = 0; i < count; i++)
        {
            currentNumber = (int)(result[i] * arrayScale);
            currentNumber = (int)MathUtils.ClampValue(
                currentNumber,
                lowerBound,
                upperBound
            );
            result[i] = currentNumber;
            currentSum += currentNumber;
        }

        int extraSum = sum - currentSum;
        int randomIndex;
        while (extraSum > 0)
        {
            randomIndex = UnityEngine.Random.Range(0, result.Length);
            if (result[randomIndex] < upperBound)
            {
                result[randomIndex]++;
                extraSum--;
            }
        }

        return result;
    }
}
