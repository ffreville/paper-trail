const ConditionType = {
    Always,
    FieldEquals,
    FieldGreaterThan,
    FieldLessThan
}

class Condition{
    constructor(conditionType, fieldName, expectedValue, numericValue){
        this.conditionType = conditionType;
        this.fieldName = fieldName;
        this.expectedValue = expectedValue;
        this.numericValue = numericValue;
    }
}