using MyDIContainer;
using NaughtyAttributes;
using UnityEngine;

public class NovelLifetimeScope : LifetimeScope
{
    [SerializeField, Required] private ScenarioRunner _scenarioRunner;
    [SerializeField] private string _url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.csv";
    [SerializeField, Required] private MessageWindow _messageWindow;
    protected override void Configure(IBuilder builder)
    {
        var scenarioSource = new WebScenarioSource(_url);

        builder.RegisterInstance<IScenarioSource>(scenarioSource);
        builder.RegisterInstance(_scenarioRunner);
        builder.RegisterInstance(_messageWindow);
    }
}
