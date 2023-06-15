using ControlSequence;
using NUnit.Framework;

namespace Test.ControlSequence;

internal class Test
{
    Worker worker = new Worker();

    [OneTimeSetUp]
    public void Setup()
    {
        Console.SetOut(TestContext.Progress);
    }

    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 31, 45, 31, 32, 55, 55, 32, 33, 32, 33, 31, 100, 31, 32, 33, 31, 32, 100 }, ExpectedResult = new int[] { 12, 13, 14, 15, 17 }, TestName = "Clear not done, multiple time the race inside")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 31, 32, 33, 31, 100 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4 }, TestName = "Multiple time first control, correct sequence")]
    [TestCase(new int[] { 31, 32, 33, 34, 100 }, new int[] { 31, 32, 33, 34, 100 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4 }, TestName = "Correct sequence")]
    [TestCase(new int[] { 31, 32, 33, 34, 100 }, new int[] { 31, 33, 32, 34, 100 }, ExpectedResult = new int[] { 0, 2, -1, 3, 4 }, TestName = "All controls in wrong order")]
    [TestCase(new int[] { 31, 32, 33, 34, 100 }, new int[] { 31, 33, 34, 32, 100 }, ExpectedResult = new int[] { 0, -1, 1, 2, 4 }, TestName = "All controls in wrong order (other best match)")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { }, ExpectedResult = new int[] { -1, -1, -1, -1, -1 }, TestName = "No controls punched")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 45, 54 }, ExpectedResult = new int[] { -1, -1, -1, -1, -1 }, TestName = "Neither one right control")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 32, 31, 100 }, ExpectedResult = new int[] { -1, 0, -1, 1, 2 }, TestName = "Missing first control and missing one in the middle")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 32, 31, 100, 31, 32, 33, 31, 100 }, ExpectedResult = new int[] { 3, 4, 5, 6, 7 }, TestName = "Clear not done with similar controls")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 32, 31, 100, 31, 32, 31, 100 }, ExpectedResult = new int[] { 3, 4, -1, 5, 6 }, TestName = "Clear not done with similar controls and final missing punch")]
    [TestCase(new int[] { 31, 32, 33, 31, 100 }, new int[] { 31, 32, 33, 31, 100, 31, 45 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4 }, TestName = "Extra controls after the last one")]
    [TestCase(new int[] { 31, 31 }, new int[] { 31, 31 }, ExpectedResult = new int[] { 0, 1 }, TestName = "Course with only duplicated controls ")]
    [TestCase(new int[] { 31, 45, 31 }, new int[] { 31, 31 }, ExpectedResult = new int[] { 0, -1, 1 }, TestName = "Punched sequence with duplicated controls")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 32, 33 }, ExpectedResult = new int[] { -1, 0, 1 }, TestName = "Missing first control")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 99, 32, 33 }, ExpectedResult = new int[] { -1, 1, 2 }, TestName = "Wrong first control")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 33 }, ExpectedResult = new int[] { 0, -1, 1 }, TestName = "Missing control in the middle")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 99, 33 }, ExpectedResult = new int[] { 0, -1, 2 }, TestName = "Wrong control in the middle")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 32 }, ExpectedResult = new int[] { 0, 1, -1 }, TestName = "Missing last control")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 32, 99 }, ExpectedResult = new int[] { 0, 1, -1 }, TestName = "Wrong last control")]
    [TestCase(new int[] { 31, 32, 45, 67, 89, 100 }, new int[] { 31, 100 }, ExpectedResult = new int[] { 0, -1, -1, -1, -1, 1 }, TestName = "Punched only first and last controls")]
    [TestCase(new int[] { 31, 32, 45, 67, 89, 100 }, new int[] { 67 }, ExpectedResult = new int[] { -1, -1, -1, 0, -1, -1 }, TestName = "Punched only one control in the middle")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 32, 33, 34, 35, 200 }, ExpectedResult = new int[] { 0, -1, 1, 2, -1, 3, 4, -1, 5 }, TestName = "Butterfly missing all central controls")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 45, 100, 32, 33, 34, 35, 100, 200 }, ExpectedResult = new int[] { 0, 2, 3, 4, -1, 5, 6, 7, 8 }, TestName = "Butterfly missing one central control and extra controls")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 45, 32, 33 }, ExpectedResult = new int[] { 0, 2, 3 }, TestName = "Extra control")]
    [TestCase(new int[] { 31, 32, 33 }, new int[] { 31, 45, 67, 89, 32, 100, 33, 99 }, ExpectedResult = new int[] { 0, 4, 6 }, TestName = "Extra controls")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, TestName = "Butterfly")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 100, 32, 33, 100, 34, 99, 98, 35, 100, 200 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4, 5, 8, 9, 10 }, TestName = "Butterfly with extra controls")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 100, 32, 33, 100, 35, 100, 200 }, ExpectedResult = new int[] { 0, 1, 2, 3, 4, -1, 5, 6, 7 }, TestName = "Butterfly missing one normal control")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 100, 32, 33, 34, 35, 100, 200 }, ExpectedResult = new int[] { 0, 1, 2, 3, -1, 4, 5, 6, 7 }, TestName = "Butterfly missing one central control")]
    [TestCase(new int[] { 31, 100, 32, 33, 100, 34, 35, 100, 200 }, new int[] { 31, 44, 32, 33, 100, 34, 35, 100, 200 }, ExpectedResult = new int[] { 0, -1, 2, 3, 4, 5, 6, 7, 8 }, TestName = "Butterfly missing the first central control")]
    public int[] Test_cases(int[] course, int[] punched)
    {
        var res = worker.Process(course, punched);

        TestContext.WriteLine(string.Join(',', res));

        if (res.All(n => n >= 0))
            Assert.IsTrue(worker.CheckCorrectness(course, punched));

        return res;
    }
    [TestCase(10)]
    [TestCase(20)]
    [TestCase(30)]
    [TestCase(50)]
    [TestCase(128)]
    [TestCase(256)]
    public void Test_many(int n)
    {
        var rand = new Random(42);
        var course = Enumerable.Range(0, n).Select(_ => rand.Next(31, 255)).ToArray();
        var punched = Enumerable.Range(0, n).Select(_ => rand.Next(31, 255)).ToArray();
        var res = worker.Process(course, punched);

        TestContext.WriteLine(string.Join(',', res));

        Assert.Pass();
    }
    [Test]
    public void Correct_courses_with_random_extra_controls()
    {
        for (int seed = 0; seed < 10000; seed++)
        {
            var rand = new Random(seed);
            var course = Enumerable.Range(0, rand.Next(10, 35)).Select(_ => rand.Next(31, 101)).ToArray();
            var punched = course.ToList();

            for (int i = 0; i < rand.Next(1, 10); i++)
            {
                punched.Insert(rand.Next(0, punched.Count - 1), rand.Next(31, 101));
            }

            Assert.IsTrue(worker.CheckCorrectness(course, punched));

            Assert.IsTrue(worker.Process(course, punched).All(n => n >= 0));
        }
    }


    [TestCase(new int[] { -1, -1, -1, -1, -1 }, ExpectedResult = PunchStatus.DNF)]
    [TestCase(new int[] { -1, -1, -1 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { -1 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { -1, 0, 1 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { -1, 3, 4 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { -1, 1, 2 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { -1, 1, -1, -1, -1, -1, 6, 7 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { -1, 1, -1, -1, -1, -1, 7, 8 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { 0, -1, 3, 4 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { 0, -1, 2, 3, 4 }, ExpectedResult = PunchStatus.WP)]
    [TestCase(new int[] { 0, 1, 2, 3, 4 }, ExpectedResult = PunchStatus.OK)]
    [TestCase(new int[] { 4, 5, 6, 10, 11 }, ExpectedResult = PunchStatus.OK)]
    [TestCase(new int[] { 0, -1, 1, 2, 3 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { 0, -1, 2, 3, 4, -1, 5 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { 0, -1, 2, 3, 4, -1 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { 0, 1, 2, 3, -1, -1, 4 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { 0, 1, 2, -1, -1, -1, -1, 3, -1, -1, 4 }, ExpectedResult = PunchStatus.MP)]
    [TestCase(new int[] { 0, 1, 2, 3, -1, -1, -1, 4 }, ExpectedResult = PunchStatus.DNF)]
    public PunchStatus TestStatus(int[] seq)
    {
        return worker.GetPunchStatus(seq);
    }

}

