using CriusNyx.Util;
using Sew;

namespace tests;

public class IOTestCase(string sewProgram, string input, string output) : DebugPrint
{
  public string SewProgram => sewProgram;
  public string Input => input;
  public string Output => output;

  public IEnumerable<(string, object)> EnumerateFields()
  {
    return
    [
      nameof(SewProgram).With(SewProgram),
      nameof(Input).With(Input),
      nameof(Output).With(Output),
    ];
  }
}

public class IOTests
{
  public static object[] IOTestCases =
  [
    new object[]
    {
      "eliminateEmptyLines lines",
      new IOTestCase("lines(some)", "\n\nHello world\n\n", "Hello world"),
    },
    new object[]
    {
      "eliminateEmptyLines loose expression",
      new IOTestCase("lines | some | join(',')", "\n\nHello world\nahh\n", "Hello world,ahh"),
    },
    new object[]
    {
      "CSV to HTML table",
      new IOTestCase(
        "lines(split(',') | tag('li') | join | tag('tr', indent:2, :nl))",
        @"1,88F7B33d2bcf9f5,Shelby,Terrell,Male,elijah57@example.net,001-084-906-7849x73518,1945-10-26,Games developer
2,f90cD3E76f1A9b9,Phillip,Summers,Female,bethany14@example.com,214.112.6044x4913,1910-03-24,Phytotherapist
3,DbeAb8CcdfeFC2c,Kristine,Travis,Male,bthompson@example.com,277.609.7938,1992-07-02,Homeopath
4,A31Bee3c201ef58,Yesenia,Martinez,Male,kaitlinkaiser@example.com,584.094.6111,2017-08-03,Market researcher
5,1bA7A3dc874da3c,Lori,Todd,Male,buchananmanuel@example.net,689-207-3558x7233,1938-12-01,Veterinary surgeon
6,bfDD7CDEF5D865B,Erin,Day,Male,tconner@example.org,001-171-649-9856x5553,2015-10-28,Waste management officer
7,bE9EEf34cB72AF7,Katherine,Buck,Female,conniecowan@example.com,+1-773-151-6685x49162,1989-01-22,Intelligence analyst
8,2EFC6A4e77FaEaC,Ricardo,Hinton,Male,wyattbishop@example.com,001-447-699-7998x88612,1924-03-26,Hydrogeologist
9,baDcC4DeefD8dEB,Dave,Farrell,Male,nmccann@example.net,603-428-2429x27392,2018-10-06,Lawyer
10,8e4FB470FE19bF0,Isaiah,Downs,Male,virginiaterrell@example.org,+1-511-372-1544x8206,1964-09-20,Engineer site",
        @"<tr>
  <li>1</li>
  <li>88F7B33d2bcf9f5</li>
  <li>Shelby</li>
  <li>Terrell</li>
  <li>Male</li>
  <li>elijah57@example.net</li>
  <li>001-084-906-7849x73518</li>
  <li>1945-10-26</li>
  <li>Games developer</li>
</tr>
<tr>
  <li>2</li>
  <li>f90cD3E76f1A9b9</li>
  <li>Phillip</li>
  <li>Summers</li>
  <li>Female</li>
  <li>bethany14@example.com</li>
  <li>214.112.6044x4913</li>
  <li>1910-03-24</li>
  <li>Phytotherapist</li>
</tr>
<tr>
  <li>3</li>
  <li>DbeAb8CcdfeFC2c</li>
  <li>Kristine</li>
  <li>Travis</li>
  <li>Male</li>
  <li>bthompson@example.com</li>
  <li>277.609.7938</li>
  <li>1992-07-02</li>
  <li>Homeopath</li>
</tr>
<tr>
  <li>4</li>
  <li>A31Bee3c201ef58</li>
  <li>Yesenia</li>
  <li>Martinez</li>
  <li>Male</li>
  <li>kaitlinkaiser@example.com</li>
  <li>584.094.6111</li>
  <li>2017-08-03</li>
  <li>Market researcher</li>
</tr>
<tr>
  <li>5</li>
  <li>1bA7A3dc874da3c</li>
  <li>Lori</li>
  <li>Todd</li>
  <li>Male</li>
  <li>buchananmanuel@example.net</li>
  <li>689-207-3558x7233</li>
  <li>1938-12-01</li>
  <li>Veterinary surgeon</li>
</tr>
<tr>
  <li>6</li>
  <li>bfDD7CDEF5D865B</li>
  <li>Erin</li>
  <li>Day</li>
  <li>Male</li>
  <li>tconner@example.org</li>
  <li>001-171-649-9856x5553</li>
  <li>2015-10-28</li>
  <li>Waste management officer</li>
</tr>
<tr>
  <li>7</li>
  <li>bE9EEf34cB72AF7</li>
  <li>Katherine</li>
  <li>Buck</li>
  <li>Female</li>
  <li>conniecowan@example.com</li>
  <li>+1-773-151-6685x49162</li>
  <li>1989-01-22</li>
  <li>Intelligence analyst</li>
</tr>
<tr>
  <li>8</li>
  <li>2EFC6A4e77FaEaC</li>
  <li>Ricardo</li>
  <li>Hinton</li>
  <li>Male</li>
  <li>wyattbishop@example.com</li>
  <li>001-447-699-7998x88612</li>
  <li>1924-03-26</li>
  <li>Hydrogeologist</li>
</tr>
<tr>
  <li>9</li>
  <li>baDcC4DeefD8dEB</li>
  <li>Dave</li>
  <li>Farrell</li>
  <li>Male</li>
  <li>nmccann@example.net</li>
  <li>603-428-2429x27392</li>
  <li>2018-10-06</li>
  <li>Lawyer</li>
</tr>
<tr>
  <li>10</li>
  <li>8e4FB470FE19bF0</li>
  <li>Isaiah</li>
  <li>Downs</li>
  <li>Male</li>
  <li>virginiaterrell@example.org</li>
  <li>+1-511-372-1544x8206</li>
  <li>1964-09-20</li>
  <li>Engineer site</li>
</tr>"
      ),
    },
  ];

  [Test, TestCaseSource(nameof(IOTestCases))]
  public void CanProcessSourceWithProgram(string testName, IOTestCase testCase)
  {
    var result = SewLang.Eval(testCase.Input, testCase.SewProgram);
    try
    {
      var actual = result.Unwrap().Output.StringJoin("\n");
      Assert.That(actual, Is.EqualTo(testCase.Output));
    }
    catch
    {
      Console.WriteLine(testCase.Debug());
      result.InspectErr((err) => Console.WriteLine(err.Debug()));
      throw;
    }
  }
}
