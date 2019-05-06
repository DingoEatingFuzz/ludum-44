using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// List of all the portraits
    /// </summary>
    public enum Character
    {
        None,
        General,
        MaxHart,
        OlympusTower,
        Shopkeep,
        Unknown
    }

    public enum Priority
    {
        None,
        Normal,
        High
    }

    /// <summary>
    /// Get the portrait from the character name
    /// </summary>
    public static class CharacterLookup
    {
        public static Dictionary<Character, Sprite> Characters =
            new Dictionary<Character, Sprite>()
            {
                {Character.None, null },
                {Character.General, Resources.Load<Sprite>("Portraits/MauraderGeneral") },
                {Character.MaxHart, Resources.Load<Sprite>("Portraits/MaxHart") },
                {Character.OlympusTower, Resources.Load<Sprite>("Portraits/OlympusTower") },
                {Character.Shopkeep, Resources.Load<Sprite>("Portraits/ShopKeep") },
                {Character.Unknown, Resources.Load<Sprite>("Portraits/Unknown") }
            };
    }

    [System.Serializable]
    public class MessagePair
    {
        public Character Character;
        [TextArea]
        public string Message;
        
        public Sprite CharacterSprite
        {
            get => CharacterLookup.Characters[Character];
        }
    }
}
