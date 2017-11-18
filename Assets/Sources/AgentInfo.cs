using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInfo
{

    /// <summary>
    /// 50 prénoms masculins
    /// </summary>
    public static readonly string[] MaleNames = new string[]
    {
        "Adam", "Alex", "Alexandre", "Alexis",
        "Anthony", "Antoine", "Benjamin", "Cédric",
        "Charles", "Christopher", "David", "Dylan",
        "Édouard", "Elliot", "Émile", "Étienne",
        "Félix", "Gabriel", "Guillaume", "Hugo",
        "Isaac", "Jacob", "Jérémy", "Jonathan",
        "Julien", "Justin", "Léo", "Logan",
        "Loïc", "Louis", "Lucas", "Ludovic",
        "Malik", "Mathieu", "Mathis", "Maxime",
        "Michaël", "Nathan", "Nicolas", "Noah",
        "Olivier", "Philippe", "Raphaël", "Samuel",
        "Simon", "Thomas", "Tommy", "Tristan",
        "Victor", "Vincent"
    };

    /// <summary>
    /// 50 prénoms féminins
    /// </summary>
    public static readonly string[] FemaleNames = new string[]
    {
        "Alexia", "Alice", "Alicia", "Amélie",
        "Anaïs", "Annabelle", "Arianne", "Audrey",
        "Aurélie", "Camille", "Catherine", "Charlotte",
        "Chloé", "Clara", "Coralie", "Daphnée",
        "Delphine", "Elizabeth", "Élodie", "Émilie",
        "Emma", "Emy", "Ève", "Florence",
        "Gabrielle", "Jade", "Juliette", "Justine",
        "Laurence", "Laurie", "Léa", "Léanne",
        "Maélie", "Maéva", "Maika", "Marianne",
        "Marilou", "Maude", "Maya", "Mégan",
        "Mélodie", "Mia", "Noémie", "Océane",
        "Olivia", "Rosalie", "Rose", "Sarah",
        "Sofia", "Victoria"
    };

    /// <summary>
    /// Retourne un genre aléatoire
    /// </summary>
    /// <returns>True si homme et false si femme</returns>
    public static bool GetGender()
    {
        return Manager.Instance.Randomizer.Next() % 2 == 0;
    }

    /// <summary>
    /// Retourne un nom masculin aléatoirement
    /// </summary>
    /// <returns>Le prénom</returns>
    public static string GetMaleName()
    {
        return MaleNames[Manager.Instance.Randomizer.Next() % 50];
    }

    /// <summary>
    /// Retourne un nom féminin aléatoirement
    /// </summary>
    /// <returns>Le prénom</returns>
    public static string GetFemaleName()
    {
        return FemaleNames[Manager.Instance.Randomizer.Next() % 50];
    }

}