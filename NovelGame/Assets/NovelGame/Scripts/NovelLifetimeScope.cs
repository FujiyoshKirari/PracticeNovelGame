using MyDIContainer;
using NaughtyAttributes;
using UnityEngine;

public class NovelLifetimeScope : LifetimeScope
{
    [SerializeField, Required] private ScenarioRunner _scenarioRunner;
    [SerializeField] private string _url = "https://fujiyoshkirari.github.io/PracticeNovelGame/scenario.csv";
    [SerializeField] private string _spriteBaseUrl = "https://fujiyoshkirari.github.io/PracticeNovelGame/Resources/";
    [SerializeField, Required] private MessageWindow _messageWindow;
    [SerializeField, Required] private CharacterStage _characterStage;
    [SerializeField, Required] private BackgroundView _bgView; 
    protected override void Configure(IBuilder builder)
    {
        var scenarioSource = new WebScenarioSource(_url);
        var spriteSource = new WebSpriteSource(_spriteBaseUrl);
        //var spriteSource = new ResourcesSpriteSource();

        builder.RegisterInstance<IScenarioSource>(scenarioSource);
        builder.RegisterInstance(_scenarioRunner);
        builder.RegisterInstance(_messageWindow);
        builder.RegisterInstance(_characterStage);
        builder.RegisterInstance(_bgView);
        builder.RegisterInstance<ISpriteSource>(spriteSource);
    }
}
