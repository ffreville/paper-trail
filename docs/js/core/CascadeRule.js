class CascadeRule{
    constructor(name, formsToTrigger, condition, delayMinutes, triggerMessage){
        this.name = name;
        this.formsToTrigger= formsToTrigger;
        this.condition = condition;
        this.delayMinutes = delayMinutes;
        this.triggerMessage = triggerMessage;
    }
}