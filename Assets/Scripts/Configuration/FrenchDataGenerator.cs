using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "French Data Generator", menuName = "Paper Trail/French Data Generator")]
public class FrenchDataGenerator : ScriptableObject
{
    [Header("French Names Database")]
    public List<string> frenchFirstNames = new List<string>
    {
        "Pierre", "Marie", "Jean", "Françoise", "Michel", "Monique", "Alain", "Catherine",
        "Philippe", "Nathalie", "Bernard", "Isabelle", "Christophe", "Sylvie", "Daniel", "Chantal",
        "Patrice", "Martine", "François", "Nicole", "Gérard", "Brigitte", "Antoine", "Hélène",
        "Laurent", "Valérie", "Thierry", "Dominique", "Pascal", "Véronique", "Bruno", "Sandrine"
    };
    
    public List<string> frenchLastNames = new List<string>
    {
        "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand",
        "Leroy", "Moreau", "Simon", "Laurent", "Lefebvre", "Michel", "Garcia", "David",
        "Bertrand", "Roux", "Vincent", "Fournier", "Morel", "Girard", "Andre", "Lefevre",
        "Mercier", "Dupont", "Lambert", "Bonnet", "François", "Martinez", "Legrand", "Garnier"
    };
    
    public List<string> frenchCities = new List<string>
    {
        "Paris", "Marseille", "Lyon", "Toulouse", "Nice", "Nantes", "Strasbourg", "Montpellier",
        "Bordeaux", "Lille", "Rennes", "Reims", "Le Havre", "Saint-Étienne", "Toulon", "Grenoble",
        "Dijon", "Angers", "Nîmes", "Villeurbanne", "Le Mans", "Aix-en-Provence", "Clermont-Ferrand",
        "Brest", "Tours", "Limoges", "Amiens", "Perpignan", "Metz", "Besançon", "Orléans"
    };
    
    public List<string> frenchStreets = new List<string>
    {
        "Rue de la République", "Avenue Jean Jaurès", "Place de la Mairie", "Rue Victor Hugo",
        "Boulevard Gambetta", "Rue de la Liberté", "Avenue Charles de Gaulle", "Rue Pasteur",
        "Place du Général de Gaulle", "Rue Jean Moulin", "Avenue de la Gare", "Rue des Écoles",
        "Boulevard Saint-Michel", "Rue de l'Église", "Avenue Foch", "Rue Nationale"
    };
    
    public List<string> frenchProfessions = new List<string>
    {
        "Comptable", "Professeur", "Infirmière", "Ingénieur", "Secrétaire", "Mécanicien",
        "Boulanger", "Pharmacien", "Architecte", "Électricien", "Coiffeur", "Médecin",
        "Avocat", "Plombier", "Journaliste", "Cuisinier", "Policier", "Agriculteur",
        "Vendeur", "Chauffeur", "Artisan", "Fonctionnaire", "Technicien", "Commercial"
    };
    
    public List<string> frenchRequestTypes = new List<string>
    {
        "Demande de congés payés", "Certificat médical de complaisance", "Autorisation de travaux",
        "Permis de construire une niche à chien", "Déclaration de perte de chaussette",
        "Formulaire de changement d'adresse de chat", "Demande d'autorisation de respirer",
        "Certificat de vie pour plante verte", "Permis de marcher sur le trottoir",
        "Autorisation de porter des chaussettes dépareillées", "Déclaration de sourire en public",
        "Formulaire de justification de bâillement", "Demande de congé pour aller aux toilettes"
    };
    
    [Header("Generation Settings")]
    public int batchSize = 50;
    
    public FrenchCitizenData GenerateRandomCitizen()
    {
        FrenchCitizenData citizen = new FrenchCitizenData();
        
        citizen.firstName = frenchFirstNames[Random.Range(0, frenchFirstNames.Count)];
        citizen.lastName = frenchLastNames[Random.Range(0, frenchLastNames.Count)];
        citizen.email = $"{citizen.firstName.ToLower()}.{citizen.lastName.ToLower()}@example.fr";
        
        string street = frenchStreets[Random.Range(0, frenchStreets.Count)];
        string city = frenchCities[Random.Range(0, frenchCities.Count)];
        citizen.address = $"{Random.Range(1, 200)} {street}, {city}";
        
        citizen.phoneNumber = $"0{Random.Range(1, 10)}.{Random.Range(10, 99)}.{Random.Range(10, 99)}.{Random.Range(10, 99)}.{Random.Range(10, 99)}";
        citizen.socialSecurityNumber = $"{Random.Range(1, 3)}{Random.Range(10, 99)}{Random.Range(01, 12)}{Random.Range(01, 99)}{Random.Range(001, 999)}{Random.Range(01, 99)}";
        
        int birthYear = Random.Range(1940, 2005);
        int birthMonth = Random.Range(1, 13);
        int birthDay = Random.Range(1, 29);
        citizen.birthDate = $"{birthDay:D2}/{birthMonth:D2}/{birthYear}";
        citizen.birthPlace = frenchCities[Random.Range(0, frenchCities.Count)];
        
        citizen.profession = frenchProfessions[Random.Range(0, frenchProfessions.Count)];
        
        // Generate common requests
        int requestCount = Random.Range(1, 4);
        for (int i = 0; i < requestCount; i++)
        {
            string request = frenchRequestTypes[Random.Range(0, frenchRequestTypes.Count)];
            if (!citizen.commonRequests.Contains(request))
            {
                citizen.commonRequests.Add(request);
            }
        }
        
        citizen.desperationLevel = Random.Range(0.1f, 1f);
        citizen.previousRequestsCount = Random.Range(0, 10);
        
        return citizen;
    }
    
    public List<FrenchCitizenData> GenerateBatch()
    {
        List<FrenchCitizenData> batch = new List<FrenchCitizenData>();
        for (int i = 0; i < batchSize; i++)
        {
            batch.Add(GenerateRandomCitizen());
        }
        return batch;
    }
}
