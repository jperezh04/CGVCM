using UnityEngine;

namespace MegaMan25D
{
    public enum LevelMode
    {
        Platformer,
        RideChaser,
        AirMission
    }

    public sealed class LevelDefinition : MonoBehaviour
    {
        public LevelMode mode = LevelMode.Platformer;
        public string levelDisplayName = "Editable Level";
        [TextArea] public string designerNotes;
    }
}
