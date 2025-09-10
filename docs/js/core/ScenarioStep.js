class ScenarioStep{
    constructor(name, description, forms, isOptional, isCompleted, chanceToAppear){
        this.name = name;
        this.description = description;
        this.forms = forms;
        this.isOptional = isOptional;
        this.isCompleted = isCompleted;
        this.chanceToAppear = chanceToAppear;
    }
}