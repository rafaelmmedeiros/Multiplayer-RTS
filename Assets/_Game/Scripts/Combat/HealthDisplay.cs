using RTS.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [Header(Headers.members)]
        [SerializeField] private Health health = null;
        [SerializeField] private GameObject healthBarParent = null;
        [SerializeField] private Image healthBarImage = null;

        private void Awake()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}