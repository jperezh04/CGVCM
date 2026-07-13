using UnityEngine;

namespace MegaMan25D
{
    public sealed class BossHealthBar : MonoBehaviour
    {
        public Damageable target;
        public string displayName = "BOSS";
        public bool visible;
        public Vector2 size = new Vector2(620f, 24f);
        public float topMargin = 26f;

        public void Show(Damageable boss, string bossName)
        {
            target = boss;
            displayName = string.IsNullOrWhiteSpace(bossName) ? "BOSS" : bossName;
            visible = target != null;
        }

        public void Hide()
        {
            visible = false;
            target = null;
        }

        private void OnGUI()
        {
            if (!visible || target == null)
            {
                return;
            }

            float width = Mathf.Min(size.x, Screen.width - 80f);
            Rect labelRect = new Rect((Screen.width - width) * 0.5f, topMargin, width, 24f);
            Rect backgroundRect = new Rect(labelRect.x, labelRect.y + 24f, width, size.y);
            Rect fillRect = new Rect(
                backgroundRect.x + 3f,
                backgroundRect.y + 3f,
                (backgroundRect.width - 6f) * target.Health01,
                backgroundRect.height - 6f
            );

            GUI.Label(labelRect, displayName);
            GUI.Box(backgroundRect, GUIContent.none);
            GUI.Box(fillRect, GUIContent.none);
        }
    }
}
