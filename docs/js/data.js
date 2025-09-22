var data = {};
data.forms = {
    "F-001" : {
        title: "Premier formulaire",
        description: "On sait pas quoi mettre dedans encore",
        code: "F-001",
        department: "Informatique",
        fields : [{
            fieldName : "object-field",
            label: "Objet de la demande",
            isRequired : true,
            isReadOnly: false
        },{
            fieldName : "name-field",
            label: "Nom du demandeur",
            isRequired : true,
            isReadOnly: false
        },{
            fieldName : "age-field",
            label: "Age",
            isRequired : true,
            isReadOnly: false
        }],
        cascadeRules : [{
            name: "majeur",
            condition: {
                conditionType: ConditionType.FieldGreaterThan,
                fieldName: "age-field",
                expectedValue : 18 
            },
            triggerMessage: "Vous êtes majeur, vous avez un formulaire en plus",
            formsToTrigger : ["F-003"]
        }]
    },
    "F-002" : {
        title: "Second formulaire",
        description: "On sait pas quoi mettre dedans non plus",
        code: "F-002",
        department: "Informatique",
        fields : [{
            fieldName : "object-field",
            label: "Objet de la demande",
            isRequired : true,
            isReadOnly: false
        },{
            fieldName : "name-field",
            label: "Nom du demandeur",
            isRequired : true,
            isReadOnly: false
        }]
    },
        "F-003" : {
        title: "Formulaire du majeur",
        description: "On sait pas quoi mettre dedans non plus",
        code: "F-003",
        department: "Informatique",
        fields : [{
            fieldName : "object-field",
            label: "Objet de la demande",
            isRequired : true,
            isReadOnly: false
        },{
            fieldName : "name-field",
            label: "Nom du demandeur",
            isRequired : true,
            isReadOnly: false
        }]
    }
};

data.scenarios = [{
    title: "Jour classique",
    description: "Jour de travail classique",
    steps : [{
        name: "Matin classique",
        description: "C'est un petit matin tout classique tout mignon",
        forms: ["F-001", "F-002"],
        isOptional: false,
        isCompleted: false,
        chanceToAppear : 10
    },{
        name: "Midi classique",
        description: "C'est un petit midi tout classique tout mignon",
        forms: ["F-001", "F-002"],
        isOptional: true,
        isCompleted: false,
        chanceToAppear : 10
    },{
        name: "Aprem classique",
        description: "C'est une petite aprem tout classique tout mignon",
        forms: ["F-001", "F-002"],
        isOptional: false,
        isCompleted: false,
        chanceToAppear : 10  
    }]
}]
