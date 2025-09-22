class GameManager {

    mainMenuDiv;
    gameDiv;

    constructor(){
    }

    initListeners(){
        window.addEventListener("game-started", (event) => {
            app.formManager.loadForm();
        });
    }

    initialize(){
        this.mainMenuDiv = getElement("#main-menu");
        this.gameDiv = getElement("#game");
        getElement("#new-game").addEventListener('click',this.startNewGame.bind(this));
        getElement("#load-game").addEventListener('click',this.loadGame.bind(this));

        this.initListeners();
    }

    startNewGame(){
        //TODO Make a screen transition here, with some blur
        this.mainMenuDiv.style.display = 'none';
        this.gameDiv.style.display = 'block';
        window.dispatchEvent(new CustomEvent("game-started"));
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
