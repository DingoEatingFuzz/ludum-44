using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
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
        None = 0,
        Normal = 1,
        High = 2
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

    /// <summary>
    /// Dialogue message and the character that is transmitting the message
    /// </summary>
    [System.Serializable]
    public class MessagePair
    {
        public Character DisplayCharacter { get => Character; }
        [Tooltip("The character that is speaking this message"), SerializeField]
        private Character Character = Character.None;

        public string DisplayMessage { get => Message; }
        [Tooltip("Message text"), TextArea, SerializeField]
        private string Message;

        public float AutoAdvance { get => _AutoAdvance; }
        [Tooltip("How long until this message advances"), SerializeField]
        private float _AutoAdvance = 7f;

        public Sprite CharacterSprite
        {
            get => CharacterLookup.Characters[Character];
        }
    }
}
