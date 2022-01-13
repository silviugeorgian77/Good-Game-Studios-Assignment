using NUnit.Framework;
using System.Text;
using System.Linq;
using System;
using UnityEngine;

public class RandomUtilsTest
{
    [Test]
    public void GenerateRandomNumbersThatAddUpToSum_TestSum100Count3LB1UB100()
    {
        GenerateRandomNumbersThatAddUpToSum_GenericTest(100, 3, 1, 100);
    }

    [Test]
    public void GenerateRandomNumbersThatAddUpToSum_TestSum50Count10LB1UB50()
    {
        GenerateRandomNumbersThatAddUpToSum_GenericTest(50, 5, 1, 50);
    }

    [Test]
    public void GenerateRandomNumbersThatAddUpToSum_TestNegativeSumThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            GenerateRandomNumbersThatAddUpToSum_GenericTest(-100, 3, 1, 100);
        });
    }

    [Test]
    public void GenerateRandomNumbersThatAddUpToSum_TestNegativeCountThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            GenerateRandomNumbersThatAddUpToSum_GenericTest(100, -3, 1, 100);
        });
    }

    private void GenerateRandomNumbersThatAddUpToSum_GenericTest(
        int sum,
        int count,
        int lowerBound,
        int upperBound)
    {
        int testCount = 10;
        int[] numbers;
        for (int i = 0; i < testCount; i++)
        {
            numbers = RandomUtils.GenerateRandomNumbersThatAddUpToSum(
                sum,
                count,
                lowerBound,
                upperBound
            );
            LogNumbers(numbers);
            Assert.AreEqual(sum, numbers.Sum());
        }
    }

    private void LogNumbers(int[] numbers)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < numbers.Length; i++)
        {
            if (i != 0)
            {
                stringBuilder.Append(", ");
            }
            stringBuilder.Append(numbers[i]);
        }
        string numbersString = stringBuilder.ToString();
        Debug.Log(numbersString);
    }
}
