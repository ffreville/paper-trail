/**
 * Classe Achievement - Représente un succès/achievement du jeu
 */
class Achievement {
    constructor(id, name, description, requirement, category = 'general') {
        this.id = id;
        this.name = name;
        this.description = description;
        this.requirement = requirement;
        this.category = category;
        this.unlocked = false;
        this.unlockedDate = null;
        this.icon = '🏆';
        this.isSecret = false;
        this.rewards = {};
        this.progress = 0;
        this.maxProgress = 1;
    }

    // Vérifier si le succès doit être débloqué
    checkUnlock(gameState) {
        if (this.unlocked) return false;
        
        let shouldUnlock = false;
        
        // Logique de vérification selon l'ID du succès
        switch (this.id) {
            case 'first_building':
                // Vérifier qu'il y a au moins un bâtiment construit (built: true)
                shouldUnlock = gameState.buildings && gameState.buildings.some(b => b.built === true);
                break;
            case 'city_hall':
                // Vérifier qu'une mairie est construite
                shouldUnlock = gameState.buildings && gameState.buildings.some(b => 
                    b.buildingType && b.buildingType.id === 'mairie' && b.built === true
                );
                break;
            case 'first_adventurer':
                shouldUnlock = gameState.adventurers && gameState.adventurers.length > 0;
                break;
            case 'gold_hoarder':
                shouldUnlock = gameState.resources && gameState.resources.gold >= 100000;
                break;
            case 'population_100':
                shouldUnlock = gameState.resources && gameState.resources.population >= 100;
                break;
            case 'day_survivor':
                shouldUnlock = gameState.day >= 7;
                break;
            case 'builder':
                // Vérifier qu'il y a au moins 5 bâtiments construits
                shouldUnlock = gameState.buildings && gameState.buildings.filter(b => b.built === true).length >= 5;
                break;
            case 'guild_master':
                shouldUnlock = gameState.adventurers && gameState.adventurers.length >= 10;
                break;
            case 'first_mission':
                shouldUnlock = gameState.completedMissions && gameState.completedMissions > 0;
                break;
            case 'researcher':
                shouldUnlock = gameState.unlockedUpgrades && gameState.unlockedUpgrades.length > 0;
                break;
        }
        
        if (shouldUnlock) {
            this.unlock();
            return true;
        }
        
        return false;
    }

    // Débloquer le succès
    unlock() {
        if (this.unlocked) return false;
        
        this.unlocked = true;
        this.unlockedDate = new Date();
        this.progress = this.maxProgress;
        
        return true;
    }

    // Obtenir les informations d'affichage
    getDisplayInfo() {
        return {
            id: this.id,
            name: this.name,
            description: this.description,
            requirement: this.requirement,
            category: this.category,
            unlocked: this.unlocked,
            unlockedDate: this.unlockedDate,
            formattedUnlockDate: this.unlockedDate ? this.formatDate(this.unlockedDate) : null,
            icon: this.icon,
            isSecret: this.isSecret,
            rewards: this.rewards,
            progress: this.progress,
            maxProgress: this.maxProgress,
            progressPercent: Math.round((this.progress / this.maxProgress) * 100)
        };
    }

    // Formater la date de déblocage
    formatDate(date) {
        return date.toLocaleDateString('fr-FR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    // Sérialisation pour la sauvegarde
    toJSON() {
        return {
            id: this.id,
            name: this.name,
            description: this.description,
            requirement: this.requirement,
            category: this.category,
            unlocked: this.unlocked,
            unlockedDate: this.unlockedDate,
            icon: this.icon,
            isSecret: this.isSecret,
            rewards: this.rewards,
            progress: this.progress,
            maxProgress: this.maxProgress
        };
    }

    static fromJSON(data) {
        const achievement = new Achievement(
            data.id,
            data.name,
            data.description,
            data.requirement,
            data.category
        );
        
        achievement.unlocked = data.unlocked || false;
        achievement.unlockedDate = data.unlockedDate ? new Date(data.unlockedDate) : null;
        achievement.icon = data.icon || '🏆';
        achievement.isSecret = data.isSecret || false;
        achievement.rewards = data.rewards || {};
        achievement.progress = data.progress || 0;
        achievement.maxProgress = data.maxProgress || 1;
        
        return achievement;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = Achievement;
}
