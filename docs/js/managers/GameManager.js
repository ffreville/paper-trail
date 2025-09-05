class GameManager {

    mainMenuDiv;
    gameDiv;

    constructor(){
    }

    initialize(){
        this.mainMenuDiv = getElement("#main-menu");
        this.gameDiv = getElement("#game");
        getElement("#new-game").addEventListener('click',this.startNewGame.bind(this));
        getElement("#load-game").addEventListener('click',this.loadGame.bind(this));
    }

    startNewGame(){
        //TODO Make a screen transition here, with some blur
        this.mainMenuDiv.style.display = 'none';
        this.gameDiv.style.display = 'block';
    }

    loadGame(){
        console.log("load game");
    }

    saveGame(){

    }
    
    autoSave(){

    }

    resetGame(){
        
    }
}
