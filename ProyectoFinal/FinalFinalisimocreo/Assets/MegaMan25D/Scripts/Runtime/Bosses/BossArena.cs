using UnityEngine;
using UnityEngine.Events;

namespace MegaMan25D
{
    [RequireComponent(typeof(Collider))]
    public sealed class BossArena : MonoBehaviour
    {
        public BossController boss;
        public GameObject entranceDoor;
        public GameObject exitDoor;
        public BossHealthBar healthBar;
        public SideScrollerCamera cameraController;
        public Vector2 cameraHorizontalBounds;
        public UnityEvent onFightStarted;
        public UnityEvent onFightWon;

        public bool FightStarted { get; private set; }
        public bool FightWon { get; private set; }

        private Collider trigger;

        private void Awake()
        {
            trigger = GetComponent<Collider>();
            trigger.isTrigger = true;

            if (entranceDoor != null)
            {
                entranceDoor.SetActive(false);
            }

            if (exitDoor != null)
            {
                exitDoor.SetActive(true);
            }

            if (boss != null)
            {
                boss.Deactivate();
                Damageable damageable = boss.GetComponent<Damageable>();
                damageable.Died += OnBossDied;
            }
        }

        private void OnDestroy()
        {
            if (boss != null)
            {
                Damageable damageable = boss.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.Died -= OnBossDied;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (FightStarted)
            {
                return;
            }

            bool playerEntered =
                other.GetComponentInParent<PlayerMotor>() != null ||
                other.GetComponentInParent<RideChaserController>() != null ||
                other.GetComponentInParent<AirVehicleController>() != null;

            if (!playerEntered)
            {
                return;
            }

            StartFight(other.transform.root);
        }

        public void StartFight(Transform player)
        {
            if (FightStarted || boss == null)
            {
                return;
            }

            FightStarted = true;
            trigger.enabled = false;

            if (entranceDoor != null)
            {
                entranceDoor.SetActive(true);
            }

            if (exitDoor != null)
            {
                exitDoor.SetActive(true);
            }

            if (cameraController == null)
            {
                cameraController = FindObjectOfType<SideScrollerCamera>();
            }

            if (cameraController != null)
            {
                cameraController.SetTemporaryHorizontalBounds(
                    cameraHorizontalBounds.x,
                    cameraHorizontalBounds.y
                );
            }

            boss.Activate(player);

            Damageable damageable = boss.GetComponent<Damageable>();
            if (healthBar != null)
            {
                healthBar.Show(damageable, boss.bossDisplayName);
            }

            onFightStarted?.Invoke();
        }

        private void OnBossDied(Damageable defeatedBoss)
        {
            FightWon = true;

            if (entranceDoor != null)
            {
                entranceDoor.SetActive(false);
            }

            if (exitDoor != null)
            {
                exitDoor.SetActive(false);
            }

            if (cameraController != null)
            {
                cameraController.ClearTemporaryHorizontalBounds();
            }

            if (healthBar != null)
            {
                healthBar.Hide();
            }

            onFightWon?.Invoke();
        }
    }
}
