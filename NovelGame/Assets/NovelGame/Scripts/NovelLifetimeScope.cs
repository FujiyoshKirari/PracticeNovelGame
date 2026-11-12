using MyDIContainer;
using UnityEngine;

public class NovelLifetimeScope : LifetimeScope
{
    [SerializeField] private ScenarioRunner _scenarioRunner;
    [SerializeField] private string _url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.csv";
    protected override void Configure(IBuilder builder)
    {
        var scenarioSource = new WebScenarioSource(_url);

        builder.RegisterInstance<IScenarioSource>(scenarioSource);
        builder.RegisterInstance(_scenarioRunner);
    }
}
