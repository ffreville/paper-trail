class FormManager {
    constructor(){

    }

    buildScenarioStep(scenarioStep){
        return new ScenarioStep(scenarioStep.name, scenarioStep.description, scenarioStep.forms,
            scenarioStep.isOptional, scenarioStep.isCompleted, scenarioStep.chanceToAppear
        )
    }

    buildScenario(scenario){
        var steps = [];
        for(const i in scenario.steps){
            steps[i] = this.buildScenarioStep(scenario.steps[i]);
        }
        return new Scenario(scenario.title, scenario.description, steps);
    }

    initForms(){
        //TODO choose a scenario
        var scenario = this.buildScenario(data.scenarios[0])
    }

    buildForm(){
        var formToBuild;
        //TODO build an HTML form
        return formToBuild;
    }
}
