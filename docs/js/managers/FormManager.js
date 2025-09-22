class FormManager {
    scenario;
    currentStep;
    currentForm;
    currentFormHTML;
    constructor(){

    }

    initialize(){
        this.initForms();

        getElement("#approve").addEventListener('click',this.approve.bind(this));
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
        this.scenario = this.buildScenario(data.scenarios[0]);
        console.log(this.scenario);
    }
    
    getHeaderForm(title){
        //TODO gérer si c'est urgent ou non
        var html = '<div class="urgent-stamp">URGENT</div>';
        html += '<div class="document-header">' + title + '</div>';
        return html;
    }

    getTextField(field){
        var html = '<div class="form-field">';
        html += '<span class="form-label">' + field.label + ':</span>';
        html += '<input type="text" id="' + field.fieldName + '" class="form-input"/>';
        return html + '</div>';
    }

    buildForm(idForm){
        var form = data.forms[idForm];
        var formToBuild = this.getHeaderForm(form.title);
        for (var id in form.fields){
            formToBuild += this.getTextField(form.fields[id]);
        }
        return formToBuild + "</div>";
    }

    loadForm(){
        //Select the form in the scenario
        this.currentStep = this.scenario.steps[0];
        this.currentForm = data.forms[this.scenario.steps[0].forms[0]];
        this.currentFormHTML = this.buildForm(this.scenario.steps[0].forms[0]);
        console.log(this.currentFormHTML);
        getElement("#current-form").innerHTML = this.currentFormHTML;
    }

    testCondition(rule, value){
        switch(rule.condition.conditionType){
            case ConditionType.FieldGreaterThan:
                if(value > rule.condition.expectedValue){
                    return true;
                }
            break;
            default:
                console.log("AUTRES");
                return false;
            break;

        }
    }

    approve(){
        //TODO vérifier que tous les fields obligatoires sont bien remplis
        var errors = false;
        for(var id in this.currentForm.fields){
            var idField = this.currentForm.fields[id].fieldName;
            if(this.currentForm.fields[id].isRequired){
                if(!getElement("#"+idField).value){
                    console.log("ERREUR : le champ " + idField + " n'est pas remplis");
                    errors = true;
                }
            }

        }
        if(!errors){
            for(var id in this.currentForm.cascadeRules){
                var rule =  this.currentForm.cascadeRules[id];
                var fieldValue = getElement("#"+rule.condition.fieldName).value;
                if( this.testCondition(rule, fieldValue)){
                    //ON remplis la condition
                    console.log("chargement du F-003");
                }
            }
        }
        //console.log(this.currentForm);
    }

    refuse(){

    }
}
