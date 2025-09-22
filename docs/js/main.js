class PaperTrail {
    constructor() {
        this.gameManager = new GameManager();
        this.formManager = new FormManager();
    }
}

// Instance globale de l'application
let app;

document.addEventListener('DOMContentLoaded', () => {
    app = new PaperTrail();
    app.gameManager.initialize();
    app.formManager.initialize();
    window.app = app;

    window.addEventListener("game-started", (event) => {
        app.formManager.loadForm();
    });
});

// Gestion des erreurs globales
window.addEventListener('error', (event) => {
    console.error('Erreur JavaScript:', event.error);
});

// Sauvegarder avant fermeture
window.addEventListener('beforeunload', (event) => {
    if (app && app.gameManager) {
        //app.gameManager.stopGameTimer();
        app.gameManager.autoSave();
    }
});

// Export pour utilisation dans d'autres modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { PaperTrail };
}
