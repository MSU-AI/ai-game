using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OK
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
            Debug.Log("You interacted.");

            // Pick up the weapon and add it to the inventory
        }
    }    
}

