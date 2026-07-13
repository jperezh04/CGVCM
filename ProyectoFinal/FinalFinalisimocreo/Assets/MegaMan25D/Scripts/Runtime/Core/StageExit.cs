using UnityEngine;
using UnityEngine.SceneManagement;

namespace MegaMan25D
{
    [RequireComponent(typeof(Collider))]
    public sealed class StageExit : MonoBehaviour
    {
        public string nextSceneName;
        public bool completeCampaign;
        public string completionMessage =
            "CAMPAIGN COMPLETE\nReplace this screen with your own ending.";

        private bool completed;

        private void Reset()
        {
            Collider trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            bool isPlayer =
                other.GetComponentInParent<PlayerMotor>() != null ||
                other.GetComponentInParent<RideChaserController>() != null ||
                other.GetComponentInParent<AirVehicleController>() != null;

            if (!isPlayer)
            {
                return;
            }

            Collider trigger = GetComponent<Collider>();
            trigger.enabled = false;

            if (completeCampaign || string.IsNullOrWhiteSpace(nextSceneName))
            {
                completed = true;
                return;
            }

            SceneManager.LoadScene(nextSceneName);
        }

        private void OnGUI()
        {
            if (!completed)
            {
                return;
            }

            float width = Mathf.Min(620f, Screen.width - 60f);
            float height = 150f;
            Rect box = new Rect(
                (Screen.width - width) * 0.5f,
                (Screen.height - height) * 0.5f,
                width,
                height
            );

            GUI.Box(box, completionMessage);
        }
    }
}
