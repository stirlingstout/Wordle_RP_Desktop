﻿using System.Collections.Immutable;
using System.Diagnostics;

static string Set(string word, int n, char newChar) =>
    word.Substring(0, n) + newChar + word.Substring(n + 1);

static bool IsGreen(string attempt, string target, int n) => target[n] == attempt[n];

static string SetAttemptIfGreen(string attempt, string target, int n) =>
    IsGreen(attempt, target, n) ? Set(attempt, n, '*') : attempt;

static string SetTargetIfGreen(string attempt, string target, int n) =>
    IsGreen(attempt, target, n) ? Set(target, n, '.') : target;

static (string, string) EvaluateGreens(string attempt, string target) =>
    Enumerable.Range(0, 5).Aggregate((attempt, target), (a, x) =>
        (SetAttemptIfGreen(a.Item1, a.Item2, x), SetTargetIfGreen(a.Item1, a.Item2, x)));

static bool IsYellow(string attempt, string target, int n) => target.Contains(attempt[n]);

static bool IsAlreadyMarkedGreen(string attempt, int n) => attempt[n] == '*';


static string SetAttemptIfYellow(string attempt, string target, int n) =>
    IsAlreadyMarkedGreen(attempt, n) ? attempt : IsYellow(attempt, target, n) ?
Set(attempt, n, '+') : Set(attempt, n, '_');


static string SetTargetIfYellow(string attempt, string target, int n) =>
     IsAlreadyMarkedGreen(attempt, n) ? target : IsYellow(attempt, target, n) ?
Set(target, target.IndexOf(attempt[n]), '.') : target;


static (string, string) EvaluateYellows(string attempt, string target) =>
    Enumerable.Range(0, 5).Aggregate((attempt, target), (a, x) =>
        (SetAttemptIfYellow(a.Item1, a.Item2, x), SetTargetIfYellow(a.Item1, a.Item2, x)));


static string MarkAttempt(string attempt, string target) =>
    EvaluateYellows(EvaluateGreens(attempt, target).Item1,
        EvaluateGreens(attempt, target).Item2).Item1;


static IEnumerable<string> PossibleAnswersAfterAttempt(
    IEnumerable<string> prior, string attempt, string mark) =>
        prior.Where(w => MarkAttempt(attempt, w) == mark);


static int WordCountLeftByWorstOutcome(IEnumerable<string> possibleWords, string attempt) =>
    possibleWords.GroupBy(w => MarkAttempt(attempt, w)).Max(g => g.Count());


static string BestAttempt(IEnumerable<string> possAnswers, IEnumerable<string> possAttempts) => possAttempts.Select(w => (WordCountLeftByWorstOutcome(possAnswers, w), w)).
           Aggregate((best, x) => (x.Item1 < best.Item1) ||
               (x.Item1 == best.Item1 && possAnswers.Contains(x.Item2)) ? x : best).Item2;

//Data definitions (stubs only - See Appendix I for the full lists):
var ValidWords = ReadAllWordsFrom("ValidWords.txt");
Debug.Assert(ValidWords.Count == 2309);
var ExtraPossibleAnswers = ReadAllWordsFrom("ExtraPossibleAnswers.txt");
Debug.Assert(ExtraPossibleAnswers.Count == 12546);

    ImmutableList<string> ReadAllWordsFrom(string filename)
{
    using (var inFile = new System.IO.StreamReader(filename))
    {
        return inFile.ReadToEnd().Split(Environment.NewLine[1]).ToImmutableList();
    }
};

// User interface:
IEnumerable<string> possible = ExtraPossibleAnswers.AddRange(ValidWords);
var outcome = "";
while (outcome != "*****")
{
    var attempt = BestAttempt(possible, ValidWords);
    Console.WriteLine(attempt);
    outcome = Console.ReadLine();
    possible = PossibleAnswersAfterAttempt(possible, attempt, outcome);
}